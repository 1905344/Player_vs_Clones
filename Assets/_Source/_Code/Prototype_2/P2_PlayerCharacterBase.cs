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