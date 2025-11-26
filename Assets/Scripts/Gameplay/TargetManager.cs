using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace UpupSun.Gameplay
{
    public class TargetManager : MonoBehaviour
    {
        [Header("目标管理")]
        public List<Target> allTargets = new List<Target>();
        public bool requireAllTargets = true;
        public int requiredTargets = 1;
        
        [Header("关卡设置")]
        public bool autoProgressToNextLevel = true;
        public float levelCompleteDelay = 2f;
        
        [Header("UI反馈")]
        public string levelCompleteMessage = "关卡完成！";
        public string allTargetsCompleteMessage = "所有目标完成！";
        
        private int completedTargets = 0;
        private bool levelCompleted = false;
        
        public static TargetManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
        
        private void Start()
        {
            InitializeTargets();
            completedTargets = 0;
            levelCompleted = false;
        }
        
        private void InitializeTargets()
        {
            // 自动查找场景中的所有目标
            Target[] foundTargets = FindObjectsOfType<Target>();
            allTargets.Clear();
            allTargets.AddRange(foundTargets);
            
            Debug.Log($"找到 {allTargets.Count} 个目标");
            
            // 如果没有设置所需目标数量，默认为全部
            if (requiredTargets <= 0)
            {
                requiredTargets = allTargets.Count;
            }
        }
        
        public void OnTargetCompleted(Target completedTarget)
        {
            if (levelCompleted) return;
            
            completedTargets++;
            Debug.Log($"目标完成进度: {completedTargets}/{requiredTargets}");
            
            CheckLevelCompletion();
        }
        
        private void CheckLevelCompletion()
        {
            bool isLevelComplete = false;
            
            if (requireAllTargets)
            {
                isLevelComplete = completedTargets >= allTargets.Count;
            }
            else
            {
                isLevelComplete = completedTargets >= requiredTargets;
            }
            
            if (isLevelComplete && !levelCompleted)
            {
                levelCompleted = true;
                StartCoroutine(HandleLevelComplete());
            }
        }
        
        private IEnumerator HandleLevelComplete()
        {
            Debug.Log(levelCompleteMessage);
            
            // 播放完成效果
            PlayLevelCompleteEffects();
            
            // 等待一段时间
            yield return new WaitForSeconds(levelCompleteDelay);
            
            // 通知游戏管理器
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CompleteLevel();
            }
            
            // 自动进入下一关
            if (autoProgressToNextLevel)
            {
                LoadNextLevel();
            }
        }
        
        private void PlayLevelCompleteEffects()
        {
            // 可以在这里添加关卡完成的全局特效
            // 例如：屏幕闪光、音效、粒子效果等
            
            // 让所有完成的目标播放特效
            foreach (var target in allTargets)
            {
                if (target != null)
                {
                    // 可以在这里调用目标的完成特效
                }
            }
        }
        
        private void LoadNextLevel()
        {
            Debug.Log("加载下一关...");
            // 这里可以添加关卡加载逻辑
        }
        
        public void ResetLevel()
        {
            completedTargets = 0;
            levelCompleted = false;
            
            // 重置所有目标
            foreach (var target in allTargets)
            {
                if (target != null)
                {
                    target.ResetTarget();
                }
            }
            
            Debug.Log("关卡已重置");
        }
        
        public void AddTarget(Target newTarget)
        {
            if (newTarget != null && !allTargets.Contains(newTarget))
            {
                allTargets.Add(newTarget);
                Debug.Log($"添加新目标: {newTarget.gameObject.name}");
            }
        }
        
        public void RemoveTarget(Target targetToRemove)
        {
            if (targetToRemove != null && allTargets.Contains(targetToRemove))
            {
                allTargets.Remove(targetToRemove);
                Debug.Log($"移除目标: {targetToRemove.gameObject.name}");
            }
        }
        
        public int GetCompletedTargetsCount()
        {
            return completedTargets;
        }
        
        public int GetTotalTargetsCount()
        {
            return allTargets.Count;
        }
        
        public float GetCompletionPercentage()
        {
            if (allTargets.Count == 0) return 0f;
            return (float)completedTargets / allTargets.Count;
        }
        
        private void OnDrawGizmos()
        {
            if (allTargets != null && allTargets.Count > 0)
            {
                Gizmos.color = Color.cyan;
                for (int i = 0; i < allTargets.Count - 1; i++)
                {
                    if (allTargets[i] != null && allTargets[i + 1] != null)
                    {
                        Gizmos.DrawLine(allTargets[i].transform.position, allTargets[i + 1].transform.position);
                    }
                }
            }
        }
    }
}