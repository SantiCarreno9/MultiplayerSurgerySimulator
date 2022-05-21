using UnityEngine;
using UnityEngine.Animations.Rigging;
using Photon.Pun;

public class ManoAuto : MonoBehaviour, IPunObservable
{
    //Movimiento
    [SerializeField] private Rig rig;            
    private Vector3 posInicial, delta, posFinal;
    private sbyte xsigno, ysigno, zsigno;
    private byte cont = 0;

    //Interacción con objeto
    private bool agarrar, agarro, soltar;
    [HideInInspector]public bool tieneObjetos;
    private Transform objeto;

    //Photon
    private PhotonView PV;

    void Start()
    {
        PV = GetComponent<PhotonView>();
        posInicial = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);        
        posFinal = new Vector3(0f, 1.3f, 0.26f);
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            #region Movimiento para agarrar objeto
            if (agarrar)
            {
                //Conteo para limitar movimiento
                cont++;
                //Cálculo distancia restante
                delta = objeto.position - transform.position;
                //Asignación signo para la dirección del movimiento en cada eje
                if (delta.x < 0) xsigno = 1;
                else xsigno = -1;
                if (delta.y < 0) ysigno = 1;
                else ysigno = -1;
                zsigno = (sbyte)(xsigno * ysigno);          

                transform.Translate(xsigno*delta.x/50, ysigno * delta.y/50, zsigno*delta.z/50, Space.Self);
                if (cont>=250)
                {
                    //Vuelta a posición de reposo
                    transform.localPosition = posInicial;
                    agarrar = false;
                    cont = 0;
                    rig.weight = 0;
                }
            }
            #endregion
            #region Movimiento para soltar objeto
            if (soltar)
            {
                //Conteo para limitar movimiento
                cont++;
                //Cálculo distancia restante
                delta = posFinal - transform.localPosition;
                //Asignación signo para la dirección del movimiento en cada eje
                if (delta.x < 0) xsigno = 1;
                else xsigno = -1;
                if (delta.y < 0) ysigno = 1;
                else ysigno = -1;
                zsigno = (sbyte)(xsigno * ysigno);

                transform.Translate(xsigno * delta.x / 50, ysigno * delta.y / 50, zsigno * delta.z / 50, Space.Self);

                if (cont>=250)
                {
                    while (transform.childCount>1)
                    {
                        //Soltar objetos agarrados
                        for (byte i = 1; i < transform.childCount; i++)
                        { transform.GetChild(i).GetComponent<ChangeOwnership>().Soltar(); }
                    }                                                            
                    //Vuelta a posición de reposo
                    transform.localPosition = posInicial;
                    soltar = false;
                    cont = 0;
                    rig.weight = 0;
                }
            }
            #endregion
            
            if (transform.childCount>1) tieneObjetos = true;
            else tieneObjetos = false;



        }
    }

    public void AgarrarObjeto(Transform objeto)
    {
        this.objeto = objeto;
        rig.weight = 1;
        agarrar = true;
        cont = 0;
    }

    public void SoltarObjeto()
    {
        soltar = true;
        cont = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Instrumento"))
        {                        
            if (other.gameObject.name.Equals(objeto.name,System.StringComparison.OrdinalIgnoreCase))
            {
                agarrar = false;                
                other.transform.SetParent(transform);
                other.GetComponent<ChangeOwnership>().Coger();
                transform.localPosition = posInicial;
                rig.weight = 0;
            }            
        }
        //else
        //{
        //    if (other.gameObject.CompareTag("Mesa"))
        //    {
        //        if (other.gameObject.name.Equals(objeto.name, System.StringComparison.OrdinalIgnoreCase))
        //        {
        //            soltar = false;
        //            for (byte i = 1; i < transform.childCount; i++)
        //            {
        //                transform.GetChild(i).GetComponent<ChangeOwnership>().Soltar();
        //            }                                                                                 
        //        }
        //    }
        //}
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext((byte)rig.weight);
        else
            this.rig.weight = (byte)stream.ReceiveNext();
    }
}
