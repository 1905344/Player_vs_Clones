using TMPro;
using UnityEngine;

public class Prototype_1_playerGameCharacter : MonoBehaviour
{
    #region Variables

    [SerializeField] private int health;
    [SerializeField] private bool isAlive;
    //[SerializeField] private 

    [Space(10)]

    [SerializeField] private int enemyBulletDamageAmount;

    [Space(10)]

    [Header("U.I. Elements")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Color32 healthTextColour;
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
        Prototype_1_GameManager.Instance.PlayerHit += OnPlayerHit;
        healthText.text = health.ToString() + "/100";
        healthText.color = healthTextColour;
    }

    public void OnPlayerHit(int damage)
    {
        #region Debug

        if (Prototype_1_GameManager.Instance.toggleDebug)
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

            Prototype_1_GameManager.Instance.OnPlayerKilled();

            if (Prototype_1_GameManager.Instance.toggleDebug)
            {
                Debug.Log("Player has been killed.");
            }
        }
    }

    private void UpdateHealth()
    {
        updateHealthText = true;

        #region UpdateHealthText Colour

        float currentHealthPercentage = health / 100;

        if (currentHealthPercentage < 0.7 && currentHealthPercentage >= 0.55)
        {
            healthTextColour = new Color(255, 205, 0, 255);
            updateHealthTextColour = true;
        }
        else if (currentHealthPercentage < 1 && currentHealthPercentage >= 0.75)
        {
            healthTextColour = new Color(255, 155, 0, 255);
            updateHealthTextColour = true;
        }
        else if (currentHealthPercentage < 1 && currentHealthPercentage >= 0.75)
        {
            healthTextColour = Color.red;
            updateHealthTextColour = true;
        }

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
            }
        }
    }
}