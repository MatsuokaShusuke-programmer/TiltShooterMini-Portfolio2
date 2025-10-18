using UnityEngine;

/// <summary>
/// UIManagerへUI情報を渡す
/// </summary>
public class UIGetter : MonoBehaviour{
    [SerializeField]UIManager.ButtonInfo[] _buttonInfos;//ボタンの情報
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        UIManager.Ins.buttonInfos=_buttonInfos;//ボタンの情報を渡す
        UIManager.Ins.currentlySelectedButtonInfoIndex=0;//ボタン情報のインデックスを渡す

        UIManager.Ins.Init();
    }
}
