using UnityEngine;

public class P2_FreezeWaypoints : MonoBehaviour
{
    #region Variables

    private Vector3 position;
    private Vector3 rotation;

    #endregion

    private void Awake() 
    { 
        position = transform.position;
        rotation = transform.eulerAngles;
    }

    private void Update()
    {
        transform.position = position;
        transform.eulerAngles = rotation;
    }
}
