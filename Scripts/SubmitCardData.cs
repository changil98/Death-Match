using TMPro;
using UnityEngine;

public class SubmitCardData : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text leftCardText;
    [SerializeField] private TMP_Text rightCardText;
    [SerializeField] private TMP_Text selectedCardText;
    
    public void Init(string nickname)
    {
        nameText.text = nickname;
    }
    
    public void Submit(string nickname, int leftCard, int rightCard)
    {
        nameText.text = nickname;
        leftCardText.fontStyle = FontStyles.Normal;
        leftCardText.text = leftCard.ToString();
        rightCardText.text = rightCard.ToString();
    }

    public void Select(string nickname, int selectedCard, int remainCard)
    {
        nameText.text = nickname;
        leftCardText.fontStyle = FontStyles.Underline;
        leftCardText.text = selectedCard.ToString();
        rightCardText.text = remainCard.ToString();
    }

    public void ResetText(string nickname)
    {
        nameText.text = nickname;
        leftCardText.fontStyle = FontStyles.Normal;
        leftCardText.text = "0";
        rightCardText.text = "0";
    }
}