using UnityEngine;
using TMPro;

public class playerGameCharacter : MonoBehaviour
{
    #region Variables

    [SerializeField] private int health;
    [SerializeField] private bool isAlive;

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
        GameManager.Instance.PlayerHit += OnPlayerHit;

        healthText.text = health.ToString() + "/100";
        healthText.color = healthTextColour;
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

            //Application.Quit();
        }
    }

    private void UpdateHealth()
    {
        updateHealthText = true;

        #region UpdateHealthText Colour

        if (health < 70 && health >= 51)
        {
            healthTextColour = new Color(255, 205, 0, 255);
            updateHealthTextColour = true;
        }
        else if (health < 50 && health > 25)
        {
            healthTextColour = new Color(255, 155, 0, 255);
            updateHealthTextColour = true;
        }
        else if (health < 25)
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
                updateHealthTextColour = false;
            }
        }
    }
}
