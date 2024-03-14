using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
//using Newtonsoft.Json;    <= Uncomment this line if using the Newtonsoft Json plugin
public class FileDataHandler
{
    //This script manages the actual save file, including encrypting and decrypting it for security
    //The save data is encrypted using XOR encryption, so it is imperative to keep the code word secret
    //because it is used to decrypt the save data

    //Use the (free) Newtonsoft Json plugin for saving any data that isn't a variable

    private string dataDirPath = "";
    private string dataFileName = "";
    private bool useEncryption = false;
    private readonly string encryptionCodeWord = "";
    private readonly string backupExtension = ".bak";


    public FileDataHandler(string dataDirPath, string dataFileName, bool useEncryption )
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
        this.useEncryption = useEncryption;
    }

    public GameData Load(string profileID, bool allowRestoreFromBackup = true)
    {
        //Base case - if the profileID is null, return right away
        if(profileID == null)
        {
            return null;
        }
        
        //use Path.Combine to account for different OS's having different path separators.
        string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);
        GameData loadedData = null;

        if (File.Exists(fullPath))
        {
            try
            {
                //Load the serialised data from the file
                string dataToLoad = "";
                using (FileStream stream = new FileStream (fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //Optionally decrypting the data for loading
                if (useEncryption)
                {
                    dataToLoad = EncryptDecrypt(dataToLoad);
                }


                //Deserialise the data from JSON back to the C# object
                //loadedData = JsonUtility.FromJson<GameData>(dataToLoad);

                //Uncomment this line if using the Newtonsoft Json plugin
                //loadedData = JsonConvert.DeserializeObject<GameData>(dataToLoad);

            }
            catch (Exception e)
            {
                //Since Load(...) is being called recursively, we need to account for the case where
                //the rollback succeeds, but the data is still failing to load for another reason,
                //which without this check, an infinite recursion loop may occur.
                if (allowRestoreFromBackup)
                {
                    Debug.LogWarning("Failed to load the data file. Attempting to roll back.\n" + e);
                    bool rollbackSuccess = AttemptRollback(fullPath);
                    if (rollbackSuccess)
                    {
                        //Try to load the data again recursively
                        loadedData = Load(profileID, false);
                    }
                }
                //If we hit this else block, one possibility is that the backup file is also corrupt
                else
                {
                    Debug.LogError("Error occurred when trying to load the file at path: "
                        + fullPath + " and backup did not work.\n" + e);
                }
            }
        }
        return loadedData;
    }

    public void Save(GameData data, string profileID)
    {
        //Base case - if the profileID is null, return right away
        if (profileID == null)
        {
            return;
        }
        
        //use Path.Combine to account for different OS's having different path separators.
        string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);
        string backupFilePath = fullPath + backupExtension;

        try
        {
            //create the directory that the save file will be written to if it doesn't exist already.
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //serialise the C# game data object into a JSON file
            string dataToStore = JsonUtility.ToJson(data, true);

            //Uncomment this line if using the Newtonsoft Json plugin
            //string dataToStore = JsonConvert.SerializeObject(data, Formatting.Indented);

            //optionally encrypting the data for saving
            if (useEncryption)
            {
                dataToStore = EncryptDecrypt(dataToStore);
            }


            //write the serialised data to the file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }

            //Verify that the newly saved file can be loaded successfully
            GameData verifiedGameData = Load(profileID);

            //if the data can be verified, back it up
            if (verifiedGameData != null)
            {
                File.Copy(fullPath, backupFilePath, true);
            }

            //Otherwise, if something went wrong, then we should throw an exception
            else
            {
                throw new Exception("Save file could not be verified and backup could not be created");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occurred when trying to save data to file: " + fullPath + "\n" + e);
        }

    }

    public void Delete(string profileID)
    {
        //Base case - if the profileID is null, return right away
        if (profileID == null)
        {
            return;
        }

        string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);
        try
        {
            //Ensure the data file exists at this path before deleting the directory
            if (File.Exists(fullPath))
            {
                //Delete the profile folder and everything within it
                Directory.Delete(Path.GetDirectoryName(fullPath), true);
            }
            else
            {
                Debug.LogWarning("Tried to delete the profile data, but the data was not found at path: " + fullPath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to delete the profile data for profileID: "
                + profileID + " at path: " + fullPath + "\n" + e);
        }
    }

    public Dictionary<string, GameData> LoadAllProfiles()
    {
        Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();

        //Loop over all the dictionary names in the data directory path
        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
        foreach (DirectoryInfo dirInfo in dirInfos)
        {
            string profileID = dirInfo.Name;

            //Defensive programming - check to see if the data file exists
            //If it doesn't, then this folder isn't a profile and should be skipped
            string fullPath = Path.Combine(dataDirPath, profileID, dataFileName);
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning("Skipping directory when loading all the profiles because it does not contain data: "
                    + profileID);
                continue;
            }

            //Load the game data for this profileID and put it in the dictionary
            GameData profileData = Load(profileID);

            //Defensive programming - ensure that the profile data isn't null,
            //because if it is, then something went wrong and we should let ourselves know
            if (profileData != null)
            {
                profileDictionary.Add(profileID, profileData);
            }
            else
            {
                Debug.LogError("Tried to load the profile, but something went wrong. ProfileID: " + profileID);
            }
        }
        return profileDictionary;
    }

    public string GetMostRecentlyUpdatedProfileID()
    {
        string mostRecentProfileID = null;

        Dictionary<string, GameData> profilesGameData = LoadAllProfiles();
        foreach (KeyValuePair<string, GameData> pair in profilesGameData)
        {
            string profileID = pair.Key;
            GameData gameData = pair.Value;

            //Skip this entry if the gamedata is null
            if(gameData == null)
            {
                continue;
            }

            //If this is the first data we've come across that exists, it's the most recent so far
            if (mostRecentProfileID == null)
            {
                mostRecentProfileID = profileID;
            }
            //Otherwise, compare to see which date is the most recent
            else
            {
                DateTime mostRecentDateTime = DateTime.FromBinary(profilesGameData[mostRecentProfileID].lastUpdated);
                DateTime newDateTime = DateTime.FromBinary(gameData.lastUpdated);

                //The greatest DateTime value is the most recent
                if(newDateTime > mostRecentDateTime)
                {
                    mostRecentProfileID = profileID;
                }
            }
        }
        return mostRecentProfileID;
    }

    //This function is a simple implementation of using XOR encryption for the save file
    private string EncryptDecrypt(string data)
    {
        string modifiedData = "";
        for (int i = 0; i < data.Length; i++)
        {
            modifiedData += (char)(data[i] ^ encryptionCodeWord[i % encryptionCodeWord.Length]);
        }
        return modifiedData;
    }

    private bool AttemptRollback(string fullPath)
    {
        bool success = false;
        string backupFilePath = fullPath + backupExtension;

        try
        {
            //If the file exists, attempt to roll back to it by overwriting the original file
            if (File.Exists(backupFilePath))
            {
                File.Copy(backupFilePath, fullPath, true);
                success = true;
                Debug.LogWarning("Had to roll back to backup file at: " + backupFilePath);
            }

            //Otherwise, we don't have a backup file yet. So there's nothing to rollback to
            else
            {
                throw new Exception("Tried to roll back, but no backup file exists to roll back to.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occurred when trying to roll back to backup file at: "
                + backupFilePath + "\n" + e);
        }

        return success;
    }
}
