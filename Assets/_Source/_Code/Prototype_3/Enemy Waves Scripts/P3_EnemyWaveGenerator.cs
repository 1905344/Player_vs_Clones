using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.AI;

public class P3_EnemyWaveGenerator : MonoBehaviour
{
    /// <summary>
    /// Code sourced from the following link:
    /// https://discussions.unity.com/t/enemy-wave-generator-spawn-system-help/108925/2
    /// </summary>

    #region Variables

    [Header("Radius Around Lighthouse and Level")]
    [SerializeField, Tooltip("Minimum radius for enemies to spawn outside")] private float innerRadius = 0f;
    [SerializeField, Tooltip("Maximum radius for enemies to spawn within")] private float outerRadius = 0f;

    [Space(5)]

    [Header("Spawn Position Variables")]
    [SerializeField] private float radius;
    [SerializeField] private float angle;
    [SerializeField] private float x;
    [SerializeField] private float z;

    [Space(5)]

    [SerializeField, Tooltip("Minimum angle around the centre of the level for an enemy to spawn at")] private float angleMinimum = 0.0f;
    [SerializeField, Tooltip("Maximum angle around the centre of the level for an enemy to spawn at\"")] private float angleMaximum = 360.0f;

    [Space(10)]
    
    [Header("Enemy Parent Transform Reference")]
    [SerializeField] private GameObject enemyParent;

    [Space(5)]

    [Header("Lighthouse and Player")]
    [SerializeField] private GameObject fpsPlayerCharacter;
    [SerializeField] private GameObject lighthouseGameObject;

    [Space(5)]

    [Header("Wave Variables")]
    private float waveDelayFactor;
    //[SerializeField, Tooltip("For printing messages about enemy waves")] private TMP_Text waveText;
    [SerializeField] private List<P3_EnemyWave> enemyWaves;
    [SerializeField] private List<P3_EnemyWave> defaultEnemyWaves;
    [SerializeField] private P3_EnemyWave currentEnemyWave;

    [Space(3)]

    [Header("Difficulty Settings")]
    [SerializeField] private float difficultyFactor = 0.9f;
    [SerializeField] private List<string> DifficultyStates = new List<string>();
    [SerializeField] private string currentDifficultyState = string.Empty;
    [SerializeField] private bool enableDifficulty = false;
    
    [Space(10)]

    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject blackEnemy;
    [SerializeField] private GameObject blueEnemy;
    [SerializeField] private GameObject greenEnemy;
    [SerializeField] private GameObject greyEnemy;
    [SerializeField] private GameObject redEnemy;
    [SerializeField] private GameObject whiteEnemy;
    [SerializeField] private GameObject yellowEnemy;

    [Space(10)]

    [SerializeField] private List<GameObject> enemyList;

    [Space(10)]

    [Header("U.I. Elements")]
    [SerializeField] private TMP_Dropdown difficultyDropDown;

    [Space(10)]

    [Header("Debug")]
    [SerializeField] private bool showInnerRadius = false;
    [SerializeField] private bool showOuterRadius = false;

    #endregion

    IEnumerator WaveSpawnLoop()
    {
        waveDelayFactor = 1.0f;

        while (true)
        {
            foreach (P3_EnemyWave wave in enemyWaves)
            {
                currentEnemyWave = wave;

                foreach (P3_EnemyWaveAction wAction in wave.waveActions)
                {
                    if (wAction.waveDelay > 0)
                    {
                        yield return new WaitForSeconds(wAction.waveDelay * waveDelayFactor);
                        //Debug.Log($"P3_EnemyWaveGenerator: delay = {wAction.waveDelay * waveDelayFactor}");
                    }

                    //Optional text priting
                    //if (wAction.Message != "")
                    //{
                    //    waveText.text = wAction.Message;
                    //}

                    if (wAction.enemyPrefab != null && wAction.enemySpawnCount > 0)
                    {
                        for (int i = 0; i < wAction.enemySpawnCount; i++)
                        {
                            //Get random radius between inner radius and outer radius 
                            //Distance using the inner and outer radii
                            radius = UnityEngine.Random.Range(innerRadius, outerRadius);

                            //Get random angle in the circle, between 0 and 360 degrees.
                            angle = UnityEngine.Random.Range(angleMinimum, angleMaximum);

                            //Get the X and Z values of the circle
                            //X = X coordinate of the centre of the circle + radius * cos(angle)
                            x = this.transform.position.x + radius * Mathf.Cos(angle);

                            //Z = Z coordinate of the centre of the circle + radius * sin(angle)
                            z = this.transform.position.z + radius * Mathf.Sin(angle);

                            //Instantiate enemy prefab/game object using the X and Z values
                            Vector3 enemySpawnPosition = new Vector3(x, 0f, z);

                            wAction.enemyPrefab = Instantiate(wAction.enemyPrefab, enemySpawnPosition, Quaternion.identity, enemyParent.transform);

                            wAction.enemyPrefab.GetComponent<P3_EnemyBase>().playerRef = fpsPlayerCharacter;
                            wAction.enemyPrefab.GetComponent<P3_EnemyBase>().lighthouseRef = lighthouseGameObject.transform;

                            enemyList.Add(wAction.enemyPrefab.gameObject);
                        }
                    }
                }

                //Preventing crash if all delays are set to 0
                yield return null;
            }

            if (enableDifficulty)
            {
                waveDelayFactor *= difficultyFactor;
            }

            //Preventing crash if all delays are set to 0
            yield return null;
        }
    }

    private void Start()
    {
        difficultyDropDown.AddOptions(DifficultyStates);
        difficultyDropDown.value = 0;

        P3_GameManager.Instance.OnStartGame += StartSpawning;
        P3_GameManager.Instance.PlayerKilled += StopSpawning;

        if (P3_GameManager.Instance.enableDebug && P3_GameManager.Instance.skipTutorial)
        {
            StartSpawning();
        }
    }

    private void StartSpawning()
    {
        StartCoroutine(WaveSpawnLoop());
    }

    private void StopSpawning()
    {
        StopCoroutine(WaveSpawnLoop());

        foreach (GameObject enemy in enemyList)
        {
            if (enemy != null)
            {
                enemy.GetComponent<NavMeshAgent>().enabled = false;
                Destroy(enemy);
            }
        }
    }

    #region Difficulty Settings

    public void SetDifficulty(int index)
    {
        switch (index)
        {
            case 0:
                {
                    currentDifficultyState = DifficultyStates[index];
                    enableDifficulty = false;
                    RemoveAllDifficultyWaves();
                    UpdateEnemyWaveList();

                    break;
                }
            case 1:
                {
                    currentDifficultyState = DifficultyStates[index];
                    enableDifficulty = true;
                    difficultyFactor = 3.5f;

                    RemoveAllDifficultyWaves();
                    UpdateEnemyWaveList();

                    AddNewWaveAction("Medium Default Increase", "Spawn 10 grey every 25 seconds", 25f, greyEnemy.transform, 10);
                    AddNewWaveAction("Medium - Blue", "Spawn 4 blue every 20 seconds", 20f, blueEnemy.transform, 4);
                    AddNewWaveAction("Medium - Yellow", "Spawn 4 yellow every 12 seconds", 12.5f, yellowEnemy.transform, 4);
                    AddNewWaveAction("Medium - Red", "Spawn 5 red every 30 seconds", 30f, redEnemy.transform, 5);
                    AddNewWaveAction("Medium - White", "Spawn 5 white every 45 seconds", 45f, whiteEnemy.transform, 5);

                    break;
                }
            case 2:
                {
                    currentDifficultyState = DifficultyStates[index];
                    enableDifficulty = true;
                    difficultyFactor = 2f;

                    RemoveAllDifficultyWaves();
                    UpdateEnemyWaveList();

                    AddNewWaveAction("Challenging Default Increase", "Spawn 10 grey every 20 seconds", 20f, greyEnemy.transform, 10);
                    AddNewWaveAction("Challenging - Blue", "Spawn 6 blue every 18 seconds", 18f, blueEnemy.transform, 6);
                    AddNewWaveAction("Challenging - Yellow", "Spawn 6 yellow every 11 seconds", 11f, yellowEnemy.transform, 6);
                    AddNewWaveAction("Challenging - Red", "Spawn 5 red every 30 seconds", 30f, redEnemy.transform, 5);
                    AddNewWaveAction("Challenging - White", "Spawn 5 white every 45 seconds", 45f, whiteEnemy.transform, 5);

                    break;
                }
            case 3:
                {
                    currentDifficultyState = DifficultyStates[index];
                    enableDifficulty = true;
                    difficultyFactor = 1.5f;

                    RemoveAllDifficultyWaves();
                    UpdateEnemyWaveList();

                    AddNewWaveAction("Hard Default Increase", "Spawn 10 grey every 15 seconds", 15f, greyEnemy.transform, 10);
                    AddNewWaveAction("Hard - Blue", "Spawn 7 blue every 15 seconds", 15f, blueEnemy.transform, 7);
                    AddNewWaveAction("Hard - Yellow", "Spawn 7 yellow every 10 seconds", 10f, yellowEnemy.transform, 7);
                    AddNewWaveAction("Hard - Red", "Spawn 5 red every 30 seconds", 30f, redEnemy.transform, 5);
                    AddNewWaveAction("Hard - White", "Spawn 5 white every 45 seconds", 45f, whiteEnemy.transform, 5);

                    break;
                }
            case 4:
                {
                    currentDifficultyState = DifficultyStates[index];
                    enableDifficulty = true;
                    difficultyFactor = 0.75f;

                    RemoveAllDifficultyWaves();
                    UpdateEnemyWaveList();

                    AddNewWaveAction("Extreme Default Increase", "Spawn 15 grey every 10 seconds", 10f, greyEnemy.transform, 10);
                    AddNewWaveAction("Extreme - Blue", "Spawn 9 blue every 12 seconds", 12f, blueEnemy.transform, 9);
                    AddNewWaveAction("Extreme - Yellow", "Spawn 9 yellow every 10 seconds", 12.5f, yellowEnemy.transform, 9);
                    AddNewWaveAction("Extreme - White", "Spawn 7 white every 25 seconds", 25f, whiteEnemy.transform, 7);
                    AddNewWaveAction("Extreme - Red", "Spawn 5 red every 20 seconds", 20f, redEnemy.transform, 5);

                    break;
                }
        }
    }

    private void AddNewWaveAction(string waveName, string waveActionName, float delayAmount, Transform enemyPrefab, int spawnCount)
    {
        //if the difficulty level is changed, then add a new wave action to the spawner
        P3_EnemyWaveAction difficultyEnemyWaveAction = new P3_EnemyWaveAction(waveActionName, delayAmount, enemyPrefab.transform, spawnCount, string.Empty);
        P3_EnemyWave newWave = new P3_EnemyWave(waveName, difficultyEnemyWaveAction);
        enemyWaves.Add(newWave);
    }

    private void RemoveAllDifficultyWaves()
    {
        enemyWaves.Clear();

        //List<string> wavesToRemove = new List<string>
        //{
        //    "Medium Default Increase",
        //    "Medium - Blue",
        //    "Medium - Yellow",
        //    "Medium - Red",
        //    "Medium - White",
        //    "Challenging Default Increase",
        //    "Challenging - Blue",
        //    "Challenging - Yellow",
        //    "Challenging - Red",
        //    "Challenging - White",
        //    "Hard Default Increase",
        //    "Hard - Blue",
        //    "Hard - Yellow",
        //    "Hard - Red",
        //    "Hard - White",
        //    "Extreme Default Increase",
        //    "Extreme - Blue",
        //    "Extreme - Yellow",
        //    "Extreme - White",
        //    "Extreme - Red"
        //};

        //foreach (P3_EnemyWave checkWave in difficultEnemyWaves)
        //{
        //    foreach (string s in wavesToRemove)
        //    {
        //        if (checkWave.enemyWaveName.Contains(s))
        //        {
        //            difficultEnemyWaves.Remove(checkWave);
        //        }
        //    }
        //}

        //enemyWaves = difficultEnemyWaves;
    }

    private void UpdateEnemyWaveList()
    {
        foreach (P3_EnemyWave defaultWave in defaultEnemyWaves)
        {
            enemyWaves.Add(defaultWave);
        }
    }

    #endregion

    #region Debug: Show Gizmos

    private void OnDrawGizmos()
    {
        if (showInnerRadius)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, innerRadius);
        }
        
        if (showOuterRadius)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, outerRadius);
        }
    }

    #endregion
}
