using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{

    /// <summary>
    /// Classe controladora da camera
    /// </summary>
    
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        cam.transform.eulerAngles = new Vector3(0, 0, 0);
    }
}
