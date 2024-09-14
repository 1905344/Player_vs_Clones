using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedDoorTrigger : MonoBehaviour
{
    #region Variables

    private BoxCollider redDoorCollider;

    [SerializeField] private GameObject levelCompletedScreen;

    #endregion

    private void Awake()
    {
        redDoorCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            InputManager.Instance.levelComplete = true;
            InputManager.Instance.DisableGameInput();
            GameManager.Instance.OnLevelCompleted();
            
            levelCompletedScreen.SetActive(true);
            this.gameObject.SetActive(false);
        }
        else
        {
            return;
        }
    }
}
