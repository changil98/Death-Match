using Photon.Pun;
using TMPro;
using UnityEngine;

public class JoinRoomButton : BaseButton
{
    [SerializeField] private TMP_InputField roomNameInputField;
    
    private void OnEnable() => SelectRoomButton.OnSelectRoom += SelectRoom;
    private void OnDisable() => SelectRoomButton.OnSelectRoom -= SelectRoom;

    protected override void OnClickButton()
    {
        if (!string.IsNullOrEmpty(roomNameInputField.text))
            PhotonNetwork.JoinRoom(roomNameInputField.text);
    }

    private void SelectRoom(string roomName)
    {
        roomNameInputField.text = roomName;
    }
}