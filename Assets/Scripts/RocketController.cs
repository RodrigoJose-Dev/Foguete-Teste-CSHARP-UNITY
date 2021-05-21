using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RocketController : MonoBehaviour
{
    /// <summary>
    /// Controle do Foguete
    /// </summary>
    
    //idéia: usar o rigidbody add force pra mover o foguete
            
    [Header("Config Teclado")] //escolher tecla do motor
    [SerializeField] KeyCode engineStartKey = KeyCode.Space;

    [Header("Base do Foguete")]
    [SerializeField] float dragForce = 20f;
    [SerializeField] float moveSpeed = 10f;
    private float horizontal;
    private float vertical;
    private Vector3 move;
    private GameObject baseGO;
    private Rigidbody rbBase;
    

    [Header("Detector de Altura")]
    [SerializeField] TextMeshProUGUI maxHeightNumberTxt;
    private GameObject heightDetectorGO;
    private float currentHeight;

    [Header("Força do Foguete")]
    [SerializeField] float rocketForce = 10f;

    //ponta do foguete
    private GameObject pontaGO;
    private Rigidbody rbPonta;

    //paraquedas
    private GameObject parachuteGO;
    private float parachuteTime = 3f;
                    
    //timer
    private float timer = 5f;

    //autorizar lançamento
    private bool startEngine = false;
    
    private void Awake()
    {
        baseGO = GameObject.Find("PrimeiroEstagio");
        pontaGO = GameObject.Find("Corpo_Nariz");
        heightDetectorGO = GameObject.Find("HeightDetector");
        parachuteGO = GameObject.Find("Paraquedas");
    }

    private void Start()
    {
        rbBase = baseGO.GetComponent<Rigidbody>();
        rbPonta = pontaGO.GetComponent<Rigidbody>();
        parachuteGO.SetActive(false);
    }

    private void FixedUpdate()
    {
        //Debug.Log(timer);

        EngineRunning();
        MaxHeight();
        Detach();

    }

    private void Update()
    {
        EngineStarting();

        #region Entrada do controle do foguete
        horizontal = Input.GetAxisRaw("Horizontal") * moveSpeed;
        vertical = Input.GetAxisRaw("Vertical") * moveSpeed;

        move = baseGO.transform.right * horizontal + baseGO.transform.forward * vertical;
        
        #endregion


    }

    void EngineStarting()
    {

        if (Input.GetKeyDown(engineStartKey))
        {
            if(startEngine == false)
            {
                startEngine = true;
            }
        }
    }

    void EngineRunning()
    {
        if (startEngine == true && timer > 0)
        {
            timer -= Time.fixedDeltaTime;
            rbBase.AddForce(Vector3.up * rocketForce);
            rbPonta.AddForce(Vector3.up * rocketForce);

            if (timer <= 0)
            {
                rbBase.AddForce(Vector3.up * 0);
                rbPonta.AddForce(Vector3.up * 0);
            }
        }
    }

    void MaxHeight()
    {

        if (rbBase.velocity.y > 0)
        {
            currentHeight = heightDetectorGO.transform.position.y;

            maxHeightNumberTxt.text = currentHeight.ToString("0m");
        }
   
    }

    void Detach()
    {
        if (heightDetectorGO.transform.position.y < currentHeight)
        {
            rbPonta.AddForce(Vector3.left * 2);
            OpenParachute();
        }
    }

    void OpenParachute()
    {
        parachuteTime -= Time.fixedDeltaTime;
                
        if (parachuteTime <= 0)
        {
            rbBase.drag = dragForce;
            rbBase.freezeRotation = true;
            parachuteGO.SetActive(true);
            RocketMove();
        }
        
    }

    void RocketMove()
    {
        rbBase.velocity = new Vector3(move.x, rbBase.velocity.y, move.z);
    }
    
}
