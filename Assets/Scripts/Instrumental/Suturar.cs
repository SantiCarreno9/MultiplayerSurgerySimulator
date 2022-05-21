using UnityEngine;

public class Suturar : MonoBehaviour
{            
    private byte nContactos=0;
    private bool agujaAgarrada;   

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name.Equals("Aguja", System.StringComparison.OrdinalIgnoreCase))
        {
            GetComponent<ChangeOwnership>().CambioColor(true);
        }
        if (other.gameObject.CompareTag("Linea") && agujaAgarrada)
        {
            GetComponent<ChangeOwnership>().CambioColor(true);                 
            nContactos++;
            if (nContactos==5)
            {
                other.transform.parent.GetComponent<TrazadoLinea>().CambioColor(byte.Parse(other.name));
                nContactos = 0;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name.Equals("Aguja", System.StringComparison.OrdinalIgnoreCase))
        {                                
            if (Input.GetKey(KeyCode.Space) && !GetComponent<Pinzas>().acciono && !agujaAgarrada)
            {
                other.transform.SetParent(transform);
                other.GetComponent<ChangeOwnership>().Coger();
                agujaAgarrada = true;
                other.transform.localEulerAngles = new Vector3(39.811f, 6.985f, -4.41f);
                other.transform.localPosition = Vector3.zero;
                other.transform.localPosition = new Vector3(-0.023f, 0.0578f, 0.04f);
                GetComponent<ChangeOwnership>().CambioColor(false);
            }
            else if (GetComponent<Pinzas>().acciono && agujaAgarrada)
            {
                other.GetComponent<ChangeOwnership>().Soltar();
                agujaAgarrada = false;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Linea", System.StringComparison.OrdinalIgnoreCase))
        {
            GetComponent<ChangeOwnership>().CambioColor(false);                        
        }
        GetComponent<ChangeOwnership>().CambioColor(false);
    }    
}
