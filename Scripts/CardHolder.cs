using System;
using UnityEngine;
using UnityEngine.UI;

public enum CardHolderType
{
    Left,
    Right,
    Remain
}

public class CardHolder : MonoBehaviour
{
    public static event Action<CardHolder> OnCardSelected;
    
    [SerializeField] private CardHolderType cardHolderType;
    
    private Button _button;
    private GameState _currentGameState;
    private DropHandler _dropHandler;

    private void Awake()
    {
        if (cardHolderType == CardHolderType.Remain) return;
        _dropHandler = GetComponent<DropHandler>();
        _button = gameObject.GetComponent<Button>();
        _button.onClick.AddListener(SelectCard);
    }

    private void OnEnable() => RoundManager.OnGameStateChange += GameStateChange;
    private void OnDisable() => RoundManager.OnGameStateChange -= GameStateChange;

    private void GameStateChange(GameState gameState)
    {
        _currentGameState = gameState;
        if (cardHolderType == CardHolderType.Remain) return;
        DropActive();
    }
    
    private void SelectCard()
    {
        if (_currentGameState != GameState.Select) return;
        OnCardSelected?.Invoke(this);
    }

    public CardHolderType GetCardHolderType()
    {
        return cardHolderType;
    }
    
    private void DropActive()
    {
        switch (_currentGameState)
        {
            case GameState.Submit: _dropHandler.enabled = true; break;
            case GameState.Select:
            case GameState.Waiting:
            case GameState.End:
                _dropHandler.enabled = false; break;
        }
    }
}
