using System.Collections;
using System.Collections.Generic;
//using TMPro;
using UnityEngine;

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
    [SerializeField] private float waveDelayFactor;
    //[SerializeField, Tooltip("For printing messages about enemy waves")] private TMP_Text waveText;

    [SerializeField] private float difficultyFactor = 0.9f;
    [SerializeField] private List<P3_EnemyWave> enemyWaves;
    [SerializeField] private P3_EnemyWave currentEnemyWave;
    
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
                            radius = Random.Range(innerRadius, outerRadius);

                            //Get random angle in the circle, between 0 and 360 degrees.
                            angle = Random.Range(angleMinimum, angleMaximum);

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
                        }
                    }
                }

                //Preventing crash if all delays are set to 0
                yield return null;
            }

            waveDelayFactor *= difficultyFactor;

            //Preventing crash if all delays are set to 0
            yield return null;
        }
    }

    private void Start()
    {
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
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, innerRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, outerRadius);
    }
}
