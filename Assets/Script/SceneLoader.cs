using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("场景设置")]
    [SerializeField] private string gameDirectorySceneName = "game directory";
    [SerializeField] private string sampleSceneName = "SampleScene";
    [SerializeField] private string startSceneName = "Start";
    [SerializeField] private string no2SceneName = "No.2";
    [SerializeField] private string no3SceneName = "No.3";
    [SerializeField] private string no4SceneName = "No.4";
    [SerializeField] private string no5SceneName = "No.5";
    [SerializeField] private string no6SceneName = "No.6";
    [SerializeField] private int gameDirectorySceneIndex = 1; // game directory 在 Build Settings 中的索引
    [SerializeField] private int sampleSceneIndex = 2; // SampleScene 在 Build Settings 中的索引
    [SerializeField] private int startSceneIndex = 0; // Start 场景在 Build Settings 中的索引
    [SerializeField] private int no2SceneIndex = 3; // No.2 场景在 Build Settings 中的索引
    [SerializeField] private int no3SceneIndex = 4; // No.3 场景在 Build Settings 中的索引
    [SerializeField] private int no4SceneIndex = 5; // No.4 场景在 Build Settings 中的索引
    [SerializeField] private int no5SceneIndex = 6; // No.5 场景在 Build Settings 中的索引
    [SerializeField] private int no6SceneIndex = 7; // No.6 场景在 Build Settings 中的索引
    
    /// <summary>
    /// 加载game directory场景（从Start场景跳转）
    /// </summary>
    public void LoadGameDirectory()
    {
        Debug.Log("LoadGameDirectory 方法被调用");
        LoadSceneByNameOrIndex(gameDirectorySceneName, gameDirectorySceneIndex, "game directory");
    }
    
    /// <summary>
    /// 加载SampleScene场景（从game directory场景跳转）
    /// </summary>
    public void LoadSampleScene()
    {
        Debug.Log("LoadSampleScene 方法被调用");
        LoadSceneByNameOrIndex(sampleSceneName, sampleSceneIndex, "SampleScene");
    }

    /// <summary>
    /// 加载 No.2 场景（game directory 的 Button2）
    /// </summary>
    public void LoadNo2Scene()
    {
        Debug.Log("LoadNo2Scene 方法被调用");
        LoadSceneByNameOrIndex(no2SceneName, no2SceneIndex, "No.2");
    }
    
    /// <summary>
    /// 加载 No.3 场景（game directory 的 Button3）
    /// </summary>
    public void LoadNo3Scene()
    {
        Debug.Log("LoadNo3Scene 方法被调用");
        LoadSceneByNameOrIndex(no3SceneName, no3SceneIndex, "No.3");
    }
    
    /// <summary>
    /// 加载 No.4 场景（game directory 的 Button4）
    /// </summary>
    public void LoadNo4Scene()
    {
        Debug.Log("LoadNo4Scene 方法被调用");
        LoadSceneByNameOrIndex(no4SceneName, no4SceneIndex, "No.4");
    }
    
    /// <summary>
    /// 加载 No.5 场景（game directory 的 Button5）
    /// </summary>
    public void LoadNo5Scene()
    {
        Debug.Log("LoadNo5Scene 方法被调用");
        LoadSceneByNameOrIndex(no5SceneName, no5SceneIndex, "No.5");
    }
    
    /// <summary>
    /// 加载 No.6 场景（game directory 的 Button6）
    /// </summary>
    public void LoadNo6Scene()
    {
        Debug.Log("LoadNo6Scene 方法被调用");
        LoadSceneByNameOrIndex(no6SceneName, no6SceneIndex, "No.6");
    }

    /// <summary>
    /// 加载Start场景（从game directory场景返回）
    /// </summary>
    public void LoadStartScene()
    {
        Debug.Log("LoadStartScene 方法被调用");
        LoadSceneByNameOrIndex(startSceneName, startSceneIndex, "Start");
    }
    
    /// <summary>
    /// 通用的场景加载方法
    /// </summary>
    /// <param name="sceneName">场景名称</param>
    /// <param name="sceneIndex">场景索引（备用）</param>
    /// <param name="displayName">显示名称（用于日志）</param>
    private void LoadSceneByNameOrIndex(string sceneName, int sceneIndex, string displayName)
    {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        Debug.Log($"Build Settings 中共有 {sceneCount} 个场景");
        
        // 直接使用场景索引加载，这是最可靠的方法
        if (sceneIndex >= 0 && sceneIndex < sceneCount)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(sceneIndex);
            string actualSceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            
            Debug.Log($"场景索引 {sceneIndex} 对应的路径: {scenePath}");
            Debug.Log($"提取的场景名称: '{actualSceneName}'");
            Debug.Log($"期望加载的场景: {displayName}");
            
            // 验证场景名称是否匹配（不区分大小写）
            bool nameMatches = actualSceneName.Equals(sceneName, System.StringComparison.OrdinalIgnoreCase) ||
                              actualSceneName.Equals(displayName, System.StringComparison.OrdinalIgnoreCase);
            
            if (nameMatches)
            {
                Debug.Log($"场景名称匹配，使用场景索引 {sceneIndex} 加载场景 '{actualSceneName}'");
            }
            else
            {
                Debug.LogWarning($"场景索引 {sceneIndex} 对应的场景名称 '{actualSceneName}' 与期望的 '{displayName}' 不匹配！");
                Debug.LogWarning("但将继续使用该索引加载场景...");
            }
            
            // 直接使用场景索引加载，避免场景名称中的空格等问题
            Debug.Log($"正在使用场景索引 {sceneIndex} 加载场景...");
            
            // 在加载前记录期望的场景信息
            Debug.Log($"准备加载场景: 索引={sceneIndex}, 名称='{actualSceneName}', 显示名称={displayName}");
            
            // 使用场景索引加载
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
            
            // 注意：由于场景切换会销毁当前 GameObject，验证需要在目标场景中进行
            // 如果场景加载失败，Unity 可能会默认加载索引 0 的场景
            // 建议在目标场景中添加 SceneVerifier 脚本进行验证
            Debug.Log($"场景加载命令已执行，请检查是否成功加载到场景索引 {sceneIndex}");
            return;
        }
        
        // 如果索引无效，尝试使用场景名称
        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning($"场景索引 {sceneIndex} 无效，尝试使用场景名称 '{sceneName}' 加载 {displayName}...");
            if (Application.CanStreamedLevelBeLoaded(sceneName))
            {
                Debug.Log($"场景 '{sceneName}' 可以加载，正在加载...");
                SceneManager.LoadScene(sceneName);
                return;
            }
        }
        
        // 如果都失败了，输出错误信息
        Debug.LogError($"无法加载场景 {displayName}！场景索引 {sceneIndex} 无效，场景名称 '{sceneName}' 也无法加载。");
        Debug.LogError($"请确保在 File > Build Settings 中添加了 {displayName} 场景！");
        Debug.LogError("当前 Build Settings 中的场景列表：");
        for (int i = 0; i < sceneCount; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            Debug.LogError($"  索引 {i}: {name} ({path})");
        }
    }
    
}

