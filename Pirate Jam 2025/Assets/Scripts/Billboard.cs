using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera theCam;

    [Range(0f, 1f)]
    public float limitRotation = 0f;

    Quaternion originalRotation;

    void Awake()
    {
        theCam = Camera.main;
    }

    private void Start()
    {
        originalRotation = transform.rotation;
    }

    void LateUpdate()
    {
        //transform.LookAt(theCam.transform);
        //transform.rotation = Quaternion.Euler(-transform.rotation.eulerAngles.x, 0f, 0f);

        Quaternion targetRotation = Camera.main.transform.rotation;
        transform.rotation = Quaternion.Lerp(originalRotation, targetRotation, 1f - limitRotation);
    }
}
