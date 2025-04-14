using UnityEngine;
using TMPro;

public class playerGameCharacter : MonoBehaviour
{
    #region Variables

    [SerializeField] private int health;
    private int maxHealth;
    [SerializeField] private bool isAlive;

    [Space(10)]

    [SerializeField] private int enemyBulletDamageAmount;

    [Space(10)]

    [Header("U.I. Elements")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Color32 healthTextColour;
    [SerializeField] private Color32 maxHealthColour;
    [SerializeField] private Color32 highHealthColour;
    [SerializeField] private Color32 middleHealthColour;
    [SerializeField] private Color32 lowHealthColour;
    private bool updateHealthTextColour;
    private bool updateHealthText = false;

    #endregion

    private void Awake()
    {
        isAlive = true;
        healthTextColour = Color.green;
    }

    private void Start()
    {
        GameManager.Instance.PlayerHit += OnPlayerHit;

        healthText.text = health.ToString() + "/100";
        healthText.color = maxHealthColour;
        maxHealth = health;
        updateHealthTextColour = true;
    }

    public void OnPlayerHit(int damage)
    {
        #region Debug

        if (GameManager.Instance.toggleDebug)
        {
            Debug.Log("Player has been hit for " + damage.ToString() + " damage!");
            Debug.Log("Player has " + health.ToString() + " health remaining.");
        }

        #endregion

        if (!isAlive)
        {
            return;
        }

        health -= damage;

        if (health <= 0)
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

            GameManager.Instance.OnPlayerKilled();

            #region Debug
            
            if (GameManager.Instance.toggleDebug)
            {
                Debug.Log("Player has been killed.");
            }

            #endregion
        }
    }

    private void UpdateHealth()
    {
        int currentHealth = (int)((double) health / (double)maxHealth * 100);

        #region UpdateHealthText Colour

        if (currentHealth < 70 && currentHealth >= 51)
        {
            healthTextColour = new Color(255, 205, 0, 255);
            updateHealthTextColour = true;
        }
        else if (currentHealth < 50 && currentHealth > 25)
        {
            healthTextColour = new Color(255, 155, 0, 255);
            updateHealthTextColour = true;
        }
        else if (currentHealth < 25)
        {
            healthTextColour = Color.red;
            updateHealthTextColour = true;
        }

        updateHealthText = true;

        #endregion
    }

    private void Update()
    {
        if (updateHealthText)
        {
            healthText.text = health.ToString() + "/100";

            if (updateHealthTextColour)
            {
                healthText.color = healthTextColour;
                updateHealthTextColour = false;
                updateHealthTextColour = false;
            }
        }
    }
}
