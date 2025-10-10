using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InRoom : MonoBehaviour
{
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private TMP_Text roomHeadCountText;
    [SerializeField] private GameObject seatPrefab;
    [SerializeField] private Transform seatParent;
    
    private void OnEnable()
    {
        PhotonManager.OnPlayerUpdate += UpdatePlayerListUI;
        UpdatePlayerListUI();
    }

    private void OnDisable()
    {
        PhotonManager.OnPlayerUpdate -= UpdatePlayerListUI;
    }

    private void UpdatePlayerListUI() // 플레이어 입장 or 퇴장
    {
        if (PhotonNetwork.CurrentRoom == null) return;
        
        // 방 정보 텍스트 업데이트
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        roomHeadCountText.text = $"{PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

        foreach (Transform child in seatParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject seat = Instantiate(seatPrefab, seatParent);
            TMP_Text text = seat.GetComponentInChildren<TMP_Text>();
            Image seatImage = seat.GetComponent<Image>();
            if (text != null)
                text.text = player.NickName;
            if (seatImage != null)
            {
                player.CustomProperties.TryGetValue("InRoomReady", out object ready);
                if (ready != null && (bool)ready)
                    seatImage.color = Color.black;
                else
                    seatImage.color = Color.gray;
            }
        }
    }
}