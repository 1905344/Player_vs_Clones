using UnityEngine;

public class P3_rotatingLights : MonoBehaviour
{
    #region Variables

    [SerializeField] private float rotationSpeed;
    private Vector3 lightRotation;

    #endregion

    void Update()
    {
        lightRotation = new Vector3(0f, rotationSpeed, 0f);
        this.gameObject.transform.Rotate(lightRotation * Time.deltaTime);
    }
}
