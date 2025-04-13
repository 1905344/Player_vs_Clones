using UnityEngine;

public class P3_CameraManager : MonoBehaviour
{
    #region Variables

    private static P3_CameraManager instance;

    public static P3_CameraManager Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField] private GameObject player1_Camera;
    [SerializeField] private GameObject player2_Camera;

    #endregion

    void Start()
    {
        P3_GameManager.Instance.changePlayerCharacter += OnCameraChanged;
    }

    private void OnCameraChanged()
    {
        if (player1_Camera.activeInHierarchy)
        {
            player1_Camera.SetActive(false);
            player2_Camera.SetActive(true);
        }
        else if (player2_Camera.activeInHierarchy)
        {
            player2_Camera.SetActive(false);
            player1_Camera.SetActive(true);
        }
    }
}
