using Unity.VisualScripting;
using UnityEngine;

public class GunplayManager : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform bulletHitPoint;
    [SerializeField] private RaycastHit _raycastHit;
    [SerializeField] private LayerMask isEnemy;
    private InputManager inputManager;

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
        if (allowFireButtonHold)
        {
            //Need to get the input manager to return the correct interaction type for holding the fire button
        }
        else
        {
            //Need to get the input manager to return the correct interaction type for tapping the fire button
        }

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

        Vector3 spreadDirection = _camera.transform.forward + new Vector3(x, y, 0);

        #endregion

        #endregion

        #region Raycast for Bullets

        if (Physics.Raycast(_camera.transform.position, spreadDirection, out _raycastHit, bulletRange, isEnemy))
        {
            Debug.Log("Bullet hit: " + _raycastHit.collider.name);

            if (_raycastHit.collider.CompareTag("Enemy"))
            {
                //Need to create an enemy script with a public function to take damage and reference it here
                //_raycastHit.collider.GetComponent<enemyScript>().TakeDamage(bulletDamage);
            }
        }

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
    }
}
