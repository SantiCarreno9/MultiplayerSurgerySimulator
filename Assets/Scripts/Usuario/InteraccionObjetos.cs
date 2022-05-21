using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InteraccionObjetos : MonoBehaviour, IPunObservable
{
    //Control mano
    private Animator animator;
    [HideInInspector]public bool coger, contacto, cogio;    

    //Roles
    private Cirugia cirugia=null;
    private Anestesia anestesia=null;
    //Photon
    private PhotonView PV;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        transform.tag = PhotonRoom.room.miNumero.ToString();//Asignación número 
        animator = transform.GetChild(0).GetComponent<Animator>();
        TryGetComponent(out cirugia);
        TryGetComponent(out anestesia);
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
                if (Input.GetMouseButtonDown(0)) coger = true;//Clic derecho
                if (coger)
                {
                    if (!contacto)
                    {
                        //Activar animación de agarrar
                        animator.SetInteger("agarrar", 0);
                        if (!animator.IsInTransition(0))
                            animator.SetBool("agarro", false);
                        coger = false;

                    }
                    else
                    {
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
            else //Si lo agarró
            {
                coger = false;
                if (Input.GetKeyDown(KeyCode.Space)) AnimacionInstrumento();
                if (Input.GetMouseButtonDown(1) && cogio) SoltarObjeto();
            }
            //Si no agarró un objeto (debe ser >1)
            if (transform.childCount == 1)
            {
                //cogio = false;                
                animator.SetBool("agarro", false);
                animator.SetInteger("accion", -1);
            }
            if (cogio)
            {
                if (transform.childCount == 1)
                {
                    GetComponent<CapsuleCollider>().enabled = true;
                    cogio = false;
                }
            }
            #endregion          
        }
    }

    //Animación mano
    private void AnimacionAgarrar()
    {
        if (cirugia) cirugia.AnimacionAgarrar();
        else if (anestesia) anestesia.AnimacionAgarrar();        
    }

    #region Interacción objeto

    private void CogerObjeto()
    {
        if (cirugia) cirugia.CogerObjeto();
        else if (anestesia) anestesia.CogerObjeto();
        cogio = true;
        GetComponent<CapsuleCollider>().enabled = false;
    }

    private void SoltarObjeto()
    {
        GetComponent<CapsuleCollider>().enabled = true;
        transform.GetChild(1).GetComponent<ChangeOwnership>().Soltar();
        cogio = false;
        animator.SetBool("agarro", false);
        animator.SetInteger("accion", -1);
    }

    private void AnimacionInstrumento()
    {
        if (cirugia) cirugia.AnimacionInstrumento();
        else if (anestesia) anestesia.AnimacionInstrumento();
    }
    
    #endregion

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(animator.GetInteger("agarrar"));//Transmite animación de mano para agarrar
            stream.SendNext(animator.GetBool("agarro"));//Transmite el contacto y agarre del objeto
            stream.SendNext(animator.GetInteger("accion"));//Transmite la interacción con el objeto
            stream.SendNext(cogio);//Transmite si se agarró un objeto
            stream.SendNext(transform.tag);//Transmite el número de usuario para ser identificado                        
        }
        else
        {
            animator.SetInteger("agarrar", (int)stream.ReceiveNext());
            animator.SetBool("agarro", (bool)stream.ReceiveNext());
            animator.SetInteger("accion", (int)stream.ReceiveNext());
            cogio = (bool)stream.ReceiveNext();//Transmite el contacto y agarre del objeto
            transform.tag = (string)stream.ReceiveNext();
        }
    }
}
