using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Diagnostics;
using System;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public static PhotonRoom room;
    private PhotonView PV;    
    private byte currentScene;    
    private byte rolValue;    
    [HideInInspector]public byte idioma;
    [HideInInspector]public byte miNumero;
    public GUIControl gui;
    private bool aceptar;

    private void Awake()
    {
        if (PhotonRoom.room == null)
        {
            PhotonRoom.room = this;
        }
        else
        {
            if (PhotonRoom.room != this)
            {
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this;
            }
        }                
        DontDestroyOnLoad(this.gameObject);
    }    
    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;        
    }
    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }    
    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = (byte)scene.buildIndex;
        if (currentScene==1) CreatePlayer(); //Escena sala de operaciones  
    }
    private void Start()
    {
        PV = GetComponent<PhotonView>();
        gui = FindObjectOfType<GUIControl>();
    }
    public override void OnJoinedRoom()
    {        
        base.OnJoinedRoom();        
        print("Creación/Ingreso a sala exitoso");
        miNumero = (byte)(PhotonNetwork.PlayerList.Length - 1);
        PhotonNetwork.NickName = miNumero.ToString();
        gui.IngresoSala();
        PV.RPC("Ingreso", RpcTarget.AllBuffered, miNumero);       
    }
    public override void OnLeftRoom()
    {
        base.OnLeftRoom();                
    }        

    public override void OnPlayerEnteredRoom(Player newPlayer)=> gui.IngresoUsuarios((byte)PhotonNetwork.PlayerList.Length);

    public override void OnPlayerLeftRoom(Player newPlayer) 
    {
        if (currentScene==0)
        {
            gui.IngresoUsuarios((byte)PhotonNetwork.PlayerList.Length);
            GameObject.FindGameObjectWithTag(newPlayer.NickName).tag = "Untagged";
            if (miNumero > byte.Parse(newPlayer.NickName))
            {
                GameObject rolTag = GameObject.FindGameObjectWithTag(miNumero.ToString());
                miNumero--;
                rolTag.tag = miNumero.ToString();
                PhotonNetwork.NickName = miNumero.ToString();
            }
            if (!aceptar) gui.EstadoRoles(true);
            else
            {
                gui.EstadoRoles(true);
                gui.EstadoRoles(false);
            }
        }
        else
        {
            Destroy(GameObject.FindGameObjectWithTag(newPlayer.NickName));
        }
    }
  
    public void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        PhotonNetwork.LoadLevel(1);       
    }

    private void CreatePlayer()
    {        
        switch (rolValue)
        {
            case (byte)Roles.Cirujano:
                PhotonNetwork.Instantiate(Path.Combine("Desktop","Cirugia",Roles.Cirujano.ToString()), new Vector3(0, 1.723f, -0.618f), Quaternion.identity, 0);
                break;
            case (byte)Roles.Residente1C:
                PhotonNetwork.Instantiate(Path.Combine("Desktop", "Cirugia", Roles.Residente1C.ToString()), new Vector3(0, 1.723f, -0.618f), Quaternion.identity, 0);
                break;
            case (byte)Roles.Residente2C:
                PhotonNetwork.Instantiate(Path.Combine("Desktop", "Cirugia", Roles.Residente2C.ToString()), new Vector3(0, 1.723f, -0.618f), Quaternion.identity, 0);
                break;
            case (byte)Roles.InternoC:
                PhotonNetwork.Instantiate(Path.Combine("Desktop", "Cirugia", Roles.InternoC.ToString()), new Vector3(0, 1.723f, -0.618f), Quaternion.identity, 0);
                break;
            case (byte)Roles.EstudianteC:
                PhotonNetwork.Instantiate(Path.Combine("Desktop", "Cirugia", Roles.EstudianteC.ToString()), new Vector3(0, 1.723f, -0.618f), Quaternion.identity, 0);
                break;
            case (byte)Roles.Anestesiologo:
                PhotonNetwork.Instantiate(Path.Combine("Desktop", "Anestesia", Roles.Anestesiologo.ToString()), new Vector3(0, 1.723f, -0.618f), Quaternion.identity, 0);
                break;
            case (byte)Roles.Residente1A:
                PhotonNetwork.Instantiate(Path.Combine("Desktop", "Anestesia", Roles.Residente1A.ToString()), new Vector3(1.401f, 1.57f, -0.064f), Quaternion.identity, 0);
                break;
            case (byte)Roles.Residente2A:
                PhotonNetwork.Instantiate(Path.Combine("Desktop", "Anestesia", Roles.Residente2A.ToString()), new Vector3(1.401f, 1.57f, -0.064f), Quaternion.identity, 0);
                break;
            case (byte)Roles.InternoA:
                PhotonNetwork.Instantiate(Path.Combine("Desktop", "Anestesia", Roles.InternoA.ToString()), new Vector3(1.401f, 1.57f, -0.064f), Quaternion.identity, 0);
                break;
            case (byte)Roles.EstudianteA:
                PhotonNetwork.Instantiate(Path.Combine("Desktop", "Anestesia", Roles.EstudianteA.ToString()), new Vector3(1.401f, 1.57f, -0.064f), Quaternion.identity, 0);
                break;
            case (byte)Roles.Instrumentador:
                PhotonNetwork.Instantiate(Path.Combine("Desktop", "Enfermeria", Roles.Instrumentador.ToString()), new Vector3(0, 1.723f, -0.618f), Quaternion.identity, 0);
                break;
            case (byte)Roles.Circulante:
                PhotonNetwork.Instantiate(Path.Combine("Desktop", "Enfermeria", Roles.Circulante.ToString()), new Vector3(1.401f, 1.57f, -0.064f), Quaternion.identity, 0);
                break;
            case (byte)Roles.Instructor:
                PhotonNetwork.Instantiate(Roles.Instructor.ToString(), new Vector3(1.401f, 1.57f, -0.064f), Quaternion.identity, 0);
                break;
            default:
                break;
        }
        
    }       

    public void AceptarClic() 
    {        
        rolValue = gui.GetRol();                        
        PV.RPC("AceptarRol", RpcTarget.AllBuffered, miNumero, rolValue);
        gui.EstadoRoles(false);
        aceptar = true;
        if (PhotonNetwork.IsMasterClient)
            gui.HabilitarBotonIniciar(true);
    }

    public void CancelarClic() 
    {        
        PV.RPC("CancelarRol", RpcTarget.AllBuffered, rolValue);
        gui.EstadoRoles(true);
        aceptar = false;
        if (PhotonNetwork.IsMasterClient)
            gui.HabilitarBotonIniciar(false);
    }

    #region Funciones RPC 
    [PunRPC]
    private void Ingreso(byte Numero) => gui.IngresoUsuarios((byte)PhotonNetwork.PlayerList.Length);

    [PunRPC]
    private void AceptarRol(byte numeroUsuario, byte posicionRol) => gui.AsignarUsuario(numeroUsuario, posicionRol);

    [PunRPC]
    private void CancelarRol(byte posicionRol) => gui.LiberarRol(posicionRol);

    #endregion
}
