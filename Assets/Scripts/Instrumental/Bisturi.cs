using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bisturi : MonoBehaviour
{    
    public void Acomodar(bool agarrado) 
    {
        if (agarrado) 
        {            
            transform.localEulerAngles = new Vector3(0, 0, -90);
            transform.localPosition = Vector3.zero;
            transform.localPosition = new Vector3( -0.03f, 0.12f, 0);                        
        }
        else transform.localEulerAngles = new Vector3(0, 0, 0);     
    }    
}