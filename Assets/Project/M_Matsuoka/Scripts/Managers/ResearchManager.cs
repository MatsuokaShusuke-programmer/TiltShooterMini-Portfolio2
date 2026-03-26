using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 経験値(研究)システムを管理するManager
/// </summary>
public class ResearchManager:PersistentSingleton<ResearchManager> {
    //現在の累計経験値
    public int CurrentEXP { get; private set; }

    /// <summary>
    /// 経験値を加算する
    /// </summary>
    /// <param name="amount">獲得した経験値量</param>
    public void AddEXP(int amount) => CurrentEXP+=amount;

    /// <summary>
    /// 経験値を消費する(研究・強化で使用)
    /// </summary>
    /// <param name="amount">消費する量</param>
    /// <returns>消費できたらturue、足りなければfalse</returns>
    public bool ConsumeEXP(int amount) {
        //足りるかチェック
        if(CurrentEXP>=amount) {
            CurrentEXP-=amount;
            return true;//成功
        }

        return false;//失敗
    }
}
