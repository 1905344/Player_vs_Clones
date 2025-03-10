using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;
using Unity.VisualScripting;

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
    [SerializeField] private TextMeshProUGUI frontText;
    [SerializeField] private TextMeshProUGUI backText;

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
        frontText.text = defaultText;
        backText.text = defaultText;

        progressBar.maxValue = hackingTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        P2_PlayerCharacterBase playerCharacterBase = null;

        if (other.gameObject.TryGetComponent<P2_PlayerCharacterBase>(out playerCharacterBase))
        {
            if (playerCharacterBase.canHack)
            {
                characterMoveScript = other.gameObject.GetComponent<P2_fpsMovement>();

                frontText.text = promptText;
                backText.text = promptText;
                canHack = true;
            }
            else
            {
                frontText.text = defaultText;
                backText.text = defaultText;
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
                frontText.text = defaultText;
                backText.text = defaultText;
            }
        }
    }

    private void OnHacking()
    {
        hackingScreen.gameObject.SetActive(true);
        characterMoveScript.DisablePlayerMovement();

        progressBar.value = hackingTimer;
        progressBarFill.color = fillGradient.Evaluate(progressBar.normalizedValue);
    }

    private void StopHacking()
    {
        isHacking = false;
        hackingTimer = 0f;
        hackingScreen.gameObject.SetActive(false);

        frontText.text = defaultText;
        backText.text = defaultText;

        characterMoveScript.EnablePlayerMovement();
    }

    private void FinishedHacking()
    {
        characterMoveScript.EnablePlayerMovement();
        hackingScreen.gameObject.SetActive(false);
        frontText.gameObject.SetActive(false);
        backText.gameObject.SetActive(false);
        door.gameObject.SetActive(false);

        hackingTimer = 0f;
        isHacking = false;
        alreadyHacked = true;
        canHack = false;
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
                StopHacking();
            }

            if (isHacking)
            {
                hackingTimer += Time.deltaTime;

                OnHacking();
            }

            if (hackingTimer >= hackingTime)
            {
                FinishedHacking();
            }
        }
    }
}