using UnityEngine;
using System.Collections.Generic;

public class CardList : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;

    private Transform _cardListParent;

    private void OnEnable() => DragHandler.OnSortCardList += SortCards;
    private void OnDisable() => DragHandler.OnSortCardList -= SortCards;

    private void Awake()
    {
        _cardListParent = GetComponent<Transform>();
    }

    private void Start()
    {
        InitializeDeck();
    }

    public void InitializeDeck() // 게임을 시작하면 카드 초기 세팅
    {
        if (_cardListParent.childCount > 0)
        {
            foreach (Transform child in _cardListParent)
            {
                Destroy(child.gameObject);
            }
        }
        
        for (int i = 1; i < 9; i++)
        {
            GameObject cardObject = Instantiate(cardPrefab, _cardListParent);
            Card card = cardObject.GetComponent<Card>();
            card.Init(i, this);
        }
    }
    
    public void SortCards()
    {
        List<Card> cards = new List<Card>();
        foreach (Transform child in _cardListParent)
        {
            Card card = child.GetComponent<Card>();
            card.GetComponent<DragHandler>().UnlockDrag();
            if (card != null)
            {
                cards.Add(card);
            }
        }

        // 카드 번호 기준 오름차순 정렬
        cards.Sort((a, b) => a.GetNum().CompareTo(b.GetNum()));
        
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.SetSiblingIndex(i); // Hierarchy 순서 변경
        }
    }

    public GameObject SubmitRandomCard()
    {
        int num = Random.Range(0, _cardListParent.childCount);
        return _cardListParent.GetChild(num).gameObject;
    }
}