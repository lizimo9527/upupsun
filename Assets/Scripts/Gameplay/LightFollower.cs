using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UpupSun.Gameplay
{
    public class LightFollower : MonoBehaviour
    {
        [Header("光线设置")]
        public Light lightSource;
        public float lightIntensity = 2f;
        public Color lightColor = Color.yellow;
        public float lightRange = 5f;
        
        [Header("跟随设置")]
        public float followSpeed = 3f;
        public float smoothTime = 0.1f;
        private Vector3 currentVelocity;
        
        [Header("粒子效果")]
        public ParticleSystem trailParticles;
        public GameObject glowEffect;
        
        private LineDrawer lineDrawer;
        private List<Vector3> pathPoints;
        private int currentPathIndex = 0;
        private bool isMoving = false;
        
        private void Start()
        {
            SetupLight();
            SetupParticles();
            FindLineDrawer();
        }
        
        private void SetupLight()
        {
            if (lightSource == null)
            {
                lightSource = GetComponent<Light>();
            }
            
            if (lightSource == null)
            {
                GameObject lightObj = new GameObject("LightSource");
                lightObj.transform.SetParent(transform);
                lightSource = lightObj.AddComponent<Light>();
            }
            
            lightSource.type = LightType.Point;
            lightSource.intensity = lightIntensity;
            lightSource.color = lightColor;
            lightSource.range = lightRange;
            lightSource.shadows = LightShadows.Soft;
        }
        
        private void SetupParticles()
        {
            if (trailParticles != null)
            {
                var main = trailParticles.main;
                main.startColor = lightColor;
                trailParticles.Play();
            }
            
            if (glowEffect != null)
            {
                Renderer renderer = glowEffect.GetComponent<Renderer>();
                if (renderer != null && renderer.material != null)
                {
                    renderer.material.color = lightColor;
                }
            }
        }
        
        private void FindLineDrawer()
        {
            lineDrawer = FindObjectOfType<LineDrawer>();
            if (lineDrawer == null)
            {
                Debug.LogWarning("未找到 LineDrawer 组件！");
            }
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartFollowingLine();
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetPosition();
            }
            
            UpdateLightFollow();
        }
        
        public void StartFollowingLine()
        {
            if (lineDrawer == null)
            {
                Debug.LogError("LineDrawer 未设置！");
                return;
            }
            
            if (!lineDrawer.IsLineDrawn())
            {
                Debug.LogWarning("请先画一条线！");
                return;
            }
            
            pathPoints = lineDrawer.GetLinePoints();
            currentPathIndex = 0;
            isMoving = true;
            
            Debug.Log($"开始沿路径移动，共 {pathPoints.Count} 个路径点");
            
            StartCoroutine(FollowPath());
        }
        
        private IEnumerator FollowPath()
        {
            while (isMoving && currentPathIndex < pathPoints.Count)
            {
                Vector3 targetPos = pathPoints[currentPathIndex];
                
                while (Vector3.Distance(transform.position, targetPos) > 0.1f && isMoving)
                {
                    transform.position = Vector3.SmoothDamp(
                        transform.position, 
                        targetPos, 
                        ref currentVelocity, 
                        smoothTime
                    );
                    
                    UpdateLightEffect();
                    yield return null;
                }
                
                currentPathIndex++;
                
                if (currentPathIndex >= pathPoints.Count)
                {
                    OnPathComplete();
                }
            }
        }
        
        private void UpdateLightEffect()
        {
            if (lightSource != null)
            {
                // 根据移动速度动态调整光强度
                float speed = currentVelocity.magnitude;
                lightSource.intensity = Mathf.Lerp(lightIntensity * 0.5f, lightIntensity * 1.5f, speed / 5f);
            }
        }
        
        private void OnPathComplete()
        {
            isMoving = false;
            Debug.Log("光线到达路径终点！");
            
            // 触发完成效果
            PlayCompletionEffect();
            
            // 通知游戏管理器
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CompleteLevel();
            }
        }
        
        private void PlayCompletionEffect()
        {
            // 播放完成特效
            if (trailParticles != null)
            {
                var emission = trailParticles.emission;
                emission.rateOverTime = 100f;
                
                Invoke(nameof(ResetParticleEmission), 1f);
            }
        }
        
        private void ResetParticleEmission()
        {
            if (trailParticles != null)
            {
                var emission = trailParticles.emission;
                emission.rateOverTime = 20f;
            }
        }
        
        public void ResetPosition()
        {
            StopAllCoroutines();
            isMoving = false;
            currentPathIndex = 0;
            transform.position = Vector3.zero;
            
            if (lineDrawer != null)
            {
                lineDrawer.ClearLine();
            }
            
            Debug.Log("重置光源位置");
        }
        
        public void SetLightColor(Color newColor)
        {
            lightColor = newColor;
            if (lightSource != null)
            {
                lightSource.color = newColor;
            }
            
            if (trailParticles != null)
            {
                var main = trailParticles.main;
                main.startColor = newColor;
            }
        }
        
        public void SetFollowSpeed(float newSpeed)
        {
            followSpeed = Mathf.Max(0.1f, newSpeed);
            smoothTime = 1f / followSpeed;
        }
        
        private void OnDrawGizmos()
        {
            if (pathPoints != null && pathPoints.Count > 0)
            {
                Gizmos.color = Color.green;
                for (int i = 0; i < pathPoints.Count - 1; i++)
                {
                    Gizmos.DrawLine(pathPoints[i], pathPoints[i + 1]);
                }
            }
        }
    }
}