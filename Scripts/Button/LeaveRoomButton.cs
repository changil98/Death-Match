using Photon.Pun;

public class LeaveRoomButton : BaseButton
{
    protected override void OnClickButton()
    {
        PhotonNetwork.LeaveRoom();
    }
}