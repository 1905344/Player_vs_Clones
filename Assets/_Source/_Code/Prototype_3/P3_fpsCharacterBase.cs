using Cinemachine;
using System;
using UnityEngine;

public class P3_fpsCharacterBase : MonoBehaviour
{
    #region Variables

    [Header("References")]
    private Guid characterID;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private P3_GunplayManager gunScript;
    [SerializeField] private CinemachineVirtualCamera characterCam;
    [SerializeField] private CapsuleCollider playerCollider;

    [Header("U.I. Elements")]
    [SerializeField] private P3_Healthbar hudHealthBar;
    private bool updateHealth = false;

    [Space(10)]

    [Header("Health Variables")]
    [SerializeField] private int currentHealth;
    [SerializeField] private int defaultHealth;
    [SerializeField] private int maxHealth = 100;

    [Space(10)]

    [Header("Debug Variables")]
    [SerializeField] public bool isCharacterActive = false;

    #endregion

    private void Awake()
    {
        currentHealth = defaultHealth;
        defaultHealth = maxHealth;
    }

    void Start()
    {
        hudHealthBar.SetCurrentHealth(currentHealth);
        hudHealthBar.SetMaxHealth(maxHealth);
    }

    #region Health Functions

    //public void UpdateHealth()
    //{
    //    if (!isCharacterActive)
    //    {
    //        return;
    //    }

    //    updateHealth = true;
    //}

    //public void IncreaseHealth(int IncreaseAmount)
    //{
    //    if (currentHealth == maxHealth)
    //    {
    //        return;
    //    }

    //    currentHealth += IncreaseAmount;
    //    updateHealth = true;
    //}

    #endregion

    #region OnTriggerEnter

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            P3_GameManager.Instance.OnPlayerKilled();

            gameObject.SetActive(false);
        }
    }

    #endregion

    public void EnableFPSCharacter()
    {
        if (!isCharacterActive)
        {
            return;
        }

        gunScript.EnableGun();

        isCharacterActive = true;
        updateHealth = true;
    }

    public void DisableFPSCharacter()
    {
        if (isCharacterActive)
        {
            return;
        }

        isCharacterActive = false;
        updateHealth = false;
        hudHealthBar.gameObject.SetActive(false);
        gunScript.DisableGun();
    }

    void Update()
    {
        if (updateHealth)
        {
            hudHealthBar.SetMaxHealth(maxHealth);
            hudHealthBar.SetCurrentHealth(currentHealth);

            updateHealth = false;
        }
    }
}