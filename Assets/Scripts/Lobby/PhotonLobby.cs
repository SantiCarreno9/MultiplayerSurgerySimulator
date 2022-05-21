using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    private GUIControl gui;
    private void Start()
    {
        gui = FindObjectOfType<GUIControl>();
    }

    public void Conectar() => PhotonNetwork.ConnectUsingSettings();         

    public override void OnConnectedToMaster()
    {
        Debug.Log("El jugador se conectó al Photon Master Server.");        
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.JoinOrCreateRoom("UMNG", new RoomOptions { MaxPlayers = 13 }, null);
        gui.CambioGUI(true);        
    }

    public override void OnDisconnected(DisconnectCause cause) => gui.CambioGUI(false);

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("RoomFailed" + returnCode + "Message" + message);
        PhotonNetwork.CreateRoom("UMNG", new RoomOptions { MaxPlayers = 13 }, null);
    }    
}
