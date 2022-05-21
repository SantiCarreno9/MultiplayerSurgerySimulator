using UnityEngine;
using Photon.Pun;

public class ChangeOwnership : MonoBehaviourPunCallbacks
{
    [HideInInspector]public bool changeOwner;
    private Vector3 posicionInicial;
    
    //Estado del objeto
    private byte acomodado=0;    

    //Photon
    private PhotonView PV;

    private void Start()
    {                
        PV = GetComponent<PhotonView>();
        posicionInicial = transform.position;
    }    

    private void Update()
    {
        //Coger
        if (acomodado==1) 
        {
            if (TryGetComponent(out Bisturi bisturi)) bisturi.Acomodar(true);
            else if (TryGetComponent(out Pinzas pinza)) pinza.Acomodar(true);
            else if (TryGetComponent(out Jeringa jeringa)) jeringa.Acomodar(true);
            acomodado = 0; 
        }
        //Soltar
        else if (acomodado==2) 
        {
            if (TryGetComponent(out Bisturi bisturi)) bisturi.Acomodar(false);
            else if (TryGetComponent(out Pinzas pinza)) pinza.Acomodar(false);
            else if (TryGetComponent(out Jeringa jeringa)) jeringa.Acomodar(false);
            acomodado = 0;
        }
    }

    public void CambioColor(bool contacto)
    {
        Renderer[] Materiales = GetComponentsInChildren<Renderer>();
        if (contacto)
            foreach (Renderer color in Materiales)
            {
                color.material.EnableKeyword("_EMISSION");
                color.material.SetColor("_EmissionColor", new Color(1, 0.1f, 0.1f));
            }
        else
            foreach (Renderer color in Materiales) 
            {
                color.material.DisableKeyword("_EMISSION");
                color.material.SetColor("_EmissionColor", Color.black);
            }
    }

    public void Coger()
    {
        base.photonView.RequestOwnership();        
        PV.RPC("CambioParametros",RpcTarget.AllBuffered,true,transform.parent.tag,transform.parent.name);
        acomodado = 1;        
    }

    public void Soltar()
    {
        PV.RPC("CambioParametros", RpcTarget.AllBuffered, false,"", "");
        acomodado = 2;        
    }

    /// <summary>
    /// Se adapta en caso de ser agarrado o no
    /// </summary>
    /// <param name="Coger"></param>
    /// <param name="tagController"> Tag del objeto padre (número)</param>
    /// <param name="nameController"> Nombre del objeto padre</param>
    [PunRPC]
    private void CambioParametros(bool Coger, string tagController, string nameController)
    {        
        //Coger objeto
        if (Coger)
        {            
            GetComponent<Rigidbody>().useGravity = false;            
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<BoxCollider>().isTrigger = true;
            changeOwner = true;
            //Busca GameObjects con determinado número de usuario
            if (!tagController.Equals("Instrumento",System.StringComparison.OrdinalIgnoreCase))
            {
                GameObject[] controllerMano = GameObject.FindGameObjectsWithTag(tagController);
                for (byte i = 0; i < controllerMano.Length; i++)
                {
                    //Busca el objeto que lo está agarrando
                    if (controllerMano[i].name.Equals(nameController))
                    {
                        //Hace al objeto su padre
                        transform.SetParent(controllerMano[i].transform);
                        break;
                    }
                }
            }
            else
            {
                transform.SetParent(GameObject.Find(nameController).transform);
            }
        }
        //Soltar objeto
        else
        {
            GetComponent<BoxCollider>().isTrigger = false;
            GetComponent<Rigidbody>().isKinematic = false;            
            GetComponent<Rigidbody>().useGravity = true;
            transform.SetParent(null);
            changeOwner = false;            
        }
    }    

    public void Levantar()
    {
        PV.RPC("LevantarRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void LevantarRPC()
    {
        transform.position = posicionInicial;
    }
}
