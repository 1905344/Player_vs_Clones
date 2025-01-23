using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GunplayManager : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform muzzle;
    private RaycastHit _raycastHit;
    [SerializeField] private LayerMask isEnemy;
    [SerializeField] private LayerMask isTarget;

    [Space(5)]

    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;
    private InputManager inputManager;

    [Space(10)]

    [Header("Visual Feedback References")]
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject bulletHoleDecal;
    [SerializeField] private TextMeshProUGUI bulletsRemainingText;
    //[SerializeField] private Material gunMaterial;

    [SerializeField, Tooltip("Parent object for the instantiated bullet decals")] private Transform bulletDecalParent;
    [SerializeField, Tooltip("Parent object for the instantiated muzzle flashes")] private Transform muzzleFlashParent;

    private List<GameObject> muzzleFlashList = new List<GameObject>();
    private List<GameObject> bulletHoleDecalList = new List<GameObject>();

    [Space(10)]

    [Header("Camera Shake")]
    [SerializeField, Tooltip("How long the camera will shake for")] private float cameraShakeAmplitude;
    [SerializeField, Tooltip("How much the camera will shake")] private float cameraShakeMagnitude;

    [Space(10)]

    [Header("Sound Effects")]
    [SerializeField] private AudioClip gunTapFireSFX;
    [SerializeField] private AudioClip gunHoldFireSFX;

    [Space(20)]

    [Header("Gun Statistics")]
    [SerializeField] private int bulletDamage = 10;
    [SerializeField] private int headShotDamage = 30;
    [SerializeField] private float timeBetweenBullets;
    [SerializeField] private float timeBetweenShooting;

    [Space(10)]

    [SerializeField] private float bulletSpread;
    [SerializeField] private float bulletRange;
    [SerializeField] private float gunReloadTime;

    [Space(10)]

    [SerializeField] private int magazineClipSize;
    [SerializeField] private int bulletsPerLeftMouseTap;
    [SerializeField] private int bulletsRemaining;
    [SerializeField] private int bulletsFired;

    [Space(10)]

    [SerializeField] private bool allowFireButtonHold = false;
    [SerializeField] private bool isShooting = false;
    [SerializeField] private bool canShoot;
    [SerializeField] private bool isReloading;

    [Space(10)]

    [SerializeField] public bool isFPSTesting = false;

    [Space(10)]

    [SerializeField] private bool updateGunPosition = false;
    [SerializeField, Tooltip("Enable this to place the gun in the 'left hand' of the character")] public bool gunInLeftHand = true;
    [SerializeField, Tooltip("Enable this to place the gun in the 'right hand' of the character")] public bool gunInRightHand = false;

    [Space(15)]

    [SerializeField] private int getCurrentCourseID = 0;

    [Space(15)]

    [Header("Visual Feedback Despawn Timers")]
    //[SerializeField] private float muzzleFlashDespawnTimer;
    [SerializeField] private float bulletDecalDespawnTimer;
    private float despawningTimer;
    private bool toggleDespawnTimer = false;
    private bool startDespawn = false;

    [Space(10)]

    [SerializeField, Tooltip("[DEBUGGING] Show a line of where the player is looking and can shoot")] private bool showFiringLine = false;

    #endregion

    private void Awake()
    {
        bulletsRemaining = magazineClipSize;
        canShoot = true;

        #region Check which hand the gun should be

        if (gunInLeftHand)
        {
            gunInRightHand = false;
            updateGunPosition = true;
        }
        else if (gunInRightHand)
        {
            gunInLeftHand = false;
            updateGunPosition = true;
        }

        this.gameObject.SetActive(true);

        #endregion

    }

    private void Start()
    {
        inputManager = InputManager.Instance;

        if (isFPSTesting)
        {
            canShoot = true;
            bulletsRemainingText.gameObject.SetActive(true);
            gameObject.SetActive(true);
        }
        else
        {
            canShoot = false;

            bulletsRemainingText.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        updateGunPosition = true;
    }

    #region Enable and Disable The Gun

    public void EnableGun(int courseID)
    {
        updateGunPosition = true;
        canShoot = true;

        if (gunInLeftHand)
        {
            leftHand.gameObject.SetActive(true);
        }
        else if (gunInRightHand)
        {
            rightHand.gameObject.SetActive(true);
        }
        
        bulletsRemainingText.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }

    public void DisableGun(int courseID)
    {
        canShoot = false;
        bulletsRemaining = magazineClipSize;

        Debug.Log("GunplayManager: Disabling the gun!");
        bulletsRemainingText.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void DisableGunAfterTraining()
    {
        if (!isFPSTesting)
        {
            canShoot = false;

            Debug.Log("GunplayManager: Disabling the gun!");
            gameObject.SetActive(false);
        }
    }

    #endregion

    private void GunInput()
    {
        #region Tap, Press or Hold Fire Button to Shoot

        if (allowFireButtonHold)
        {
            isShooting = inputManager.IsPlayerHoldingTheFireButton;
        }
        else
        {
            isShooting = inputManager.IsPlayerTappingTheFireButton;
        }

        #endregion

        #region Firing the Gun

        if (canShoot && isShooting && !isReloading && bulletsRemaining > 0)
        {
            bulletsFired = bulletsPerLeftMouseTap;
            FireGun();
        }

        #endregion

        #region Reloading 

        if (inputManager.PlayerPressedReload() && bulletsRemaining < magazineClipSize && !isReloading)
        {
            ReloadGun();
        }

        #endregion
    }

    #region Gun Functions

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_camera.transform.position, _camera.transform.position + _camera.transform.forward * bulletRange);
    }

    private void FireGun()
    {
        canShoot = false;

        #region Bullet Spread

        float x = UnityEngine.Random.Range(-bulletSpread, bulletSpread);
        float y = UnityEngine.Random.Range(-bulletSpread, bulletSpread);

        #region Calculate bullet spread direction

        Vector3 spreadDirection = _camera.transform.forward;

        #endregion

        #endregion

        #region Raycast for Bullets

        if (Physics.Raycast(_camera.transform.position, spreadDirection, out _raycastHit, bulletRange, isEnemy))
        {
            #region Debugging

            if (GameManager.Instance.toggleDebug)
            {
                if (showFiringLine)
                {
                    //To view the raycast in engine
                    Debug.DrawLine(_camera.transform.position, _raycastHit.point, Color.red, 5f);
                }

                Debug.Log("GunplayManager: Gun fired!");
                Debug.Log("GunplayManager: Bullet hit: " + _raycastHit.collider.name);
            }

            #endregion

            Guid enemyGuid = new Guid();

            if (_raycastHit.collider.CompareTag("Enemy"))
            {
                enemyGuid = Guid.Empty;
                enemyGuid = _raycastHit.collider.GetComponentInParent<enemyAiController>().enemyID;

                if (GameManager.Instance.toggleDebug)
                {
                    Debug.Log("GunplayManager: Hit an enemy! ID is: " + enemyGuid);
                }

                //Using events
                if (_raycastHit.collider.name == "Enemy Head")
                {
                    //Additional damage if raycast hits enemy head
                    GameManager.Instance.OnEnemyHit(enemyGuid, headShotDamage);
                }
                else
                {
                    GameManager.Instance.OnEnemyHit(enemyGuid, bulletDamage);
                }
            }
        }

        #endregion

        #region Camera Shake

        CinemachineShake.Instance.ShakeCamera(cameraShakeMagnitude, cameraShakeAmplitude);

        #endregion

        #region Visual Feedback: Bullet Hole & Muzzle Flash

        //Instantiate(bulletHoleDecal, _raycastHit.point, Quaternion.Euler(0, 180, 0));
        //Instantiate(muzzleFlash, muzzle.position, Quaternion.identity);

        CreateVisualFeedback(muzzleFlash, bulletHoleDecal, _raycastHit.point);

        #endregion

        #region Audio Feedback: Gun Fired SFX

        if (allowFireButtonHold)
        {
            SoundManager.instance.PlaySFX(gunHoldFireSFX);
        }
        else
        {
            SoundManager.instance.PlaySFX(gunTapFireSFX);
        }
        

        #endregion

        bulletsRemaining--;
        bulletsFired--;

        Invoke(nameof(ResetShot), timeBetweenShooting);

        if (bulletsFired > 0 && bulletsRemaining > 0)
        {
            Invoke(nameof(FireGun), timeBetweenBullets);
        }
    }

    private void ResetShot()
    {
        canShoot = true;
    }

    private void ReloadGun()
    {
        isReloading = true;
        Invoke(nameof(ReloadingComplete), gunReloadTime);
    }

    private void ReloadingComplete()
    {
        bulletsRemaining = magazineClipSize;
        isReloading = false;
    }

    #endregion

    #region Visual Feedback

    private void CreateVisualFeedback(GameObject flash, GameObject bulletHole, Vector3 raycastHitPoint)
    {
        //if (muzzleFlashParent.childCount > 0)
        //{
        //    GameObject child = muzzleFlashParent.GetChild(0).gameObject;
        //    DestroyImmediate(child);
        //}

        //if (bulletDecalParent.childCount > 0)
        //{
        //    GameObject child = bulletDecalParent.GetChild(0).gameObject;
        //    DestroyImmediate(child);
        //}

        Instantiate(flash, muzzle.position, Quaternion.identity, muzzleFlashParent) ;
        Instantiate(bulletHole, raycastHitPoint, Quaternion.Euler(0, 180, 0), bulletDecalParent);

        DespawnDecals();
    }

    private void DespawnDecals()
    {
        despawningTimer = 0f;
        toggleDespawnTimer = true;
    }

    private void DespawnNow()
    {
        if (bulletDecalParent.childCount >= 10)
        {
            for (int i = 0; i > 10; i++)
            {
                DestroyImmediate(bulletDecalParent.GetChild(i).gameObject);
            }
        }
        else
        {
            toggleDespawnTimer = false;
        }
    }

    #endregion

    private void Update()
    {
        GunInput();

        if (isReloading)
        {
            bulletsRemainingText.text = "Reloading...";
        }
        else
        {
            bulletsRemainingText.SetText(bulletsRemaining + " / " + magazineClipSize);
        }

        if (!isFPSTesting)
        {
            if (updateGunPosition)
            {
                if (gunInLeftHand)
                {
                    transform.SetParent(leftHand.transform);
                }
                else if (gunInRightHand)
                {
                    transform.SetParent(rightHand.transform);
                }

                updateGunPosition = false;
            }
        }
        else
        {
            if (updateGunPosition)
            {
                if (gunInLeftHand)
                {
                    transform.SetParent(leftHand.transform);
                }
                else if (gunInRightHand)
                {
                    transform.SetParent(rightHand.transform);
                }

                updateGunPosition = false;
            }
        }

        if (toggleDespawnTimer)
        {
            despawningTimer = Time.deltaTime;

            if (despawningTimer <= bulletDecalDespawnTimer)
            {
                DespawnNow();
            }
        }
    }
}
