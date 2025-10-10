using Photon.Pun;
using UnityEngine.SceneManagement;

public class ReturnToLobbyButton : BaseButton
{
    protected override void OnClickButton()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("LobbyScene");
    }
}
