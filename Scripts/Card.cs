using TMPro;
using UnityEngine;

public class Card : MonoBehaviour
{
    private DragHandler _dragHandler;
    private TextMeshProUGUI _numberText;
    private int _cardNumber;
    private GameState _currentGameState;
    private CardList _cardList;
    private bool _isDragging;
    private Transform _previousParent;

    private void Awake()
    {
        _dragHandler = GetComponent<DragHandler>();
        _numberText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        RoundManager.OnGameStateChange += GameStateChange;
        Timer.OnStopDragging += ReturnToPreviousParent;
        _dragHandler.OnStartDragging += StartDragging;
        _dragHandler.OnStopDragging += StopDragging;
    }

    private void OnDisable()
    {
        RoundManager.OnGameStateChange -= GameStateChange;
        Timer.OnStopDragging -= ReturnToPreviousParent;
        _dragHandler.OnStartDragging -= StartDragging;
        _dragHandler.OnStopDragging -= StopDragging;
    }

    public void Init(int num, CardList cardList)
    {
        _cardNumber = num;
        _numberText.text = _cardNumber.ToString();
        _cardList = cardList;
    }

    private void GameStateChange(GameState gameState)
    {
        _currentGameState = gameState;
        DragActive();
    }

    private void DragActive()
    {
        switch (_currentGameState)
        {
            case GameState.Submit: _dragHandler.enabled = true; break;
            case GameState.Select:
            case GameState.Waiting:
            case GameState.End:
                _dragHandler.enabled = false; break;
        }
    }

    public int GetNum()
    {
        return _cardNumber;
    }

    private void StartDragging(Transform previousParent)
    {
        _isDragging = true;
        _previousParent = previousParent;
    }

    private void StopDragging()
    {
        _isDragging = false;
    }

    private void ReturnToPreviousParent()
    {
        if (!_isDragging) return;
        gameObject.transform.SetParent(_previousParent);
        gameObject.transform.position = _previousParent.position;
        _cardList.SortCards();
    }
}