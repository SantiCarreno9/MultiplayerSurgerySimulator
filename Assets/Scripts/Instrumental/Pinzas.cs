using UnityEngine;
using Photon.Pun;

public class Pinzas : MonoBehaviour
{ 
    [HideInInspector] public bool acciono;
    private Animator animator;
    private PhotonView PV;

    // Start is called before the first frame update
    private void Start()
    {        
        animator = GetComponent<Animator>();
        PV = GetComponent<PhotonView>();
    }

    public void PlayAnimaciones()
    {
        PV.RPC("PlayAnimacionesRPC", RpcTarget.AllBuffered, acciono);
        acciono = !acciono;
    }

    [PunRPC]
    private void PlayAnimacionesRPC(bool acciono)
    {
        if (!acciono) animator.Play("Abrir");
        else animator.Play("Cerrar");
    }

    public void Acomodar(bool agarrado)
    {
        if (agarrado)
        {            
            animator.enabled = false;            
            transform.localEulerAngles = new Vector3(-32.38f, 19, -111.66f);
            transform.localPosition = Vector3.zero;
            transform.localPosition = new Vector3(-0.051f, 0.179f, -0.047f);
            animator.enabled = true;                       
        }
        else transform.localEulerAngles = new Vector3(0, 0, 0);
    }

}
