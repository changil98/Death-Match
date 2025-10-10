using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public enum GameState
{
    Submit,
    Select,
    Waiting,
    End
}

public class RoundManager : MonoBehaviourPunCallbacks
{
    public static event Action<GameState> OnGameStateChange;
    public static event Action<int> OnTimerStart;

    [SerializeField] private GameObject submitData;
    [SerializeField] private Transform submitDataParent;
    [SerializeField] private GameObject scoreData;
    [SerializeField] private Transform scoreDataParent;

    private PhotonView _photonView;
    private GameState _currentGameState;
    private int _currentRound = 1;
    private const int TotalRounds = 6;
    private readonly List<GameObject> _submits = new List<GameObject>();
    private readonly List<GameObject> _scores = new List<GameObject>();

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SetGameState(GameState.Submit);
            PhotonNetwork.CurrentRoom.SetCurrentRound(_currentRound);
        }

        InitializeUI();
    }

    private void InitializeUI()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            var submit = Instantiate(submitData, submitDataParent);
            var score = Instantiate(scoreData, scoreDataParent);
            _submits.Add(submit);
            _scores.Add(score);
            submit.GetComponent<SubmitCardData>().Init(player.NickName);
            score.GetComponent<Score>().Init(player.NickName);
        }
    }

    // 방의 상태가 업데이트 될 때
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("GameState"))
        {
            _currentGameState = PhotonNetwork.CurrentRoom.GetGameState();

            OnGameStateChange?.Invoke(_currentGameState);

            if (_currentGameState == GameState.End)
                _photonView.RPC(nameof(Automatically), RpcTarget.All);
        }

        if (propertiesThatChanged.ContainsKey("CurrentRound"))
            _currentRound = PhotonNetwork.CurrentRoom.GetCurrentRound();
    }

    [PunRPC]
    private void Automatically()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
    }

    // 플레이어들의 제출 정보가 업데이트 될 때
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        // 점수 변경되었으면 실행
        if (changedProps.ContainsKey("score"))
        {
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (PhotonNetwork.PlayerList[i] == targetPlayer)
                {
                    _scores[i].GetComponent<Score>().UpdateScore(targetPlayer.NickName, (int)changedProps["score"]);
                    break;
                }
            }
        }

        if (!PhotonNetwork.IsMasterClient)
            return;
        
        if (_currentGameState == GameState.Submit)
        {
            if (changedProps.ContainsKey("isSubmit"))
                CheckAllPlayers("isSubmit", GameState.Select);
        }
        else if (_currentGameState == GameState.Select)
        {
            if (changedProps.ContainsKey("isSelect"))
                CheckAllPlayers("isSelect", GameState.Waiting);
        }
        else if (_currentGameState == GameState.Waiting)
        {
            if (changedProps.ContainsKey("isReady"))
                CheckAllPlayers("isReady", GameState.Submit);
        }
    }

    private void CheckAllPlayers(string key, GameState gameState)
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!player.CustomProperties.ContainsKey(key) ||
                !(bool)player.CustomProperties[key])
                return;
        }

        if (_currentGameState == GameState.Select)
        {
            CalculateScores();
            _currentRound++;
        }

        _photonView.RPC(nameof(UpdateUiRPC), RpcTarget.All);

        if (_currentRound > TotalRounds)
        {
            PhotonNetwork.CurrentRoom.SetGameState(GameState.End);
            return;
        }
        PhotonNetwork.CurrentRoom.SetCurrentRound(_currentRound);
        PhotonNetwork.CurrentRoom.SetGameState(gameState);

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            player.SetCustomProperties(new Hashtable() { { key, false } });
        }

        OnTimerStart?.Invoke(10);
    }

    private void CalculateScores()
    {
        var selectedCards = new Dictionary<Player, int>();
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("selectedCard", out object cardValue))
                selectedCards.Add(player, (int)cardValue);
        }

        var cardCounts = new Dictionary<int, int>();
        foreach (int card in selectedCards.Values)
        {
            if (!cardCounts.TryAdd(card, 1))
                cardCounts[card]++;
        }

        var uniqueCards = new List<int>();
        foreach (var kvp in cardCounts)
        {
            if (kvp.Value == 1)
                uniqueCards.Add(kvp.Key);
        }

        if (uniqueCards.Count == 0) return;

        uniqueCards.Sort();
        int winningCardValue = uniqueCards[0];
        Player winner = null;
        foreach (var submission in selectedCards)
        {
            if (submission.Value == winningCardValue)
            {
                winner = submission.Key;
                break;
            }
        }

        if (winner != null)
        {
            winner.CustomProperties.TryGetValue("score", out object scoreValue);
            int score = (scoreValue == null) ? 0 : (int)scoreValue;
            int newScore = score + winningCardValue;
            Hashtable scoreProp = new Hashtable { { "score", newScore } };
            winner.SetCustomProperties(scoreProp);
        }
    }

    [PunRPC]
    private void UpdateUiRPC()
    {
        int index = 0;
        if (_currentGameState == GameState.Submit)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                player.CustomProperties.TryGetValue("leftCardNum", out object leftCardObj);
                player.CustomProperties.TryGetValue("rightCardNum", out object rightCardObj);
                if (leftCardObj != null && rightCardObj != null)
                    _submits[index].GetComponent<SubmitCardData>()
                        .Submit(player.NickName, (int)leftCardObj, (int)rightCardObj);
                index++;
            }
        }
        else if (_currentGameState == GameState.Select)
        {
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                player.CustomProperties.TryGetValue("selectedCard", out object selectedCardObj);
                player.CustomProperties.TryGetValue("remainCard", out object remainCardObj);
                if (selectedCardObj != null && remainCardObj != null)
                    _submits[index].GetComponent<SubmitCardData>()
                        .Select(player.NickName, (int)selectedCardObj, (int)remainCardObj);
                index++;
            }
        }
    }
}