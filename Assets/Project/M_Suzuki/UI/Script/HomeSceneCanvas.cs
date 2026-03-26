using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeSceneCanvas : MonoBehaviour
{
    const string DEAD_SCENE = "Hangar";
    const string NOT_DEAD_SCENE = "Title";

    public static HomeSceneCanvas Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == NOT_DEAD_SCENE)//タイトルシーンでドントデストロイに登録
        {
            DontDestroyOnLoad(gameObject);
        }

            
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().name == DEAD_SCENE)//ホームシーンではドントデストロイを削除
        {
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        }
        else if(SceneManager.GetActiveScene().name != NOT_DEAD_SCENE)//タイトルでもホームでもなければオブジェクトを即破壊
        {
            Destroy(gameObject);
        }
    }

    public void ChangeScene(string sceneName)
    {
        GameManager.Instance.ChangeScene(sceneName);
    }
}
