using System;
using System.Collections;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static event Action OnRandomCard;
    public static event Action OnStopDragging;
    
    private TMP_Text _timerText;
    private float _limitTime;
    private bool _isOnClick;
    private GameState _currentGameState;
    private PhotonView _photonView;
    private Coroutine _timerCoroutine;

    private void OnEnable()
    {
        RoundManager.OnTimerStart += StartTimer;
        ReadyButton.OnReadyButtonClicked += OnClick;
        RoundManager.OnGameStateChange += GameStateChange;
    }

    private void OnDisable()
    {
        RoundManager.OnTimerStart -= StartTimer;
        ReadyButton.OnReadyButtonClicked -= OnClick;
        RoundManager.OnGameStateChange -= GameStateChange;
    }

    private void Awake()
    {
        _timerText = GetComponent<TMP_Text>();
        _photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartTimer(10);
        }
    }

    private IEnumerator TimerCoroutine(float time)
    {
        _isOnClick = false;
        _limitTime = time;
        while (_limitTime > 0)
        {
            _limitTime -= Time.deltaTime;
            _photonView.RPC(nameof(ShowTimer), RpcTarget.All, _limitTime);
            yield return null;

            if (_currentGameState == GameState.End)
            {
                _photonView.RPC(nameof(ShowTimer), RpcTarget.All, 0f);
                yield break;
            }
            
            if (_limitTime <= 0)
            {
                OnStopDragging?.Invoke();
                if (!_isOnClick)
                    OnRandomCard?.Invoke();
                yield break;
            }
        }
    }
    
    private void StartTimer(int time)
    {
        _photonView.RPC(nameof(StartTimerAll), RpcTarget.All, time);
    }
    
    [PunRPC]
    private void StartTimerAll(int time)
    {
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
        }
        _timerCoroutine = StartCoroutine(TimerCoroutine(time));
    }

    [PunRPC]
    private void ShowTimer(float time)
    {
        if (time < 0) time = 0;
        _timerText.text = time.ToString("F1");
    }

    private void OnClick()
    {
        _isOnClick = true;
    }
    
    private void GameStateChange(GameState gameState)
    {
        _currentGameState = gameState;
    }
}