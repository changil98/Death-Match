using TMPro;
using Photon.Pun;
using UnityEngine;

public class ConnectServerButton : BaseButton
{
    [SerializeField] private TMP_InputField userNameInputField;

    protected override void OnClickButton()
    {
        if (string.IsNullOrEmpty(userNameInputField.text)) return;
        // 유저 이름 설정 (게임 속에 보여지는 이름)
        PhotonNetwork.NickName = userNameInputField.text;
        // 로비 접속
        PhotonNetwork.JoinLobby();
    }
}