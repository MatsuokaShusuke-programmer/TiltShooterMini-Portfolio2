using UnityEngine;

/// <summary>
/// InGameを始めるクラス
/// </summary>
public class InGameStarter:MonoBehaviour {
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        //仮でゲームモードをノーマルで始める
        GameManager.Instance.GameStart(GameManager.GameMode.Normal);
    }
}
