using Photon.Pun;

public class LeaveLobbyButton : BaseButton
{
    protected override void OnClickButton()
    {
        PhotonNetwork.LeaveLobby();
    }
}