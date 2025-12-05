using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Linq;
using UnityEngine.UI;

/// <summary>
/// 编辑器工具：为所有关卡场景配置 gaginPanel
/// 使用方法：Window -> Gagin Panel Setup Tool
/// </summary>
public class GaginPanelSetupTool : EditorWindow
{
    private string[] levelSceneNames = { "SampleScene", "No.2", "No.3", "No.4", "No.5", "No.6" };
    private Vector2 scrollPosition;
    
    [MenuItem("Tools/Gagin Panel Setup Tool")]
    public static void ShowWindow()
    {
        GetWindow<GaginPanelSetupTool>("Gagin Panel Setup");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("关卡场景 gaginPanel 配置工具", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "此工具用于为所有关卡场景自动配置 gaginPanel。\n\n" +
            "功能：\n" +
            "1. 检查所有关卡是否已配置 gaginPanel\n" +
            "2. 自动从 SampleScene 复制 gaginPanel 到未配置的关卡\n" +
            "3. 自动配置 GameManager 的引用",
            MessageType.Info
        );
        
        GUILayout.Space(10);
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        foreach (string sceneName in levelSceneNames)
        {
            DrawSceneStatus(sceneName);
        }
        
        EditorGUILayout.EndScrollView();
        
        GUILayout.Space(10);
        
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("检查所有关卡", GUILayout.Height(30)))
        {
            CheckAllLevels();
        }
        
        GUI.enabled = EditorSceneManager.GetActiveScene().name != "SampleScene";
        if (GUILayout.Button("自动配置所有关卡", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("确认", 
                "此操作将从 SampleScene 复制 gaginPanel 到所有未配置的关卡场景。\n\n" +
                "请确保已保存当前场景的修改。\n\n" +
                "是否继续？", "继续", "取消"))
            {
                AutoSetupAllLevels();
            }
        }
        GUI.enabled = true;
        
        EditorGUILayout.EndHorizontal();
    }
    
    private void DrawSceneStatus(string sceneName)
    {
        EditorGUILayout.BeginVertical("box");
        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(sceneName, EditorStyles.boldLabel);
        
        bool isConfigured = IsGaginPanelConfigured(sceneName);
        string status = isConfigured ? "✓ 已配置" : "✗ 未配置";
        Color originalColor = GUI.color;
        GUI.color = isConfigured ? Color.green : Color.red;
        GUILayout.Label(status, EditorStyles.miniLabel);
        GUI.color = originalColor;
        
        if (GUILayout.Button("打开场景", GUILayout.Width(80)))
        {
            OpenScene(sceneName);
        }
        
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.EndVertical();
        GUILayout.Space(5);
    }
    
    private bool IsGaginPanelConfigured(string sceneName)
    {
        string scenePath = $"Assets/Scenes/{sceneName}.unity";
        if (!System.IO.File.Exists(scenePath))
        {
            return false;
        }
        
        // 打开场景（不保存）
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        
        GameManager[] gameManagers = FindObjectsOfType<GameManager>();
        bool configured = false;
        
        foreach (GameManager gm in gameManagers)
        {
            if (gm.gaginPanel != null)
            {
                configured = true;
                break;
            }
        }
        
        // 关闭场景（不保存）
        EditorSceneManager.CloseScene(scene, false);
        
        return configured;
    }
    
    private void OpenScene(string sceneName)
    {
        string scenePath = $"Assets/Scenes/{sceneName}.unity";
        if (System.IO.File.Exists(scenePath))
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(scenePath);
            }
        }
        else
        {
            EditorUtility.DisplayDialog("错误", $"场景文件不存在：{scenePath}", "确定");
        }
    }
    
    private void CheckAllLevels()
    {
        int configuredCount = 0;
        int totalCount = levelSceneNames.Length;
        
        foreach (string sceneName in levelSceneNames)
        {
            if (IsGaginPanelConfigured(sceneName))
            {
                configuredCount++;
            }
        }
        
        string message = $"检查完成！\n\n已配置：{configuredCount}/{totalCount} 个关卡";
        EditorUtility.DisplayDialog("检查结果", message, "确定");
    }
    
    private void AutoSetupAllLevels()
    {
        // 首先打开 SampleScene 获取 gaginPanel 的引用
        string sampleScenePath = "Assets/Scenes/SampleScene.unity";
        if (!System.IO.File.Exists(sampleScenePath))
        {
            EditorUtility.DisplayDialog("错误", "找不到 SampleScene 场景文件", "确定");
            return;
        }
        
        // 保存当前场景
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }
        
        // 打开 SampleScene
        Scene sampleScene = EditorSceneManager.OpenScene(sampleScenePath, OpenSceneMode.Single);
        
        // 查找 gaginPanel
        GameObject gaginPanelSource = GameObject.Find("gaginPanel");
        if (gaginPanelSource == null)
        {
            EditorUtility.DisplayDialog("错误", "在 SampleScene 中找不到 gaginPanel GameObject", "确定");
            return;
        }
        
        // 查找 Canvas（gaginPanel 应该在 Canvas 下）
        Canvas canvas = gaginPanelSource.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("错误", "在 SampleScene 中找不到 gaginPanel 的父 Canvas", "确定");
            return;
        }
        
        // 查找按钮引用（从 SampleScene 的 GameManager 获取）
        GameManager sampleGameManager = Object.FindObjectOfType<GameManager>();
        Button restartButton = null;
        Button homeButton = null;
        
        if (sampleGameManager != null)
        {
            restartButton = sampleGameManager.gaginRestartButton;
            homeButton = sampleGameManager.gaginHomeButton;
        }
        
        // 如果从 GameManager 找不到，尝试从 gaginPanel 子对象查找
        if (restartButton == null || homeButton == null)
        {
            Button[] buttons = gaginPanelSource.GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                if (btn.name.Contains("again") || btn.name.Contains("Again") || btn.name.Contains("restart") || btn.name.Contains("Restart"))
                {
                    if (restartButton == null) restartButton = btn;
                }
                if (btn.name.Contains("home") || btn.name.Contains("Home") || btn.name.Contains("again") || btn.name.Contains("Again"))
                {
                    if (homeButton == null) homeButton = btn;
                }
            }
            
            // 如果还是找不到，使用第一个按钮作为 homeButton
            if (buttons.Length > 0)
            {
                if (homeButton == null) homeButton = buttons[0];
                if (restartButton == null && buttons.Length > 1) restartButton = buttons[1];
            }
        }
        
        int successCount = 0;
        int skipCount = 0;
        
        // 遍历所有关卡场景
        foreach (string sceneName in levelSceneNames)
        {
            if (sceneName == "SampleScene")
            {
                continue; // 跳过 SampleScene 本身
            }
            
            string scenePath = $"Assets/Scenes/{sceneName}.unity";
            if (!System.IO.File.Exists(scenePath))
            {
                Debug.LogWarning($"场景文件不存在：{scenePath}");
                continue;
            }
            
            // 检查是否已配置
            if (IsGaginPanelConfigured(sceneName))
            {
                skipCount++;
                Debug.Log($"{sceneName} 已配置，跳过");
                continue;
            }
            
            // 打开目标场景
            Scene targetScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            
            // 查找或创建 Canvas
            Canvas targetCanvas = FindObjectOfType<Canvas>();
            if (targetCanvas == null)
            {
                // 创建 Canvas
                GameObject canvasGO = new GameObject("Canvas");
                targetCanvas = canvasGO.AddComponent<Canvas>();
                targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();
            }
            
            // 复制 gaginPanel
            GameObject gaginPanelCopy = Instantiate(gaginPanelSource, targetCanvas.transform);
            gaginPanelCopy.name = "gaginPanel";
            gaginPanelCopy.SetActive(false);
            
            // 查找 GameManager 并配置引用
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.gaginPanel = gaginPanelCopy;
                
                // 配置按钮引用
                if (restartButton != null)
                {
                    Button restartBtnCopy = gaginPanelCopy.transform.Find(restartButton.name)?.GetComponent<Button>();
                    if (restartBtnCopy == null)
                    {
                        Button[] allButtons = gaginPanelCopy.GetComponentsInChildren<Button>();
                        if (allButtons.Length > 0)
                        {
                            restartBtnCopy = allButtons[0];
                        }
                    }
                    gameManager.gaginRestartButton = restartBtnCopy;
                }
                
                if (homeButton != null)
                {
                    Button homeBtnCopy = gaginPanelCopy.transform.Find(homeButton.name)?.GetComponent<Button>();
                    if (homeBtnCopy == null)
                    {
                        Button[] allButtons = gaginPanelCopy.GetComponentsInChildren<Button>();
                        if (allButtons.Length > 1)
                        {
                            homeBtnCopy = allButtons[1];
                        }
                        else if (allButtons.Length > 0)
                        {
                            homeBtnCopy = allButtons[0];
                        }
                    }
                    gameManager.gaginHomeButton = homeBtnCopy;
                }
                
                // 确保场景名称正确
                gameManager.gameDirectorySceneName = "game directory";
                
                EditorUtility.SetDirty(gameManager);
                successCount++;
                Debug.Log($"已为 {sceneName} 配置 gaginPanel");
            }
            else
            {
                Debug.LogWarning($"在 {sceneName} 中找不到 GameManager，无法自动配置");
                DestroyImmediate(gaginPanelCopy);
            }
            
            // 保存场景
            EditorSceneManager.SaveScene(targetScene);
        }
        
        // 重新打开 SampleScene（不保存）
        EditorSceneManager.OpenScene(sampleScenePath, OpenSceneMode.Single);
        
        string resultMessage = $"配置完成！\n\n" +
                              $"成功配置：{successCount} 个关卡\n" +
                              $"已跳过：{skipCount} 个关卡（已配置）";
        EditorUtility.DisplayDialog("完成", resultMessage, "确定");
    }
}

