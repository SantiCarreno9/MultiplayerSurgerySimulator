using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Anestesia : MonoBehaviour
{
    private InteraccionObjetos interaccion;

    //Control mano
    private Animator animator;

    //Instrumento
    private GameObject objeto;
    private bool instrumentoAnimacion;
    private bool acciono;
    private string objetoNombre = "";
    
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

    }

    //Animación mano
    public void AnimacionAgarrar()
    {
        if (objetoNombre.Equals("Jeringa", System.StringComparison.OrdinalIgnoreCase))
        { animator.SetInteger("agarrar", 1); instrumentoAnimacion = true; }        
        else { animator.SetInteger("agarrar", 0); }
    }

    public void CogerObjeto()
    {
        //Hace al objeto hijo
        objeto.transform.SetParent(transform);
        objeto.GetComponent<ChangeOwnership>().Coger();
        animator.SetBool("agarro", true);
        objeto.GetComponent<ChangeOwnership>().CambioColor(false);
    }
    #region Interacción objeto


    public void AnimacionInstrumento()
    {
        if (instrumentoAnimacion)
            objeto.GetComponent<Jeringa>().PlayAnimaciones();

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
        if (other.gameObject.tag.Equals("Anestesia", System.StringComparison.OrdinalIgnoreCase))
        {
            other.gameObject.GetComponent<ChangeOwnership>().CambioColor(true);
            interaccion.contacto = true;
            objeto = other.gameObject;
            objetoNombre = other.gameObject.name;
        }        
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Anestesia", System.StringComparison.OrdinalIgnoreCase))
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
    }
    #endregion
}
