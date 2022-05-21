using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Voice.Unity;
using Photon.Pun;
using UnityEngine.Rendering.Universal;
using Unity.Mathematics;

public class Instructor : MonoBehaviour
{
    private bool activar;    
    private CharacterController controller;    
    [SerializeField] private Camera camara;
    [SerializeField] private AudioListener AL;
    private bool camaraOn = true;
    private Vector3 move;
    private float z, x, MouseX, MouseY, xRotation = 0;    

    //Lipoma
    private RaycastHit raycastHit;
    private Ray laser;    
    public GameObject lipoma, lipomaCopia;
    private bool crearLipoma, modificarLipoma;
    private bool lipomaCreado;
    //Photon
    private PhotonView PV;
    void Start()
    {
        //Movimiento
        controller = GetComponent<CharacterController>();

        //Visión
        Cursor.lockState = CursorLockMode.Locked;

        //Photon
        PV = transform.GetComponent<PhotonView>();
        
        if (!PV.IsMine)
        {
            //Configuración inicial
            Destroy(camara.GetUniversalAdditionalCameraData());
            Destroy(camara);
            Destroy(AL);
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(true);
            controller.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (PV.IsMine)
        {
            #region Básico
            //Movimiento
            z = Input.GetAxis("Vertical");
            x = Input.GetAxis("Horizontal");
            move = transform.forward * z + transform.right * x;
            controller.Move(move * Time.deltaTime * 2f);

            //Visión                                    
            if (Input.GetKeyDown(KeyCode.L)) 
            { 
                camaraOn = !camaraOn;
                if (Cursor.lockState != CursorLockMode.Locked)
                    Cursor.lockState = CursorLockMode.Locked;
                else Cursor.lockState = CursorLockMode.None;
            }

            if (camaraOn)
            {
                MouseX = Input.GetAxis("Mouse X") * 200f * Time.deltaTime;
                MouseY = Input.GetAxis("Mouse Y") * 200f * Time.deltaTime;
                xRotation -= MouseY;
                xRotation = Mathf.Clamp(xRotation, -90f, 80f);
                camara.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
                transform.Rotate(Vector3.up * MouseX);
            }

            //Microfono
            if (Input.GetKeyDown(KeyCode.Space)) Microfono();
            #endregion
            #region Lipoma
            if (crearLipoma)
            {                
                laser = camara.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(laser, out raycastHit))
                {
                    if (raycastHit.transform.CompareTag("Paciente"))
                    {
                        if (Input.GetMouseButtonDown(0) && !lipomaCreado)
                        {
                            print(raycastHit.normal);
                            lipomaCopia = Instantiate(lipoma, raycastHit.point, Quaternion.FromToRotation(Vector3.up,raycastHit.normal));
                            lipomaCreado = true;
                            crearLipoma = false;
                        }
                    }                                        
                }
            }
            else if (modificarLipoma)
            {
                laser = camara.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(laser, out raycastHit))
                {
                    if (raycastHit.transform.CompareTag("Lipoma"))
                    {
                        raycastHit.transform.parent.GetComponent<Lipoma>().CambioColor(1);
                        if (Input.GetMouseButtonDown(0))
                        {
                            lipomaCopia = raycastHit.transform.parent.gameObject;
                            raycastHit.transform.parent.GetComponent<Lipoma>().CambioColor(2);
                            modificarLipoma = false;
                        }
                    }
                    else
                    {
                        if (lipomaCopia!=null)
                        {
                            lipomaCopia.GetComponent<Lipoma>().CambioColor(0);
                        }                        
                    }
                }
            }
            #endregion
        }
    }
    private void Microfono()
    {
        activar = !activar;
        GetComponent<Recorder>().TransmitEnabled = activar;
        transform.GetChild(1).GetChild(1).GetComponent<SpriteRenderer>().enabled = !activar;
    }

    public void CrearLipoma()
    {
        crearLipoma = true;
        modificarLipoma = false;
    }

    public void ModficarLipoma()
    {
        crearLipoma = false;
        modificarLipoma = true;
    }

    public void DimensionarLipoma(float multiplicador)
    {
        lipomaCopia.transform.localScale = new Vector3(multiplicador, multiplicador, multiplicador);
    }

    public void Aceptar()
    {
        lipomaCreado = false;
    }
}
