using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 手动为所有关卡添加 gaginPanel 的脚本
/// 使用方法：在 Unity Editor 中，菜单 Tools -> Add Gagin Panel To All Levels
/// </summary>
public class AddGaginPanelToAllLevels : EditorWindow
{
    [MenuItem("Tools/Add Gagin Panel To All Levels")]
    public static void AddGaginPanel()
    {
        string[] levelScenes = { "No.2", "No.3", "No.4", "No.5", "No.6" };
        
        // 保存当前场景
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            return;
        }
        
        // 打开 SampleScene 获取 gaginPanel
        string sampleScenePath = "Assets/Scenes/SampleScene.unity";
        Scene sampleScene = EditorSceneManager.OpenScene(sampleScenePath, OpenSceneMode.Single);
        
        // 查找 gaginPanel
        GameObject gaginPanelSource = GameObject.Find("gaginPanel");
        if (gaginPanelSource == null)
        {
            EditorUtility.DisplayDialog("错误", "在 SampleScene 中找不到 gaginPanel", "确定");
            return;
        }
        
        // 查找 Canvas
        Canvas canvas = gaginPanelSource.GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("错误", "在 SampleScene 中找不到 Canvas", "确定");
            return;
        }
        
        // 获取按钮引用
        GameManager sampleGM = Object.FindObjectOfType<GameManager>();
        Button restartBtn = sampleGM != null ? sampleGM.gaginRestartButton : null;
        Button homeBtn = sampleGM != null ? sampleGM.gaginHomeButton : null;
        
        int successCount = 0;
        
        // 为每个关卡添加 gaginPanel
        foreach (string sceneName in levelScenes)
        {
            string scenePath = $"Assets/Scenes/{sceneName}.unity";
            if (!System.IO.File.Exists(scenePath))
            {
                Debug.LogWarning($"场景不存在: {scenePath}");
                continue;
            }
            
            // 打开目标场景
            Scene targetScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            
            // 检查是否已存在
            GameObject existingPanel = GameObject.Find("gaginPanel");
            if (existingPanel != null)
            {
                Debug.Log($"{sceneName} 已存在 gaginPanel，跳过");
                continue;
            }
            
            // 查找或创建 Canvas
            Canvas targetCanvas = Object.FindObjectOfType<Canvas>();
            if (targetCanvas == null)
            {
                GameObject canvasGO = new GameObject("Canvas");
                targetCanvas = canvasGO.AddComponent<Canvas>();
                targetCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<CanvasScaler>();
                canvasGO.AddComponent<GraphicRaycaster>();
            }
            
            // 复制 gaginPanel
            GameObject gaginPanelCopy = Object.Instantiate(gaginPanelSource, targetCanvas.transform);
            gaginPanelCopy.name = "gaginPanel";
            gaginPanelCopy.SetActive(false);
            
            // 更新按钮引用（如果需要）
            if (restartBtn != null)
            {
                Button[] buttons = gaginPanelCopy.GetComponentsInChildren<Button>(true);
                foreach (Button btn in buttons)
                {
                    if (btn.name == "againButton" || btn.name.Contains("again"))
                    {
                        // 这个按钮应该绑定到 GameManager
                    }
                }
            }
            
            // 查找 GameManager 并配置引用
            GameManager gm = Object.FindObjectOfType<GameManager>();
            if (gm != null)
            {
                gm.gaginPanel = gaginPanelCopy;
                
                // 查找按钮
                Button[] allButtons = gaginPanelCopy.GetComponentsInChildren<Button>(true);
                if (allButtons.Length > 0)
                {
                    // 第一个按钮作为 restartButton
                    gm.gaginRestartButton = allButtons[0];
                    
                    // 查找 againButton 作为 homeButton
                    foreach (Button btn in allButtons)
                    {
                        if (btn.name == "againButton" || btn.name.Contains("again"))
                        {
                            gm.gaginHomeButton = btn;
                            break;
                        }
                    }
                    
                    // 如果没有找到 againButton，使用第一个按钮
                    if (gm.gaginHomeButton == null && allButtons.Length > 0)
                    {
                        gm.gaginHomeButton = allButtons[0];
                    }
                }
                
                gm.gameDirectorySceneName = "game directory";
                EditorUtility.SetDirty(gm);
                
                successCount++;
                Debug.Log($"已为 {sceneName} 添加 gaginPanel");
            }
            else
            {
                Debug.LogWarning($"在 {sceneName} 中找不到 GameManager");
            }
            
            // 保存场景
            EditorSceneManager.SaveScene(targetScene);
        }
        
        // 重新打开 SampleScene
        EditorSceneManager.OpenScene(sampleScenePath, OpenSceneMode.Single);
        
        EditorUtility.DisplayDialog("完成", 
            $"已为 {successCount} 个关卡添加 gaginPanel", "确定");
    }
}

