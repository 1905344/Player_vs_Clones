using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public class P2_HackableDoor : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField] private GameObject door;
    [SerializeField] private BoxCollider doorCollider;

    [Space(5)]

    public bool isHacking = false;
    public bool hackable = false;
    public bool alreadyHacked = false;

    [Space(10)]

    [Header("U.I.")]
    [SerializeField] private GameObject hackingScreen;
    [SerializeField] private TMP_Text hackScreenText;
    [SerializeField] private TMP_Text frontText;
    [SerializeField] private TMP_Text backText;

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

    [Space(10)]

    [Header("Sound Effects")]
    [SerializeField] private AudioClip startHackingSFX;
    [SerializeField] private AudioClip stopHackingSFX;
    [SerializeField] private AudioClip hackingCompletedSFX;

    private bool characterChanged = false;

    #endregion

    private void Awake()
    {
        frontText.text = defaultText;
        backText.text = defaultText;
        progressBar.maxValue = hackingTime;
    }

    private void Start()
    {
        #region Debug

        if (alreadyHacked && P2_GameManager.Instance.enableDebug)
        {
            door.gameObject.SetActive(false);
            frontText.gameObject.SetActive(false);
            backText.gameObject.SetActive(false);
        }

        #endregion
    }

    private void OnTriggerStay(Collider other)
    {
        P2_PlayerCharacterBase playerCharacterBase = null;

        if (other.gameObject.TryGetComponent<P2_PlayerCharacterBase>(out playerCharacterBase))
        {
            if (playerCharacterBase.canHack && playerCharacterBase.isCharacterActive)
            {
                characterMoveScript = other.gameObject.GetComponent<P2_fpsMovement>();

                frontText.text = promptText;
                backText.text = promptText;
                hackable = true;
                characterChanged = false;
            }
            else if (playerCharacterBase.isCharacterActive)
            {
                frontText.text = defaultText;
                backText.text = defaultText;
                hackable = false;
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
        SoundManager.instance.PlaySFX(stopHackingSFX);

        isHacking = false;
        hackingTimer = 0f;
        hackingScreen.gameObject.SetActive(false);

        frontText.text = defaultText;
        backText.text = defaultText;

        if (!characterChanged)
        {
            characterMoveScript.EnablePlayerMovement();
        }
    }

    private void FinishedHacking()
    {
        SoundManager.instance.PlaySFX(hackingCompletedSFX);

        characterMoveScript.EnablePlayerMovement();
        hackingScreen.gameObject.SetActive(false);
        frontText.gameObject.SetActive(false);
        backText.gameObject.SetActive(false);
        door.gameObject.SetActive(false);
        doorCollider.enabled = false;

        hackingTimer = 0f;
        isHacking = false;
        alreadyHacked = true;
        hackable = false;
    }

    private void OnInteraction()
    {
        SoundManager.instance.PlaySFX(startHackingSFX);

        //Change text to hacking
        hackScreenText.text = duringHackingText;
        isHacking = true;
    }


    private void Update()
    {
        if (hackable)
        {
            if (P2_InputManager.Instance.PlayerChangedCharacters())
            {
                characterChanged = true;
            }

            //InputManager code for when the player starts hacking
            if (P2_InputManager.Instance.PlayerPressedHackButton() && !isHacking)
            {
                OnInteraction();
                //characterMoveScript.DisablePlayerMovement();
            }
            else if (P2_InputManager.Instance.PlayerPressedHackButton() || (P2_InputManager.Instance.PlayerChangedCharacters() && isHacking))
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