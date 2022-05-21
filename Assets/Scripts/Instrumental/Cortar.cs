using UnityEngine;

public class Cortar : MonoBehaviour
{
    private RaycastHit hit;
    private Ray ray;    
    private bool contacto, envioCoordenadas;
    public Transform puntoCorte;
    private GameObject lipoma=null;
    private Vector3 puntoColision;
    private byte extremoCorte = 0;

    void Update()
    {
        if (contacto)
        {
            ray = new Ray(puntoCorte.position, puntoCorte.forward);
            if (Physics.Raycast(ray, out hit, 0.015f))
            {
                if (hit.transform.CompareTag("Piel"))
                {
                    lipoma = hit.transform.gameObject;
                    hit.transform.GetComponent<TrazadoLinea>().corte = true;
                    for (int i = 0; i < 3; i++)
                    {
                        if (hit.point[i] != puntoColision[i])
                        {
                            envioCoordenadas = true;
                            break;
                        }
                    }
                    puntoColision = hit.point;
                                                                        
                    hit.transform.GetComponent<Lipoma>().corte = true;
                    hit.transform.GetComponent<Lipoma>().CortarPiel(hit.triangleIndex);
                }
                if (lipoma != null)
                {
                    if (envioCoordenadas)
                    {
                        print(puntoColision);
                        //lipoma.transform.GetComponent<TrazadoLinea>().TrazadoCorte(puntoColision.x, puntoColision.y, puntoColision.z, extremoCorte);
                        lipoma.transform.GetComponent<TrazadoLinea>().TrazadoCorte(puntoColision, extremoCorte);
                        if (extremoCorte == 0) extremoCorte = 1;
                        envioCoordenadas = false;
                    }                        
                }                
            }
        }
        else
        {
            if (lipoma!=null)
            {                               
                if (extremoCorte==2)
                {
                    //lipoma.GetComponent<TrazadoLinea>().TrazadoCorte(puntoColision.x, puntoColision.y, puntoColision.z, extremoCorte);
                    lipoma.transform.GetComponent<TrazadoLinea>().TrazadoCorte(puntoColision, extremoCorte);
                    lipoma.GetComponent<SonidosCirugia>().PlaySonido(0);
                    lipoma = null;                    
                }                
            }
        }        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Piel"))
        {
            GetComponent<ChangeOwnership>().CambioColor(true);
            contacto = true;
            extremoCorte = 0;            
            other.gameObject.GetComponent<SonidosCirugia>().PlaySonido(0);
        }
    }

    private void OnTriggerExit(Collider other)
    {        
        GetComponent<ChangeOwnership>().CambioColor(false);            
        extremoCorte = 2;            
        contacto = false;                 
    }    
}
