using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class CreateRoomButton : BaseButton
{
    [SerializeField] private TMP_InputField roomNameInputField;
    [SerializeField] private TMP_InputField maxPlayerInputField;
    [SerializeField] private GameObject createRoomPanel;
    
    protected override void OnClickButton()
    {
        RoomOptions roomOptions = new RoomOptions();
        // 방 이름 입력
        if (string.IsNullOrEmpty(roomNameInputField.text)) return;
        // 방 최대인원 입력 (2~6인)
        if (string.IsNullOrEmpty(maxPlayerInputField.text)) return;
        
        if (int.TryParse(maxPlayerInputField.text, out int num))
        {
            if (num is < 2 or > 8) return;
            roomOptions.MaxPlayers = num;
        }
        else return;
        
        roomOptions.IsOpen = true; // 열려있는지
        roomOptions.IsVisible = true; // 로비에서 보여지는지
        // 방 생성
        PhotonNetwork.CreateRoom(roomNameInputField.text, roomOptions);
        createRoomPanel.SetActive(false);
    }
}