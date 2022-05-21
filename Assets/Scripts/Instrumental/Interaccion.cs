using UnityEngine;
using Photon.Pun;
using UnityEngine.Animations.Rigging;
using UnityEngine.Rendering;

public class Interaccion : MonoBehaviour, IPunObservable
{    
    //Control mano
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject rigs;    
    private bool coger, cogio, soltar, contacto;

    //Instrumento
    private GameObject instrumento;
    private bool instrumentoAnimacion;
    private bool acciono;
    private string herramienta="";

    //Photon
    private PhotonView PV;
    
    private void Start()
    {
        PV=GetComponent<PhotonView>();
        transform.tag = PhotonRoom.room.miNumero.ToString();//Asignación número 
    }    

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {                                              
            #region Interacción Objeto
            //Si no agarró un objeto           
            if (!cogio)
            {
                if (Input.GetMouseButtonDown(0)) coger = true;
                if (coger)
                {
                    if (!contacto)
                    {
                        //Activar animación de agarrar
                        animator.SetInteger("agarrar", 0);
                        animator.SetBool("agarro", false);
                        coger = false;
                    }
                    else
                    {
                        //Activar animación específica
                        AnimacionAgarrar();
                        CogerObjeto();                        
                    }
                }
                else
                {
                    //Si no está en animación, activar valor por default
                    if (!animator.IsInTransition(0)) animator.SetInteger("agarrar", -1);
                }                               
            }
            else 
            {
                coger = false;                
                if (Input.GetKeyDown(KeyCode.Space)) AnimacionInstrumento();
                if (Input.GetMouseButtonDown(1) && cogio) SoltarObjeto();
            }
            //Si no agarró un objeto (debe ser >1)
            if (transform.childCount==1)
            {
                cogio = false;
                animator.SetBool("agarro", false);
                animator.SetInteger("accion", -1);
            }
            #endregion          
        }
    }

    //Animación mano
    private void AnimacionAgarrar()
    {
        if (herramienta.Equals("Bisturi", System.StringComparison.OrdinalIgnoreCase))
        { animator.SetInteger("agarrar", 1); }
        else if (herramienta.Equals("PinzaKellyRecta", System.StringComparison.OrdinalIgnoreCase) || herramienta.Equals("PinzaKellyCurva", System.StringComparison.OrdinalIgnoreCase))
        { animator.SetInteger("agarrar", 2); instrumentoAnimacion = true; }
    }

    #region Interacción objeto

    private void CogerObjeto()
    {
        //Hace al objeto hijo
        instrumento.transform.SetParent(transform);        
        instrumento.GetComponent<ChangeOwnership>().Coger();        
        cogio = true;
        animator.SetBool("agarro", true);
        instrumento.GetComponent<ChangeOwnership>().CambioColor(false);
    }

    private void SoltarObjeto()
    {
        transform.GetChild(1).GetComponent<ChangeOwnership>().Soltar(); 
        cogio = false;
        animator.SetBool("agarro", false);
        animator.SetInteger("accion", -1);
    }    

    private void AnimacionInstrumento()
    {
        if (instrumentoAnimacion)
            instrumento.GetComponent<Pinzas>().PlayAnimaciones();

        if (!acciono)
        {            
            animator.SetInteger("accion", 1);            
        }
        else 
        {            
            animator.SetInteger("accion", 0);            
        }        
        acciono = !acciono;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Instrumento", System.StringComparison.OrdinalIgnoreCase))
        {
            other.gameObject.GetComponent<ChangeOwnership>().CambioColor(true);
            contacto = true;
            instrumento = other.gameObject;
            herramienta = other.gameObject.name;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Instrumento", System.StringComparison.OrdinalIgnoreCase))
        {
            other.gameObject.GetComponent<ChangeOwnership>().CambioColor(false);
            contacto = false;
            herramienta = "";
            acciono = false;
        }
    }
    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(animator.GetInteger("agarrar"));//Transmite animación de mano para agarrar
            stream.SendNext(animator.GetBool("agarro"));//Transmite el contacto y agarre del objeto
            stream.SendNext(animator.GetInteger("accion"));//Transmite la interacción con el objeto
            stream.SendNext(transform.tag);//Transmite el número de usuario para ser identificado
        }
        else
        {            
            animator.SetInteger("agarrar", (int)stream.ReceiveNext());            
            animator.SetBool("agarro", (bool)stream.ReceiveNext());            
            animator.SetInteger("accion", (int)stream.ReceiveNext());
            transform.tag = (string)stream.ReceiveNext();            
        }
    }
    
}
