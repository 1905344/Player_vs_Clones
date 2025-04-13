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
}
