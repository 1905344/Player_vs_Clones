using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Events;

//Informing Unity that the player character must have these items
//Change, comment out or remove any of these if they aren't needed
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))] //Change this if you're using a different collider type
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class topDownController : MonoBehaviour
{
    #region Character Components
    //Components attached to the player character
    [Header("Character Components")]
    private Rigidbody2D playerBody;
    private BoxCollider2D playerColl;
    private Animator playerAnimator;
    private SpriteRenderer playerSprite;
    public AudioSource playerAudio;

    [Space(10)]

    //Components not attached to the player characer
    [Header("External Components")]
    [SerializeField] Vector3 respawnPoint;

    [Space(10)]

    [Header("Sound Effects")]
    [SerializeField] private AudioSource injuredSFX;

    #endregion

    #region Movement Variables
    private float horizontal;
    private float vertical;
    
    [SerializeField] public float moveSpeed = 20.0f;

    #endregion



    private void Awake()
    {
        //Get all of the player components
        playerBody = GetComponent<Rigidbody2D>();
        playerColl = GetComponent<BoxCollider2D>();     //Change this variable if using a different collider type
        playerSprite = GetComponent<SpriteRenderer>();
        playerAnimator = GetComponent<Animator>();
        playerAudio = GetComponent<AudioSource>();

    }

    void Start()
    {

    }

    void Update()
    {

    }

    private void FixedUpdate()
    {

    }

    #region Saving & Loading
    public void SaveData(GameData data)
    {

    }
    public void LoadData(GameData data)
    {

    }

    #endregion
}