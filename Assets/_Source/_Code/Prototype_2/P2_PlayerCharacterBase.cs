using Cinemachine;
using System;
using UnityEngine;
using TMPro;

public class P2_PlayerCharacterBase : MonoBehaviour
{
    #region Variables

    [Header("References")]
    private Guid characterID;
    [SerializeField] private P2_fpsMovement moveScript;
    [SerializeField] private P2_GunplayManager gunScript;
    [SerializeField] private string characterIDString;
    [SerializeField] private CinemachineVirtualCamera characterCam;
    [SerializeField] private P2_CameraID cameraID;
    [SerializeField] public string characterName;

    [Header("Pushing Objects")]
    [SerializeField] private LayerMask pushLayerMask;
    [SerializeField] public bool canPush;
    [SerializeField, Range(0.5f, 5f)] private float pushStrength = 1.1f;
    [SerializeField] private float pushHeightRestriction = -0.3f;

    [Header("U.I. Elements")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Color32 healthTextColour;
    [SerializeField] private Color32 maxHealthColour;
    [SerializeField] private Color32 upperMiddleHealthColour;
    [SerializeField] private Color32 middleHealthColour;
    [SerializeField] private Color32 lowHealthColour;
    private bool updateHealthTextColour;
    private bool updateHealthText = false;

    [Space(10)]

    [SerializeField] private int currentHealth;
    [SerializeField] private int maxHealth;
    [SerializeField] private bool isAlive;

    [SerializeField] public bool isCharacterActive = false;
    [SerializeField] private bool characterGunEnabled;

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

        moveScript = GetComponent<P2_fpsMovement>();
        characterCam = GetComponent<CinemachineVirtualCamera>();
        
        isAlive = true;
        healthTextColour = maxHealthColour;
        currentHealth = maxHealth;
    }

    void Start()
    {
        cameraID.SetCameraID(characterIDString);
        moveScript.SetIDString(characterIDString);

        P2_GameManager.Instance.PlayerHit += OnCharacterHit;
        //P2_GameManager.Instance.PlayerHit -= OnCharacterHit;

        healthText.text = currentHealth.ToString() + "/" + maxHealth;
        healthText.color = healthTextColour;
        updateHealthTextColour = true;
    }

    #region Game Functions

    private void OnCharacterHit(Guid characterID, int damageAmount)
    {
        #region Debug

        if (P2_GameManager.Instance.enableDebug)
        {
            Debug.Log("Player has been hit for " + damageAmount.ToString() + " damage!");
            Debug.Log("Player has " + currentHealth.ToString() + " currentHealth remaining.");
        }

        #endregion

        if (!isAlive)
        {
            return;
        }

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

        healthText.text = currentHealth.ToString() + "/" + maxHealth;
        healthText.color = healthTextColour;
        updateHealthText = true;

        #region UpdateHealthText Colour

        int getHealth = (int)((double)currentHealth / (double)maxHealth * 100);

        if (getHealth < 70 && getHealth >= 51)
        {
            healthTextColour = upperMiddleHealthColour;
            updateHealthTextColour = true;
        }
        else if (getHealth < 50 && getHealth > 25)
        {
            healthTextColour = middleHealthColour;
            updateHealthTextColour = true;
        }
        else if (getHealth < 25)
        {
            healthTextColour = lowHealthColour;
            updateHealthTextColour = true;
        }

        #endregion
    }

    private void OnCharacterDeath(Guid getGuid)
    {
        if (getGuid != characterID || getGuid == null)
        {
            return;
        }

        P2_GameManager.Instance.OnPlayerCharacterKilled(getGuid);
        Destroy(gameObject);
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
            healthText.text = currentHealth.ToString() + "/" + maxHealth;

            if (updateHealthText)
            {
                if (updateHealthTextColour)
                {
                    healthText.color = healthTextColour;
                    updateHealthTextColour = false;
                    updateHealthTextColour = false;
                }
            }
        }
        else
        {
            updateHealthText = false;
            return;
        }
    }
}