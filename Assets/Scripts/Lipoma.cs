using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using Photon.Pun;

public class Lipoma : MonoBehaviour
{    
    public float sizeMultiplier;
    public Transform lipoma;
    private bool movimiento, retiroLipoma;
    private Mesh mesh;
    private PhotonView PV;
    [HideInInspector]public bool corte;
    public Vector3[] puntosCorte= new Vector3[100];   
    private byte contCortes=0;
    //public Transform transform;

    public Lipoma(float sizeMultiplier)
    {
        this.sizeMultiplier = sizeMultiplier;        
    }    

    public void CambioColor(byte estado)
    {                
        if (estado==1)//Highlight
            GetComponent<Renderer>().material.SetColor("Emision",Color.blue);
        else if (estado == 2)//Seleccionado
            GetComponent<Renderer>().material.SetColor("Emision", Color.red);
        else
            GetComponent<Renderer>().material.SetColor("Emision", Color.black);
    }

    private void Start()
    {        
        mesh = GetComponent<MeshFilter>().mesh;               
        CortarPiel(100);
        PV = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!retiroLipoma)
        {
            if (transform.parent.childCount == 1)
            {                
                transform.Translate(0, 0, -0.015f);
                retiroLipoma = true;
            }
        }

    }

    public void CortarPiel(int index)
    {
        Destroy(transform.GetComponent<MeshCollider>());        
        Mesh mesh = transform.GetComponent<MeshFilter>().mesh;
        int[] oldTriangles = mesh.triangles;
        int[] newTriangles = new int[mesh.triangles.Length - 3];
        
        int i = 0;
        int j = 0;
        while (j < mesh.triangles.Length)
        {
            if (j != index * 3)
            {
                newTriangles[i++] = oldTriangles[j++];
                newTriangles[i++] = oldTriangles[j++];
                newTriangles[i++] = oldTriangles[j++];
            }
            else
            {
                j += 3;
            }
        }        
        transform.GetComponent<MeshFilter>().mesh.triangles = newTriangles;
        gameObject.AddComponent<MeshCollider>();        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="accion">0 para mover, 1 para habilitar</param>
    public void InteraccionLipoma(byte accion)
    {
        if (accion == 0)
            PV.RPC("MoverLipoma", RpcTarget.AllBuffered);
        else PV.RPC("HabilitarLipoma", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void MoverLipoma()
    {
        if (!movimiento)
        {
            lipoma.Translate(0, 0, 0.01f);            
            movimiento = true;
            GetComponent<SonidosCirugia>().PlaySonido(1);
        }        
    }

    [PunRPC]
    public void HabilitarLipoma()
    {
        lipoma.GetComponent<BoxCollider>().enabled = true;
        transform.GetComponent<MeshCollider>().enabled = false;
    }
}
