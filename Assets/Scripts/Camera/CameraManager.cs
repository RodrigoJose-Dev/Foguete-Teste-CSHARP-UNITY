using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    /// <summary>
    /// Gerenciamento da camera
    /// </summary>

    public static CameraManager Instance { get; private set; }

    [Header("Config Tecla de Troca de Camera")]
    KeyCode changeCameraKey = KeyCode.E;

    [SerializeField] Camera cam1;
    [SerializeField] Camera cam2;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        cam1.enabled = true;
        cam2.enabled = false;
    }

    private void Update()
    {
        cam1.transform.eulerAngles = new Vector3(0, 0, 0);
        
    }

    public void ChangeCam()
    {
        if (Input.GetKeyDown(changeCameraKey))
        {
            cam1.enabled = !cam1.enabled;
            cam2.enabled = !cam2.enabled;
        }
    }
}
