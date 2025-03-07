using UnityEngine;

[RequireComponent (typeof(BoxCollider))]
public class P2_Escape_Door : MonoBehaviour
{
    #region Variables

    private BoxCollider exitDoorCollider;

    #endregion

    private void Awake()
    {
        exitDoorCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            P2_GameManager.Instance.OnLevelCompleted();
        }
    }
}
