using UnityEngine;
using UnityEngine.UI;

public abstract class BaseButton : MonoBehaviour
{
    protected Button Button;

    protected virtual void Awake()
    {
        Button = GetComponent<Button>();
        if (Button != null)
            Button.onClick.AddListener(OnClickButton);
    }

    protected abstract void OnClickButton();
}
