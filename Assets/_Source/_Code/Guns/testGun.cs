using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testGun : MonoBehaviour
{
    #region Variables

    [SerializeField] private float gunDamage = 10f;
    [SerializeField] private float range = 100f;

    private Camera fpsCam;

    #endregion

    void Update()
    {
        if (InputManager.Instance.isPressingFireButton)
        {
            FireGun();
        }
    }

    void FireGun()
    {
        RaycastHit bulletHit;

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out bulletHit, range)) 
        { 
            if (GameManager.Instance.toggleDebug == true)
            {
                //To view the raycast in engine
                Debug.DrawLine(fpsCam.transform.position, bulletHit.point, Color.red, 5f);

                //Return the name of what the bullet has hit
                Debug.Log("Bullet hit: " + bulletHit.collider.name);
            }
        }
    }
}
