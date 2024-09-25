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
    [SerializeField] public bool isFPSTesting = false;

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
        GameManager.Instance.TrainingCourseEnded += DisableGun;
        GameManager.Instance.FinishedTraining += DisableGunAfterTraining;

        if (!isFPSTesting)
        {
            isPlayerInTrainingCourse = false;

            canShoot = true;
            bulletsRemainingText.gameObject.SetActive(true);
            gameObject.SetActive(true);
        }
        else
        {
            isPlayerInTrainingCourse = true;
            isFPSTesting = false;

            canShoot = true;

            bulletsRemainingText.gameObject.SetActive(true);
            gameObject.SetActive(true);
        }

        updateGunPosition = true;
    }

    #region Enable and Disable The Gun

    public void EnableGun(int courseID)
    {
        isPlayerInTrainingCourse = true;
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
        isPlayerInTrainingCourse = false;
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
            isPlayerInTrainingCourse = false;
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
        //Gizmos.DrawLine(_camera.transform.position, _camera.transform.position + _camera.transform.forward * bulletRange);
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

        //Debug.DrawLine(_camera.transform.position, spreadDirection * bulletRange, Color.red,5f);

        if (isPlayerInTrainingCourse)
        {
            if (Physics.Raycast(_camera.transform.position, spreadDirection, out _raycastHit, bulletRange, isTarget))
            {
                //To view the raycast in engine
                Debug.DrawLine(_camera.transform.position, _raycastHit.point, Color.red, 5f);

                Debug.Log("Bullet hit: " + _raycastHit.collider.name);

                if (_raycastHit.collider.CompareTag("Target"))
                {
                    Debug.Log("Hit a target!");
                    Guid guid = _raycastHit.collider.GetComponent<Target>().targetGuid;
                    GameManager.Instance.TargetHit(guid, bulletDamage);
                    //_raycastHit.collider.GetComponent<Target>().OnHitTriggerEvent(bulletDamage);
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

        //Instantiate(bulletHoleDecal, _raycastHit.point, Quaternion.Euler(0, 180, 0));
        //Instantiate(muzzleFlash, muzzle.position, Quaternion.identity);

        CreateVisualFeedback(muzzleFlash, bulletHoleDecal);

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

    private void CreateVisualFeedback(GameObject flash, GameObject bulletHole)
    {
        if (muzzleFlashParent.childCount > 0)
        {
            GameObject child = muzzleFlashParent.GetChild(0).gameObject;
            DestroyImmediate(child);
        }

        if (bulletDecalParent.childCount > 0)
        {
            GameObject child = bulletDecalParent.GetChild(0).gameObject;
            DestroyImmediate(child);
        }

        Instantiate(flash, muzzle.position, Quaternion.identity, muzzleFlashParent);

        Instantiate(bulletHole, _raycastHit.point, Quaternion.Euler(0, 180, 0), bulletDecalParent);
    }

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

        if (isPlayerInTrainingCourse && !isFPSTesting)
        {
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
        else if (isFPSTesting && !isPlayerInTrainingCourse)
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
            
    }
}
