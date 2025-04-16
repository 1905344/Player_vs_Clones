using TMPro;
using UnityEngine;

public class Prototype_1_playerGameCharacter : MonoBehaviour
{
    #region Variables

    [Header("Health Variables")]
    [SerializeField] private int currentHealth;
    [SerializeField] private int defaultHealth;
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private bool isAlive;

    [Space(10)]

    [SerializeField] private int enemyBulletDamageAmount;

    [Space(10)]

    [Header("Sound Effects")]
    [SerializeField] private AudioClip playerHitSFX;
    [SerializeField] private AudioClip playerDeathSFX;

    [Space(10)]

    [Header("U.I. Elements")]
    [SerializeField] private Prototype_1_HealthBar hudHealthBar;
    private bool updateHealth = false;

    #endregion

    private void Awake()
    {
        currentHealth = defaultHealth;
        defaultHealth = maxHealth;
    }

    private void Start()
    {
        Prototype_1_GameManager.Instance.PlayerHit += OnPlayerHit;

        hudHealthBar.SetCurrentHealth(currentHealth);
        hudHealthBar.SetMaxHealth(maxHealth);
    }

    public void OnPlayerHit(int damage)
    {
        #region Debug

        if (Prototype_1_GameManager.Instance.enableDebug)
        {
            Debug.Log("Player has been hit for " + damage.ToString() + " damage!");
            Debug.Log("Player has " + currentHealth.ToString() + " health remaining.");    
        }

        #endregion

        if (!isAlive)
        {
            return;
        }

        SoundManager.instance.PlaySFX(playerHitSFX);

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            OnPlayerDeath();
        }

        UpdateHealth();
    }

    private void OnPlayerDeath()
    {
        if (isAlive)
        {
            isAlive = false;

            Prototype_1_GameManager.Instance.OnPlayerKilled();
            SoundManager.instance.PlaySFX(playerDeathSFX);

            #region Debug

            if (Prototype_1_GameManager.Instance.enableDebug)
            {
                Debug.Log("Player has been killed.");
            }

            #endregion
        }
    }

    private void UpdateHealth()
    {
        if (!isAlive)
        {
            return;
        }

        updateHealth = true;
    }

    private void Update()
    {
        if (updateHealth)
        {
            hudHealthBar.SetMaxHealth(maxHealth);
            hudHealthBar.SetCurrentHealth(currentHealth);
        }
    }
}