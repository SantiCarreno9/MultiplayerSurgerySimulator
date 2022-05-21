using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;
using TMPro;

public class MostrarMenu : MonoBehaviour
{
    [SerializeField] private Transform canvas;
    [SerializeField] private Transform[] textos;    
    [SerializeField] private TextMeshProUGUI textoMicrofono;
    [SerializeField] private Transform indicacionRecoger;
    [HideInInspector] public bool levantar;
    private byte idioma=0;
    private bool escape, activar=true;
    // Start is called before the first frame update
    void OnEnable()
    {
        idioma = PhotonRoom.room.idioma;
        if (GetComponent<PhotonView>().IsMine)
        {
            canvas.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<PhotonView>().IsMine)
        {            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                escape = !escape;                       
            }
            if (Input.GetKey(KeyCode.Q) && !escape)
            {
                for (byte i = 0; i < textos.Length; i++)
                {
                    if (i > 1 && i < 4)
                    {
                        ActivarGUI(textos[i].parent.GetComponent<CanvasGroup>(), true);
                        ActivarGUI(textos[i].GetChild(idioma).GetComponent<CanvasGroup>(), true);
                    }
                    else
                    {
                        ActivarGUI(textos[i].parent.GetComponent<CanvasGroup>(), false);
                        ActivarGUI(textos[i].GetChild(idioma).GetComponent<CanvasGroup>(), false);
                    }
                }
            }
            else if (escape)
            {                
                for (byte i = 0; i < textos.Length; i++)
                {
                    if (i >= 4)
                    {
                        ActivarGUI(textos[i].parent.GetComponent<CanvasGroup>(), true);
                        ActivarGUI(textos[i].GetChild(idioma).GetComponent<CanvasGroup>(), true);
                    }
                    else
                    {
                        ActivarGUI(textos[i].parent.GetComponent<CanvasGroup>(), false);
                        ActivarGUI(textos[i].GetChild(idioma).GetComponent<CanvasGroup>(), false);
                    }
                }
            }
            else
            {
                for (byte i = 0; i < textos.Length; i++)
                {
                    if (i < 2)
                    {
                        ActivarGUI(textos[i].parent.GetComponent<CanvasGroup>(), true, 0.5f);
                        ActivarGUI(textos[i].GetChild(idioma).GetComponent<CanvasGroup>(), true);
                    }
                    else
                    {
                        ActivarGUI(textos[i].parent.GetComponent<CanvasGroup>(), false);
                        ActivarGUI(textos[i].GetChild(idioma).GetComponent<CanvasGroup>(), false);
                    }
                }
            }
            if (escape) { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
            else { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
        }
        
    }

    public void ActivarGUI(CanvasGroup gui, bool activo, float alpha=1)
    {
        if (activo) gui.alpha = alpha;
        else gui.alpha = 0;        
    }

    public void Salir()
    {
        Application.Quit();
    }

    public void Volver()
    {
        escape = false;
    }

    public void Microfono()
    {
        activar = !activar;
        GetComponent<Recorder>().TransmitEnabled = activar;
        if (activar) textoMicrofono.text = "ON";
        else textoMicrofono.text = "OFF";
    }

    public void MostrarInstruccionRecoger(bool mostrar)
    {        
        ActivarGUI(indicacionRecoger.parent.GetComponent<CanvasGroup>(), mostrar, 0.5f);
        ActivarGUI(indicacionRecoger.GetChild(idioma).GetComponent<CanvasGroup>(), mostrar);        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Instrumento"))
        {
            MostrarInstruccionRecoger(true);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Instrumento"))
        {
            if (Input.GetKey(KeyCode.Space))
            {
                other.GetComponent<ChangeOwnership>().Levantar();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Instrumento"))
        {
            MostrarInstruccionRecoger(false);
        }
    }
}
