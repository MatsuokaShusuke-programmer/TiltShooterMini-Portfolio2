using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class EnemyManager:SceneSingleton<EnemyManager> {
    [Header("実行するステージ")]
    [SerializeField] StageData _currentStageData;

    [Header("敵の出現位置")]
    [SerializeField] Transform[] _spawnPoints;

    public event Action OnAllEnemiesDie;

    //生存してるEnemyを管理するリスト
    List<GameObject> _activeEnemies=new List<GameObject>();

    //デバッグ用
    private void Start() {
        StartSpawning(_currentStageData);
    }

    private void Update() {
        //リストの中身を見て、消滅した敵を削除
        _activeEnemies.RemoveAll(enemy => enemy==null);
    }

    /// <summary>
    /// スポーン開始命令
    /// </summary>
    /// <param name="stageData">ステージデータ</param>
    public void StartSpawning(StageData stageData) {
        Debugger.Log("スポーン開始");
        _currentStageData=stageData;
        StartCoroutine(SpawnAllWaves());
    }

    /// <summary>
    /// 現在のステージデータに基づき、敵のウェーブを順次生成
    /// <para>
    /// エンドレスモードの場合は、全ウェーブ終了後にループして再実行
    /// </para>
    /// </summary>
    IEnumerator SpawnAllWaves() {
        //エンドレスモード用にループさせる
        do {
            //StageDataのwavesリストを順番に実行
            foreach(WaveData wave in _currentStageData.waves) {
                //1Wave分の敵グループを実行
                foreach(
                    EnemySpawnInfo enemyGroup in wave.ememiesToSpawn
                ) {
                    //Enemyをcount分生成
                    for(int i = 0;i<enemyGroup.count;i++) {
                        SpawnEnemy(enemyGroup.enemyPrefab);

                        //生成時間をある程度ランダムにする
                        float interval
                            = enemyGroup.spawnInterval
                                +Random.Range(
                                    -enemyGroup.spawnIntervalError,
                                    enemyGroup.spawnIntervalError
                                );

                        //次の1体を生成するまで待機
                        //0以上にする
                        yield return
                            new WaitForSeconds(Mathf.Max(0f,interval));
                    }

                    //生成時間をある程度ランダムにする
                    float nextSpawnEnemyWait
                        = enemyGroup.timeToNextSpawnEnemy
                            +Random.Range(
                                -enemyGroup.timeToNextSpawnEnemyError,
                                enemyGroup.timeToNextSpawnEnemyError
                            );

                    //次のEnemy群を生成するまで待機
                    yield return new WaitForSeconds(nextSpawnEnemyWait);
                }

                //生成時間をある程度ランダムにする
                float nextWaveWait
                    = wave.timeToNextWave
                        +Random.Range(
                            -wave.timeToNextWaveError,
                            wave.timeToNextWaveError
                        );

                //このウェーブが完了したら、次のウェーブまで待機
                yield return new WaitForSeconds(Mathf.Max(0f,nextWaveWait));
            }

            //ノーマルモードなら、敵が全滅するまでまってからクリア
            if(GameManager.Instance.CurrentGameMode==
                GameManager.GameMode.Normal) {
                //敵が0体になるまで待つ
                yield return new WaitUntil(()=>_activeEnemies.Count <= 0);

                //全滅したのでクリア
                OnAllEnemiesDie?.Invoke();
            }

            //エンドレスモードなら、次の周回まで休憩を入れる
            if(GameManager.Instance.CurrentGameMode
                    ==GameManager.GameMode.Endless)
                yield return new WaitForSeconds(3.0f);

        //GameManagerのモードを見てループするか決める
        } while(
            GameManager.Instance.CurrentGameMode
            ==GameManager.GameMode.Endless
        );
    }

    void SpawnEnemy(GameObject enemyPrefab) {
        //スポーン地点をランダムに選ぶ
        Transform randomSpawnPoint
            = _spawnPoints[Random.Range(0,_spawnPoints.Length)];

        //敵を生成
        GameObject newEnemy=Instantiate(
            enemyPrefab,
            randomSpawnPoint.position,
            randomSpawnPoint.rotation
        );

        _activeEnemies.Add(newEnemy);
    }
}