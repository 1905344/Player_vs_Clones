using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Prototype_1_EnemyManager : MonoBehaviour
{
    #region Variables

    private static Prototype_1_EnemyManager instance;

    public static Prototype_1_EnemyManager Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField] private List<GameObject> enemies = new(7);

    [SerializeField, Tooltip("How long to delay loading the enemies by")] private float delayEnemySpawnTimer;
    private float timer = 0f;
    private bool startTimer = false;
    private bool loadEnemies = false;

    #endregion

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        foreach (GameObject enemy in enemies)
        {
            enemy.gameObject.SetActive(false);
        }

        Prototype_1_GameManager.Instance.OnStartGame += EnableAgents;
    }

    private void EnableAgents()
    {
        startTimer = true;
    }

    private void LoadNavAgents()
    {
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<NavMeshAgent>().Warp(enemy.transform.position);
            enemy.gameObject.SetActive(true);
        }

        timer = 0f;
        loadEnemies = false;
        this.enabled = false;
    }

    private void Update()
    {
        if (startTimer)
        {
            timer += Time.deltaTime;
        }

        if (timer > delayEnemySpawnTimer)
        {
            startTimer = false;
            loadEnemies = true;
            LoadNavAgents();
        }

    }
}
