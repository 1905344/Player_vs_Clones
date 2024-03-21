using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    //This script defines the dictionary that will store the save data, 
    //and manages the serialization and deserialization of the save data.

    //The dictionary will be serialized into JSON format, which requires the all data saved are variables

    //Keys are the unique IDs representing each of the variables being saved
    [SerializeField] private List<TKey> keys = new List<TKey>();

    //Values are the values of the variables being saved    
    [SerializeField] private List<TValue> values = new List<TValue>();

    // Save the dictionary to the lists
   public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    //Load the dictionary from the lists
    public void OnAfterDeserialize()
    {
        this.Clear();

        if (keys.Count != values.Count)
        {
            Debug.LogError("Attempted to deserialize a SerializableDictionary, but the amount of keys ("
                + keys.Count + ") does not match the number of values (" + values.Count
                + ") which indicates something went wrong.");
        }
        
        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }
}
