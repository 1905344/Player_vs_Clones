using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class P2_PlayerDetector : MonoBehaviour
{
    #region Variables

    [Header("Detection Settings")]
    private float radius;
    [SerializeField] public float detectionRadius;
    [SerializeField] public float extendedDetectionRadius;
    [SerializeField] private List<GameObject> playerCharacters = new(3);
    [SerializeField] private LayerMask detectionMask;
    [SerializeField] private float detectionHeight = 2f;
    [SerializeField] private SphereCollider _sphereCollider;
    private Vector3 centrePostion;

    [Space(5)]

    [Header("U.I. References")]
    [SerializeField] private GameObject warningScreen;
    [SerializeField] private TMP_Text warningText;

    [Space(10)]

    [Header("Debug: Show Gizmos")]
    [SerializeField] private bool showDetectionRadius = false;
    [SerializeField] private bool showLineOfSight = false;

    [Space(3)]

    private float gizmosRadius = 1f;
    [SerializeField] private Color32 detectionRadiusColour;
    [SerializeField] private Color32 detectedColour;
    [SerializeField] private Color32 undetectedColour;

    [SerializeField] private GameObject activePlayer;

    [SerializeField] private int indexPos;

    [SerializeField] private List<GameObject> playersInRange = new(3);
    [SerializeField] private List<GameObject> playersOutOfRange = new(3);
    
    #endregion

    private void Awake()
    {
        radius = detectionRadius;
        _sphereCollider.radius = radius;
        gizmosRadius = radius;
    }

    private void Start()
    {
        P2_GameManager.Instance.changePlayerCharacter += UpdateActiveCharacter;
        P2_GameManager.Instance.playerCharacterKilled += RemoveCharacter;

        activePlayer = playerCharacters[0];
    }

    #region Debug: Show Gizmos

    private void OnDrawGizmosSelected()
    {
        //Visualising the radius of the sphere collider

        if (showDetectionRadius)
        {
            Gizmos.color = activePlayer ? detectedColour : undetectedColour;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        if (showLineOfSight)
        {
            Gizmos.color = detectionRadiusColour;
            Gizmos.DrawWireSphere(transform.position + Vector3.up * detectionHeight, 0.3f);
        }
    }

    #endregion

    private void UpdateActiveCharacter()
    {
        indexPos++;

        indexPos %= playerCharacters.Count;
        activePlayer = playerCharacters[indexPos];
    }

    private void RemoveCharacter(string id)
    {
        if (warningScreen.activeInHierarchy)
        {
            warningScreen.SetActive(false);
        }

        this.enabled = false;

        for (int i = 0; i < playerCharacters.Count; i++)
        {
            string characterID = playerCharacters[i].gameObject.GetComponent<P2_PlayerCharacterBase>().GetCharacterIDString();
            GameObject characterToRemove = playerCharacters[i].gameObject;

            if (id == characterID)
            {
               playerCharacters.RemoveAt(i);
               
               if (playersInRange.Contains(characterToRemove))
               {
                    playersInRange.Remove(characterToRemove);
               }

               if (playersOutOfRange.Contains(characterToRemove)) 
               { 
                    playersOutOfRange.Remove(characterToRemove);
               }
            }
            else
            {
                return;
            }
        }
    }

    #region OnTriggerStay and OnTriggerExit

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!playersInRange.Contains(other.gameObject))
            {
                playersInRange.Add(other.gameObject);
            }

            if (playersOutOfRange.Contains(other.gameObject))
            {
                playersOutOfRange.Remove(other.gameObject);
            }

            if (playersOutOfRange.Count == 0)
            {
                warningScreen.gameObject.SetActive(false);
            }

            //Allow the player from switching characters
            P2_InputManager.Instance.canChangeCharacter = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        P2_PlayerCharacterBase playerCharacterBase = null;

        if (other.gameObject.TryGetComponent<P2_PlayerCharacterBase>(out playerCharacterBase))
        {
            if (!playersOutOfRange.Contains(other.gameObject))
            {
                playersOutOfRange.Add(other.gameObject);
            }

            if (playersInRange.Contains(other.gameObject))
            {
                playersInRange.Remove(other.gameObject);
            }

            if (playersOutOfRange.Count > 0)
            {
                warningScreen.gameObject.SetActive(true);
            }

            //Prevent the player from switching characters
            //P2_InputManager.Instance.canChangeCharacter = false;
        }
    }

    #endregion

    private void Update()
    {
        #region Debug

        //Debug.Log($"Players in range: {playersInRange.Count}");
        //Debug.Log($"Players out of range: {playersOutOfRange.Count}");

        if (P2_GameManager.Instance.enableDebug)
        {
            _sphereCollider.radius = detectionRadius;
        }

        #endregion

        #region Extending Detection Radius For M Character

        if (indexPos == 2)
        {
            radius = extendedDetectionRadius;
            
            #region Debug

            if (P2_GameManager.Instance.enableDebug)
            {
                Debug.Log($"P2_PlayerDetector: Player character 3 is active. Radius = {radius}");
            }

            #endregion
        }
        else
        {
            radius = detectionRadius;
        }

        #endregion

        #region Apply Changes to the Active Player

        Vector3 originPoint = transform.position + Vector3.up * detectionHeight;
        float maxDistance = Vector3.Distance(activePlayer.transform.position, originPoint);
        float distanceIncreasing = maxDistance - detectionRadius;

        float defaultPlayerMoveSpeed = 12f;
        float defaultPlayerSprintSpeed = 28f;
        float slowPlayerMoveSpeed = 8f;

        //Apply changes to the active player only
        if (playersInRange.Contains(activePlayer.gameObject))
        {
            if (indexPos == 2)
            {
                activePlayer.GetComponent<P2_PlayerCharacterBase>().canHack = true;
            }

            activePlayer.GetComponent<P2_fpsMovement>().moveSpeed = defaultPlayerMoveSpeed;
            activePlayer.GetComponent<P2_fpsMovement>().sprintSpeed = defaultPlayerSprintSpeed;

            //if (distanceIncreasing < 0)
            //{
            //    slowPlayerMoveSpeed -= Time.deltaTime;

            //    if (slowPlayerMoveSpeed <= 2f)
            //    {
            //        slowPlayerMoveSpeed = 2.15f;
            //    }
            //}
            //else
            //{
            //    slowPlayerMoveSpeed += Time.deltaTime;

            //    if (slowPlayerMoveSpeed >= defaultPlayerMoveSpeed)
            //    {
            //        slowPlayerMoveSpeed = 0f;
            //        activePlayer.GetComponent<P2_fpsMovement>().moveSpeed = defaultPlayerMoveSpeed;
            //    }
            //}

            //activePlayer.GetComponent<P2_fpsMovement>().moveSpeed = slowPlayerMoveSpeed;
            //activePlayer.GetComponent<P2_fpsMovement>().sprintSpeed = 0;
        }
        else if (playersOutOfRange.Contains(activePlayer.gameObject))
        {
            if (indexPos == 2)
            {
                activePlayer.GetComponent<P2_PlayerCharacterBase>().canHack = false;
            }

            slowPlayerMoveSpeed -= Time.deltaTime;

            if (slowPlayerMoveSpeed <= 2f)
            {
                slowPlayerMoveSpeed = 2.15f;
            }

            activePlayer.GetComponent<P2_fpsMovement>().moveSpeed = slowPlayerMoveSpeed;
            activePlayer.GetComponent<P2_fpsMovement>().sprintSpeed = 0;
        }

        #endregion

        #region Applying or Revoking Changes To All Characters

        foreach (GameObject character in playersInRange)
        {
            //Apply changes to all characters within range
            
        }

        warningText.text = string.Empty;

        foreach (GameObject character in playersOutOfRange)
        {
            //Apply changes to all characters out of range
            P2_PlayerCharacterBase playerScript = character.GetComponent<P2_PlayerCharacterBase>();

            warningText.text += $"Player {playerScript.characterName} is too far from the gold cube!\n"/* + "\nPlease return to the gold cube."*/;

            //Prevent the player from switching characters
            //P2_InputManager.Instance.canChangeCharacter = false;
        }

        #endregion
    }
}
