using UnityEngine;
using Cinemachine;
using TMPro;

public class GunplayManager : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera vCam;
    [SerializeField] private Transform muzzle;
    [SerializeField] private RaycastHit _raycastHit;
    [SerializeField] private LayerMask isEnemy;
    [SerializeField] private LayerMask isTarget;
    private InputManager inputManager;

    [Space(10)]

    [Header("Visual Feedback References")]
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private GameObject bulletHoleDecal;
    [SerializeField] private TextMeshProUGUI bulletsRemainingText;

    [Space(10)]
    [Header("Camera Shake")]
    [SerializeField, Tooltip("How much the camera will shake")] private float cameraShakeMagnitude;
    [SerializeField, Tooltip("How long the camera will shake for")] private float cameraShakeAmplitude;

    [Space(20)]

    [Header("Gun Statistics")]
    [SerializeField] private int bulletDamage;
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

    [SerializeField] private bool allowFireButtonHold;
    [SerializeField] private bool isShooting;
    [SerializeField] private bool canShoot;
    [SerializeField] private bool isReloading;

    [Space(10)]

    [SerializeField] public bool isPlayerInTrainingCourse = false;

    #endregion

    private void Awake()
    {
        bulletsRemaining = magazineClipSize;
        canShoot = true;
    }

    private void Start()
    {
        inputManager = InputManager.Instance;
    }

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

    private void FireGun()
    {
        canShoot = false;

        #region Bullet Spread

        float x = Random.Range(-bulletSpread, bulletSpread);
        float y = Random.Range(-bulletSpread, bulletSpread);

        #region Calculate bullet spread direction

        Vector3 spreadDirection = vCam.transform.forward + new Vector3(x, y, 0);

        #endregion

        #endregion

        #region Raycast for Bullets

        if (isPlayerInTrainingCourse)
        {
            if (Physics.Raycast(vCam.transform.position, spreadDirection, out _raycastHit, bulletRange, isTarget))
            {
                Debug.Log("Bullet hit: " + _raycastHit.collider.name);

                if (_raycastHit.collider.CompareTag("Enemy"))
                {
<<<<<<< Updated upstream
                    //Need to create an enemy script with a public function to take damage and reference it here
                    //_raycastHit.collider.GetComponent<enemyScript>().TakeDamage(bulletDamage);
=======
                    _raycastHit.collider.GetComponent<Target>().TargetHit(bulletDamage);
>>>>>>> Stashed changes
                }
            }
        }
        else
        {
            if (Physics.Raycast(vCam.transform.position, spreadDirection, out _raycastHit, bulletRange, isEnemy))
            {
                Debug.Log("Bullet hit: " + _raycastHit.collider.name);

                if (_raycastHit.collider.CompareTag("Target"))
                {
                    //Need to create an enemy script with a public function to take damage and reference it here
                    //_raycastHit.collider.GetComponent<Target>().TargetHit(bulletDamage);
                }
            }
        }

        #endregion

        #region Camera Shake

        CinemachineShake.Instance.ShakeCamera(3f, .1f);

        #endregion

        #region Visual Feedback - Bullet Hole & Muzzle Flash

        Instantiate(bulletHoleDecal, _raycastHit.point, Quaternion.Euler(0, 180, 0));
        Instantiate(muzzleFlash, muzzle.position, Quaternion.identity);

        #endregion

        bulletsRemaining--;
        bulletsFired--;
        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsFired > 0 && bulletsRemaining > 0)
        {
            Invoke("Shoot", timeBetweenBullets);
        }
    }

    private void ResetShot()
    {
        canShoot = true;
    }

    private void ReloadGun()
    {
        isReloading = true;
        Invoke("ReloadingComplete", gunReloadTime);
    }

    private void ReloadingComplete()
    {
        bulletsRemaining = magazineClipSize;
        isReloading = false;
    }

    #endregion

    private void Update()
    {
        GunInput();

        bulletsRemainingText.SetText(bulletsRemaining + " / " + magazineClipSize);
    }
}
