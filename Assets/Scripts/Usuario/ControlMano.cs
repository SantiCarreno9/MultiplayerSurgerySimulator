using UnityEngine;
using Photon.Pun;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

public class ControlMano : MonoBehaviourPunCallbacks, IPunObservable
{
    // Start is called before the first frame update       
    [SerializeField] private Rig rig;    
    private PhotonView PV;
    private float mouseX, mouseY, movX = 0, movZ = 0, movY=1.3f;
    private float handRotation = 0;
    private bool movOn, primerActivacion;    

    void Start()
    {        
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {        
        if (PV.IsMine)
        {                                    
            if (Input.GetKeyDown(KeyCode.M))
            {
                rig.weight = 1;
                if (!primerActivacion)
                { GetComponent<CapsuleCollider>().enabled = true; primerActivacion = true; }
                movOn = !movOn;
            }                        
            if (movOn)
            {                             
                mouseX = Input.GetAxis("Mouse X");
                mouseY = Input.GetAxis("Mouse Y");
                //Habilita movimiento hacia el frente y lados
                if (!Input.GetKey(KeyCode.Z) && !Input.GetKey(KeyCode.F))
                {
                    movX += mouseX / 50;
                    movX = Mathf.Clamp(movX, -0.2f, 0.34f);//Limitación movimiento lados

                    movZ += mouseY / 50;
                    movZ = Mathf.Clamp(movZ, 0.2f, 0.335f);//Limitación movimiento frente

                    //Rotación mano a medida que se mueve en los lados
                    handRotation += mouseX * 3;
                    handRotation = Mathf.Clamp(handRotation, -45f, 25f);
                    transform.localEulerAngles = new Vector3(90, handRotation, 0);
                }
                else if (Input.GetKey(KeyCode.F))
                {
                    //Habilita el movimiento frente
                    movZ += mouseY / 50;
                    movZ = Mathf.Clamp(movZ, 0.2f, 0.335f);//Limitación movimiento frente
                }
                else if(Input.GetKey(KeyCode.Z))
                {
                    //Habilita el movimiento vertical
                    movY += mouseY / 50;
                    movY = Mathf.Clamp(movY, 1.05f, 1.4f);//Limitación movimiento vertical                    
                }
                transform.localPosition = new Vector3(movX, movY, movZ);                
            }            
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext((byte)rig.weight);            
        }
        else
        {
            this.rig.weight = (byte)stream.ReceiveNext();            
        }
        
    }
}


