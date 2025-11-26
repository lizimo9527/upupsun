using UnityEngine;

namespace UpupSun.Environment
{
    public class BackgroundController : MonoBehaviour
    {
        [Header("背景设置")]
        public Material backgroundMaterial;
        public Color backgroundColor = new Color(0.05f, 0.05f, 0.15f, 1f);
        public Color accentColor = new Color(0.1f, 0.1f, 0.3f, 1f);
        
        [Header("动态背景")]
        public bool enableDynamicBackground = true;
        public float colorChangeSpeed = 0.5f;
        public float minBrightness = 0.1f;
        public float maxBrightness = 0.3f;
        
        [Header("星空效果")]
        public bool enableStars = true;
        public GameObject starPrefab;
        public int starCount = 100;
        public Vector2 starArea = new Vector2(20f, 15f);
        public float starTwinkleSpeed = 2f;
        
        private Material dynamicMaterial;
        private Color targetColor;
        private float currentBrightness;
        private GameObject[] stars;
        
        private void Start()
        {
            SetupBackground();
            CreateStarField();
        }
        
        private void SetupBackground()
        {
            // 创建背景四边形
            CreateBackgroundQuad();
            
            // 设置材质
            SetupMaterial();
        }
        
        private void CreateBackgroundQuad()
        {
            // 创建一个覆盖整个屏幕的四边形作为背景
            GameObject backgroundQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            backgroundQuad.name = "Background";
            backgroundQuad.transform.SetParent(transform);
            backgroundQuad.transform.localPosition = Vector3.zero;
            backgroundQuad.transform.localScale = Vector3.one * 50f; // 足够大以覆盖视界
            
            // 移除碰撞体
            Collider collider = backgroundQuad.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }
            
            // 设置层级
            backgroundQuad.layer = LayerMask.NameToLayer("Background");
        }
        
        private void SetupMaterial()
        {
            // 如果没有指定材质，创建一个基础材质
            if (backgroundMaterial == null)
            {
                backgroundMaterial = new Material(Shader.Find("UI/Unlit/Transparent"));
            }
            
            dynamicMaterial = new Material(backgroundMaterial);
            
            // 应用到背景
            Transform background = transform.Find("Background");
            if (background != null)
            {
                Renderer renderer = background.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material = dynamicMaterial;
                }
            }
            
            targetColor = backgroundColor;
        }
        
        private void CreateStarField()
        {
            if (!enableStars) return;
            
            stars = new GameObject[starCount];
            
            for (int i = 0; i < starCount; i++)
            {
                CreateStar(i);
            }
        }
        
        private void CreateStar(int index)
        {
            if (starPrefab != null)
            {
                stars[index] = Instantiate(starPrefab, transform);
            }
            else
            {
                stars[index] = CreateDefaultStar();
            }
            
            // 随机位置
            float x = Random.Range(-starArea.x / 2f, starArea.x / 2f);
            float y = Random.Range(-starArea.y / 2f, starArea.y / 2f);
            stars[index].transform.position = new Vector3(x, y, 10f);
            
            // 随机大小
            float scale = Random.Range(0.1f, 0.5f);
            stars[index].transform.localScale = Vector3.one * scale;
            
            // 设置星星颜色
            Renderer starRenderer = stars[index].GetComponent<Renderer>();
            if (starRenderer != null)
            {
                Color starColor = Color.Lerp(Color.white, Color.yellow, Random.Range(0f, 0.5f));
                starColor.a = Random.Range(0.3f, 1f);
                starRenderer.material.color = starColor;
            }
        }
        
        private GameObject CreateDefaultStar()
        {
            GameObject star = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            star.name = "Star";
            
            // 移除碰撞体
            Collider collider = star.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }
            
            // 创建材质
            Material starMaterial = new Material(Shader.Find("Particles/Standard Unlit"));
            starMaterial.color = Color.white;
            
            Renderer renderer = star.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = starMaterial;
            }
            
            return star;
        }
        
        private void Update()
        {
            if (enableDynamicBackground)
            {
                UpdateDynamicBackground();
            }
            
            if (enableStars)
            {
                UpdateStars();
            }
        }
        
        private void UpdateDynamicBackground()
        {
            // 平滑过渡到目标颜色
            Color currentColor = dynamicMaterial.GetColor("_Color") ?? backgroundColor;
            dynamicMaterial.color = Color.Lerp(currentColor, targetColor, Time.deltaTime * colorChangeSpeed);
            
            // 随机改变目标亮度
            if (Random.Range(0f, 1f) < 0.01f) // 1%的概率每帧
            {
                float targetBrightness = Random.Range(minBrightness, maxBrightness);
                targetColor = backgroundColor * targetBrightness;
            }
        }
        
        private void UpdateStars()
        {
            for (int i = 0; i < stars.Length; i++)
            {
                if (stars[i] != null)
                {
                    // 闪烁效果
                    float twinkle = Mathf.Sin(Time.time * starTwinkleSpeed + i) * 0.5f + 0.5f;
                    
                    Renderer starRenderer = stars[i].GetComponent<Renderer>();
                    if (starRenderer != null)
                    {
                        Color color = starRenderer.material.color;
                        color.a = twinkle * Random.Range(0.3f, 1f);
                        starRenderer.material.color = color;
                    }
                }
            }
        }
        
        public void SetBackgroundColor(Color newColor)
        {
            backgroundColor = newColor;
            targetColor = newColor;
        }
        
        public void SetAccentColor(Color newColor)
        {
            accentColor = newColor;
            // 可以在这里应用强调色到特定元素
        }
        
        public void TriggerBackgroundPulse(float intensity = 1f, float duration = 1f)
        {
            StartCoroutine(BackgroundPulseCoroutine(intensity, duration));
        }
        
        private System.Collections.IEnumerator BackgroundPulseCoroutine(float intensity, float duration)
        {
            Color originalColor = targetColor;
            Color pulseColor = backgroundColor * (1f + intensity);
            float elapsedTime = 0f;
            
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / duration;
                
                // 从脉冲颜色回到原始颜色
                targetColor = Color.Lerp(pulseColor, originalColor, t);
                yield return null;
            }
            
            targetColor = originalColor;
        }
    }
}