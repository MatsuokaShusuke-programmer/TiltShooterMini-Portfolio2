using TMPro;
using UnityEngine;

/// <summary>
/// スコアを管理・表示するクラス
/// </summary>
public class ScoreManager:MonoBehaviour{
    public int Score{get;private set;}=0;
    public TextMeshProUGUI scoreText;

    void Start(){
        scoreText=gameObject.GetComponent<TextMeshProUGUI>();
        UpdateScoreText();
    }

    /// <summary>
    /// スコアを加算
    /// </summary>
    /// <param name="value"></param>
    public void AddScore(int value){
        Score+=value;
        UpdateScoreText();
    }

    /// <summary>
    /// スコア表示を更新
    /// </summary>
    void UpdateScoreText(){
        if (scoreText)scoreText.text=Score.ToString();
    }
}