using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence
{
    //This script defines the interface that other scripts can use 
    //in order to save and load data. 
    
    //All scripts using this interface will require the following functions

    void LoadData(GameData data);

    void SaveData(GameData data);
}
