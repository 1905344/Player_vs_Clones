using System;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Cinemachine;

public class GunplayManager : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform muzzle;
    [SerializeField] private RaycastHit _raycastHit;
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

    [Space(10)]

    [Header("Camera Shake")]
    [SerializeField, Tooltip("How long the camera will shake for")] private float cameraShakeAmplitude;
    [SerializeField, Tooltip("How much the camera will shake")] private float cameraShakeMagnitude;
    

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

    [SerializeField] private bool allowFireButtonHold = false;
    [SerializeField] private bool isShooting = false;
    [SerializeField] private bool canShoot;
    [SerializeField] private bool isReloading;

    [Space(10)]

    [SerializeField] public bool isPlayerInTrainingCourse = false;

    [Space(10)]

    [SerializeField] private bool updateGunPosition = false;
    [SerializeField, Tooltip("Enable this to place the gun in the 'left hand' of the character")] public bool gunInLeftHand = true;
    [SerializeField, Tooltip("Enable this to place the gun in the 'right hand' of the character")] public bool gunInRightHand = false;

    [Space(15)]

    [SerializeField] private int getCurrentCourseID = 0;

    #endregion

    private void Awake()
    {
        bulletsRemaining = magazineClipSize;
        canShoot = true;

        #region Check which hand the gun should be

        if (isPlayerInTrainingCourse)
        {
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
        }
        else
        {
            this.gameObject.SetActive(false);
        }

        #endregion

    }

    private void Start()
    {
        inputManager = InputManager.Instance;
        GameManager.Instance.TrainingCourseStarted += EnableGun;
        GameManager.Instance.FinishedTraining += DisableGun;
    }

    #region Enable and Disable The Gun

    public void EnableGun(int courseID)
    {
        isPlayerInTrainingCourse = true;
        //canShoot = true;

        //if (courseID == 1)
        //{
        //    timeBetweenBullets = 0.1f;
        //    timeBetweenShooting = 0.7f;

        //    bulletSpread = 5f;
        //    bulletRange = 50f;
        //    gunReloadTime = 5f;

        //    magazineClipSize = 27;
        //    bulletsRemaining = magazineClipSize;
        //    bulletsPerLeftMouseTap = 3;

        //    allowFireButtonHold = false;
        //}
        //else if (courseID == 2) 
        //{
        //    timeBetweenBullets = 0.1f;
        //    timeBetweenShooting = 0.1f;

        //    bulletSpread = 11f;
        //    bulletRange = 100f;
        //    gunReloadTime = 6f;

        //    magazineClipSize = 75;
        //    bulletsRemaining = magazineClipSize;
        //    bulletsPerLeftMouseTap = 1;

        //    allowFireButtonHold = true;
        //}
        //else if (courseID == 3)
        //{
        //    timeBetweenBullets = 0.1f;
        //    timeBetweenShooting = 0.22f;

        //    bulletSpread = 6f;
        //    bulletRange = 35f;
        //    gunReloadTime = 1.5f;

        //    magazineClipSize = 15;
        //    bulletsRemaining = magazineClipSize;
        //    bulletsPerLeftMouseTap = 1;

        //    allowFireButtonHold = false;
        //}

        if (gunInLeftHand)
        {
            leftHand.gameObject.SetActive(true);
        }
        else if (gunInRightHand)
        {
            rightHand.gameObject.SetActive(true);
        }
        
        updateGunPosition = true;
        this.gameObject.SetActive(true);
    }

    public void DisableGun()
    {
        isPlayerInTrainingCourse = false;
        canShoot = false;

        leftHand.gameObject.SetActive(false);
        rightHand.gameObject.SetActive(false);
        this.gameObject.SetActive(false);
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

    private void FireGun()
    {
        canShoot = false;

        #region Bullet Spread

        float x = UnityEngine.Random.Range(-bulletSpread, bulletSpread);
        float y = UnityEngine.Random.Range(-bulletSpread, bulletSpread);

        #region Calculate bullet spread direction

        Vector3 spreadDirection = _camera.transform.forward + new Vector3(x, y, 0);

        #endregion

        #endregion

        #region Raycast for Bullets

        if (isPlayerInTrainingCourse)
        {
            if (Physics.Raycast(_camera.transform.position, spreadDirection, out _raycastHit, bulletRange, isTarget))
            {
                Debug.Log("Bullet hit: " + _raycastHit.collider.name);

                if (_raycastHit.collider.CompareTag("Target"))
                {
                    Debug.Log("Hit a target!");
                    //Guid guid = _raycastHit.collider.GetComponent<Target>().targetGuid;
                    //GameManager.Instance.TargetHit(guid, bulletDamage);
                    _raycastHit.collider.GetComponent<Target>().OnHitTriggerEvent(bulletDamage);
                }
            }
        }
        else
        {
            if (Physics.Raycast(_camera.transform.position, spreadDirection, out _raycastHit, bulletRange, isEnemy))
            {
                Debug.Log("Bullet hit: " + _raycastHit.collider.name);

                if (_raycastHit.collider.CompareTag("Enemy"))
                {
                    //Need to create an enemy script with a public function to take damage and reference it here
                    //_raycastHit.collider.GetComponent<enemyScript>().TakeDamage(bulletDamage);
                    Debug.Log("Hit an enemy!");
                }
            }
        }

        #endregion

        #region Camera Shake

        CinemachineShake.Instance.ShakeCamera(cameraShakeMagnitude, cameraShakeAmplitude);

        #endregion

        #region Visual Feedback: Bullet Hole & Muzzle Flash

        Instantiate(bulletHoleDecal, _raycastHit.point, Quaternion.Euler(0, 180, 0));
        Instantiate(muzzleFlash, muzzle.position, Quaternion.identity);

        #endregion

        bulletsRemaining--;
        bulletsFired--;

        Invoke("ResetShot", timeBetweenShooting);

        if (bulletsFired > 0 && bulletsRemaining > 0)
        {
            Invoke("FireGun", timeBetweenBullets);
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

        if (isReloading)
        {
            bulletsRemainingText.text = "Reloading...";
        }
        else
        {
            bulletsRemainingText.SetText(bulletsRemaining + " / " + magazineClipSize);
        }

        if (isPlayerInTrainingCourse)
        {
            //gameObject.SetActive(true);

            getCurrentCourseID = TrainingCourseManager.Instance.currentTrainingCourse;

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

            
    }
}
