using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject connection;
    [SerializeField] private GameObject lobby;
    [SerializeField] private GameObject inRoom;
    
    public static event Action<List<RoomInfo>> OnRoomListUpdated;
    public static event Action OnPlayerUpdate;

    private void Start()
    {
        // 씬이 로드될 때 현재 포톤 네트워크 상태에 따라 UI를 정리
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.InRoom)
            {
                connection.SetActive(false);
                lobby.SetActive(false);
                inRoom.SetActive(true);
            }
            else if (PhotonNetwork.InLobby)
            {
                connection.SetActive(false);
                lobby.SetActive(true);
                inRoom.SetActive(false);
            }
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings(); // 서버 접속
            connection.SetActive(true);
            lobby.SetActive(false);
            inRoom.SetActive(false);
        }
    }

    // 포톤 서버에 접속 후 호출되는 콜백 함수
    public override void OnConnectedToMaster()
    {
        if (PhotonNetwork.NickName != "")
            PhotonNetwork.JoinLobby();
    }

    // 로비에 접속 후 호출되는 콜백 함수
    public override void OnJoinedLobby()
    {
        connection.SetActive(false);
        lobby.SetActive(true);
        inRoom.SetActive(false);
    }

    // 로비에서 나갈 때 호출되는 콜백 함수
    public override void OnLeftLobby()
    {
        lobby.SetActive(false);
        connection.SetActive(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        OnRoomListUpdated?.Invoke(roomList);
    }

    public override void OnJoinedRoom()
    {
        lobby.SetActive(false);
        inRoom.SetActive(true);
        // 변경점: 마스터 클라이언트는 입장 시 자동으로 Ready 상태가 되도록 설정
        if (PhotonNetwork.IsMasterClient)
            SetLocalPlayerReady(true);
        else
            SetLocalPlayerReady(false);
        
        OnPlayerUpdate?.Invoke(); // 플레이어 목록이 변경되었음을 알림
    }

    public override void OnLeftRoom()
    {
        inRoom.SetActive(false);
        lobby.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        OnPlayerUpdate?.Invoke();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        OnPlayerUpdate?.Invoke();
    }

    // 변경점: 마스터 클라이언트가 변경되었을 때 호출
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        // 새로운 마스터 클라이언트가 나 자신이라면, 자동으로 Ready 상태로 변경
        if (newMasterClient.IsLocal)
        {
            SetLocalPlayerReady(true);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("InRoomReady"))
        {
            OnPlayerUpdate?.Invoke();
        }
    }
    
    public static void SetLocalPlayerReady(bool ready)
    {
        Hashtable props = new Hashtable() { { "InRoomReady", ready } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
}