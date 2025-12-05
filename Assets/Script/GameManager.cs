 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public List<Vector2> pointList = new List<Vector2>();
    private bool canDraw;
    public LayerMask layerMask;
    
    [Header("Line Physics Settings")]
    public float lineGravityScale = 2f; // 线的重力缩放（增加让线条更容易掉落）
    public float lineMass = 0.5f; // 线的质量（降低质量让线条更轻，反应更快）
    public float lineDrag = 0.5f; // 线的阻力（增加阻力让线条运动更平滑）
    public float lineAngularDrag = 0.5f; // 线的角阻力
    public float lineInitialDownwardForce = 0.5f; // 线条完成时的初始向下力（帮助线条开始掉落）
    public PhysicsMaterial2D linePhysicsMaterial; // 线条的物理材质（可在Inspector中设置，降低摩擦力）
    
    [Header("Sunshine Settings")]
    public GameObject sun; // 太阳对象，需要在Inspector中赋值
    public GameObject sunshinePrefab; // Sunshine预制体，需要在Inspector中赋值（可选）
    public int sunshineCount = 5; // 每次生成的sunshine数量
    public float sunshineSpawnInterval = 0.15f; // 每个sunshine之间的生成间隔（秒）
    public float sunshineInitialForce = 1f; // 初始力的大小
    public float sunshineHorizontalSpread = 0.3f; // 水平方向的随机扩散范围
    public float sunshineSpawnDelay = 0.1f; // 开始生成前的延迟（秒）
    
    public enum SunshineDirection
    {
        VerticalDown,      // 垂直向下
        HorizontalRight,   // 水平向右斜向下
        HorizontalLeft     // 水平向左斜向下
    }
    
    [Tooltip("Sunshine喷射方向：VerticalDown=垂直向下，HorizontalRight=向右斜向下，HorizontalLeft=向左斜向下")]
    public SunshineDirection sunshineDirection = SunshineDirection.VerticalDown;
    
    [Tooltip("水平喷射时的水平力度（0-1，1为完全水平，0为完全垂直）")]
    [Range(0f, 1f)]
    public float sunshineHorizontalForceRatio = 0.9f; // 水平力度比例（默认0.9，更偏向水平）
    
    [Tooltip("水平喷射时的垂直力度范围（向下为负值，绝对值越小水平分量越明显）")]
    public Vector2 sunshineVerticalForceRange = new Vector2(-0.5f, -0.3f); // 垂直力度范围（减小绝对值让水平更明显）
    
    [Header("Progress & Stars")]
    public Slider inkSlider; // Slider 进度条（显示墨水剩余量）
    public GameObject star1; // 第一颗星星
    public GameObject star2; // 第二颗星星
    public GameObject star3; // 第三颗星星
    
    [Header("Win Conditions")]
    public int sunshineToWin = 60; // 通关所需的 sunshine 数量（tree收集到60颗星星）
    
    [Header("Pass Panel")]
    public GameObject passPanel; // 通关弹窗
    public GameObject passPanelStar1; // PassPanel中的第一颗星
    public GameObject passPanelStar2; // PassPanel中的第二颗星
    public GameObject passPanelStar3; // PassPanel中的第三颗星
    public Button passPanelRestartButton; // PassPanel中的重新开始按钮
    public Button passPanelNextButton; // PassPanel中的下一关按钮
    public string nextSceneName = "No.2"; // 下一关的场景名称
    
    [Header("Fireworks Effect")]
    public FireworksEffect fireworksEffect; // 烟花效果控制器（可选）
    public bool enableFireworks = true; // 是否启用烟花效果
    
    [Header("Ink System")]
    public float maxInkAmount = 100f; // 最大墨水量
    public float inkUsedForOneUnit = 1f; // 每单位长度消耗的墨水量
    [Tooltip("墨水消耗的加速度，增大可让进度条下降更快")]
    public float inkUsageMultiplier = 4f;
    public float oneStarInkThreshold = 80f; // 一星条件：使用墨水超过80%
    public float twoStarInkThreshold = 50f; // 二星条件：使用墨水超过50%
    public float threeStarInkThreshold = 30f; // 三星条件：使用墨水超过30%（用的越少越好）
    
    [Header("Delayed Drop Settings")]
    public bool triggerDelayedDropsOnLineComplete = true; // 完成画线后触发延迟掉落
    
    private bool hasGeneratedSunshine = false; // 标记是否已经生成过sunshine
    private int collectedSunshineCount = 0; // 已收集的 sunshine 数量
    private bool gameWon = false; // 游戏是否已通关
    private float totalInkUsed = 0f; // 已使用的墨水量（累计画线长度）
    private float currentInkRemaining = 0f; // 当前剩余墨水量
    private int starsEarned = 1; // 通关后获得的星星数量

    public void Awake()
    {
        canDraw = true;
        lineRenderer.positionCount = 0;
        hasGeneratedSunshine = false;
        collectedSunshineCount = 0;
        gameWon = false;
        totalInkUsed = 0f;
        currentInkRemaining = maxInkAmount;
        
        ConfigureInkSlider();
        
        // 初始化星星为点亮状态
        SetStarActive(star1, true);
        SetStarActive(star2, true);
        SetStarActive(star3, true);
        
        UpdateStarStates();
        UpdateInkProgressBar();
        
        if (passPanel != null)
        {
            passPanel.SetActive(false);
            SetupPassPanelButtons();
        }
    }

    /// <summary>
    /// 当 tree 收集到 sunshine 时调用
    /// </summary>
    public void OnSunshineCollected()
    {
        collectedSunshineCount++;
        CheckWinConditions();
    }
    
    /// <summary>
    /// 更新墨水进度条（显示剩余墨水量）
    /// </summary>
    private void UpdateInkProgressBar()
    {
        if (inkSlider == null)
        {
            return;
        }
        
        float inkRemainingPercent = Mathf.Clamp01(currentInkRemaining / Mathf.Max(0.0001f, maxInkAmount));
        // 将 0-1 的百分比转换为 0-10 的值，匹配场景中的 maxValue = 10
        inkSlider.value = inkRemainingPercent * 10f;
    }
    
    /// <summary>
    /// 检查通关条件和星星条件
    /// </summary>
    private void CheckWinConditions()
    {
        // 检查通关条件：收集到60个sunshine即可通关
        if (!gameWon && collectedSunshineCount >= sunshineToWin)
        {
            gameWon = true;
            // 通关时根据墨水使用量判定星星
            CalculateStars();
            OnGameWin();
        }
    }
    
    private void TriggerDelayedDrops()
    {
        if (!triggerDelayedDropsOnLineComplete)
        {
            return;
        }

        DelayedDrop[] delayedDrops = FindObjectsOfType<DelayedDrop>();
        foreach (var drop in delayedDrops)
        {
            drop.StartDrop(this);
        }
    }
    
    /// <summary>
    /// 根据墨水使用量计算星星数量
    /// </summary>
    private void CalculateStars()
    {
        float inkUsedPercent = GetInkUsedPercent();
        starsEarned = DetermineStars(inkUsedPercent);
        UpdateStarStates();
        
        Debug.Log($"获得{starsEarned}颗星！使用墨水: {inkUsedPercent:F1}%");
    }
    
    /// <summary>
    /// 设置星星的激活状态（亮起或灰色）
    /// </summary>
    private void SetStarActive(GameObject star, bool active)
    {
        if (star == null)
        {
            return;
        }
        
        // 允许星星在World空间或UI Canvas中显示
        SpriteRenderer spriteRenderer = star.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.color = active ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);
            return;
        }
        
        Image image = star.GetComponent<Image>();
        if (image != null)
        {
            image.color = active ? Color.white : new Color(0.5f, 0.5f, 0.5f, 1f);
        }
    }
    
    /// <summary>
    /// 初始化进度条的显示方式（水平方向、从右往左递减）
    /// </summary>
    private void ConfigureInkSlider()
    {
        if (inkSlider == null)
        {
            return;
        }
        
        // 只设置必要的配置，避免覆盖场景中的其他设置（如位置、大小等）
        inkSlider.minValue = 0f;
        inkSlider.maxValue = 10f; // 匹配场景中的配置
        inkSlider.wholeNumbers = false;
        inkSlider.direction = Slider.Direction.RightToLeft;
        
        // 根据当前墨水剩余量设置初始值，而不是硬编码
        // 这样如果场景中设置了不同的初始值，也会被正确使用
        float initialInkPercent = Mathf.Clamp01(currentInkRemaining / Mathf.Max(0.0001f, maxInkAmount));
        inkSlider.value = initialInkPercent * 10f;
        
        inkSlider.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// 计算当前墨水使用百分比
    /// </summary>
    /// <returns>0-100的使用百分比</returns>
    private float GetInkUsedPercent()
    {
        if (maxInkAmount <= 0f)
        {
            return 0f;
        }
        
        return Mathf.Clamp01(totalInkUsed / maxInkAmount) * 100f;
    }
    
    /// <summary>
    /// 根据当前墨水使用量实时更新星星显示
    /// </summary>
    private void UpdateStarStates()
    {
        float inkUsedPercent = GetInkUsedPercent();
        
        // 三星条件：使用量 <= threeStarInkThreshold
        SetStarActive(star3, inkUsedPercent <= threeStarInkThreshold);
        // 二星条件：使用量 <= twoStarInkThreshold
        SetStarActive(star2, inkUsedPercent <= twoStarInkThreshold);
        // 一星始终保留，表示最少的一颗星
        SetStarActive(star1, true);
    }
    
    /// <summary>
    /// 游戏通关时的处理
    /// </summary>
    private void OnGameWin()
    {
        Debug.Log($"游戏通关！收集了 {collectedSunshineCount} 个 sunshine，使用了 {totalInkUsed:F2} 单位墨水");
        ShowPassPanel();
    }
    
    /// <summary>
    /// 根据墨水使用量返回星星数量
    /// </summary>
    private int DetermineStars(float inkUsedPercent)
    {
        if (inkUsedPercent <= threeStarInkThreshold)
        {
            return 3;
        }
        if (inkUsedPercent <= twoStarInkThreshold)
        {
            return 2;
        }
        return 1;
    }
    
    /// <summary>
    /// 显示通关弹窗并同步星星状态
    /// </summary>
    private void ShowPassPanel()
    {
        if (passPanel == null)
        {
            Debug.LogWarning("PassPanel 未分配，无法显示通关面板。请在 GameManager 组件中设置 PassPanel 引用。");
            return;
        }
        
        passPanel.SetActive(true);
        UpdatePassPanelStars();
        
        // 播放烟花效果
        if (enableFireworks)
        {
            PlayFireworks();
        }
    }
    
    /// <summary>
    /// 播放烟花效果
    /// </summary>
    private void PlayFireworks()
    {
        if (fireworksEffect != null)
        {
            fireworksEffect.PlayFireworks();
        }
        else
        {
            // 如果没有手动指定，尝试自动查找或创建
            fireworksEffect = FindObjectOfType<FireworksEffect>();
            if (fireworksEffect == null)
            {
                // 创建一个新的烟花效果对象
                GameObject fireworksObj = new GameObject("FireworksEffect");
                fireworksEffect = fireworksObj.AddComponent<FireworksEffect>();
            }
            
            if (fireworksEffect != null)
            {
                fireworksEffect.PlayFireworks();
            }
        }
    }
    
    /// <summary>
    /// 将通关弹窗中的星星与实际获得的星星数量同步
    /// </summary>
    private void UpdatePassPanelStars()
    {
        if (passPanel == null)
        {
            return;
        }
        
        if (starsEarned <= 0)
        {
            starsEarned = 1;
        }
        
        SetStarActive(passPanelStar1, starsEarned >= 1);
        SetStarActive(passPanelStar2, starsEarned >= 2);
        SetStarActive(passPanelStar3, starsEarned >= 3);
    }
    
    /// <summary>
    /// 绑定PassPanel按钮事件
    /// </summary>
    private void SetupPassPanelButtons()
    {
        if (passPanelRestartButton != null)
        {
            passPanelRestartButton.onClick.RemoveAllListeners();
            passPanelRestartButton.onClick.AddListener(RestartCurrentLevel);
        }
        else
        {
            Debug.LogWarning("PassPanel Restart Button 未设置，无法绑定重新开始事件。");
        }
        
        if (passPanelNextButton != null)
        {
            passPanelNextButton.onClick.RemoveAllListeners();
            passPanelNextButton.onClick.AddListener(LoadNextLevel);
        }
        else
        {
            Debug.LogWarning("PassPanel Next Button 未设置，无法绑定下一关事件。");
        }
    }
    
    /// <summary>
    /// 重新开始当前关卡
    /// </summary>
    private void RestartCurrentLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    
    /// <summary>
    /// 跳转至下一关
    /// </summary>
    private void LoadNextLevel()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("未设置下一关场景名称，无法跳转。");
            return;
        }
        
        if (!Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            Debug.LogWarning($"场景 {nextSceneName} 无法加载，请检查是否已加入Build Settings。");
            return;
        }
        
        SceneManager.LoadScene(nextSceneName);
    }
    
    private void Update()
    {
        // 如果游戏暂停，不允许画线
        if (Time.timeScale == 0f)
        {
            return;
        }
        
        if (Input.GetMouseButton(0)&&canDraw)
        {
            Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if(pointList.Count <=1 &&Physics2D.Raycast(position, Vector3.forward, 100, layerMask))
            {
                return;
            }
            if(pointList.Count > 1)
            {
                RaycastHit2D raycast = Physics2D.Raycast(position, (pointList[lineRenderer.positionCount - 1] - position).normalized, 
                    (position - pointList[lineRenderer.positionCount-1]).magnitude, layerMask);
                if (raycast)
                {
                    return ;
                }
            }
            if (!pointList.Contains(position))
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, position);
                
                // 如果有上一个点，计算线段长度并消耗墨水
                if (pointList.Count > 0)
                {
                    Vector2 previousPosition = pointList[pointList.Count - 1];
                    float segmentLength = Vector2.Distance(previousPosition, position);
                    
                    // 计算并消耗墨水
                    float inkConsumed = segmentLength * inkUsedForOneUnit * Mathf.Max(0.01f, inkUsageMultiplier);
                    totalInkUsed += inkConsumed;
                    currentInkRemaining = Mathf.Max(0f, maxInkAmount - totalInkUsed);
                    
                    // 更新墨水进度条和星星状态
                    UpdateInkProgressBar();
                    UpdateStarStates();
                }
                
                pointList.Add(position);

                if(pointList.Count > 1)
                {
                    Vector2 point1 = pointList[lineRenderer.positionCount - 2];
                    Vector2 point2 = pointList[lineRenderer.positionCount - 1];
                    
                    GameObject go = new GameObject("Collider");
                    go.transform.parent = lineRenderer.transform;
                    go.transform.localPosition = (point1 + point2) / 2;
                    BoxCollider2D boxCollider = go.AddComponent<BoxCollider2D>();
                    // 使用更大的宽度确保能接住sunshine，并确保长度覆盖完整线段
                    float lineWidth = Mathf.Max(lineRenderer.startWidth, lineRenderer.endWidth);
                    float lineLength = (point1 - point2).magnitude;
                    // 确保长度至少有一个最小值，避免碰撞体太小
                    lineLength = Mathf.Max(lineLength, 0.1f);
                    // 增加长度，让碰撞体之间有轻微重叠，避免间隙导致卡住
                    lineLength = lineLength * 1.05f; // 增加5%的长度，确保连接紧密
                    boxCollider.size = new Vector2(lineLength, lineWidth * 1.2f); // 增加20%的宽度容错
                    go.transform.right = (point1 - point2).normalized;
                    
                    // 应用物理材质，降低摩擦力，避免线条卡住
                    if (linePhysicsMaterial != null)
                    {
                        boxCollider.sharedMaterial = linePhysicsMaterial;
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            // 如果没有真正画出一条线（少于2个点），则认为是无效点击：不生成sunshine，也不锁死画线
            if (pointList == null || pointList.Count < 2)
            {
                // 清空可能遗留的渲染点，保持可以继续画线
                if (lineRenderer != null)
                {
                    lineRenderer.positionCount = 0;
                }
                canDraw = true;
                return;
            }

            canDraw = false;
            Rigidbody2D lineRb = lineRenderer.gameObject.GetComponent<Rigidbody2D>();
            if (lineRb == null)
            {
                lineRb = lineRenderer.gameObject.AddComponent<Rigidbody2D>();
            }
            // 设置为Dynamic，让线受到重力影响可以掉落
            lineRb.bodyType = RigidbodyType2D.Dynamic;
            lineRb.gravityScale = lineGravityScale;
            lineRb.mass = lineMass;
            lineRb.drag = lineDrag; // 设置阻力，让线条运动更平滑
            lineRb.angularDrag = lineAngularDrag; // 设置角阻力
            // 设置连续碰撞检测，防止快速移动时穿透
            lineRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            // 不冻结旋转，让线条可以自然旋转和掉落（这样更自然）
            // lineRb.freezeRotation = true; // 注释掉，让线条可以旋转
            
            // 确保所有子碰撞体都应用了物理材质
            if (linePhysicsMaterial != null)
            {
                BoxCollider2D[] colliders = lineRenderer.GetComponentsInChildren<BoxCollider2D>();
                foreach (BoxCollider2D col in colliders)
                {
                    if (col.sharedMaterial == null)
                    {
                        col.sharedMaterial = linePhysicsMaterial;
                    }
                }
            }
            
            // 给线条一个小的初始向下力，帮助它开始掉落（避免卡在某个位置）
            if (lineInitialDownwardForce > 0)
            {
                lineRb.AddForce(Vector2.down * lineInitialDownwardForce, ForceMode2D.Impulse);
            }
            
            TriggerDelayedDrops();
            
            // 画线完成后生成sunshine
            if (!hasGeneratedSunshine && sun != null)
            {
                StartCoroutine(SpawnSunshineSequence());
                hasGeneratedSunshine = true;
            }
        }
    }
    
    /// <summary>
    /// 按顺序生成sunshine的协程，像流水一样一个一个生成
    /// 注意：使用WaitForSeconds会在游戏暂停时自动暂停，这是期望的行为
    /// </summary>
    private IEnumerator SpawnSunshineSequence()
    {
        // 等待初始延迟（游戏暂停时也会暂停）
        yield return new WaitForSeconds(sunshineSpawnDelay);
        
        if (sun == null)
        {
            Debug.LogWarning("Sun对象未设置！");
            yield break;
        }
        
        Vector2 sunPosition = sun.transform.position;
        
        // 一个一个地生成sunshine
        for (int i = 0; i < sunshineCount; i++)
        {
            // 在sun位置附近生成，添加小的水平随机偏移
            Vector2 spawnPosition = sunPosition + new Vector2(
                Random.Range(-sunshineHorizontalSpread, sunshineHorizontalSpread),
                0f // 从sun的位置开始
            );
            
            GameObject sunshine;
            
            // 如果有预制体，使用预制体；否则动态创建
            if (sunshinePrefab != null)
            {
                sunshine = Instantiate(sunshinePrefab, spawnPosition, Quaternion.identity);
                // 确保生成的Sunshine对象是激活的
                sunshine.SetActive(true);
            }
            else
            {
                sunshine = CreateSunshineObject(spawnPosition);
            }
            
            // 给sunshine添加初始力，让它自然掉落
            Rigidbody2D rb = sunshine.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // 设置连续碰撞检测，防止快速移动时穿透
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                
                Vector2 fallDirection;
                
                switch (sunshineDirection)
                {
                    case SunshineDirection.HorizontalRight:
                        // 水平向右斜向下
                        {
                            // 使用水平力度比例直接控制水平分量，垂直分量作为补充
                            float horizontalForce = sunshineHorizontalForceRatio;
                            float verticalForce = Random.Range(sunshineVerticalForceRange.x, sunshineVerticalForceRange.y);
                            // 归一化以确保力度一致，但水平分量会占主导
                            fallDirection = new Vector2(horizontalForce, verticalForce).normalized;
                        }
                        break;
                        
                    case SunshineDirection.HorizontalLeft:
                        // 水平向左斜向下
                        {
                            // 使用水平力度比例直接控制水平分量，垂直分量作为补充
                            float horizontalForce = -sunshineHorizontalForceRatio; // 向左为负
                            float verticalForce = Random.Range(sunshineVerticalForceRange.x, sunshineVerticalForceRange.y);
                            // 归一化以确保力度一致，但水平分量会占主导
                            fallDirection = new Vector2(horizontalForce, verticalForce).normalized;
                        }
                        break;
                        
                    case SunshineDirection.VerticalDown:
                    default:
                        // 垂直向下：主要向下，带一点小的水平随机偏移
                        fallDirection = new Vector2(
                            Random.Range(-0.2f, 0.2f), // 小的水平偏移
                            -1f // 主要向下
                        ).normalized;
                        break;
                }
                
                rb.AddForce(fallDirection * sunshineInitialForce, ForceMode2D.Impulse);
            }
            
            // 确保Sunshine有碰撞体
            if (sunshine.GetComponent<Collider2D>() == null)
            {
                CircleCollider2D collider = sunshine.AddComponent<CircleCollider2D>();
                collider.radius = 0.25f;
            }
            
            // 等待间隔时间再生成下一个（游戏暂停时也会暂停）
            yield return new WaitForSeconds(sunshineSpawnInterval);
        }
    }
    
    /// <summary>
    /// 如果没有预制体，动态创建sunshine对象
    /// </summary>
    private GameObject CreateSunshineObject(Vector2 position)
    {
        GameObject sunshine = new GameObject("Sunshine");
        sunshine.transform.position = position;
        
        // 添加SpriteRenderer（如果有sunlight图片）
        SpriteRenderer sr = sunshine.AddComponent<SpriteRenderer>();
        // 可以在这里设置sprite，如果有资源的话
        
        // 添加TrailRenderer
        TrailRenderer trail = sunshine.AddComponent<TrailRenderer>();
        trail.time = 0.5f;
        trail.startWidth = 0.2f;
        trail.endWidth = 0.05f;
        trail.material = new Material(Shader.Find("Sprites/Default"));
        trail.startColor = new Color(1f, 1f, 0.2f, 1f);
        trail.endColor = new Color(1f, 1f, 0.5f, 0f);
        
        // 添加CircleCollider2D
        CircleCollider2D collider = sunshine.AddComponent<CircleCollider2D>();
        collider.radius = 0.25f;
        
        // 添加Rigidbody2D
        Rigidbody2D rb = sunshine.AddComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        // 设置连续碰撞检测，防止快速移动时穿透
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        return sunshine;
    }
}
