using UnityEngine;

public class Managers : MonoBehaviour
{
    private static Managers _instance;
    static Managers Instance
    {
        get
        {
            Init();
            return _instance;
        }
    }


    //MonoBehaviour
    private GameManager _game;
    private SoundManager _sound;
    private InputManager _input = new InputManager();
    private LanguageManager _language = new LanguageManager();
    private PoolManager _pool = new PoolManager();
    private ResourceManager _resource = new ResourceManager();
    private RestApiManager _restApi = new RestApiManager();
    private SaveManager _save = new SaveManager();
    private SceneManagerEx _scene = new SceneManagerEx();
    private SettingManager _setting = new SettingManager();
    private UIManager _ui = new UIManager();

    public static GameManager Game => Instance._game;
    public static SoundManager Sound => Instance._sound;
    public static InputManager Input => Instance._input;
    public static LanguageManager Language => Instance._language;
    public static PoolManager Pool => Instance._pool;
    public static ResourceManager Resource => Instance._resource;
    public static RestApiManager RestApi => Instance._restApi;
    public static SaveManager Save => Instance._save;
    public static SceneManagerEx Scene => Instance._scene;
    public static SettingManager Setting => Instance._setting;
    public static UIManager UI => Instance._ui;
    
    void Update()
    {
        _input.OnUpdate();
    }
    
    //매니저들의 초기화 시발점
    private static void Init()
    {
        if (_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
                go = new GameObject { name = "@Managers" };

            DontDestroyOnLoad(go);
            _instance = go.GetOrAddComponent<Managers>();
            _instance._language.Init();
            _instance._pool.Init();
            _instance._setting.Init();
            _instance._ui.Init();
            
            //GameManager
            if (_instance._game == null)
                _instance._game = go.GetOrAddComponent<GameManager>();
            
            //SoundManager
            if (_instance._sound == null)
                _instance._sound = go.GetOrAddComponent<SoundManager>();
            _instance._sound.Init();
        }
    }
    
    public static void Clear()
    {
        Input.Clear();
        Scene.Clear();
        UI.Clear();
        Pool.Clear();
    }
}
