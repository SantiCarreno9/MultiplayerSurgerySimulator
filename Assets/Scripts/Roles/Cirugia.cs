using UnityEngine;
using Photon.Pun;

public class Cirugia : MonoBehaviour
{
    private InteraccionObjetos interaccion;
    //Control mano
    private Animator animator;        

    //Instrumento
    private GameObject objeto;
    private bool instrumentoAnimacion;
    private bool acciono;
    private string objetoNombre = "";

    //Lipoma
    private bool movLipoma, contactoLipoma;
    //Photon
    private PhotonView PV;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        transform.tag = PhotonRoom.room.miNumero.ToString();//Asignación número 
        animator = transform.GetChild(0).GetComponent<Animator>();
        interaccion = GetComponent<InteraccionObjetos>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            if (contactoLipoma)
            {
                if (animator.IsInTransition(0))
                {
                    objeto.GetComponent<Lipoma>().InteraccionLipoma(0);
                    movLipoma = true;
                }                    
            }
        }
    }

    //Animación mano
    public void AnimacionAgarrar()
    {
        if (objetoNombre.Equals("Bisturi", System.StringComparison.OrdinalIgnoreCase))
        { animator.SetInteger("agarrar", 1); }
        else if (objetoNombre.Equals("PinzaKellyRecta", System.StringComparison.OrdinalIgnoreCase) || objetoNombre.Equals("PinzaKellyCurva", System.StringComparison.OrdinalIgnoreCase) || objetoNombre.Equals("PortaAgujas", System.StringComparison.OrdinalIgnoreCase))
        { animator.SetInteger("agarrar", 2); instrumentoAnimacion = true; }
        else { animator.SetInteger("agarrar", 0);}
    }

    public void CogerObjeto()
    {
        //Hace al objeto hijo
        objeto.transform.SetParent(transform);
        objeto.GetComponent<ChangeOwnership>().Coger();
        for (int i = 0; i < objeto.transform.childCount; i++)
        {
            if (objeto.transform.GetChild(i).TryGetComponent<ChangeOwnership>(out ChangeOwnership co))
            {                
                co.Coger();
                objeto.transform.GetChild(i).localEulerAngles = new Vector3(39.811f, 6.985f, -4.41f);
                objeto.transform.GetChild(i).localPosition = Vector3.zero;
                objeto.transform.GetChild(i).localPosition = new Vector3(-0.023f, 0.0578f, 0.04f);
            }
        }
        animator.SetBool("agarro", true);
        objeto.GetComponent<ChangeOwnership>().CambioColor(false);
        if (objeto.CompareTag("Lipoma"))
        {
            objeto.transform.localPosition = Vector3.zero;
            objeto.transform.localPosition = new Vector3(0, 0.09f, 0.016f);
        }
    }
    #region Interacción objeto


    public void AnimacionInstrumento()
    {
        print(objeto.name);
        print(objeto.GetComponent<Pinzas>());
        if (instrumentoAnimacion)
            objeto.GetComponent<Pinzas>().PlayAnimaciones();

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
            interaccion.contacto = true;
            objeto = other.gameObject;
            objetoNombre = other.gameObject.name;            
        }
        else if (other.gameObject.tag.Equals("Lipoma", System.StringComparison.OrdinalIgnoreCase))
        {
            other.gameObject.GetComponent<ChangeOwnership>().CambioColor(true);
            interaccion.contacto = true;
            objeto = other.gameObject;
            objetoNombre = other.gameObject.name;
        }
        else if (other.gameObject.tag.Equals("Piel", System.StringComparison.OrdinalIgnoreCase))
        {            
            if (transform.childCount==1)
            {                
                if (other.gameObject.GetComponent<Lipoma>().corte)
                {                    
                    other.gameObject.GetComponent<Lipoma>().CambioColor(1);                    
                    objeto = other.gameObject;
                    interaccion.contacto = false;
                    contactoLipoma = true;
                }                             
            }            
        }
    }

    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Instrumento", System.StringComparison.OrdinalIgnoreCase))
        {
            if (!interaccion.cogio)
            {
                other.gameObject.GetComponent<ChangeOwnership>().CambioColor(false);
                interaccion.contacto = false;
                objetoNombre = "";
                acciono = false;
                objeto = null;
            }            
        }
        else if (other.gameObject.tag.Equals("Lipoma", System.StringComparison.OrdinalIgnoreCase))
        {
            other.gameObject.GetComponent<ChangeOwnership>().CambioColor(false);
            interaccion.contacto = false;
            objetoNombre = "";
            acciono = false;
            objeto = null;
        }
        else if (other.gameObject.tag.Equals("Piel", System.StringComparison.OrdinalIgnoreCase))
        {
            other.gameObject.GetComponent<Lipoma>().CambioColor(0);
            if (movLipoma)
            {                    
                objeto.GetComponent<Lipoma>().InteraccionLipoma(1);
                interaccion.contacto = false;
                objetoNombre = "";
                acciono = false;
                movLipoma = false;
                objeto = null;
                contactoLipoma = false;
            }          
        }        
    }
    #endregion
}
