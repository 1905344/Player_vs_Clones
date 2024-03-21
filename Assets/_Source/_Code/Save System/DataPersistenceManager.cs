using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    public static DataPersistenceManager instance { get; private set; }

    //For enabling the debugging for the save data in the editor. Especially useful when testing and loading scenes without the game data being present.
    [Header("Debugging")]
    [SerializeField] private bool disableDataPersistence = false;
    [SerializeField] private bool initilizeDataIfNull = false;
    [SerializeField] private bool overrideSelectedProfileID = false;
    [SerializeField] private string testSelectedProfileID = "";

    //For the management of multiple save slots
    private string selectedProfileID = "";

    //Managing the save file 
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    [Header("Auto-saving Configuration")]
    [SerializeField] private float autoSaveTimeSeconds = 60f;

    //Save file data management
    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    //Character management across multiple scenes
    //The current method requires the Newtonsoft Json plugin for saving the character data
    //and requires a scriptable object that contains all of the different characters/skins

    //public PlayerSkinScriptableObject[] playerSkins;
    //public PlayerSkinScriptableObject GetCurrentSkin()
    //{
    //    return playerSkins[gameData.currentCharacter];
    //}
    //public PlayerSkinScriptableObject GetSkin(int skinIndex)
    //{
    //    return playerSkins[skinIndex];
    //}

    //Coroutine to improve auto-saving to prevent data loss if game suddenly crashes or closes
    private Coroutine autoSaveCoroutine;

    public string GetSavedSceneName()
    {
        //Error out and return null if there is no game data yet
        if (gameData == null)
        {
            Debug.LogError("Tried to get the scene name, but the data was null");
            return null;
        }

        //Otherwise, return the value from the data
        return gameData.currentSceneName;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one 'Data Persistence Manager' in the scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (disableDataPersistence)
        {
            Debug.LogWarning("Data Persistence is currently disabled!");
        }

        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);

        InitialiseSelectedProfileID();
    }

    public void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();

        //Start up the auto-saving coroutine
        if (autoSaveCoroutine != null)
        {
            StopCoroutine(autoSaveCoroutine);
        }
        autoSaveCoroutine = StartCoroutine(AutoSave());
    }

    #region Multiple Save Slots
    public void ChangeSelectedProfileID(string newProfileID)
    {
        //Update the profile for to use for saving and loading
        this.selectedProfileID = newProfileID;

        //Load the game, which will then use that profile, updating our game data accordingly
        LoadGame();
    }
    

    public void DeleteProfileData(string profileID)
    {
        //Delete the data for this profile ID
        dataHandler.Delete(profileID);

        //Initialise the selected profile ID
        InitialiseSelectedProfileID();

        //Reload the game so that the data matches the newly selected profile ID
        LoadGame();
    }

    private void InitialiseSelectedProfileID()
    {
        this.selectedProfileID = dataHandler.GetMostRecentlyUpdatedProfileID();
        if (overrideSelectedProfileID)
        {
            this.selectedProfileID = testSelectedProfileID;
            Debug.LogWarning("Overrode the selected profile's ID with the test id: " + testSelectedProfileID);
        }
    }

    #endregion

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        //Return right away if the data persistence is disabled
        if (disableDataPersistence)
        {
            return;
        }

        //Load any saved data from a file using the file data handler script
        this.gameData = dataHandler.Load(selectedProfileID);

        //Start a new game if the data is null and it is configured to initialise the data for debugging purposes
        if (this.gameData == null && initilizeDataIfNull)
        {
            NewGame();
        }

        //If no data can be loaded, don't continue
        if (this.gameData == null)
        {
            Debug.Log("No data was found. A new game needs to be started before data can be loaded.");
            return;
        }

        //Push the loaded data to all the other scripts that need it
        foreach(IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }


        ///* Example of logging the death count when loading the game data
        //Debug.Log("Loaded death count = " + gameData.deathCount);*/
        ////Debug.Log("Loaded jump count = " + gameData.jumpCount);
    }

    public void SaveGame()
    {
        //Return right away if the data persistence is disabled
        if (disableDataPersistence)
        {
            return;
        }

        //If we don't have any data to save, log a warning here
        if (this.gameData == null)
        {
            Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
            return;
            //NewGame();
        }

        //Pass the data to other scripts so that they can update it.
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(gameData);
        }

        //Timestamp the data so we know when it was last saved
        gameData.lastUpdated = System.DateTime.Now.ToBinary();

        //Update the current scene in our data
        Scene scene = SceneManager.GetActiveScene();

        //Don't save the scene for specific scenes, such as the Main Menu scene.
        if (!scene.name.Equals("MainMenu"))
        {
            gameData.currentSceneName = scene.name;
        }

        //Save that data to a file using the data handler
        dataHandler.Save(gameData, selectedProfileID);

    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        //FindObjectsofType takes in an optional boolean to include inactive gameobjects
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true)
            .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public bool HasGameData()
    {
        return gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData()
    {
        return dataHandler.LoadAllProfiles();
    }

    private IEnumerator AutoSave()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoSaveTimeSeconds);

            SaveGame();
            Debug.Log("Auto-Saved Game");
        }
    }
}
