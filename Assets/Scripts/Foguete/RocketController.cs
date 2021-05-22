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
                
    [Header("Config Teclado")] //escolher tecla do motor
    [SerializeField] KeyCode engineStartKey = KeyCode.Space;
    [SerializeField] KeyCode changeRocketPosKey = KeyCode.Q;

    [Header("Base do Foguete")]
    [SerializeField] float dragForce = 1f;
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

    //Efeitos
    private ParticleSystem effectPS;
    
    private void Awake()
    {
        //gameobjects
        baseGO = GameObject.Find("PrimeiroEstagio");
        pontaGO = GameObject.Find("Corpo_Nariz");
        heightDetectorGO = GameObject.Find("HeightDetector");
        parachuteGO = GameObject.Find("Paraquedas");
        
        //efeitos
        effectPS = GetComponentInChildren<ParticleSystem>();

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
        Debug.Log(timer);
        EngineStarting();
        CameraManager.Instance.ChangeCam();

        #region Entrada do controle do foguete (input rocket)
        horizontal = Input.GetAxisRaw("Horizontal") * moveSpeed;
        vertical = Input.GetAxisRaw("Vertical") * moveSpeed;

        move = baseGO.transform.right * horizontal - baseGO.transform.up * vertical;
        
        #endregion

    }
    

    void EngineStarting()
    {

        if (Input.GetKeyDown(engineStartKey))
        {
            if(startEngine == false)
            {
                startEngine = true;
                effectPS.Play();
                SoundManager.Instance.StartRocketAudio();
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
                effectPS.Stop();
                SoundManager.Instance.StopRocketAudio();
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
    
    void ChangeRocketPosition()
    {
        if (Input.GetKeyDown(changeRocketPosKey))
        {
            //pontaGO
            //baseGO.transform.rotation = new Vector3(-38738, 90, -90);

        }
    }
}
