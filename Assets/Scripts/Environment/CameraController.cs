using UnityEngine;

namespace UpupSun.Environment
{
    public class CameraController : MonoBehaviour
    {
        [Header("相机设置")]
        public float orthographicSize = 10f;
        public Vector3 cameraOffset = Vector3.zero;
        
        [Header("跟随设置")]
        public bool followTarget = false;
        public Transform targetToFollow;
        public float followSmoothTime = 0.3f;
        
        [Header("边界设置")]
        public bool useBounds = false;
        public Vector2 minBounds = Vector2.zero;
        public Vector2 maxBounds = Vector2.one * 20f;
        
        private Camera mainCamera;
        private Vector3 currentVelocity;
        private Vector3 targetPosition;
        
        private void Start()
        {
            SetupCamera();
        }
        
        private void SetupCamera()
        {
            mainCamera = GetComponent<Camera>();
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            
            if (mainCamera != null)
            {
                mainCamera.orthographic = true;
                mainCamera.orthographicSize = orthographicSize;
                
                // 设置背景颜色为深色
                mainCamera.backgroundColor = new Color(0.1f, 0.1f, 0.2f, 1f);
            }
        }
        
        private void LateUpdate()
        {
            UpdateCameraPosition();
        }
        
        private void UpdateCameraPosition()
        {
            if (followTarget && targetToFollow != null)
            {
                // 跟随目标
                targetPosition = targetToFollow.position + cameraOffset;
                targetPosition.z = transform.position.z; // 保持Z位置不变
                
                // 平滑移动
                transform.position = Vector3.SmoothDamp(
                    transform.position, 
                    targetPosition, 
                    ref currentVelocity, 
                    followSmoothTime
                );
            }
            
            // 应用边界限制
            if (useBounds)
            {
                ApplyBounds();
            }
        }
        
        private void ApplyBounds()
        {
            Vector3 pos = transform.position;
            
            // 计算相机视界边界
            float vertExtent = mainCamera.orthographicSize;
            float horzExtent = vertExtent * Screen.width / Screen.height;
            
            // 限制位置在边界内
            pos.x = Mathf.Clamp(pos.x, minBounds.x + horzExtent, maxBounds.x - horzExtent);
            pos.y = Mathf.Clamp(pos.y, minBounds.y + vertExtent, maxBounds.y - vertExtent);
            
            transform.position = pos;
        }
        
        public void SetTarget(Transform newTarget)
        {
            targetToFollow = newTarget;
        }
        
        public void SetFollowMode(bool follow)
        {
            followTarget = follow;
        }
        
        public void SetCameraBounds(Vector2 min, Vector2 max)
        {
            minBounds = min;
            maxBounds = max;
            useBounds = true;
        }
        
        public void ZoomToSize(float newSize, float duration = 1f)
        {
            StartCoroutine(ZoomCoroutine(newSize, duration));
        }
        
        private System.Collections.IEnumerator ZoomCoroutine(float targetSize, float duration)
        {
            float startSize = mainCamera.orthographicSize;
            float elapsedTime = 0f;
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                
                mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, t);
                yield return null;
            }
            
            mainCamera.orthographicSize = targetSize;
        }
        
        public void ShakeCamera(float intensity = 0.5f, float duration = 0.3f)
        {
            StartCoroutine(ShakeCoroutine(intensity, duration));
        }
        
        private System.Collections.IEnumerator ShakeCoroutine(float intensity, float duration)
        {
            Vector3 originalPosition = transform.position;
            float elapsedTime = 0f;
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                
                float x = Random.Range(-intensity, intensity);
                float y = Random.Range(-intensity, intensity);
                
                transform.position = originalPosition + new Vector3(x, y, 0);
                yield return null;
            }
            
            transform.position = originalPosition;
        }
        
        private void OnDrawGizmosSelected()
        {
            if (useBounds)
            {
                // 绘制边界
                Gizmos.color = Color.yellow;
                Vector3 center = (Vector3)(minBounds + maxBounds) / 2f;
                Vector3 size = new Vector3(maxBounds.x - minBounds.x, maxBounds.y - minBounds.y, 0);
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
}