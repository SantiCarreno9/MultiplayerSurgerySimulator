using UnityEngine;
using Photon.Pun;

public class Jeringa : MonoBehaviour
{
    private bool acciono;
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
        if (!acciono) animator.Play("Vaciar");
        else animator.Play("Llenar");
    }

    public void Acomodar(bool agarrado)
    {
        if (agarrado)
        {
            animator.enabled = false;
            transform.localEulerAngles = new Vector3(-180, 0, 0);
            transform.localPosition = Vector3.zero;
            transform.localPosition = new Vector3(-0.0192f, 0.188f, 0);
            animator.enabled = true;
        }
        else transform.localEulerAngles = new Vector3(-90, 0, 0);
    }
}
