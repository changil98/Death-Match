using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static event Action OnSortCardList;
    public event Action<Transform> OnStartDragging;
    public event Action OnStopDragging;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Transform _previousParent;
    private bool _isLocked;
    private bool _isChange;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isLocked) return;
        
        _isChange = false;
        _previousParent = _rectTransform.parent;
        OnStartDragging?.Invoke(_previousParent);

        transform.SetParent(transform.root);
        transform.SetAsLastSibling();

        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isLocked) return;
        _rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isLocked) return;

        if (!_isChange)
            _rectTransform.SetParent(_previousParent);
        
        _canvasGroup.blocksRaycasts = true;
        OnStopDragging?.Invoke();
        OnSortCardList?.Invoke();
    }

    public void OnDropped(Transform newParent)
    {
        if (_previousParent != null && newParent.childCount > 0)
        {
            var existing = newParent.GetComponentInChildren<DragHandler>();
            existing?.Move(_previousParent);
            existing?.UnlockDrag();
        }
        
        Move(newParent);
        LockDrag();
        OnStopDragging?.Invoke();
        OnSortCardList?.Invoke();
    }

    private void Move(Transform target) // 카드 위치 변경
    {
        _isChange = true;
        _rectTransform.SetParent(target);
        _rectTransform.position = target.position;
    }
    
    private void LockDrag()
    {
        _isLocked = true;
        _canvasGroup.blocksRaycasts = false;
    }

    public void UnlockDrag()
    {
        _isLocked = false;
        _canvasGroup.blocksRaycasts = true;
    }
}