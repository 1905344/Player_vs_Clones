using System;
using TMPro;
using UnityEngine;

public class P2_GunplayManager : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform muzzle;
    private RaycastHit _raycastHit;
    [SerializeField] private Transform applyRecoilTarget;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;

    [Space(10)]

    [Header("Layer Masks")]
    [SerializeField] private LayerMask detectionMask;

    [Space(10)]

    [Header("Visual Feedback References")]
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject bulletHoleDecal;
    [SerializeField] private P2_AmmoHudBar ammoHudBar;
    [SerializeField] private TextMeshProUGUI reloadingText;
    private bool updateHUD = false;
    //[SerializeField] private Material gunMaterial;

    [SerializeField, Tooltip("Parent object for the instantiated bullet decals")] private Transform bulletDecalParent;
    private Vector3 bulletHoleDecalSpawnLocation;

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

    [SerializeField] private bool updateGunPosition = false;
    [SerializeField, Tooltip("Enable this to place the gun in the 'left hand' of the character")] public bool gunInLeftHand = true;
    [SerializeField, Tooltip("Enable this to place the gun in the 'right hand' of the character")] public bool gunInRightHand = false;

    [Space(15)]

    [Header("Visual Feedback Despawning")]
    [SerializeField] private float bulletDecalDespawnTimer;
    private float despawningTimer;
    private bool despawnBulletHoleDecals = false;
    private bool startDespawn = false;
    private bool spawnBulletDecal = false;
    [SerializeField, Tooltip("The maximum number of bullet decals spawned by the player")] private float bulletDecalLimit;
    [SerializeField, Tooltip("The number of bullet decals to destroy when the limit is reached")] private float bulletDecalDestroyAmount;

    [Space(10)]

    [Header("Gun Recoil Variables")]
    [SerializeField, Tooltip("Degrees of deflection up.")] private AnimationCurve gunRecoilUp;
    [SerializeField, Tooltip("Degrees of deflection right.")] private AnimationCurve gunRecoilRight;
    [SerializeField, Tooltip("How long the gun recoil should last.")] private float gunRecoilTimeInterval;
    private float gunRecoilTimer;
    [SerializeField] private bool isGunRecoiling;

    [Space(10)]

    [SerializeField, Tooltip("[DEBUGGING] Show a line of where the player is looking and can shoot")] private bool showFiringLine = false;

    private float betweenShotsTimer;
    private float reloadingTimer;
    private float betweenBulletsTimer;

    [SerializeField] public bool isActiveGun = false;

    #endregion

    private void Awake()
    {
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
        bulletsRemaining = magazineClipSize;
        canShoot = true;

        ammoHudBar.gameObject.SetActive(true);
        ammoHudBar.SetMaxAmmo(magazineClipSize);
        ammoHudBar.SetCurrentAmmo(bulletsRemaining);
        updateGunPosition = true;
    }

    #region Enable and Disable The Gun

    public void EnableGun()
    {
        isActiveGun = true;
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

        ammoHudBar.gameObject.SetActive(true);
        ammoHudBar.SetMaxAmmo(magazineClipSize);
        ammoHudBar.SetCurrentAmmo(bulletsRemaining);
    }

    public void DisableGun()
    {
        isActiveGun = false;
        canShoot = false;
        ammoHudBar.gameObject.SetActive(false);

        Debug.Log("P2_GunplayManager: Disabling the gun!");
    }

    #endregion

    private void GunInput()
    {
        #region Tap, Press or Hold Fire Button to Shoot

        if (allowFireButtonHold)
        {
            isShooting = P2_InputManager.Instance.IsPlayerHoldingTheFireButton;
        }
        else if (!allowFireButtonHold)
        {
            isShooting = P2_InputManager.Instance.IsPlayerTappingTheFireButton;
        }

        #endregion

        #region Firing the Gun

        if (canShoot && isShooting && !isReloading && bulletsRemaining > 0 )
        {
            //bulletsFired = bulletsPerLeftMouseTap;
            FireGun();
        }

        #endregion

        #region Reloading 

        if (P2_InputManager.Instance.PlayerPressedReload() && bulletsRemaining < magazineClipSize && !isReloading)
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
        if (!canShoot)
        {
            return;
        }

        #region Bullet Spread

        float x = UnityEngine.Random.Range(-bulletSpread, bulletSpread);
        float y = UnityEngine.Random.Range(-bulletSpread, bulletSpread);

        #region Calculate bullet spread direction

        Vector3 spreadDirection = _camera.transform.forward;

        #endregion

        #endregion

        #region Raycast for Bullets

        if (Physics.Raycast(_camera.transform.position, spreadDirection, out _raycastHit, bulletRange, detectionMask))
        {
            #region Debug

            if (P2_GameManager.Instance.enableDebug)
            {
                if (showFiringLine)
                {
                    //To view the raycast in engine
                    Debug.DrawLine(_camera.transform.position, _raycastHit.point, Color.red, 5f);
                }

                Debug.Log("P2_GunplayManager: Gun fired!");
                Debug.Log("P2_GunplayManager: Bullet hit: " + _raycastHit.collider.name);
            }

            #endregion

            Guid enemyGuid = new Guid();

            if (_raycastHit.collider != null && _raycastHit.collider.CompareTag("Enemy"))
            {
                enemyGuid = Guid.Empty;
                enemyGuid = _raycastHit.collider.GetComponentInParent<P2_enemyHealth>().enemyID;

                #region Debug

                if (P2_GameManager.Instance.enableDebug)
                {
                    Debug.Log("P2_GunplayManager: Hit an enemy! ID is: " + enemyGuid);
                }

                #endregion

                //Using events
                if (_raycastHit.collider.name == "Enemy Head")
                {
                    //Additional damage if raycast hits enemy head
                    P2_GameManager.Instance.OnEnemyHit(enemyGuid, headShotDamage);

                    #region Debug

                    if (P2_GameManager.Instance.enableDebug)
                    {
                        Debug.Log("P2_GunplayManager: Hit enemy " + enemyGuid + "'s head!");
                    }

                    #endregion
                }
                else
                {
                    P2_GameManager.Instance.OnEnemyHit(enemyGuid, bulletDamage);
                }
            }
            else if (_raycastHit.collider != null && _raycastHit.collider.CompareTag("Wall") || _raycastHit.collider.CompareTag("Ground") 
                || _raycastHit.collider.CompareTag("Environment") || _raycastHit.collider.CompareTag("Pushable"))
            {
                bulletHoleDecalSpawnLocation = _raycastHit.point;

                spawnBulletDecal = true;

                #region Debug

                if (P2_GameManager.Instance.enableDebug)
                {
                    Debug.Log($"P2_GunplayManager: Hit {_raycastHit.collider.tag}. Spawning a bullet decal.");
                }

                #endregion
            }
        }

        #endregion

        #region Camera Shake

        P2_CinemachineShake.Instance.ShakeCamera(cameraShakeMagnitude, cameraShakeAmplitude);

        #endregion

        #region Visual Feedback: Bullet Hole & Muzzle Flash

        CreateVisualFeedback(muzzleFlash, bulletHoleDecal, bulletHoleDecalSpawnLocation);

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

        #region Recoil

        GunRecoil();

        #endregion

        updateHUD = true;
        bulletsRemaining--;
        bulletsFired++;

        betweenShotsTimer = 0f;
        canShoot = false;
    }

    private void ReloadGun()
    {
        canShoot = false;
        isReloading = true;
    }

    private void GunRecoil()
    {
        #region Debug

        if (P2_GameManager.Instance.enableDebug)
        {
            Debug.Log("P2_GunplayManager: Gun recoil function has been called.");
        }

        #endregion

        //P2_GameManager.Instance.OnGunFired();
        isGunRecoiling = true;
    }

    private void ApplyRecoil(float recoilAngle)
    {
        float up = gunRecoilUp.Evaluate(recoilAngle);
        float right = gunRecoilRight.Evaluate(recoilAngle);

        if (recoilAngle == 0)
        {
            up = 0;
            right = 0;
        }

        up = -up;

        applyRecoilTarget.localRotation = Quaternion.Euler(up, right, 0);
    }

    #endregion

    #region Visual Feedback

    private void CreateVisualFeedback(GameObject flash, GameObject bulletHole, Vector3 raycastHitPoint)
    {
        if (spawnBulletDecal)
        {
            Instantiate(bulletHole, raycastHitPoint, Quaternion.Euler(0, 180, 0), bulletDecalParent);
            spawnBulletDecal = false;
        }

        Instantiate(flash, muzzle.position, Quaternion.identity);
    }

    private void DespawnBulletHoleDecals()
    {
        if (!despawnBulletHoleDecals)
        {
            return;
        }

        //Could change the for loop to remove all but X amount from the limit
        //Example 1: i < bulletDecalLimit - 1
        //Example 2: i < bulletDecalLimit - bulletDecalDestroyAmount


        for (int i = 0; i < bulletDecalDestroyAmount; i++)
        {
            Destroy(bulletDecalParent.GetChild(i).gameObject);

            #region Debug

            if (P2_GameManager.Instance.enableDebug)
            {
                Debug.Log("P2_GunplayManager: Destroying bullet hole decals.");
            }

            #endregion
        }

        despawnBulletHoleDecals = false;
    }

    #endregion

    private void Update()
    {
        if (isActiveGun)
        {
            GunInput();

            #region Shooting

            if (!canShoot)
            {
                betweenShotsTimer += Time.deltaTime;
            }

            //Time between shots fired
            if (betweenShotsTimer > timeBetweenShooting)
            {
                canShoot = true;
            }

            //Time between bullets
            if (betweenBulletsTimer > timeBetweenBullets)
            {
                if (bulletsFired > 0 && bulletsRemaining > 0)
                {
                    FireGun();
                }
            }

            #endregion

            #region Update Ammo HUD 

            if (updateHUD)
            {
                ammoHudBar.SetCurrentAmmo(bulletsRemaining);
                updateHUD = false;
            }

            #endregion

            #region Reloading

            if (isReloading)
            {
                reloadingTimer += Time.deltaTime;

                reloadingText.gameObject.SetActive(true);
                reloadingText.text = "Reloading";

                if (P2_GameManager.Instance.enableReloadPromptTextAsTimer)
                {
                    P2_GameManager.Instance.HideReloadPrompt();
                }
            }
            else
            {
                reloadingText.gameObject.SetActive(false);
                ammoHudBar.SetCurrentAmmo(bulletsRemaining);
            }

            if (isShooting && !isReloading && bulletsRemaining == 0 && canShoot)
            {
                if (P2_GameManager.Instance.enableReloadPromptTextAsTimer)
                {
                    P2_GameManager.Instance.ShowReloadPrompt();

                    #region Debug

                    if (P2_GameManager.Instance.enableDebug)
                    {
                        Debug.Log("P2_GunplayManager: Prompting reload text.");
                    }

                    #endregion
                }
                else
                {
                    P2_GameManager.Instance.HideReloadPrompt();
                }
            }

            if (reloadingTimer > gunReloadTime)
            {
                canShoot = true;
                isReloading = false;
                bulletsRemaining = magazineClipSize;
                reloadingTimer = 0f;
            }

            #endregion

            #region Gun Position

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

            #endregion

            #region Despawning Bullet Hole Decals

            if (bulletDecalParent.childCount >= bulletDecalLimit)
            {
                despawnBulletHoleDecals = true;
                DespawnBulletHoleDecals();
            }

            #endregion

            #region Recoil timer

            if (gunRecoilTimer == 0)
            {
                if (isGunRecoiling)
                {
                    gunRecoilTimer = Time.deltaTime;
                }
            }

            if (gunRecoilTimer > 0)
            {
                float recoil = gunRecoilTimer / gunRecoilTimeInterval;

                gunRecoilTimer += Time.deltaTime;

                if (gunRecoilTimer > gunRecoilTimeInterval)
                {
                    gunRecoilTimer = 0;
                    recoil = 0;
                    isGunRecoiling = false;
                }

                ApplyRecoil(recoil);
            }

            #endregion
        }
    }
}
