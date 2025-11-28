using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 控制Setting按钮点击显示/隐藏Panel
/// </summary>
public class SettingPanelController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button settingButton;   // Setting按钮
    [SerializeField] private GameObject settingPanel; // Setting面板
    [SerializeField] private Button backButton;       // Panel中的Back按钮（如果为空，会自动查找）
    
    [Header("Options")]
    [SerializeField] private bool pauseGameWhenOpened = false; // 打开面板时是否暂停游戏（Start场景通常不需要暂停）
    
    private void Awake()
    {
        // 初始化时隐藏面板
        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }
        
        // 绑定Setting按钮事件
        if (settingButton != null)
        {
            settingButton.onClick.RemoveAllListeners();
            settingButton.onClick.AddListener(TogglePanel);
        }
        else
        {
            // 如果没有手动指定，尝试自动查找
            settingButton = GetComponent<Button>();
            if (settingButton == null)
            {
                // 尝试查找名为"setting"的对象
                GameObject settingObj = GameObject.Find("setting");
                if (settingObj != null)
                {
                    settingButton = settingObj.GetComponent<Button>();
                }
            }
            
            if (settingButton != null)
            {
                settingButton.onClick.RemoveAllListeners();
                settingButton.onClick.AddListener(TogglePanel);
            }
            else
            {
                Debug.LogWarning("SettingPanelController: 未找到Setting按钮！请在Inspector中设置Setting Button引用。");
            }
        }
        
        // 绑定Back按钮事件
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(HidePanel);
        }
        else
        {
            // 如果没有手动指定，尝试自动查找
            if (settingPanel != null)
            {
                // 在Panel的子对象中查找BackButton
                Transform backButtonTransform = settingPanel.transform.Find("BackButton");
                if (backButtonTransform != null)
                {
                    backButton = backButtonTransform.GetComponent<Button>();
                }
            }
            
            // 如果还是没找到，尝试全局查找
            if (backButton == null)
            {
                GameObject backButtonObj = GameObject.Find("BackButton");
                if (backButtonObj != null)
                {
                    backButton = backButtonObj.GetComponent<Button>();
                }
            }
            
            if (backButton != null)
            {
                backButton.onClick.RemoveAllListeners();
                backButton.onClick.AddListener(HidePanel);
            }
            else
            {
                Debug.LogWarning("SettingPanelController: 未找到Back按钮！请在Inspector中设置Back Button引用，或确保Panel中有名为'BackButton'的子对象。");
            }
        }
    }
    
    /// <summary>
    /// 切换面板显示/隐藏
    /// </summary>
    public void TogglePanel()
    {
        if (settingPanel == null)
        {
            Debug.LogWarning("SettingPanelController: Setting Panel未设置！");
            return;
        }
        
        bool isActive = settingPanel.activeSelf;
        settingPanel.SetActive(!isActive);
        
        // 如果需要，控制游戏暂停
        if (pauseGameWhenOpened)
        {
            if (!isActive)
            {
                // 打开面板时暂停
                Time.timeScale = 0f;
                AudioListener.pause = true;
            }
            else
            {
                // 关闭面板时恢复
                Time.timeScale = 1f;
                AudioListener.pause = false;
            }
        }
    }
    
    /// <summary>
    /// 显示面板
    /// </summary>
    public void ShowPanel()
    {
        if (settingPanel == null)
        {
            Debug.LogWarning("SettingPanelController: Setting Panel未设置！");
            return;
        }
        
        settingPanel.SetActive(true);
        
        if (pauseGameWhenOpened)
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }
    }
    
    /// <summary>
    /// 隐藏面板
    /// </summary>
    public void HidePanel()
    {
        if (settingPanel == null)
        {
            return;
        }
        
        settingPanel.SetActive(false);
        
        if (pauseGameWhenOpened)
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }
    }
    
    private void OnDestroy()
    {
        // 清理事件监听
        if (settingButton != null)
        {
            settingButton.onClick.RemoveAllListeners();
        }
        
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
        }
        
        // 恢复游戏时间（如果被暂停）
        if (pauseGameWhenOpened)
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
        }
    }
}

