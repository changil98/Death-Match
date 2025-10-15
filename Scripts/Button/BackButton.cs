using UnityEngine;

public class BackButton : BaseButton
{
    [SerializeField] private GameObject obj;

    protected override void OnClickButton()
    {
        if (obj != null && obj.activeSelf)
            obj.SetActive(false);
    }
}