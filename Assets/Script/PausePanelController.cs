using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 控制 PausePanel：主视图按钮打开，面板按钮执行跳转/重开
/// </summary>
public class PausePanelController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button triggerButton;   // 主视图按钮
    [SerializeField] private GameObject pausePanel;  // 需要弹出的面板
    [SerializeField] private Button homeButton;      // 面板中的 Home
    [SerializeField] private Button restartButton;   // 面板中的 Restart
    [SerializeField] private Button closeButton;     // 面板中的关闭按钮（可选）

    [Header("Options")]
    [SerializeField] private string homeSceneName = "game directory";
    [SerializeField] private bool pauseGameWhenOpened = true;

    private void Awake()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        BindButton(triggerButton, ShowPanel, "Trigger Button");
        BindButton(homeButton, GoHome, "Home Button");
        BindButton(restartButton, RestartLevel, "Restart Button");
        if (closeButton != null)
        {
            BindButton(closeButton, HidePanel, "Close Button");
        }
    }

    private void OnDestroy()
    {
        ResumeGameTime();
    }

    private void ShowPanel()
    {
        if (pausePanel == null)
        {
            Debug.LogWarning("PausePanel 未设置，无法显示。");
            return;
        }

        if (pauseGameWhenOpened)
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
        }

        pausePanel.SetActive(true);
    }

    public void HidePanel()
    {
        if (pausePanel == null)
        {
            return;
        }

        pausePanel.SetActive(false);
        ResumeGameTime();
    }

    private void GoHome()
    {
        ResumeGameTime();

        if (string.IsNullOrEmpty(homeSceneName))
        {
            Debug.LogWarning("homeSceneName 为空，无法跳转。");
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(homeSceneName))
        {
            Debug.LogWarning($"场景 {homeSceneName} 未加入 Build Settings 或名称不匹配。");
            return;
        }

        SceneManager.LoadScene(homeSceneName);
    }

    private void RestartLevel()
    {
        ResumeGameTime();
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    private void ResumeGameTime()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }

    private void BindButton(Button button, UnityEngine.Events.UnityAction action, string name)
    {
        if (button == null)
        {
            Debug.LogWarning($"{name} 未绑定。");
            return;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
    }
}

