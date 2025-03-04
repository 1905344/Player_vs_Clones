using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.ComponentModel.Design;
using UnityEngine.Rendering;

public class P2_PlayerDetector : MonoBehaviour
{
    #region Variables

    [Header("Detection Settings")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private List<GameObject> playerCharacters = new(3);
    [SerializeField] private LayerMask detectionMask;
    [SerializeField] private float detectionHeight = 2f;
    private SphereCollider _sphereCollider;

    [Space(5)]

    [Header("U.I. References")]
    [SerializeField] private GameObject distortionScreen;
    [SerializeField] private GameObject warningScreen;
    [SerializeField] private TextMeshProUGUI warningText;

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

    #endregion

    private void Start()
    {
        P2_GameManager.Instance.changePlayerCharacter += UpdateActiveCharacter;

        _sphereCollider = GetComponent<SphereCollider>();
        _sphereCollider.isTrigger = true;
        _sphereCollider.radius = detectionRadius;

        gizmosRadius = _sphereCollider.radius;
        activePlayer = playerCharacters[0];
    }

    #region Debug: Show Gizmos

    private void OnDrawGizmosSelected()
    {
        //Visualising the radius of the sphere collider

        if (showDetectionRadius)
        {
            Gizmos.color = activePlayer ? detectedColour : undetectedColour;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
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

    private void Update()
    {
        Vector3 originPoint = transform.position + Vector3.up * detectionHeight;
        RaycastHit hit;

        Vector3 directionToTarget = activePlayer.transform.position - originPoint;
        float maxDistance = Vector3.Distance(activePlayer.transform.position, originPoint);
        float distanceIncreasing = detectionRadius - maxDistance;
        Physics.Raycast(originPoint, directionToTarget.normalized, out hit, maxDistance * 2, detectionMask);

        float defaultPlayerMoveSpeed = 12f;
        float defaultPlayerSprintSpeed = 28f;
        float slowPlayerMoveSpeed = activePlayer.GetComponent<P2_fpsMovement>().moveSpeed / 4;

        #region Can probably be commented out

        if (hit.collider != null && hit.collider.gameObject == activePlayer)
        {
            distortionScreen.gameObject.SetActive(false);

            activePlayer.GetComponent<P2_fpsMovement>().moveSpeed = defaultPlayerMoveSpeed;
            activePlayer.GetComponent<P2_fpsMovement>().sprintSpeed = defaultPlayerSprintSpeed;
        }
        else if (hit.collider != null && hit.collider.gameObject != activePlayer)
        {
            distortionScreen.gameObject.SetActive(true);

            if (warningScreen.activeSelf)
            {
                warningScreen.gameObject.SetActive(false);
            }

            if (distanceIncreasing > 0)
            {
                slowPlayerMoveSpeed -= Time.deltaTime;

                if (slowPlayerMoveSpeed < 0)
                {
                    slowPlayerMoveSpeed = 1f;
                }
            }

            activePlayer.GetComponent<P2_fpsMovement>().moveSpeed = slowPlayerMoveSpeed;
            activePlayer.GetComponent<P2_fpsMovement>().sprintSpeed = 0;
        }

        #endregion

        foreach (GameObject character in playerCharacters)
        {
            #region This code can probably be commented out

            Vector3 _directionToTarget = character.transform.position - originPoint;
            float _maxDistance = Vector3.Distance(character.transform.position, originPoint);
            Physics.Raycast(originPoint, _directionToTarget.normalized, out hit, _maxDistance * 2, detectionMask);

            if (hit.collider != null)
            {
                Debug.Log($"Raycast hit. {hit.transform.tag}");
            }

            if (hit.collider != null && hit.collider.gameObject == character)
            {
                #region Debug

                if (P2_GameManager.Instance.enableDebug)
                {
                    if (showLineOfSight && this.enabled)
                    {
                        Debug.DrawLine(transform.position + Vector3.up * detectionHeight, character.transform.position, detectedColour);
                    }
                }

                #endregion

                activePlayer.GetComponent<P2_PlayerCharacterBase>().canPush = true;
                warningScreen.gameObject.SetActive(false);
            }
            else if (hit.collider != null && hit.collider.gameObject != character)
            {
                string name = character.GetComponent<P2_PlayerCharacterBase>().characterName;

                activePlayer.GetComponent<P2_PlayerCharacterBase>().canPush = false;
                warningText.text = $"{name} is too far from heist";
                warningScreen.gameObject.SetActive(true);
            }

            #endregion

            //This code still needs to be tested
            //if (hit.collider != null && hit.collider.gameObject == character && hit.collider.gameObject == activePlayer)
            //{
            //    activePlayer.GetComponent<P2_PlayerCharacterBase>().canPush = true;
            //    warningScreen.gameObject.SetActive(false);

            //    distortionScreen.gameObject.SetActive(false);

            //    activePlayer.GetComponent<P2_fpsMovement>().moveSpeed = defaultPlayerMoveSpeed;
            //    activePlayer.GetComponent<P2_fpsMovement>().sprintSpeed = defaultPlayerSprintSpeed;
            //}
            //else if (hit.collider != null && (hit.collider.gameObject != character || (hit.collider.gameObject != activePlayer)))
            //{
            //    distortionScreen.gameObject.SetActive(true);

            //    if (warningScreen.activeSelf)
            //    {
            //        warningScreen.gameObject.SetActive(false);
            //    }

            //    if (distanceIncreasing > 0)
            //    {
            //        slowPlayerMoveSpeed -= Time.deltaTime;

            //        if (slowPlayerMoveSpeed < 0)
            //        {
            //            slowPlayerMoveSpeed = 1f;
            //        }
            //    }

            //    activePlayer.GetComponent<P2_fpsMovement>().moveSpeed = slowPlayerMoveSpeed;
            //    activePlayer.GetComponent<P2_fpsMovement>().sprintSpeed = 0;
            //}
        }
    }
}
