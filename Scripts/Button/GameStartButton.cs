using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameStartButton : BaseButton
{
    [SerializeField] private GameObject ask;
    private PhotonView _photonView;
    
    protected override void Awake()
    {
        base.Awake();
        _photonView = GetComponent<PhotonView>();
    }

    protected override void OnClickButton()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        int currentPlayers = PhotonNetwork.PlayerList.Length;
        int maxPlayers = PhotonNetwork.CurrentRoom.MaxPlayers;
        if (currentPlayers < 2 || currentPlayers > maxPlayers) return;
        if (!CheckReady()) return;
        _photonView.RPC(nameof(Automatically), RpcTarget.All);
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("GameScene");
    }

    private bool CheckReady()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!player.CustomProperties.ContainsKey("InRoomReady") ||
                !(bool)player.CustomProperties["InRoomReady"])
                return false;
        }
        return true;
    }

    [PunRPC]
    private void Automatically()
    {
        // 방장이 게임을 시작하면 모든 플레이어 신이동 가능하게하는 것
        PhotonNetwork.AutomaticallySyncScene = true;
    }
}