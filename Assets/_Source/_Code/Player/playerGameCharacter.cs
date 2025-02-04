using UnityEngine;
using TMPro;
using UnityEngine.Rendering.UI;
using UnityEngine.UIElements;

public class playerGameCharacter : MonoBehaviour
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
    [SerializeField] private bool updateHealthTextColour;

    #endregion

    private void Awake()
    {
        isAlive = true;
        healthTextColour = Color.green;
        updateHealthTextColour = true;
    }

    private void Start()
    {
        GameManager.Instance.PlayerHit += OnPlayerHit;
    }

    public void OnPlayerHit(int damage)
    {
        #region Debug

        if (GameManager.Instance.toggleDebug)
        {
            Debug.Log("Player has been hit for " + damage + " damage!");
            Debug.Log("Player has " + health + " health remaining.");
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

    private void OnPlayerDeath()
    {
        if (isAlive)
        {
            isAlive = false;

            GameManager.Instance.OnPlayerKilled();

            if (GameManager.Instance.toggleDebug)
            {
                Debug.Log("Player has been killed.");
            }
        }
    }

    private void Update()
    {
        if (updateHealthTextColour)
        {
            healthText.color = healthTextColour;
            updateHealthTextColour = false;
        }
    }
}
