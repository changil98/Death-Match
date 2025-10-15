using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text scoreText;

    public void Init(string nickname)
    {
        nameText.text = nickname;
    }

    public void UpdateScore(string nickname, int score)
    {
        nameText.text = nickname;
        scoreText.text = score.ToString();
    }

    public void ResetText(string nickname)
    {
        nameText.text = nickname;
        scoreText.text = "0";
    }
}
