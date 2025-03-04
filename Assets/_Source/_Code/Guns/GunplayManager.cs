using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class GunplayManager : MonoBehaviour
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
    [SerializeField] private LayerMask isEnemy;
    [SerializeField] private LayerMask isGround;
    [SerializeField] private LayerMask isWall;
    [SerializeField] private LayerMask isEnvironment;

    [Space(10)]

    [Header("Visual Feedback References")]
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject bulletHoleDecal;
    [SerializeField] private TextMeshProUGUI bulletsRemainingText;
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
        canShoot = true;
        bulletsRemainingText.gameObject.SetActive(true);
        gameObject.SetActive(true);
        updateGunPosition = true;
    }

    #region Enable and Disable The Gun

    public void EnableGun()
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

    public void DisableGun()
    {
        canShoot = false;
        bulletsRemaining = magazineClipSize;

        Debug.Log("GunplayManager: Disabling the gun!");
        bulletsRemainingText.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    #endregion

    private void GunInput()
    {
        #region Tap, Press or Hold Fire Button to Shoot

        if (allowFireButtonHold)
        {
            isShooting = InputManager.Instance.IsPlayerHoldingTheFireButton;
        }
        else
        {
            isShooting = InputManager.Instance.IsPlayerTappingTheFireButton;
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

        if (InputManager.Instance.PlayerPressedReload() && bulletsRemaining < magazineClipSize && !isReloading)
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
            #region Debug

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
                enemyGuid = _raycastHit.collider.GetComponentInParent<enemyHealth>().enemyID;

                #region Debug

                if (GameManager.Instance.toggleDebug)
                {
                    Debug.Log("GunplayManager: Hit an enemy! ID is: " + enemyGuid);
                }

                #endregion

                //Using events
                if (_raycastHit.collider.name == "Enemy Head")
                {
                    //Additional damage if raycast hits enemy head
                    GameManager.Instance.OnEnemyHit(enemyGuid, headShotDamage);

                    #region Debug

                    if (GameManager.Instance.toggleDebug)
                    {
                        Debug.Log("GunplayManager: Hit enemy " + enemyGuid + "'s head!");
                    }

                    #endregion
                }
                else
                {
                    GameManager.Instance.OnEnemyHit(enemyGuid, bulletDamage);
                }
            }
        }
        else if (Physics.Raycast(_camera.transform.position, spreadDirection, out _raycastHit, bulletRange, isWall))
        {
            bulletHoleDecalSpawnLocation = _raycastHit.point;

            spawnBulletDecal = true;

            #region Debug

            if (GameManager.Instance.toggleDebug)
            {
                Debug.Log("GunplayManager: Hit a wall. Spawning a bullet decal.");
            }

            #endregion
        }
        else if (Physics.Raycast(_camera.transform.position, spreadDirection, out _raycastHit, bulletRange, isGround))
        {
            bulletHoleDecalSpawnLocation = _raycastHit.point;

            spawnBulletDecal = true;

            #region Debug

            if (GameManager.Instance.toggleDebug)
            {
                Debug.Log("GunplayManager: Hit the ground. Spawning a bullet decal.");
            }

            #endregion
        }
        else if (Physics.Raycast(_camera.transform.position, spreadDirection, out _raycastHit, bulletRange, isEnvironment))
        {
            bulletHoleDecalSpawnLocation = _raycastHit.point;

            spawnBulletDecal = true;

            #region Debug

            if (GameManager.Instance.toggleDebug)
            {
                Debug.Log("GunplayManager: Hit an environment object. Spawning a bullet decal.");
            }

            #endregion
        }

        #endregion

        #region Camera Shake

        CinemachineShake.Instance.ShakeCamera(cameraShakeMagnitude, cameraShakeAmplitude);

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

    private void GunRecoil()
    {
        #region Debug

        if (GameManager.Instance.toggleDebug)
        {
            Debug.Log("GunplayManager: Gun recoil function has been called.");
        }

        #endregion

        //GameManager.Instance.OnGunFired();
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

            if (GameManager.Instance.toggleDebug)
            {
                Debug.Log("GunplayManager: Destroying bullet hole decals.");
            }

            #endregion
        }

        despawnBulletHoleDecals = false;
    }

    #endregion

    private void Update()
    {
        GunInput();

        #region Reloading

        if (isReloading)
        {
            bulletsRemainingText.text = "Reloading...";
            GameManager.Instance.HideReloadPrompt();

            if (GameManager.Instance.enableReloadPromptTextAsTimer)
            {
                GameManager.Instance.HideReloadPrompt();
            }
        }
        else
        {
            bulletsRemainingText.SetText(bulletsRemaining + " / " + magazineClipSize);
        }

        if (isShooting && !isReloading && bulletsRemaining == 0 && canShoot)
        {
            if (GameManager.Instance.enableReloadPromptTextAsTimer)
            {
                GameManager.Instance.ShowReloadPrompt();

                #region Debug

                if (GameManager.Instance.toggleDebug)
                {
                    Debug.Log("GunplayManager: Prompting reload text.");
                }

                #endregion
            }
            else
            {
                GameManager.Instance.HideReloadPrompt();
            }
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
