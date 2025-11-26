using UnityEngine;
using System.Collections;

namespace UpupSun.Gameplay
{
    public class Target : MonoBehaviour
    {
        [Header("目标设置")]
        public bool isActive = true;
        public GameObject targetVisual;
        public float activationDistance = 0.5f;
        
        [Header("特效设置")]
        public ParticleSystem activationEffect;
        public GameObject successVisual;
        public Color successColor = Color.green;
        
        [Header("音效设置")]
        public AudioClip activationSound;
        public AudioClip successSound;
        
        private AudioSource audioSource;
        private Material targetMaterial;
        private Color originalColor;
        private bool isActivated = false;
        
        private void Start()
        {
            SetupTarget();
            SetupAudio();
        }
        
        private void SetupTarget()
        {
            if (targetVisual == null)
            {
                targetVisual = transform.GetChild(0)?.gameObject;
            }
            
            if (targetVisual != null)
            {
                Renderer renderer = targetVisual.GetComponent<Renderer>();
                if (renderer != null)
                {
                    targetMaterial = renderer.material;
                    originalColor = targetMaterial.color;
                }
            }
        }
        
        private void SetupAudio()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D音效
        }
        
        private void Update()
        {
            if (!isActive || isActivated) return;
            
            CheckLightProximity();
        }
        
        private void CheckLightProximity()
        {
            // 查找场景中的光源
            LightFollower[] lightFollowers = FindObjectsOfType<LightFollower>();
            
            foreach (var lightFollower in lightFollowers)
            {
                float distance = Vector3.Distance(transform.position, lightFollower.transform.position);
                
                if (distance <= activationDistance)
                {
                    ActivateTarget();
                    break;
                }
            }
        }
        
        private void ActivateTarget()
        {
            if (isActivated) return;
            
            isActivated = true;
            Debug.Log($"目标 {gameObject.name} 被激活！");
            
            StartCoroutine(ActivationSequence());
        }
        
        private IEnumerator ActivationSequence()
        {
            // 播放激活音效
            if (activationSound != null)
            {
                audioSource.PlayOneShot(activationSound);
            }
            
            // 改变颜色表示激活
            if (targetMaterial != null)
            {
                Color currentColor = targetMaterial.color;
                float elapsedTime = 0f;
                float duration = 0.5f;
                
                while (elapsedTime < duration)
                {
                    elapsedTime += Time.deltaTime;
                    float t = elapsedTime / duration;
                    targetMaterial.color = Color.Lerp(currentColor, successColor, t);
                    yield return null;
                }
            }
            
            // 播放激活特效
            if (activationEffect != null)
            {
                activationEffect.Play();
            }
            
            // 显示成功视觉
            if (successVisual != null)
            {
                successVisual.SetActive(true);
            }
            
            // 播放成功音效
            if (successSound != null)
            {
                audioSource.PlayOneShot(successSound);
            }
            
            // 通知游戏管理器
            OnTargetCompleted();
            
            // 禁用碰撞体（如果有）
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = false;
            }
        }
        
        private void OnTargetCompleted()
        {
            // 可以在这里添加目标完成后的逻辑
            TargetManager targetManager = FindObjectOfType<TargetManager>();
            if (targetManager != null)
            {
                targetManager.OnTargetCompleted(this);
            }
        }
        
        public void ResetTarget()
        {
            isActivated = false;
            isActive = true;
            
            // 恢复原始颜色
            if (targetMaterial != null)
            {
                targetMaterial.color = originalColor;
            }
            
            // 隐藏成功视觉
            if (successVisual != null)
            {
                successVisual.SetActive(false);
            }
            
            // 启用碰撞体
            Collider collider = GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }
            
            // 停止特效
            if (activationEffect != null && activationEffect.isPlaying)
            {
                activationEffect.Stop();
            }
        }
        
        public void SetActive(bool active)
        {
            isActive = active;
            if (targetVisual != null)
            {
                targetVisual.SetActive(active);
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            if (isActive && !isActivated)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, activationDistance);
            }
        }
    }
}