using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;

[RequireComponent(typeof(BoxCollider))]
public class P2_HackableDoor : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField] private GameObject door;

    [Space(5)]

    public bool isHacking = false;
    public bool canHack = false;
    public bool alreadyHacked = false;

    [Space(10)]

    [Header("U.I.")]
    [SerializeField] private GameObject hackingScreen;
    [SerializeField] private TextMeshProUGUI hackScreenText;
    [SerializeField] private TextMeshProUGUI doorText;

    [Space(5)]

    [SerializeField] private string defaultText;
    [SerializeField] private string promptText;
    [SerializeField] private string duringHackingText;

    [Space(5)]

    [SerializeField] private Slider progressBar;
    [SerializeField] private Image progressBarFill;
    [SerializeField] private Gradient fillGradient;
    [SerializeField, Tooltip("How long does it take to hack a door")] private float hackingTime;
    private float hackingTimer;

    private P2_fpsMovement characterMoveScript;

    #endregion

    private void Awake()
    {
        doorText.text = defaultText;
    }

    private void OnTriggerEnter(Collider other)
    {
        P2_PlayerCharacterBase playerCharacterBase = null;

        if (other.gameObject.TryGetComponent<P2_PlayerCharacterBase>(out playerCharacterBase))
        {
            if (playerCharacterBase.canHack)
            {
                characterMoveScript = other.gameObject.GetComponent<P2_fpsMovement>();

                doorText.text = promptText;
                canHack = true;
            }
            else
            {
                doorText.text = defaultText;
                canHack = false;
                return;
            }
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!alreadyHacked)
            {
                doorText.text = defaultText;
            }
        }
    }

    private void Update()
    {
        if (canHack)
        {
            //InputManager code for when the player starts hacking
            if (P2_InputManager.Instance.PlayerPressedHackButton() && !isHacking)
            {
                //Change text to hacking
                hackScreenText.text = duringHackingText;
                isHacking = true;

                characterMoveScript.DisablePlayerMovement();
            }
            else if ((P2_InputManager.Instance.PlayerPressedHackButton() || P2_InputManager.Instance.PlayerChangedCharacters()) && isHacking)
            {
                isHacking = false;
                hackingTimer = 0f;

                hackingScreen.gameObject.SetActive(false);
                doorText.text = defaultText;

                characterMoveScript.EnablePlayerMovement();
            }

            if (isHacking)
            {
                hackingTimer += Time.deltaTime;

                hackingScreen.gameObject.SetActive(true);
                characterMoveScript.DisablePlayerMovement();

                float progress = Mathf.Clamp(hackingTimer, 0f, hackingTime);
                progressBar.value = progress;
                progressBarFill.color = fillGradient.Evaluate(progressBar.normalizedValue);
            }

            if (hackingTimer >= hackingTime)
            {
                characterMoveScript.EnablePlayerMovement();
                hackingScreen.gameObject.SetActive(false);
                doorText.gameObject.SetActive(false);
                door.gameObject.SetActive(false);

                alreadyHacked = true;


                hackingTimer = 0f;
                isHacking = false;
                canHack = false;
            }
        }
    }
}