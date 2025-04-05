using UnityEngine;

public class P2_FaceCamera : MonoBehaviour
{
    #region Variables

    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject healthBar;

    #endregion
    
    void Update()
    {
        healthBar.gameObject.transform.LookAt(mainCamera.transform);
        healthBar.gameObject.transform.localRotation *= Quaternion.Euler(0, -180, 0);
    }
}
