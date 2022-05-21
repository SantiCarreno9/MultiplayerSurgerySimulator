using UnityEngine;
using Photon.Pun;
using UnityEngine.Rendering.Universal;

public class Usuario : MonoBehaviourPunCallbacks, IPunObservable
{
    //Movimiento
    private CharacterController controller;
    private Vector3 move;
    private float z, x;
    private Animator anim;

    //Visión
    [SerializeField] private Camera camara;
    [SerializeField] private AudioListener AL;
    private float MouseX, MouseY, xRotation = 0;
    private bool camaraOn = true;
    private InteraccionObjetos interaccion;

    //Photon
    private PhotonView PV;

    void Start()
    {
        //Movimiento
        controller = GetComponent<CharacterController>();
        anim = transform.GetChild(1).GetComponent<Animator>();

        //Visión
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Photon
        PV = transform.GetComponent<PhotonView>();
        if (!PV.IsMine)
        {
            //Configuración inicial
            Destroy(camara.GetUniversalAdditionalCameraData());
            Destroy(camara);
            Destroy(AL);
        }

        interaccion = GetComponentInChildren<InteraccionObjetos>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!anim)
        {
            anim = transform.GetChild(1).GetComponent<Animator>();
        }
        if (PV.IsMine)
        {
            //Movimiento
            z = Input.GetAxis("Vertical");
            x = Input.GetAxis("Horizontal");
            move = transform.forward * z + transform.right * x;
            controller.Move(move * Time.deltaTime * 2f);
            anim.SetFloat("Vy", z);
            anim.SetFloat("Vx", x);

            //Visión                                    
            if (Input.GetKeyDown(KeyCode.M)) camaraOn = !camaraOn;

            if (Cursor.lockState != CursorLockMode.None)
            {
                if (camaraOn)
                {
                    MouseX = Input.GetAxis("Mouse X") * 200f * Time.deltaTime;
                    MouseY = Input.GetAxis("Mouse Y") * 200f * Time.deltaTime;
                    xRotation -= MouseY;
                    xRotation = Mathf.Clamp(xRotation, -90f, 80f);
                    camara.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                    transform.Rotate(Vector3.up * MouseX);
                }
            }

            //if (!camaraOn && interaccion.cogio)
            //{
            //    camara.transform.LookAt(interaccion.transform);
            //}
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Transmisión de animación de movimiento
        if (stream.IsWriting)
        {
            stream.SendNext(anim.GetFloat("Vy"));
            stream.SendNext(anim.GetFloat("Vx"));
        }
        else
        {
            this.anim.SetFloat("Vy", (float)stream.ReceiveNext());
            this.anim.SetFloat("Vx", (float)stream.ReceiveNext());
        }
    }
}
