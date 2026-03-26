using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// どの敵が何体どの順番で出現するかを定義するデータ
/// </summary>
[CreateAssetMenu(fileName="StageData",menuName="Data/StageData")]
public class StageData:ScriptableObject{
    /// <summary>
    /// Inspectorで最初に表示され、このアセットの名前となる
    /// コード自体の実行には影響しないけど、便利
    /// </summary>
    public string stageName="StageData";

    [Header("ステージ情報")]
    [Tooltip("ステージの難易度")]
    public int difficultyRank;

    [Tooltip("クリア時に獲得する経験値")]
    public int awardEXP;

    [Header("敵のスポーン情報")]
    public List<WaveData> waves;
}

///<summary>
///ステージにおける敵の1ウェーブ卯文の設定データ
///StageDataAseetに埋め込まれて使用される
/// </summary>
//これがないとStageDataのInspectorでリストとして表示されない
[System.Serializable]
public class WaveData {
    public string waveName = "Wave";
    //Enemyの生成情報のリスト
    public List<EnemySpawnInfo> ememiesToSpawn;
    public float timeToNextWave;
    //次のウェーブの時間の誤差
    public float timeToNextWaveError;
}


/// <summary>
/// Enemyの生成情報
/// </summary>
[System.Serializable]
public class EnemySpawnInfo {
    public GameObject enemyPrefab;
    public int count=5;
    public float spawnInterval;
    public float spawnIntervalError;
    public float timeToNextSpawnEnemy;
    public float timeToNextSpawnEnemyError;
}