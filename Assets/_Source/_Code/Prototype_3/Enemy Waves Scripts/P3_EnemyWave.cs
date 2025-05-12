using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class P3_EnemyWave
{
    /// <summary>
    /// Code sourced from the following link:
    /// https://discussions.unity.com/t/enemy-wave-generator-spawn-system-help/108925/2
    /// </summary>

    public string enemyWaveName;
    public List<P3_EnemyWaveAction> waveActions;

    public P3_EnemyWave (string w_Name, P3_EnemyWaveAction action)
    {
        this.enemyWaveName = w_Name;
        this.waveActions = new List<P3_EnemyWaveAction>();
        waveActions.Add(action);
    }
}
