using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignosVitales : MonoBehaviour
{
    public static SignosVitales signos;
    public Text pulso1, pulso2;
    public Text numeroPleth, numeroResp, numeroABP;
    private byte pulso, pleth, resp, sumaABP;
    private bool ciclo=true;
    private PhotonView PV;

    private void Awake()
    {
        if (SignosVitales.signos == null)
        {
            SignosVitales.signos = this;
        }
        else
        {
            if (SignosVitales.signos != this)
            {
                Destroy(SignosVitales.signos.gameObject);
                SignosVitales.signos = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        PV = GetComponent<PhotonView>();
    }
    // Update is called once per frame
    void Update()
    {
        if (ciclo) 
        {
            pulso = (byte)Random.Range(79, 81);
            pleth = (byte)Random.Range(97, 99);
            resp = (byte)Random.Range(31, 36);
            sumaABP = (byte)Random.Range(1, 10);
            PV.RPC("DatosPantalla", RpcTarget.AllBuffered, pulso, pleth, resp, sumaABP);
            ciclo = false; 
        }
    }
    
    [PunRPC]
    private void DatosPantalla(byte pulso, byte pleth, byte resp, byte sumaABP)
    {
        this.pulso=pulso;
        this.pleth=pleth;
        this.resp=resp;
        this.sumaABP=sumaABP;
        StartCoroutine(CambioSignos());        
    }

    private IEnumerator CambioSignos()
    {
        //Pulso        
        pulso1.text = pulso.ToString();
        pulso2.text = pulso.ToString();
        yield return new WaitForSecondsRealtime(Random.Range(2,5));
        //Pleth        
        numeroPleth.text = pleth.ToString();
        yield return new WaitForSecondsRealtime(Random.Range(2, 5));
        //Resp        
        numeroResp.text = resp.ToString();
        yield return new WaitForSecondsRealtime(Random.Range(1, 5));
        //ABP        
        numeroABP.text = $"{121+sumaABP}/{(byte)(82+sumaABP/2)}";
        yield return new WaitForSecondsRealtime(Random.Range(1, 5));
        ciclo = true;
    }    
}
