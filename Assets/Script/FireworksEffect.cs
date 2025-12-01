using UnityEngine;
using System.Collections;

/// <summary>
/// 烟花效果控制器，在通关时显示烟花效果
/// </summary>
public class FireworksEffect : MonoBehaviour
{
    [Header("Fireworks Settings")]
    [SerializeField] private GameObject fireworkPrefab; // 烟花预制体（可选，如果没有会自动创建）
    [SerializeField] private int fireworkCount = 5; // 烟花数量
    [SerializeField] private float spawnInterval = 0.3f; // 烟花生成间隔
    [SerializeField] private float fireworkDuration = 2f; // 每个烟花的持续时间
    
    [Header("Spawn Area")]
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-5f, -3f); // 生成区域最小值
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(5f, 3f); // 生成区域最大值
    [SerializeField] private bool useScreenSpace = true; // 是否使用屏幕空间坐标
    
    [Header("Visual Settings")]
    [SerializeField] private Color[] fireworkColors = new Color[] 
    { 
        Color.red, 
        Color.yellow, 
        Color.green, 
        Color.blue, 
        Color.magenta,
        Color.cyan
    }; // 烟花颜色数组
    
    private Camera mainCamera;
    
    private void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            mainCamera = FindObjectOfType<Camera>();
        }
    }
    
    /// <summary>
    /// 播放烟花效果
    /// </summary>
    public void PlayFireworks()
    {
        StartCoroutine(SpawnFireworksSequence());
    }
    
    /// <summary>
    /// 按顺序生成烟花
    /// </summary>
    private IEnumerator SpawnFireworksSequence()
    {
        for (int i = 0; i < fireworkCount; i++)
        {
            Vector2 spawnPosition = GetRandomSpawnPosition();
            CreateFirework(spawnPosition, fireworkColors[Random.Range(0, fireworkColors.Length)]);
            
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    /// <summary>
    /// 获取随机生成位置
    /// </summary>
    private Vector2 GetRandomSpawnPosition()
    {
        if (useScreenSpace && mainCamera != null)
        {
            // 使用屏幕空间，转换为世界坐标
            float screenX = Random.Range(Screen.width * 0.1f, Screen.width * 0.9f);
            float screenY = Random.Range(Screen.height * 0.3f, Screen.height * 0.9f);
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenX, screenY, mainCamera.nearClipPlane + 1f));
            return new Vector2(worldPos.x, worldPos.y);
        }
        else
        {
            // 使用世界空间
            return new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );
        }
    }
    
    /// <summary>
    /// 创建单个烟花
    /// </summary>
    private void CreateFirework(Vector2 position, Color color)
    {
        GameObject firework;
        
        if (fireworkPrefab != null)
        {
            firework = Instantiate(fireworkPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
        }
        else
        {
            // 如果没有预制体，动态创建粒子系统
            firework = CreateFireworkParticleSystem(position, color);
        }
        
        // 自动销毁
        Destroy(firework, fireworkDuration);
    }
    
    /// <summary>
    /// 动态创建烟花粒子系统
    /// </summary>
    private GameObject CreateFireworkParticleSystem(Vector2 position, Color color)
    {
        GameObject firework = new GameObject("Firework");
        firework.transform.position = new Vector3(position.x, position.y, 0);
        
        // 添加粒子系统
        ParticleSystem ps = firework.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = ps.main;
        main.startLifetime = 1.5f;
        main.startSpeed = 5f;
        main.startSize = 0.2f;
        main.startColor = color;
        main.maxParticles = 50;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        
        // 设置发射器
        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0.0f, 30) // 在0秒时发射30个粒子
        });
        
        // 设置形状（圆形爆炸）
        ParticleSystem.ShapeModule shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.1f;
        
        // 设置速度（向外爆炸）
        ParticleSystem.VelocityOverLifetimeModule velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;
        velocity.radial = new ParticleSystem.MinMaxCurve(3f);
        
        // 设置颜色渐变
        ParticleSystem.ColorOverLifetimeModule colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(Color.white, 0.5f), new GradientColorKey(color, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 0.5f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        colorOverLifetime.color = gradient;
        
        // 设置大小渐变
        ParticleSystem.SizeOverLifetimeModule sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0.0f, 0.3f);
        sizeCurve.AddKey(0.5f, 0.5f);
        sizeCurve.AddKey(1.0f, 0.1f);
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1.0f, sizeCurve);
        
        // 播放
        ps.Play();
        
        return firework;
    }
    
    /// <summary>
    /// 停止所有烟花效果
    /// </summary>
    public void StopFireworks()
    {
        StopAllCoroutines();
        
        // 销毁所有烟花对象
        GameObject[] fireworks = GameObject.FindGameObjectsWithTag("Firework");
        foreach (GameObject fw in fireworks)
        {
            if (fw.name.Contains("Firework"))
            {
                Destroy(fw);
            }
        }
    }
}


