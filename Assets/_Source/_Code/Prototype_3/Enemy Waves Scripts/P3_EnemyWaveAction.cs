using System;
using UnityEngine;

[System.Serializable]
public class P3_EnemyWaveAction
{
    /// <summary>
    /// Code sourced from the following link:
    /// https://discussions.unity.com/t/enemy-wave-generator-spawn-system-help/108925/2
    /// </summary>

    public string waveActionName;
    public float waveDelay;
    public Transform enemyPrefab;
    public float enemySpawnCount;
    public string Message;

    public P3_EnemyWaveAction(string w_ActionName, float w_Delay, Transform w_EnemyPrefab, float w_EnemySpawnCount, string w_Message)
    {
        this.waveActionName = w_ActionName;
        this.waveDelay = w_Delay;
        this.enemyPrefab = w_EnemyPrefab;
        this.enemySpawnCount = w_EnemySpawnCount;
        Message = w_Message;
    }
}
