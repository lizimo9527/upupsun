using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    
    private void Start()
    {
        // 如果在 game directory 场景中，自动绑定按钮
        // 使用 Start 而不是 Awake，确保所有对象都已初始化
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"SceneLoader Start: 当前场景名称 = '{currentSceneName}'");
        
        if (currentSceneName == "game directory" || currentSceneName.Contains("game directory"))
        {
            Debug.Log("SceneLoader: 检测到 game directory 场景，开始绑定按钮...");
            // 延迟一帧绑定，确保所有UI元素都已创建
            StartCoroutine(DelayedBindButtons());
        }
    }
    
    private System.Collections.IEnumerator DelayedBindButtons()
    {
        // 等待一帧，确保所有对象都已初始化
        yield return null;
        BindGameDirectoryButtons();
    }
    
    /// <summary>
    /// 自动绑定 game directory 场景中的按钮
    /// </summary>
    private void BindGameDirectoryButtons()
    {
        // 检查 EventSystem
        if (UnityEngine.EventSystems.EventSystem.current == null)
        {
            Debug.LogError("GameDirectory: 未找到 EventSystem！按钮点击将无法工作。");
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Debug.Log("GameDirectory: 已创建 EventSystem");
        }

        // 清除所有按钮的旧监听器并重新绑定
        // backButton -> Start 场景
        BindButtonByName("backButton", LoadStartScene, "backButton -> Start");

        // NoButton1 -> SampleScene 场景
        BindButtonByName("NoButton1", LoadLevel1, "NoButton1 -> SampleScene");

        // NoButton2 -> No.2 场景
        BindButtonByName("NoButton 2", LoadLevel2, "NoButton2 -> No.2");

        // NoButton3 -> No.3 场景
        BindButtonByName("NoButton 3", LoadLevel3, "NoButton3 -> No.3");

        // NoButton4 -> No.4 场景
        BindButtonByName("NoButton 4", LoadLevel4, "NoButton4 -> No.4");

        // NoButton5 -> No.5 场景
        BindButtonByName("NoButton 5", LoadLevel5, "NoButton5 -> No.5");

        // NoButton6 -> No.6 场景
        BindButtonByName("NoButton 6", LoadLevel6, "NoButton6 -> No.6");
        
        Debug.Log("GameDirectory: 所有按钮绑定完成！");
    }

    /// <summary>
    /// 通用的按钮绑定方法
    /// </summary>
    private void BindButtonByName(string buttonName, UnityEngine.Events.UnityAction action, string logName)
    {
        GameObject buttonObj = GameObject.Find(buttonName);
        if (buttonObj == null)
        {
            Debug.LogError($"GameDirectory: 未找到按钮 '{buttonName}'！");
            return;
        }
        
        Button button = buttonObj.GetComponent<Button>();
        if (button == null)
        {
            // 尝试在子对象中查找
            button = buttonObj.GetComponentInChildren<Button>();
            if (button == null)
            {
                Debug.LogError($"GameDirectory: 按钮 '{buttonName}' 及其子对象中未找到 Button 组件！");
                return;
            }
        }
        
        // 清除所有运行时监听器
        button.onClick.RemoveAllListeners();
        
        // 禁用所有持久化事件（如果场景中还有残留的）
        int persistentCount = button.onClick.GetPersistentEventCount();
        for (int i = persistentCount - 1; i >= 0; i--)
        {
            button.onClick.SetPersistentListenerState(i, UnityEngine.Events.UnityEventCallState.Off);
        }
        
        // 确保按钮是可交互和启用的
        if (!button.interactable)
        {
            Debug.LogWarning($"GameDirectory: {logName} 的 interactable 为 false，正在启用...");
            button.interactable = true;
        }
        if (!button.enabled)
        {
            Debug.LogWarning($"GameDirectory: {logName} 的 enabled 为 false，正在启用...");
            button.enabled = true;
        }
        
        // 检查按钮的RaycastTarget
        UnityEngine.UI.Image buttonImage = buttonObj.GetComponent<UnityEngine.UI.Image>();
        if (buttonImage != null && !buttonImage.raycastTarget)
        {
            Debug.LogWarning($"GameDirectory: {logName} 的 Image RaycastTarget 为 false，正在启用...");
            buttonImage.raycastTarget = true;
        }
        
        // 检查Canvas和GraphicRaycaster
        Canvas canvas = buttonObj.GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            UnityEngine.UI.GraphicRaycaster raycaster = canvas.GetComponent<UnityEngine.UI.GraphicRaycaster>();
            if (raycaster == null)
            {
                Debug.LogError($"GameDirectory: {logName} 的 Canvas 没有 GraphicRaycaster 组件！");
            }
            else if (!raycaster.enabled)
            {
                Debug.LogWarning($"GameDirectory: {logName} 的 GraphicRaycaster 未启用，正在启用...");
                raycaster.enabled = true;
            }
        }
        
        // 验证按钮最终状态
        Debug.Log($"GameDirectory: {logName} 最终状态 - Interactable: {button.interactable}, Enabled: {button.enabled}, GameObject Active: {buttonObj.activeInHierarchy}");
        
        // 绑定新的监听器
        button.onClick.AddListener(() => {
            Debug.Log($"========== GameDirectory: {logName} 被点击！==========");
            Debug.Log($"GameDirectory: 准备调用 action，action 是否为 null: {action == null}");
            try
            {
                if (action != null)
                {
                    action.Invoke();
                    Debug.Log($"GameDirectory: {logName} 的 action 已成功调用");
                }
                else
                {
                    Debug.LogError($"GameDirectory: {logName} 的 action 为 null！");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"GameDirectory: {logName} 的 action 调用失败: {e.Message}");
                Debug.LogError($"StackTrace: {e.StackTrace}");
            }
        });
        
        Debug.Log($"GameDirectory: {logName} 绑定成功（已清除 {persistentCount} 个持久化事件）");
    }
    
    /// <summary>
    /// 加载game directory场景（从Start场景跳转）
    /// </summary>
    public void LoadGameDirectory()
    {
        Debug.Log("LoadGameDirectory 方法被调用");
        LoadSceneByNameOrIndex(gameDirectorySceneName, gameDirectorySceneIndex, "game directory");
    }
    
    /// <summary>
    /// 加载关卡1（SampleScene）
    /// </summary>
    public void LoadLevel1()
    {
        Debug.Log("========== LoadLevel1 方法被调用 ==========");
        Debug.Log($"LoadLevel1: sampleSceneName='{sampleSceneName}', sampleSceneIndex={sampleSceneIndex}");
        LoadSceneByNameOrIndex(sampleSceneName, sampleSceneIndex, "关卡1 (SampleScene)");
    }

    /// <summary>
    /// 加载关卡2（No.2）
    /// </summary>
    public void LoadLevel2()
    {
        Debug.Log("========== LoadLevel2 方法被调用 ==========");
        Debug.Log($"LoadLevel2: no2SceneName='{no2SceneName}', no2SceneIndex={no2SceneIndex}");
        LoadSceneByNameOrIndex(no2SceneName, no2SceneIndex, "关卡2 (No.2)");
    }
    
    /// <summary>
    /// 加载关卡3（No.3）
    /// </summary>
    public void LoadLevel3()
    {
        Debug.Log("LoadLevel3 方法被调用 - 跳转到关卡3 (No.3)");
        LoadSceneByNameOrIndex(no3SceneName, no3SceneIndex, "关卡3 (No.3)");
    }
    
    /// <summary>
    /// 加载关卡4（No.4）
    /// </summary>
    public void LoadLevel4()
    {
        Debug.Log("LoadLevel4 方法被调用 - 跳转到关卡4 (No.4)");
        LoadSceneByNameOrIndex(no4SceneName, no4SceneIndex, "关卡4 (No.4)");
    }
    
    /// <summary>
    /// 加载关卡5（No.5）
    /// </summary>
    public void LoadLevel5()
    {
        Debug.Log("LoadLevel5 方法被调用 - 跳转到关卡5 (No.5)");
        LoadSceneByNameOrIndex(no5SceneName, no5SceneIndex, "关卡5 (No.5)");
    }
    
    /// <summary>
    /// 加载关卡6（No.6）
    /// </summary>
    public void LoadLevel6()
    {
        Debug.Log("LoadLevel6 方法被调用 - 跳转到关卡6 (No.6)");
        LoadSceneByNameOrIndex(no6SceneName, no6SceneIndex, "关卡6 (No.6)");
    }

    // 保留旧方法以保持向后兼容性
    /// <summary>
    /// 加载SampleScene场景（从game directory场景跳转）- 已废弃，请使用 LoadLevel1
    /// </summary>
    [System.Obsolete("请使用 LoadLevel1() 代替")]
    public void LoadSampleScene()
    {
        LoadLevel1();
    }

    /// <summary>
    /// 加载 No.2 场景（game directory 的 Button2）- 已废弃，请使用 LoadLevel2
    /// </summary>
    [System.Obsolete("请使用 LoadLevel2() 代替")]
    public void LoadNo2Scene()
    {
        LoadLevel2();
    }
    
    /// <summary>
    /// 加载 No.3 场景（game directory 的 Button3）- 已废弃，请使用 LoadLevel3
    /// </summary>
    [System.Obsolete("请使用 LoadLevel3() 代替")]
    public void LoadNo3Scene()
    {
        LoadLevel3();
    }
    
    /// <summary>
    /// 加载 No.4 场景（game directory 的 Button4）- 已废弃，请使用 LoadLevel4
    /// </summary>
    [System.Obsolete("请使用 LoadLevel4() 代替")]
    public void LoadNo4Scene()
    {
        LoadLevel4();
    }
    
    /// <summary>
    /// 加载 No.5 场景（game directory 的 Button5）- 已废弃，请使用 LoadLevel5
    /// </summary>
    [System.Obsolete("请使用 LoadLevel5() 代替")]
    public void LoadNo5Scene()
    {
        LoadLevel5();
    }
    
    /// <summary>
    /// 加载 No.6 场景（game directory 的 Button6）- 已废弃，请使用 LoadLevel6
    /// </summary>
    [System.Obsolete("请使用 LoadLevel6() 代替")]
    public void LoadNo6Scene()
    {
        LoadLevel6();
    }

    /// <summary>
    /// 加载Start场景（从game directory场景返回）
    /// </summary>
    public void LoadStartScene()
    {
        Debug.Log("========== LoadStartScene 方法被调用 ==========");
        Debug.Log($"LoadStartScene: startSceneName='{startSceneName}', startSceneIndex={startSceneIndex}");
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
            try
            {
                Debug.Log($"正在调用 SceneManager.LoadScene({sceneIndex})...");
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
                Debug.Log($"场景加载命令已执行，场景索引 {sceneIndex} 应该正在加载...");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"场景加载失败: {e.Message}");
                Debug.LogError($"StackTrace: {e.StackTrace}");
            }
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

