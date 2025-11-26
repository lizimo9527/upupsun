using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace UpupSun.UI
{
    public class GameUI : MonoBehaviour
    {
        [Header("UI元素")]
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI targetsText;
        public TextMeshProUGUI timerText;
        public Button restartButton;
        public Button nextLevelButton;
        
        [Header("菜单面板")]
        public GameObject gameMenu;
        public GameObject levelCompletePanel;
        public GameObject pauseMenu;
        
        [Header("进度条")]
        public Slider progressSlider;
        public Image progressFill;
        
        [Header("提示文本")]
        public TextMeshProUGUI hintText;
        public string[] hints = {
            "按住鼠标画线",
            "按空格键让光线跟随",
            "引导光线到达所有目标",
            "R键重置光线位置"
        };
        
        private TargetManager targetManager;
        private GameManager gameManager;
        private int currentHintIndex = 0;
        private float hintTimer = 0f;
        private float hintInterval = 5f;
        
        private void Start()
        {
            InitializeUI();
            SetupEventListeners();
            StartHintRotation();
        }
        
        private void InitializeUI()
        {
            targetManager = TargetManager.Instance;
            gameManager = GameManager.Instance;
            
            // 初始化UI状态
            UpdateLevelDisplay();
            UpdateTargetsDisplay();
            HideAllPanels();
            
            // 设置进度条
            if (progressSlider != null)
            {
                progressSlider.minValue = 0f;
                progressSlider.maxValue = 1f;
                progressSlider.value = 0f;
            }
        }
        
        private void SetupEventListeners()
        {
            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartLevel);
            }
            
            if (nextLevelButton != null)
            {
                nextLevelButton.onClick.AddListener(NextLevel);
            }
        }
        
        private void Update()
        {
            UpdateProgressDisplay();
            UpdateTimer();
            HandleInput();
        }
        
        private void UpdateProgressDisplay()
        {
            if (targetManager != null)
            {
                float completion = targetManager.GetCompletionPercentage();
                
                if (progressSlider != null)
                {
                    progressSlider.value = completion;
                }
                
                if (progressFill != null)
                {
                    // 根据进度改变颜色
                    Color fillColor = Color.Lerp(Color.red, Color.green, completion);
                    progressFill.color = fillColor;
                }
                
                if (targetsText != null)
                {
                    int completed = targetManager.GetCompletedTargetsCount();
                    int total = targetManager.GetTotalTargetsCount();
                    targetsText.text = $"目标: {completed}/{total}";
                }
            }
        }
        
        private void UpdateLevelDisplay()
        {
            if (levelText != null && gameManager != null)
            {
                levelText.text = $"关卡: {gameManager.currentLevel}";
            }
        }
        
        private void UpdateTimer()
        {
            // 这里可以添加计时器逻辑
            // if (timerText != null)
            // {
            //     float elapsedTime = Time.time - levelStartTime;
            //     timerText.text = FormatTime(elapsedTime);
            // }
        }
        
        private string FormatTime(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            return $"{minutes:00}:{seconds:00}";
        }
        
        private void HandleInput()
        {
            // ESC键暂停
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePauseMenu();
            }
        }
        
        private void StartHintRotation()
        {
            if (hintText != null && hints.Length > 0)
            {
                StartCoroutine(RotateHints());
            }
        }
        
        private IEnumerator RotateHints()
        {
            while (true)
            {
                hintText.text = hints[currentHintIndex];
                yield return new WaitForSeconds(hintInterval);
                
                currentHintIndex = (currentHintIndex + 1) % hints.Length;
            }
        }
        
        public void ShowLevelCompletePanel()
        {
            if (levelCompletePanel != null)
            {
                levelCompletePanel.SetActive(true);
                Time.timeScale = 0f; // 暂停游戏
            }
        }
        
        public void HideLevelCompletePanel()
        {
            if (levelCompletePanel != null)
            {
                levelCompletePanel.SetActive(false);
                Time.timeScale = 1f; // 恢复游戏
            }
        }
        
        public void ShowPauseMenu()
        {
            if (pauseMenu != null)
            {
                pauseMenu.SetActive(true);
                Time.timeScale = 0f;
            }
        }
        
        public void HidePauseMenu()
        {
            if (pauseMenu != null)
            {
                pauseMenu.SetActive(false);
                Time.timeScale = 1f;
            }
        }
        
        public void TogglePauseMenu()
        {
            if (pauseMenu != null && pauseMenu.activeSelf)
            {
                HidePauseMenu();
            }
            else
            {
                ShowPauseMenu();
            }
        }
        
        public void RestartLevel()
        {
            HideAllPanels();
            Time.timeScale = 1f;
            
            if (targetManager != null)
            {
                targetManager.ResetLevel();
            }
            
            // 重置UI
            UpdateProgressDisplay();
        }
        
        public void NextLevel()
        {
            HideAllPanels();
            Time.timeScale = 1f;
            
            if (gameManager != null)
            {
                gameManager.StartLevel(gameManager.currentLevel);
            }
            
            UpdateLevelDisplay();
        }
        
        private void HideAllPanels()
        {
            if (levelCompletePanel != null)
            {
                levelCompletePanel.SetActive(false);
            }
            
            if (pauseMenu != null)
            {
                pauseMenu.SetActive(false);
            }
        }
        
        public void ShowTemporaryMessage(string message, float duration = 2f)
        {
            if (hintText != null)
            {
                StartCoroutine(ShowMessageCoroutine(message, duration));
            }
        }
        
        private IEnumerator ShowMessageCoroutine(string message, float duration)
        {
            string originalText = hintText.text;
            hintText.text = message;
            
            yield return new WaitForSeconds(duration);
            
            hintText.text = originalText;
        }
        
        public void SetHintInterval(float interval)
        {
            hintInterval = Mathf.Max(1f, interval);
        }
        
        public void UpdateUIElements()
        {
            UpdateLevelDisplay();
            UpdateProgressDisplay();
        }
        
        private void OnDestroy()
        {
            // 清理事件监听
            if (restartButton != null)
            {
                restartButton.onClick.RemoveAllListeners();
            }
            
            if (nextLevelButton != null)
            {
                nextLevelButton.onClick.RemoveAllListeners();
            }
        }
    }
}