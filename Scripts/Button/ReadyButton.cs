using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ReadyButton : BaseButton
{
    public static event Action OnReadyButtonClicked;

    [SerializeField] private GameObject leftCardHolder;
    [SerializeField] private GameObject rightCardHolder;
    [SerializeField] private GameObject remainCardHolder;
    [SerializeField] private GameObject cardListObj;
    [SerializeField] private GameObject resultObj;
    
    private TextMeshProUGUI _buttonText;
    private GameState _currentGameState;

    private Card _selectedCard;
    private CardList _cardList;
    private CardHolderType _selectCardHolderType;
    
    private Result _result;

    protected override void Awake()
    {
        base.Awake();
        _buttonText = GetComponentInChildren<TextMeshProUGUI>();
        _cardList = cardListObj.GetComponent<CardList>();
        if (resultObj != null)
            _result = resultObj.GetComponent<Result>();
    }

    private void OnEnable()
    {
        RoundManager.OnGameStateChange += GameStateChange;
        CardHolder.OnCardSelected += SelectCard;
        Timer.OnRandomCard += RandomCard;
    }

    private void OnDisable()
    {
        RoundManager.OnGameStateChange -= GameStateChange;
        CardHolder.OnCardSelected -= SelectCard;
        Timer.OnRandomCard -= RandomCard;
    }

    private void GameStateChange(GameState gameState)
    {
        _currentGameState = gameState;
        UpdateUI();
    }
    
    protected override void OnClickButton()
    {
        if (_currentGameState == GameState.Submit)
        {
            if (leftCardHolder.transform.childCount <= 0 || rightCardHolder.transform.childCount <= 0) return;
            int leftCardNum = leftCardHolder.transform.GetChild(0).GetComponent<Card>().GetNum();
            int rightCardNum = rightCardHolder.transform.GetChild(0).GetComponent<Card>().GetNum();
            Hashtable properties = new Hashtable
            {
                { "isSubmit", true },
                { "leftCardNum", leftCardNum },
                { "rightCardNum", rightCardNum }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
        }
        else if (_currentGameState == GameState.Select)
        {
            if (_selectedCard == null) return;
            int remainCard = CardPositionChange();
            Hashtable properties = new Hashtable
            {
                { "isSelect", true },
                { "selectedCard", _selectedCard.GetNum() },
                { "remainCard", remainCard }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
        }
        else if (_currentGameState == GameState.Waiting)
        {
            Hashtable properties = new Hashtable
            {
                { "isReady", true },
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(properties);
        }
        
        _buttonText.text = "대기중";
        Button.interactable = false;
        OnReadyButtonClicked?.Invoke();
    }

    private void SelectCard(CardHolder cardHolder)
    {
        if (cardHolder.transform.childCount <= 0) return;
        _selectCardHolderType = cardHolder.GetCardHolderType();
        _selectedCard = cardHolder.GetComponentInChildren<Card>();
    }

    private void RandomCard()
    {
        switch (_currentGameState)
        {
            case GameState.Submit:
                if (leftCardHolder.transform.childCount <= 0)
                {
                    GameObject leftCard = _cardList.SubmitRandomCard();
                    leftCard.transform.SetParent(leftCardHolder.transform);
                    leftCard.transform.position = leftCardHolder.transform.position;
                }

                if (rightCardHolder.transform.childCount <= 0)
                {
                    GameObject rightCard = _cardList.SubmitRandomCard();
                    rightCard.transform.SetParent(rightCardHolder.transform);
                    rightCard.transform.position = rightCardHolder.transform.position;
                }
                break;
            case GameState.Select:
                if (_selectedCard == null)
                {
                    int randomNum = Random.Range(0, 2);
                    if (randomNum == 0)
                        SelectCard(leftCardHolder.GetComponent<CardHolder>());
                    else
                        SelectCard(rightCardHolder.GetComponent<CardHolder>());
                }
                break;
            case GameState.Waiting:
            case GameState.End:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        OnClickButton();
    }

    private int CardPositionChange()
    {
        switch (_selectCardHolderType)
        {
            case CardHolderType.Left:
                Destroy(_selectedCard.gameObject);
                var rightCard = rightCardHolder.transform.GetChild(0).gameObject;
                ChangePosition(rightCard);
                return rightCard.GetComponent<Card>().GetNum();
            case CardHolderType.Right:
                Destroy(_selectedCard.gameObject);
                var leftCard = leftCardHolder.transform.GetChild(0).gameObject;
                ChangePosition(leftCard);
                return leftCard.GetComponent<Card>().GetNum();
            default:
                return 0;
        }
    }

    private void ChangePosition(GameObject obj)
    {
        if (remainCardHolder.transform.childCount > 0)
        {
            GameObject remainCard = remainCardHolder.transform.GetChild(0).gameObject;
            remainCard.transform.SetParent(cardListObj.transform);
            cardListObj.GetComponent<CardList>().SortCards();
        }

        obj.transform.SetParent(remainCardHolder.transform);
        obj.transform.position = remainCardHolder.transform.position;
    }

    private void UpdateUI()
    {
        switch (_currentGameState)
        {
            case GameState.Submit: _buttonText.text = "제출완료"; break;
            case GameState.Select: _buttonText.text = "선택완료"; break;
            case GameState.Waiting: _buttonText.text = "준비완료"; break;
            case GameState.End: 
                resultObj.SetActive(true);
                _result.Init();
                break;
        }
        Button.interactable = true;
    }
}