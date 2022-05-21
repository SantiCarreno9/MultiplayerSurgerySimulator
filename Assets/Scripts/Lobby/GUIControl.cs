using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Roles
{
    Cirujano=1,
    Residente1C,
    Residente2C,
    InternoC,
    EstudianteC,
    Anestesiologo=7,
    Residente1A,
    Residente2A,
    InternoA,
    EstudianteA,
    Instrumentador=13,
    Circulante,
    Instructor
};

public class GUIControl : MonoBehaviour
{    
    private GameObject menuIdioma;
    private GameObject menuSala;    
    private ToggleGroup rolesToggleGroup;    
    private GameObject botonIniciar;
    [HideInInspector] public byte idiomaSeleccionado, rolSeleccionado;
    [SerializeField] private Transform[] textos;
    
    private void Start()
    {
        menuIdioma = transform.GetChild(0).gameObject;
        idiomaSeleccionado = 0;                      
    }

    public void SeleccionarIdioma(int idiomaSeleccionado) 
    { 
        this.idiomaSeleccionado = (byte)idiomaSeleccionado;
        PhotonRoom.room.idioma = (byte)idiomaSeleccionado;
        for (byte i = 0; i < 2; i++)
        {
            for (byte j = 0; j < textos.Length; j++)
            {
                if (i == idiomaSeleccionado)                
                    EstadoGUI(textos[j].GetChild(i).GetComponent<CanvasGroup>(), true);
                else
                    EstadoGUI(textos[j].GetChild(i).GetComponent<CanvasGroup>(), false);                                
            }                        
        } 
    }

    /// <summary>
    /// Cambia los menús de la aplicación según el estado de conexión
    /// </summary>
    /// <param name="conexion"> Indica el estado de conexión</param> 
    public void CambioGUI(bool conexion)
    {        
        //menuIdioma.SetActive(!Conexion);//Desactiva/Activa menú selección de idioma
        EstadoGUI(menuIdioma.GetComponent<CanvasGroup>(),!conexion);//Desactiva/Activa menú selección de idioma
        EstadoGUI(transform.GetChild(1).GetComponent<CanvasGroup>(),conexion);//Activa/Desactiva fondo menú de sala
        EstadoGUI(transform.GetChild(1).GetChild(idiomaSeleccionado).GetComponent<CanvasGroup>(),conexion);//Activa/Desactiva fondo menú de sala
        menuSala = transform.GetChild(1).GetChild(idiomaSeleccionado).gameObject;//Almacena el menú de la sala              
    }

    /// <summary>
    /// Activa o desactiva menú
    /// </summary>
    /// <param name="gui"></param>
    /// <param name="activo"></param>
    public void EstadoGUI(CanvasGroup gui, bool activo)
    {
        if (activo) gui.alpha = 1;
        else gui.alpha = 0;

        gui.blocksRaycasts = activo;
        gui.interactable = activo;
    }

    /// <summary>
    /// Activa y desactiva componentes al ingresar a la sala
    /// </summary>
    /// <param name="ingreso"> Indica si entra/sale de la sala</param>
    public void IngresoSala()
    {
        //Obtiene objetos
        rolesToggleGroup = menuSala.transform.GetComponentInChildren<ToggleGroup>();
        botonIniciar = menuSala.transform.GetChild(0).GetChild(2).gameObject;                                             
    }

    /// <summary>
    /// Actualiza el número de usuarios presentes en la sala
    /// </summary>
    /// <param name="cantidadUsuarios"> Numero de usuarios en la sala</param>    
    public void IngresoUsuarios(byte cantidadUsuarios) => menuSala.transform.GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = $"{cantidadUsuarios}/12";                    
        
    /// <summary>
    /// Retorna el rol seleccionado
    /// </summary>
    /// <returns></returns>
    public byte GetRol()
    {
        Toggle rolButton = rolesToggleGroup.ActiveToggles().FirstOrDefault();
        byte posicion=0;
        for (byte i = 0; i < rolesToggleGroup.transform.childCount; i++)
        {
            if (rolesToggleGroup.transform.GetChild(i).name.Equals(rolButton.name))
            {
                posicion = i;
                break;
            }
        }
        return posicion;
    }

    /// <summary>
    /// Asigna el número de usuario al rol correspondiente
    /// </summary>
    /// <param name="numeroUsuario"></param>
    /// <param name="numeroRol"></param>
    public void AsignarUsuario(byte numeroUsuario, byte numeroRol)
    {
        rolesToggleGroup.transform.GetChild(numeroRol).tag = numeroUsuario.ToString();
        rolesToggleGroup.transform.GetChild(numeroRol).GetComponent<Toggle>().enabled = false;
        rolesToggleGroup.transform.GetChild(numeroRol).GetComponent<Image>().color = new Color(32f / 255f, 125f / 255f, 15f / 255f);
    }

    /// <summary>
    /// Libera el rol seleccionado previamente
    /// </summary>
    /// <param name="numeroRol"></param>
    public void LiberarRol(byte numeroRol)
    {
        rolesToggleGroup.transform.GetChild(numeroRol).tag = "Untagged";
        rolesToggleGroup.transform.GetChild(numeroRol).GetComponent<Toggle>().enabled = true;
        rolesToggleGroup.transform.GetChild(numeroRol).GetComponent<Image>().color = new Color(56f / 255f, 56f / 255f, 56f / 255f);
    }

    /// <summary>
    /// Habilita/Deshabilita los roles
    /// </summary>
    /// <param name="habilitar"></param>
    public void EstadoRoles(bool habilitar)
    {
        foreach (Toggle item in rolesToggleGroup.GetComponentsInChildren<Toggle>())
        {
            if (habilitar)
            {
                if (item.CompareTag("Untagged"))
                {
                    item.enabled = true;
                    item.GetComponent<Image>().color = new Color(56f / 255f, 56f / 255f, 56f / 255f);
                }                
            }
            else item.enabled = false;
        }
    }

    public void HabilitarBotonIniciar(bool habilitar) => botonIniciar.SetActive(habilitar);
    
}
