using Cinemachine;
using System;
using UnityEngine;
using TMPro;

public class P2_PlayerCharacterBase : MonoBehaviour
{
    #region Variables

    [Header("References")]
    private Guid characterID;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private P2_fpsMovement moveScript;
    [SerializeField] private P2_GunplayManager gunScript;
    [SerializeField] private GameObject playerCanvas;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private CinemachineVirtualCamera characterCam;
    [SerializeField] private string characterIDString;
    [SerializeField] private P2_CameraID cameraID;
    [SerializeField] public string characterName;

    [Header("Pushing Objects")]
    [SerializeField] private LayerMask pushLayerMask;
    [SerializeField] public bool canPush;
    [SerializeField, Range(0.5f, 5f)] private float pushStrength = 1.1f;
    [SerializeField] private float pushHeightRestriction = -0.3f;

    [Header("U.I. Elements")]
    [SerializeField] private P2_Healthbar hudHealthBar;
    [SerializeField] public P2_Healthbar otherHealthBar;
    private bool updateHealth = false;

    [Space(10)]

    [Header("Sound Effects")]
    [SerializeField] private AudioClip playerHitSFX; 
    [SerializeField] private AudioClip playerDeathSFX;

    [Space(10)]

    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth;
    [SerializeField] private bool isAlive;

    [SerializeField] public bool isCharacterActive = false;
    [SerializeField] private bool characterGunEnabled;

    public bool canHack = false;

    private static Guid GenerateID()
    {
        return Guid.NewGuid();
    }

    public Guid GetCharacterID()
    {
        return characterID;
    }

    public string GetCharacterIDString()
    {
        return characterIDString;
    }

    public bool CharacterGunStatus()
    {
        return characterGunEnabled = gunScript.gameObject.activeSelf;
    }

    #endregion

    private void Awake()
    {
        characterID = GenerateID();
        characterIDString = characterID.ToString();

        isAlive = true;
        currentHealth = maxHealth;

        otherHealthBar.gameObject.SetActive(!isCharacterActive);
    }

    void Start()
    {
        cameraID.SetCameraID(characterIDString);
        moveScript.SetIDString(characterIDString);
        hudHealthBar.SetMaxHealth(maxHealth);

        P2_GameManager.Instance.PlayerHit += OnCharacterHit;

        characterNameText.text = characterName;
    }

    #region Game Functions

    private void OnCharacterHit(string characterID, int damageAmount)
    {
        #region Debug

        if (P2_GameManager.Instance.enableDebug)
        {
            Debug.Log("Player has been hit for " + damageAmount.ToString() + " damage!");
            Debug.Log("Player has " + currentHealth.ToString() + " currentHealth remaining.");
        }

        #endregion

        if (!isAlive || characterID != characterIDString)
        {
            return;
        }

        SoundManager.instance.PlaySFX(playerHitSFX);

        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            OnCharacterDeath(characterID);
        }

        UpdateHealth();
    }

    public void UpdateHealth()
    {
        if (!isCharacterActive)
        {
            return;
        }

        updateHealth = true;
    }

    private void OnCharacterDeath(string getGuid)
    {
        if (getGuid != characterIDString || getGuid == null)
        {
            return;
        }

        P2_GameManager.Instance.OnPlayerCharacterKilled(getGuid);
        //P2_GameManager.Instance.OnCharacterChanged();

        SoundManager.instance.PlaySFX(playerDeathSFX);

        isAlive = false;
        gameObject.SetActive(false);
    }

    #endregion

    #region Pushing Functions

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (canPush)
        {
            OnRigidbodyPush(hit);
        }
    }

    private void OnRigidbodyPush(ControllerColliderHit hit)
    {
        //Check if rigidbody has kinematic enabled or rigidbody is
        //not equal to null
        Rigidbody rb = hit.collider.attachedRigidbody;

        if (rb == null || rb.isKinematic)
        {
            return;
        }

        //Check the layermasks are correct
        var rbLayerMask = 1 << rb.gameObject.layer;
        if ((rbLayerMask & pushLayerMask.value) == 0)
        {
            return;
        }

        //Check to make sure there's no objects below the character
        if (hit.moveDirection.y < pushHeightRestriction)
        {
            return;
        }

        //Calculate the push direction from the move direction, 
        //appling horizontal motion only
        Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);

        //Apply the push force, taking strength into account
        rb.AddForce(pushDirection * pushStrength, ForceMode.Impulse);
    }

    #endregion

    void Update()
    {
        if (isCharacterActive)
        {
            hudHealthBar.SetMaxHealth(maxHealth);
            hudHealthBar.SetCurrentHealth(currentHealth);

            if (updateHealth)
            {
                hudHealthBar.SetMaxHealth(maxHealth);
                hudHealthBar.SetCurrentHealth(currentHealth);
            }
        }
        else
        {
            updateHealth = false;
            otherHealthBar.transform.LookAt(mainCamera.transform);
            otherHealthBar.transform.localRotation *= Quaternion.Euler(0, -180, 0);

            characterNameText.transform.LookAt(mainCamera.transform);
            characterNameText.transform.localRotation *= Quaternion.Euler(0, -180, 0);
        }

        playerCanvas.SetActive(!isCharacterActive);
        otherHealthBar.gameObject.SetActive(!isCharacterActive);
        otherHealthBar.SetMaxHealth(maxHealth);
        otherHealthBar.SetCurrentHealth(currentHealth);
    }
}