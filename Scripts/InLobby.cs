using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class InLobby : MonoBehaviour
{
    [SerializeField] private TMP_Text userNickname;
    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private Transform roomParent;
    private readonly Dictionary<string, RoomInfo> _roomList = new Dictionary<string, RoomInfo>();
    private readonly List<GameObject> _roomListObjects = new List<GameObject>();
    
    private void OnEnable()
    {
        PhotonManager.OnRoomListUpdated += RefreshRoomList;
        InitLobby();
    }

    private void OnDisable()
    {
        PhotonManager.OnRoomListUpdated -= RefreshRoomList;
    }

    private void InitLobby()
    {
        if (PhotonNetwork.IsConnected)
            userNickname.text = PhotonNetwork.NickName;
    }
    
    private void RefreshRoomList(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                if (_roomList.ContainsKey(room.Name))
                    _roomList.Remove(room.Name);
            }
            else
            {
                _roomList[room.Name] = room;
            }
        }

        UpdateRoomListUI();
    }

    private void UpdateRoomListUI()
    {
        foreach (Transform child in roomParent)
        {
            child.gameObject.SetActive(false);
        }
        
        int index = 0;
        if (_roomListObjects.Count >= _roomList.Count)
        {
            foreach (var room in _roomList)
            {
                _roomListObjects[index].SetActive(true);
                SelectRoomButton btn = _roomListObjects[index].GetComponent<SelectRoomButton>();
                btn.Init(room.Value);
                index++;
            }
        }
        else
        {
            foreach (var room in _roomList)
            {
                if (index < _roomListObjects.Count)
                {
                    _roomListObjects[index].SetActive(true);
                    SelectRoomButton btn = _roomListObjects[index].GetComponent<SelectRoomButton>();
                    btn.Init(room.Value);
                    index++;
                }
                else
                {
                    GameObject obj = Instantiate(roomPrefab, roomParent);
                    SelectRoomButton btn = obj.GetComponent<SelectRoomButton>();
                    btn.Init(room.Value);
                    _roomListObjects.Add(obj);
                }
            }
        }
    }
}
