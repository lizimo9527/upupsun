using UnityEngine;

namespace UpupSun.Game.Controllers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        
        [Header("游戏状态")]
        public bool isPlaying = false;
        public int currentLevel = 1;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            InitializeGame();
        }
        
        private void InitializeGame()
        {
            isPlaying = true;
            Debug.Log("画线引光游戏初始化完成");
        }
        
        public void StartLevel(int level)
        {
            currentLevel = level;
            isPlaying = true;
        }
        
        public void CompleteLevel()
        {
            Debug.Log($"关卡 {currentLevel} 完成！");
            currentLevel++;
        }
        
        public void RestartLevel()
        {
            Debug.Log($"重启关卡 {currentLevel}");
        }
    }
}