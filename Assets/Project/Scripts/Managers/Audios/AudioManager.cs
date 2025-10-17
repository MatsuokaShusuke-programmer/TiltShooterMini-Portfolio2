using UnityEngine;

public class AudioManager:PersistentSingleton<AudioManager>{
    [System.Serializable]struct AudioInfo{
        public AudioClip audioClip;
        public float volume;
    }
    [Header("Audio Clips")]
    [SerializeField]AudioInfo[] _bgmAudioInfo;
    [SerializeField]AudioInfo[] _seAudioInfo;
    
    [Header("Audio Sources")]
    [SerializeField]AudioSource _bgmAudioSource;
    [SerializeField]AudioSource _seAudioSource;
    
    [Header("BGM Index")]
    public int titleBGMIndex=0;
    public int gameBGMIndex=1;

    [Header("SE Index")]
    public int shootSEIndex=0;      //発射音
    public int explosionSEIndex=1;  //爆破音
    public int hitSEIndex=2;        //ヒット音
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        //現在のシーンによってBGMを再生
        switch(MySceneManager.Ins.CurrentSceneType){
            case MySceneManager.SceneType.TITLE:
                PlayBGM(titleBGMIndex);
                break;
            
            case MySceneManager.SceneType.GAME:
                PlayBGM(gameBGMIndex);
                break;
            
            default:
                PlayBGM(gameBGMIndex);
                break;
        }
    }

    public void PlayBGM(int i){
        if(!IsValidIndex(_bgmAudioInfo,i))return;
        
        _bgmAudioSource.clip=_bgmAudioInfo[i].audioClip;
        _bgmAudioSource.volume=_bgmAudioInfo[i].volume;
        _bgmAudioSource.Play();
    }

    public void PlayOneShotSE(int i){
        if(!IsValidIndex(_seAudioInfo,i))return;
        
        _seAudioSource.PlayOneShot(_seAudioInfo[i].audioClip,_seAudioInfo[i].volume);
    }
    
    //有効なインデックスかどうか
    bool IsValidIndex(AudioInfo[] array,int i)
        =>array!=null&&i>=0&&i<array.Length;
}