using Cinemachine;
using System;
using UnityEngine;
using TMPro;

public class P3_fpsCharacterBase : MonoBehaviour
{
    #region Variables

    [Header("References")]
    private Guid characterID;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private P3_fpsMovement moveScript;
    [SerializeField] private P3_GunplayManager gunScript;
    [SerializeField] private GameObject playerCanvas;
    [SerializeField] private CinemachineVirtualCamera characterCam;
    [SerializeField] private CapsuleCollider playerCollider;

    [Header("U.I. Elements")]
    [SerializeField] private P3_Healthbar hudHealthBar;
    private bool updateHealth = false;

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

    public bool CharacterGunStatus()
    {
        return characterGunEnabled = gunScript.gameObject.activeSelf;
    }

    #endregion

    private void Awake()
    {
        characterID = GenerateID();

        isAlive = true;
        currentHealth = maxHealth;
    }

    void Start()
    {
        hudHealthBar.SetMaxHealth(maxHealth);
    }

    #region Health Functions

    public void UpdateHealth()
    {
        if (!isCharacterActive)
        {
            return;
        }

        updateHealth = true;
    }

    public void IncreaseHealth()
    {

    }

    #endregion

    #region OnTriggerEnter

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            P3_GameManager.Instance.OnPlayerKilled();

            isAlive = false;
            gameObject.SetActive(false);
        }
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
    }
}