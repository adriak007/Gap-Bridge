using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankingEntry : MonoBehaviour
{
    [SerializeField] public TMP_Text rankText;
    [SerializeField] public TMP_Text scoreText;
    [SerializeField] public Image    background;

    public void Setup(int rank, int score)
    {
        gameObject.SetActive(true);

        if (rankText)  rankText.text  = rank > 0 ? "#" + rank : "-";
        if (scoreText) scoreText.text = score > 0 ? score + " pts" : "---";

        if (background)
        {
            switch (rank)
            {
                case 1:  background.color = new Color32(255, 215,   0, 70); break;
                case 2:  background.color = new Color32(192, 192, 192, 50); break;
                case 3:  background.color = new Color32(205, 127,  50, 50); break;
                default: background.color = new Color32(255, 255, 255, 15); break;
            }
        }
    }

    public void SetEmpty()
    {
        if (rankText)   rankText.text  = "-";
        if (scoreText)  scoreText.text = "---";
        if (background) background.color = new Color32(255, 255, 255, 10);
    }
}
