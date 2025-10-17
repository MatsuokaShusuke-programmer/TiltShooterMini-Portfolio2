using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 3D領域内に、要求された数を必ず "重ならずに" 生成するスポナー。
/// 
/// アルゴリズムの概要:
/// 1.領域体積と要求数から、理論的に可能な最大距離を計算。
/// 2.ポアソンディスクサンプリング（Bridson法）を実行し、サンプル座標を取得。
/// 3.サンプル座標が足りなければ、距離を緩めて再試行。
/// 4.最終的に必ず指定された数の敵を配置。
/// </summary>
public class EnemySpawner:SceneSingleton<EnemySpawner>{

    //敵のスポーン情報
    [System.Serializable]struct EnemySpawnInfo{

        public GameObject enemy;//スポーンする敵
        public int spawnNum;//生成する敵の数

    }

    [SerializeField]EnemySpawnInfo[] _enemySpawnInfos;

    [Header("生成領域")]
    [SerializeField]float _minX=-10f;
    [SerializeField]float _maxX=10f;
    [SerializeField]float _minY=-5f;
    [SerializeField]float _maxY=5f;
    [SerializeField]float _minZ=15f;
    [SerializeField]float _maxZ=5f;

    [Header("制約")]
    [SerializeField]float _minDistance=2f;//希望の最小中心間距離
    [SerializeField]int _numberOfTrials=30;//候補数
    [SerializeField]int _maxRadiusRelaxSteps=6;//うまく取れないときに距離を緩める試行回数

    public int currentEnemyNum=0;//現在のEnemyの数

    //Enemyの角度
    static readonly Quaternion Rotation=Quaternion.Euler(0,180,0);

    // 球の密集度（close packing、3Dの最大詰め込み率に近い値）
    const float PackingDensity3D=0.74048f;

    void Start(){
        Spawn();
    }

    /// <summary>
    /// 敵を生成するメイン処理
    /// </summary>
    public void Spawn(){
        int totalSpawnNum=0;//全ての敵の数の合計
        foreach (EnemySpawnInfo info in _enemySpawnInfos){
            totalSpawnNum+=info.spawnNum;
        }

        // 領域の体積を計算
        float width=Mathf.Max(0.0001f,_maxX-_minX);
        float height=Mathf.Max(0.0001f,_maxY-_minY);
        float depth=Mathf.Max(0.0001f,_maxZ-_minZ);
        float volume=width*height*depth;

        //入力のminDistanceは中心間距離なので、球半径に換算(*0.5f)
        float desiredRadius=_minDistance*0.5f;

        //理論上、指定された数の球が重ならずに収まるための最大半径を計算
        //（体積 / (敵の数 * 球の体積の定数)）の3乗根
        float sphereDenom=(4f/3f)*Mathf.PI;
        float maxFeasibleRadius
            =Mathf.Pow(
                (volume*PackingDensity3D)/(totalSpawnNum*sphereDenom),
                1f/3f
            );

        //実際に使用する半径を、希望の半径と理論上の最大半径の小さい方に設定
        float useRadius=Mathf.Min(desiredRadius,maxFeasibleRadius);

        // 数値的な安定性を確保
        useRadius=Mathf.Max(useRadius,0.0001f);

        // Bridsonのアルゴリズムでサンプルを生成（必要なら段階的に半径を緩める）
        List<Vector3> samples=null;
        float radius=useRadius;
        for (int relaxStep=0; relaxStep<=_maxRadiusRelaxSteps; relaxStep++){
            //ポアソンディスクサンプリングを実行(配置可能な点を取得)
            samples
                =PoissonDiskSampling3D(radius,_numberOfTrials,width,height,depth);

            // 十分な数の点が生成されたらループを抜ける
            if(samples.Count>=totalSpawnNum)break;

            // 点が足りない場合、半径を少し緩めて再試行
            radius*=0.85f;
            if(radius<0.0001f){
                radius=0.0001f;
                break;
            }
        }

        //samplesがnullのとき実行
        samples??=PoissonDiskSampling3D(radius,_numberOfTrials,width,height,depth);

        // サンプルをランダムな順序にするためシャッフル
        Shuffle(samples);

        int sampleIndex=0;//サンプルのをどこまで使ったか
        foreach (EnemySpawnInfo info in _enemySpawnInfos){
            int spawned=0;//infoの種類を何体スポーンできたか
            
            //サンプルが残っていればそこから配置
            for (; spawned<info.spawnNum&&sampleIndex<samples.Count;spawned++,sampleIndex++){
                Vector3 p=samples[sampleIndex];//サンプル
                //生成
                Instantiate(info.enemy,ToWorldPosition(p,width,height,depth),Rotation);
                currentEnemyNum++;
            }
            
            //サンプルが足りなければランダムに保管
            while (spawned<info.spawnNum){
                Vector3 p=GenerateRandomPosition();
                Instantiate(info.enemy,p,Rotation);
                currentEnemyNum++;
                spawned++;
            }
        }
    }

    /// <summary>
    /// 敵が全てやられたら再度スポーン
    /// </summary>
    public void ReSpawn(){
        if(currentEnemyNum<=0) Spawn();
    }

    /// <summary>
    /// Bridsonのアルゴリズムによるポアソンディスクサンプリング
    /// </summary>
    /// <param name="radius">サンプル点間の最小距離（球の半径）</param>
    /// <param name="k">新しい候補点を探す試行回数</param>
    /// <param name="width">生成領域の幅</param>
    /// <param name="height">生成領域の高さ</param>
    /// <param name="depth">生成領域の奥行き</param>
    /// <returns>重ならない点のリスト</returns>
    List<Vector3> PoissonDiskSampling3D(
        float radius,int k,float width,float height,float depth
    ){
        List<Vector3> samples=new List<Vector3>();
        //生成領域の奥行が0以下の場合リターン
        if(radius<=0f) return samples;

        // グリッドセルのサイズを計算（radius/sqrt(3)）
        float cellSize=radius/Mathf.Sqrt(3f);
        int gridX=Mathf.CeilToInt(width/cellSize);//x方向にあるセルの数
        int gridY=Mathf.CeilToInt(height/cellSize);//y方向にあるセルの数
        int gridZ=Mathf.CeilToInt(depth/cellSize);//z方向にあるセルの数

        //グリッドを初期化
        int[] grid=new int[gridX*gridY*gridZ];//セルの個数(gridX*gridY*gridZ)
        for (int i=0; i<grid.Length; i++) grid[i]=-1;//-1で初期化

        //敵の生成できる可能性のある場所を管理するためのリスト
        List<Vector3> active=new List<Vector3>();

        // 最初の点をランダムに生成
        Vector3 init=new Vector3(Random.value*width,Random.value*height,Random.value*depth);
        samples.Add(init);
        active.Add(init);
        SetGrid(grid,gridX,gridY,gridZ,init,0,cellSize);

        //敵が生成できる可能性のある場所がある限りループ
        while (active.Count>0){
            // アクティブな点からランダムに1つ選択
            int idx=Random.Range(0,active.Count);
            Vector3 point=active[idx];
            bool isFound=false;//新しい候補がみつかったか

            // k回、新しい候補点を試行
            for (int i=0; i<k; i++){
                // [radius, 2radius]の範囲で新しい点を生成
                Vector3 dir=Random.onUnitSphere;//半径1の球の表面上のランダムな点(ランダムな方向)
                float r=Random.Range(radius,2f*radius);
                Vector3 cand=point+dir*r;

                // 候補点が領域内にあるかチェック
                if(cand.x<0||cand.x>width||cand.y<0||cand.y>height||cand.z<0||cand.z>depth)
                    continue;

                // 近傍のグリッドをチェックし、重なりがないか確認
                if(IsValidCandidate(
                       grid,
                       gridX,
                       gridY,
                       gridZ,
                       cand,
                       samples,
                       radius,
                       cellSize
                   )){
                    samples.Add(cand);
                    active.Add(cand);
                    SetGrid(
                        grid,
                        gridX,
                        gridY,
                        gridZ,
                        cand,
                        samples.Count-1,
                        cellSize
                    );
                    isFound=true;
                    break;
                }
            }

            // k回の試行で有効な点が見つからなかった場合、そのアクティブ点をリストから削除
            if(!isFound) active.RemoveAt(idx);
        }

        //配置可能な点を全て返す
        return samples;
    }

    /// <summary>
    /// 点をグリッドに登録する
    /// </summary>
    /// <param name="grid">グリッド配列</param>
    /// <param name="gx">グリッドのX方向のサイズ</param>
    /// <param name="gy">グリッドのY方向のサイズ</param>
    /// <param name="gz">グリッドのZ方向のサイズ</param>
    /// <param name="p">登録する点の位置</param>
    /// <param name="sampleIndex">点のインデックス</param>
    /// <param name="cellSize">グリッドのセルのサイズ</param>
    void SetGrid(int[] grid,int gx,int gy,int gz,Vector3 p,int sampleIndex,float cellSize){
        //点の位置pをグリッドの変数に変換
        int ix=Mathf.Clamp((int)(p.x/cellSize),0,gx-1);
        int iy=Mathf.Clamp((int)(p.y/cellSize),0,gy-1);
        int iz=Mathf.Clamp((int)(p.z/cellSize),0,gz-1);

        //3Dグリッドインデックスを1D配列に変換して、点のインデックスを記録
        grid[(ix*gy+iy)*gz+iz]=sampleIndex;
    }

    /// <summary>
    /// 候補点が他の点と指定距離以上離れているか判定する
    /// </summary>
    /// <param name="grid">点のインデックスが記録されたグリッド配列</param>
    /// <param name="gx">グリッドのX方向のセル数</param>
    /// <param name="gy">グリッドのY方向のセル数</param>
    /// <param name="gz">グリッドのZ方向のセル数</param>
    /// <param name="cand">検証する候補点の位置</param>
    /// <param name="samples">これまでに配置された全点のリスト</param>
    /// <param name="radius">点間の最小距離（球の半径）</param>
    /// <param name="cellSize">グリッドのセルの1辺の長さ</param>
    /// <returns>有効な候補点ならtrue、そうでなければfalse</returns>
    bool IsValidCandidate(
        int[] grid,
        int gx,int gy,int gz,
        Vector3 cand,List<Vector3> samples,float radius,float cellSize
    ){
        //候補の点のグリッド上の距離を計算
        int ix=Mathf.Clamp((int)(cand.x/cellSize),0,gx-1);
        int iy=Mathf.Clamp((int)(cand.y/cellSize),0,gy-1);
        int iz=Mathf.Clamp((int)(cand.z/cellSize),0,gz-1);

        int neigh=2;// チェックする隣接セルの範囲
        float r2=radius*radius;//距離の2乗のキャッシュ

        //候補点の周囲のグリッドセルを全てチェック
        for (int dx=-neigh; dx<=neigh; dx++){
            int nx=ix+dx;
            if(nx<0||nx>=gx) continue;
            for (int dy=-neigh; dy<=neigh; dy++){
                int ny=iy+dy;
                if(ny<0||ny>=gy) continue;
                for (int dz=-neigh; dz<=neigh; dz++){
                    int nz=iz+dz;
                    if(nz<0||nz>=gz) continue;

                    //3Dのグリッドインデックスを1D配列のインデックスに変換
                    int gridIndex=(nx*gy+ny)*gz+nz;
                    int sidx=grid[gridIndex];

                    //セルが空で無ければ、距離をチェック
                    if(sidx!=-1){
                        Vector3 sp=samples[sidx];
                        //距離の2乗が半径の2乗より小さければ、false
                        if((sp-cand).sqrMagnitude<r2) return false;
                    }
                }
            }
        }

        //全てのチェックを通過(一定以上離れている)した場合true
        return true;
    }

    /// <summary>
    /// ローカル座標をワールド座標に変換
    /// </summary>
    Vector3 ToWorldPosition(Vector3 local,float width,float height,float depth){
        float x=_minX+local.x;
        float y=_minY+local.y;
        float z=_minZ+local.z;
        return new Vector3(x,y,z);
    }

    /// <summary>
    /// 指定範囲内のランダムな位置を生成
    /// </summary>
    Vector3 GenerateRandomPosition(){
        float x=Random.Range(_minX,_maxX);
        float y=Random.Range(_minY,_maxY);
        float z=Random.Range(_minZ,_maxZ);
        return new Vector3(x,y,z);
    }

    /// <summary>
    /// Fisher-Yates シャッフル
    /// </summary>
    void Shuffle<T>(List<T> list){
        for (int i=list.Count-1; i>0; i--){
            int r=Random.Range(0,i+1);
            (list[i],list[r])=(list[r],list[i]);
        }
    }

}