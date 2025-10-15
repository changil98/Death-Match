using UnityEngine;

public class SetCreateRoomBaseButton : BaseButton
{
    [SerializeField] private GameObject createRoomPanel;

    protected override void Awake()
    {
        base.Awake();
        createRoomPanel.SetActive(false);
    }
    
    protected override void OnClickButton()
    {
        if (createRoomPanel.activeSelf) return;
        createRoomPanel.SetActive(true);
    }
}