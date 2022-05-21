using System.Collections;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class Circulante : MonoBehaviour
{
    //Indicador posibles acciones
    private RaycastHit RaycastHit;
    private Ray Laser;
    private Transform camara;
    [SerializeField]
    private SpriteRenderer ReticulaN, ReticulaO;
    [SerializeField]
    private TextMeshProUGUI text;

    //Control mano e interacción objetos
    private ManoAuto mano;    
    private Animator armario;

    //Cambio habitación
    private GameObject[] spawnPos;
    [SerializeField]
    private Animator transicion;

    //Photon
    private PhotonView PV;                

    private void Start()
    {
        spawnPos = GameObject.FindGameObjectsWithTag("Respawn");
        camara = transform.GetChild(0);
        PV = GetComponent<PhotonView>();
        mano = GetComponentInChildren<ManoAuto>();
    }
    
    private void Update()
    {
        if (PV.IsMine)
        {
            Laser = new Ray(camara.position, camara.forward);
            if (Physics.Raycast(Laser, out RaycastHit, 1.5f))
            {
                #region Cambio habitación
                if (RaycastHit.transform.CompareTag("Puerta"))
                {
                    //Indicativos
                    ReticulaO.enabled = true; ReticulaN.enabled = false; text.enabled = true;
                    text.text = "Haz clic para cambiar de habitación";
                    //Interacción
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (RaycastHit.transform.name.Equals("PuertaSI", System.StringComparison.OrdinalIgnoreCase))                        
                            StartCoroutine(CambioHabitación(1));                                                    
                        else                        
                            StartCoroutine(CambioHabitación(0));                                                                     
                    }
                }
                #endregion
                #region Animaciones Armario
                else if (RaycastHit.transform.CompareTag("Armario"))
                {
                    //Indicativos
                    ReticulaO.enabled = true; ReticulaN.enabled = false; text.enabled = true;
                    armario = RaycastHit.transform.GetComponent<Animator>();
                    text.text = "Haz clic para interactuar";
                    //Interacción
                    if (Input.GetMouseButtonDown(0))
                    {
                        //Animación actual del objeto
                        if (!armario.GetCurrentAnimatorStateInfo(0).IsName("Abrir")) armario.Play("Abrir");
                        else armario.Play("Cerrar");
                    }
                }
                #endregion
                #region Tomar objeto
                else if (RaycastHit.transform.CompareTag("Instrumento"))
                {
                    //Indicativos
                    ReticulaO.enabled = true; ReticulaN.enabled = false; text.enabled = true;
                    text.text = "Haz clic para agarrar";
                    //Interacción
                    if (Input.GetMouseButtonDown(0))                    
                        mano.AgarrarObjeto(RaycastHit.transform);                                            
                }
                #endregion
                #region Soltar Objeto
                else if (RaycastHit.transform.CompareTag("Mesa"))
                {                    
                    if (mano.tieneObjetos)
                    {
                        //Indicativos
                        ReticulaO.enabled = true; ReticulaN.enabled = false; text.enabled = true;
                        text.text = "Haz para dejar el objeto";
                        //Interacción
                        if (Input.GetMouseButtonDown(0))                        
                            mano.SoltarObjeto();
                        
                    }                    
                }
                #endregion
                else { ReticulaN.enabled = true; ReticulaO.enabled = false; text.enabled = false; }
                
            }
            else { ReticulaN.enabled = true; ReticulaO.enabled = false; text.enabled = false; }
        }        
    }

    private IEnumerator CambioHabitación(byte puerta)
    {
        transicion.Play("Transicion");
        //Esperar mitad de la animación
        yield return new WaitForSeconds(1f);
        //Si es la puerta de la sala de cirugía
        if (puerta==0)
        {
            transform.position = spawnPos[1].transform.position;
            transform.localRotation = spawnPos[1].transform.localRotation;
            Physics.SyncTransforms();
        }
        //Si es la puerta de la sala de instrumentos
        else
        {
            transform.position = spawnPos[0].transform.position;
            transform.localRotation = spawnPos[0].transform.localRotation;
            Physics.SyncTransforms();
        }
    }
}
