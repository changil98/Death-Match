using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class Result : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    private readonly Dictionary<string, int> _scores = new Dictionary<string, int>();
    
    public void Init()
    {
        text.text = string.Empty;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            player.CustomProperties.TryGetValue("score", out object score);
            if (score != null) _scores.Add(player.NickName, (int)score);
        }

        var keyValuePairs = _scores.OrderBy(x => x.Value);

        foreach (var kvp in keyValuePairs)
        {
            text.text += $"{kvp.Key}: {kvp.Value}\n";
        }
    }
}
