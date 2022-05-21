using UnityEngine;
using Photon.Pun;

public class TrazadoLinea : MonoBehaviour
{
    private LineRenderer linea=null;
    [SerializeField] private Material corteM,suturaM;
    private Vector3 posicion;
    private byte nCortes = 0;
    public bool contacto, corte;
    public bool inicio, final;
    public byte extremo=0;
    private byte puntoLinea = 0;
    private bool trazado;
    private PhotonView PV;

    private void Update()
    {
        if (!PV)
        {
            PV = GetComponent<PhotonView>();
        }
        if (corte)
        {                        
            if (extremo == 0)
            {
                if (linea == null) CrearLinea();

                linea.SetPosition(0, posicion);                
            }
            else if (extremo == 2 && linea)
            {
                if (!linea.TryGetComponent<MeshCollider>(out MeshCollider col))
                {
                    MeshCollider meshCollider = linea.gameObject.AddComponent<MeshCollider>();
                    Mesh mesh = new Mesh();
                    linea.BakeMesh(mesh,FindObjectOfType<Camera>(), true);
                    meshCollider.sharedMesh = mesh;
                }
                else 
                {
                    linea = null;
                    nCortes++;
                    corte = false;
                    puntoLinea = 0;
                }                
            }
            if (linea!=null && !trazado)
            {
                for (byte i = puntoLinea; i < linea.positionCount; i++)
                {
                    //linea.SetPosition(i, new Vector3(posicion.x,posicion.y-0.005f,posicion.z));                    
                    linea.SetPosition(i, posicion);                    
                }
                trazado = true;                
            }            
        }        
    }

    private void CrearLinea()
    {        
        if (corte)
        {
            linea = new GameObject(nCortes.ToString()).AddComponent<LineRenderer>();
            linea.transform.SetParent(this.transform);
            linea.tag = "Linea";
            linea.material = corteM;
            linea.startWidth = 0.003f;
            linea.endWidth = 0.003f;
        }
        linea.positionCount = 10;        
        linea.numCapVertices = 50;
        linea.useWorldSpace = false;

    }

    public void TrazadoCorte(float x, float y, float z, byte extremoCorte)
    {
        PV.RPC("TrazadoCorteRPC", RpcTarget.AllBuffered, x, y, z, extremoCorte);
        print($"x: {x}, y: {y}, z: {z}");
    }

    [PunRPC]
    public void TrazadoCorteRPC(float x, float y, float z, byte extremoCorte)
    {
        extremo = extremoCorte;
        this.posicion = new Vector3(x, y, z);
        puntoLinea++;
        trazado = false;        
        if (linea !=null && puntoLinea>=linea.positionCount-1)
        {
            puntoLinea = (byte)(linea.positionCount - 1);
        }        
    }

    public void TrazadoCorte(Vector3 posicion, byte extremoCorte)
    {
        extremo = extremoCorte;
        this.posicion = posicion;
        puntoLinea++;
        trazado = false;
        if (linea != null && puntoLinea >= linea.positionCount - 1)
        {
            puntoLinea = (byte)(linea.positionCount - 1);
        }
    }

    public void CambioColor(byte numeroCorte)
    {
        PV.RPC("CambioColorRPC", RpcTarget.AllBuffered, numeroCorte);
    }

    [PunRPC]
    private void CambioColorRPC(byte numeroCorte)
    {        
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Equals(numeroCorte.ToString(),System.StringComparison.OrdinalIgnoreCase))
            {
                transform.GetChild(i).GetComponent<LineRenderer>().material = suturaM;
                transform.GetChild(i).GetComponent<LineRenderer>().startWidth = 0.005f;
                transform.GetChild(i).GetComponent<LineRenderer>().endWidth = 0.005f;

                break;
            }
        }
    }
   
}
