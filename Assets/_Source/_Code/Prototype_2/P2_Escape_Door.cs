using UnityEngine;

[RequireComponent (typeof(BoxCollider))]
public class P2_Escape_Door : MonoBehaviour
{
    #region Variables

    [SerializeField] private BoxCollider exitDoorCollider;
    private bool isCubeRequired = true;

    #endregion

    private void Start()
    {
        P2_GameManager.Instance.playerCharacterKilled += UpdateTrigger;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCubeRequired)
        {
            if (other.gameObject.name == "Heist Object Trigger")
            {
                P2_GameManager.Instance.OnLevelCompleted();
            }
        }
        else
        {
            if (other.CompareTag("Player"))
            {
                P2_GameManager.Instance.OnLevelCompleted();
            }
        }
    }

    private void UpdateTrigger(string character)
    {
        isCubeRequired = false;
    }
}
