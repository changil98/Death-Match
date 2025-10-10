using System;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class SelectRoomButton : BaseButton
{
    public static event Action<string> OnSelectRoom;
    
    [SerializeField] private TMP_Text text;
    
    private RoomInfo _roomInfo;
    
    protected override void OnClickButton()
    {
        OnSelectRoom?.Invoke(_roomInfo.Name);
    }

    public void Init(RoomInfo roomInfo)
    {
        _roomInfo = roomInfo;
        text.text = "방이름 : " + _roomInfo.Name + " / " +_roomInfo.PlayerCount + 
                    " / " + _roomInfo.MaxPlayers;
    }
}