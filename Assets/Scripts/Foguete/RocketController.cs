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
    [SerializeField] float moveSpeed = 20f; //velocidade do controle do foguete
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
    [SerializeField] float rocketForce = 10f; //velocidade do lançamento do foguete

    //ponta do foguete
    private GameObject pontaGO;
    private Rigidbody rbPonta;

    //paraquedas
    private GameObject parachuteGO;
    private float parachuteTime = 3f; //tempo para ativar o paraquedas
                    
    //tempo do combustível
    private float timer = 5f;

    //autorizar lançamento
    private bool startEngine = false;

    //Efeitos
    private ParticleSystem effectPS;

    //alterar tipo de lançamento
    private bool ballisticLaunch = false;
    private bool authorizedLaunch = true;
    
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
        //rigidbody
        rbBase = baseGO.GetComponent<Rigidbody>();
        rbPonta = pontaGO.GetComponent<Rigidbody>();

        parachuteGO.SetActive(false);
    }

    private void FixedUpdate()
    {
        
        EngineRunning(); //propulsor operando
        MaxHeight(); //altura máxima
        Detach(); //separando as 2 partes do foguete
        
    }

    private void Update()
    {
        #region Entrada do controle do foguete (input rocket)
        horizontal = Input.GetAxisRaw("Horizontal") * moveSpeed;
        vertical = Input.GetAxisRaw("Vertical") * moveSpeed;

        move = baseGO.transform.right * horizontal - baseGO.transform.up * vertical;

        #endregion
        EngineStarting(); //ligando propulsor
        CameraManager.Instance.ChangeCam(); //alternar camera
        ChangeRocketPos(); //alternar tipo de lançamento
        
        
    }

    void EngineStarting() //ligando o propulsor
    {

        if (Input.GetKeyDown(engineStartKey)) //tecla que liga o propulsor
        {
            if(startEngine == false)
            {
                startEngine = true;
                authorizedLaunch = false;
                effectPS.Play(); //ligando efeito do propulsor
                SoundManager.Instance.StartRocketAudio(); //ligando som do foguete
            }
        }
    }

    void EngineRunning() //propulsor operando
    {
        if (ballisticLaunch == false)
        {  
            if (startEngine == true && timer > 0) //alterando força caso o seja um lançamento normal
            {
                timer -= Time.fixedDeltaTime;
                rbBase.AddForce(Vector3.up * rocketForce);
                rbPonta.AddForce(Vector3.up * rocketForce);

                if (timer <= 0)//desligando propulsor após 5 segundos
                {
                    effectPS.Stop();
                    SoundManager.Instance.StopRocketAudio();
                    rbBase.AddForce(0f, 0f, 0f);
                }
            }
        }
        else
        {
            if (startEngine == true && timer > 0) //alterando força caso o seja um lançamento balístico
            {
                timer -= Time.fixedDeltaTime;

                rbBase.isKinematic = false;
                rbPonta.isKinematic = false;

                rbBase.freezeRotation = true;
                rbPonta.freezeRotation = true;
                
                rbBase.AddForce(1f * rocketForce, 1f * rocketForce, 0f);
               
                if (timer <= 0)//desligando propulsor após 5 segundos
                {
                    effectPS.Stop();
                    SoundManager.Instance.StopRocketAudio();
                    rbBase.AddForce(0f, 0f, 0f);
                }
            }
        }
        
    }

    void MaxHeight() //Detector de altura máxima
    {

        if (rbBase.velocity.y > 0)
        {
            currentHeight = heightDetectorGO.transform.position.y;

            maxHeightNumberTxt.text = currentHeight.ToString("0m");
        }


   
    }

    void Detach() //Desacoplar as 2 partes do foguete
    {
        if (heightDetectorGO.transform.position.y < currentHeight)
        {
            rbPonta.AddForce(Vector3.left * 2);
            OpenParachute();
        }
    }

    void OpenParachute() //paraquedas
    {
        parachuteTime -= Time.fixedDeltaTime;
                
        if (parachuteTime <= 0) //abrindo paraquedas após desacoplar em 3 segundos
        {
            rbBase.drag = dragForce; //adicionando drag após o paraquedas aberto
            Vector3 rotation = new Vector3(transform.rotation.x, transform.rotation.y,
                0f);
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.Euler(rotation), Time.deltaTime);
                
            rbBase.freezeRotation = true;
            parachuteGO.SetActive(true);
            RocketMove();
        }
        
    }

    void RocketMove() //controlar o foguete após o paraquedas abrir
    {
        rbBase.velocity = new Vector3(move.x, rbBase.velocity.y, move.z);
        StopMove();
    }

    void StopMove() //parar movimentação do foguete ao chegar no solo
    {
        if (rbBase.velocity.y == 0)
        {
            rbBase.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    void ChangeRocketPos()//alterar o tipo de lançamento
    {
        if (authorizedLaunch == true) //se o lançamento estiver autorizado, pode mudar o tipo de lançamento
        {
            //lançamento balístico
            if (Input.GetKeyDown(changeRocketPosKey) && ballisticLaunch == false)
            {
                rbBase.isKinematic = true;
                rbPonta.isKinematic = true;
                gameObject.transform.position = new Vector3(193.61f, 0.94f, 237.3542f);
                gameObject.transform.eulerAngles = new Vector3(0f, 0f, -53.93f);
                ballisticLaunch = true;
            }//lançamento normal
            else if (Input.GetKeyDown(changeRocketPosKey) && ballisticLaunch == true)
            {
                rbBase.isKinematic = false;
                rbPonta.isKinematic = false;
                gameObject.transform.position = new Vector3(203.7922f, 0.07f, 237.3542f);
                gameObject.transform.eulerAngles = new Vector3(0f, 0f, 0f);
                ballisticLaunch = false;
            }
        }
    }
}
