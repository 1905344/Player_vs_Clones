using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class P2_EnemyManager : MonoBehaviour
{
    #region Variables

    private static P2_EnemyManager instance;

    public static P2_EnemyManager Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField] private List<GameObject> enemies = new(7);

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

        P2_GameManager.Instance.OnStartGame += EnableAgents;
    }

    private void EnableAgents()
    {
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<NavMeshAgent>().Warp(enemy.transform.position);
            enemy.gameObject.SetActive(true);
        }
    }
}