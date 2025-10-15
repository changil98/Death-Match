using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class RoomReadyButton : BaseButton
{
    protected override void OnClickButton()
    {
        Player player = PhotonNetwork.LocalPlayer;
        if (player.IsMasterClient) return;
        Hashtable readyProps = player.CustomProperties;
        if (readyProps.ContainsKey("InRoomReady"))
        {
            bool ready = (bool)readyProps["InRoomReady"];
            if (ready)
                readyProps["InRoomReady"] = false;
            else
                readyProps["InRoomReady"] = true;
        }
        Debug.Log("OnClickButton");
        Debug.Log(readyProps["InRoomReady"]);
        PhotonNetwork.SetPlayerCustomProperties(readyProps);
    }
}