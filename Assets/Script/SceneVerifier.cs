using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 场景验证脚本 - 在场景加载后验证场景是否正确
/// 将此脚本添加到场景中，设置期望的场景索引和名称
/// </summary>
public class SceneVerifier : MonoBehaviour
{
    [Header("场景验证设置")]
    [Tooltip("期望的场景索引（在 Build Settings 中的索引）")]
    public int expectedSceneIndex = -1;
    
    [Tooltip("期望的场景名称（可选，用于额外验证）")]
    public string expectedSceneName = "";
    
    [Tooltip("是否在验证失败时输出详细错误信息")]
    public bool logDetailedErrors = true;
    
    private void Awake()
    {
        VerifyScene();
    }
    
    private void VerifyScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        int currentBuildIndex = currentScene.buildIndex;
        string currentSceneName = currentScene.name;
        
        Debug.Log($"[SceneVerifier] 场景验证开始");
        Debug.Log($"[SceneVerifier] 当前场景: 索引={currentBuildIndex}, 名称='{currentSceneName}'");
        
        // 如果设置了期望的场景索引，进行验证
        if (expectedSceneIndex >= 0)
        {
            Debug.Log($"[SceneVerifier] 期望场景: 索引={expectedSceneIndex}, 名称='{expectedSceneName}'");
            
            if (currentBuildIndex == expectedSceneIndex)
            {
                Debug.Log($"[SceneVerifier] ✓ 场景验证成功！当前场景索引 {currentBuildIndex} 与期望的索引 {expectedSceneIndex} 匹配。");
                
                // 如果还设置了场景名称，也进行验证
                if (!string.IsNullOrEmpty(expectedSceneName))
                {
                    if (currentSceneName.Equals(expectedSceneName, System.StringComparison.OrdinalIgnoreCase))
                    {
                        Debug.Log($"[SceneVerifier] ✓ 场景名称也匹配！");
                    }
                    else
                    {
                        Debug.LogWarning($"[SceneVerifier] ⚠ 场景名称不匹配！当前名称 '{currentSceneName}' 与期望名称 '{expectedSceneName}' 不一致。");
                    }
                }
            }
            else
            {
                Debug.LogError($"[SceneVerifier] ✗ 场景验证失败！");
                Debug.LogError($"[SceneVerifier] 当前场景索引 {currentBuildIndex} 与期望的索引 {expectedSceneIndex} 不匹配！");
                Debug.LogError($"[SceneVerifier] 当前场景名称: '{currentSceneName}'");
                Debug.LogError($"[SceneVerifier] 期望场景名称: '{expectedSceneName}'");
                
                if (logDetailedErrors)
                {
                    Debug.LogError($"[SceneVerifier] 这可能是因为：");
                    Debug.LogError($"[SceneVerifier] 1. Build Settings 中的场景顺序不正确");
                    Debug.LogError($"[SceneVerifier] 2. 场景加载失败，Unity 默认加载了索引 0 的场景");
                    Debug.LogError($"[SceneVerifier] 3. 其他脚本在场景加载后立即切换了场景");
                    
                    // 输出所有场景信息
                    int sceneCount = SceneManager.sceneCountInBuildSettings;
                    Debug.LogError($"[SceneVerifier] Build Settings 中共有 {sceneCount} 个场景：");
                    for (int i = 0; i < sceneCount; i++)
                    {
                        string path = SceneUtility.GetScenePathByBuildIndex(i);
                        string name = System.IO.Path.GetFileNameWithoutExtension(path);
                        string marker = (i == currentBuildIndex) ? " ← 当前场景" : "";
                        Debug.LogError($"[SceneVerifier]   索引 {i}: {name} ({path}){marker}");
                    }
                }
            }
        }
        else
        {
            Debug.Log($"[SceneVerifier] 未设置期望的场景索引，跳过验证。");
        }
    }
}

