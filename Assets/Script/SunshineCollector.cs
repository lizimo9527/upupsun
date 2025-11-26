using UnityEngine;
using System.Collections.Generic;

public class SunshineCollector : MonoBehaviour
{
    private GameManager gameManager;
    public Transform containerTransform; // 梯形容器的Transform，用于存储sunshine
    
    // 存储收集到的sunshine列表
    private List<GameObject> collectedSunshine = new List<GameObject>();
    
    // 容器参数
    public float containerTopWidth = 4f; // 容器顶部宽度
    public float containerBottomWidth = 2f; // 容器底部宽度
    public float containerHeight = 3f; // 容器高度
    public float containerVerticalOffset = 0f; // 容器垂直偏移（向上为正，向下为负）
    public float sunshineSpacing = 0.3f; // sunshine之间的间距
    
    void Start()
    {
        // 查找 GameManager
        gameManager = FindObjectOfType<GameManager>();
        
        // 确保tree有SpriteMask组件用于遮罩效果
        SetupSpriteMask();
        
        // 如果没有指定容器，创建一个
        if (containerTransform == null)
        {
            CreateContainer();
        }
        else
        {
            // 如果容器已存在，更新其形状
            UpdateContainer();
        }
    }
    
    /// <summary>
    /// 设置SpriteMask，使sunshine在tree后面的部分被遮住
    /// </summary>
    void SetupSpriteMask()
    {
        // 检查是否已有SpriteMask组件
        SpriteMask spriteMask = GetComponent<SpriteMask>();
        if (spriteMask == null)
        {
            // 如果没有，添加SpriteMask组件
            spriteMask = gameObject.AddComponent<SpriteMask>();
            
            // 获取tree的SpriteRenderer，使用相同的sprite作为遮罩
            SpriteRenderer treeSR = GetComponent<SpriteRenderer>();
            if (treeSR != null && treeSR.sprite != null)
            {
                spriteMask.sprite = treeSR.sprite;
                
                // 获取tree的sortingOrder，设置遮罩范围
                int treeSortingOrder = treeSR.sortingOrder;
                spriteMask.isCustomRangeActive = true;
                spriteMask.frontSortingOrder = treeSortingOrder; // tree的sorting order
                spriteMask.backSortingOrder = treeSortingOrder - 1; // sunshine的sorting order应该在这个范围内
            }
            else
            {
                // 如果没有SpriteRenderer，使用默认值
                spriteMask.isCustomRangeActive = true;
                spriteMask.frontSortingOrder = 1;
                spriteMask.backSortingOrder = 0;
            }
        }
    }
    
    /// <summary>
    /// 在Inspector中修改参数时自动更新容器
    /// </summary>
    void OnValidate()
    {
        // 只在编辑器中且容器已存在时更新
        if (containerTransform != null)
        {
            UpdateContainer();
        }
    }
    
    /// <summary>
    /// 创建梯形容器
    /// </summary>
    void CreateContainer()
    {
        // 创建容器GameObject
        GameObject container = new GameObject("SunshineContainer");
        container.transform.SetParent(transform);
        // 放在tree底部，根据tree的sprite大小调整位置
        SpriteRenderer treeSR = GetComponent<SpriteRenderer>();
        float baseY;
        if (treeSR != null)
        {
            // 获取tree的bounds，将容器放在tree底部
            Bounds treeBounds = treeSR.bounds;
            baseY = -treeBounds.size.y * 0.3f;
        }
        else
        {
            baseY = -1.5f;
        }
        // 应用垂直偏移
        container.transform.localPosition = new Vector3(0, baseY + containerVerticalOffset, 0);
        
        // 创建梯形的碰撞体（用于检测sunshine是否在容器内）
        PolygonCollider2D polygonCollider = container.AddComponent<PolygonCollider2D>();
        polygonCollider.isTrigger = true;
        
        // 设置梯形顶点（相对于容器中心）
        Vector2[] points = new Vector2[4];
        float halfTop = containerTopWidth / 2f;
        float halfBottom = containerBottomWidth / 2f;
        float halfHeight = containerHeight / 2f;
        
        // 梯形顶点：左上、右上、右下、左下
        points[0] = new Vector2(-halfTop, halfHeight);
        points[1] = new Vector2(halfTop, halfHeight);
        points[2] = new Vector2(halfBottom, -halfHeight);
        points[3] = new Vector2(-halfBottom, -halfHeight);
        
        polygonCollider.points = points;
        
        containerTransform = container.transform;
    }
    
    /// <summary>
    /// 更新容器的形状（当参数改变时调用）
    /// 可以在Inspector中右键组件选择此方法来手动更新
    /// </summary>
    [ContextMenu("Update Container")]
    public void UpdateContainer()
    {
        if (containerTransform == null)
        {
            return;
        }
        
        // 更新容器位置（应用垂直偏移）
        SpriteRenderer treeSR = GetComponent<SpriteRenderer>();
        float baseY;
        if (treeSR != null)
        {
            Bounds treeBounds = treeSR.bounds;
            baseY = -treeBounds.size.y * 0.3f;
        }
        else
        {
            baseY = -1.5f;
        }
        containerTransform.localPosition = new Vector3(0, baseY + containerVerticalOffset, 0);
        
        // 更新PolygonCollider2D的形状
        PolygonCollider2D polygonCollider = containerTransform.GetComponent<PolygonCollider2D>();
        if (polygonCollider != null)
        {
            // 设置梯形顶点（相对于容器中心）
            Vector2[] points = new Vector2[4];
            float halfTop = containerTopWidth / 2f;
            float halfBottom = containerBottomWidth / 2f;
            float halfHeight = containerHeight / 2f;
            
            // 梯形顶点：左上、右上、右下、左下
            points[0] = new Vector2(-halfTop, halfHeight);
            points[1] = new Vector2(halfTop, halfHeight);
            points[2] = new Vector2(halfBottom, -halfHeight);
            points[3] = new Vector2(-halfBottom, -halfHeight);
            
            polygonCollider.points = points;
        }
        
        // 如果运行时已收集了sunshine，重新排列它们
        if (Application.isPlaying && collectedSunshine.Count > 0)
        {
            for (int i = 0; i < collectedSunshine.Count; i++)
            {
                if (collectedSunshine[i] != null)
                {
                    Vector3 newPosition = CalculateContainerPosition(i);
                    collectedSunshine[i].transform.localPosition = newPosition;
                }
            }
        }
    }
    
    /// <summary>
    /// 在Scene视图中绘制容器边界（Gizmos），方便调整
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Vector3 center;
        if (containerTransform == null)
        {
            // 如果容器不存在，根据当前参数绘制预览（使用tree的本地坐标）
            SpriteRenderer treeSR = GetComponent<SpriteRenderer>();
            float baseY;
            if (treeSR != null)
            {
                Bounds treeBounds = treeSR.bounds;
                baseY = -treeBounds.size.y * 0.3f;
            }
            else
            {
                baseY = -1.5f;
            }
            center = transform.TransformPoint(new Vector3(0, baseY + containerVerticalOffset, 0));
        }
        else
        {
            // 绘制实际容器位置（世界坐标）
            center = containerTransform.position;
        }
        
        DrawContainerGizmo(center);
    }
    
    /// <summary>
    /// 绘制容器边界的Gizmo
    /// </summary>
    void DrawContainerGizmo(Vector3 center)
    {
        // 计算梯形顶点（世界坐标）
        // 注意：这里假设容器是水平放置的，如果需要旋转，需要应用transform的旋转
        float halfTop = containerTopWidth / 2f;
        float halfBottom = containerBottomWidth / 2f;
        float halfHeight = containerHeight / 2f;
        
        Vector3 topLeft = center + new Vector3(-halfTop, halfHeight, 0);
        Vector3 topRight = center + new Vector3(halfTop, halfHeight, 0);
        Vector3 bottomRight = center + new Vector3(halfBottom, -halfHeight, 0);
        Vector3 bottomLeft = center + new Vector3(-halfBottom, -halfHeight, 0);
        
        // 设置Gizmo颜色
        Gizmos.color = new Color(1f, 1f, 0f, 0.8f); // 半透明黄色
        
        // 绘制梯形边界
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
        
        // 绘制中心点
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, 0.1f);
        
        // 绘制尺寸标注线（帮助理解尺寸）
        Gizmos.color = new Color(0f, 1f, 1f, 0.6f); // 半透明青色
        // 顶部宽度线
        Gizmos.DrawLine(topLeft + Vector3.up * 0.2f, topRight + Vector3.up * 0.2f);
        // 底部宽度线
        Gizmos.DrawLine(bottomLeft + Vector3.down * 0.2f, bottomRight + Vector3.down * 0.2f);
        // 高度线（右侧）
        Gizmos.DrawLine(center + new Vector3(halfTop + 0.2f, -halfHeight, 0), 
                       center + new Vector3(halfTop + 0.2f, halfHeight, 0));
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        // 检测是否收集到 sunshine
        if (other.gameObject.name.Contains("Sunshine") || other.CompareTag("Sunshine"))
        {
            // 不要立即销毁，而是移动到容器中
            CollectSunshine(other.gameObject);
        }
    }
    
    /// <summary>
    /// 收集sunshine到容器中
    /// </summary>
    void CollectSunshine(GameObject sunshine)
    {
        // 防止重复收集
        if (collectedSunshine.Contains(sunshine))
        {
            return;
        }
        
        // 添加到列表
        collectedSunshine.Add(sunshine);
        
        // 通知 GameManager 收集到一个 sunshine
        if (gameManager != null)
        {
            gameManager.OnSunshineCollected();
        }
        
        // 停止sunshine的物理运动
        Rigidbody2D rb = sunshine.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = 0f; // 取消重力
            rb.isKinematic = true; // 设置为运动学，不再受物理影响
        }
        
        // 禁用碰撞体，避免继续触发
        Collider2D col = sunshine.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
        
        // 计算在容器中的位置（梯形排列）
        Vector3 containerPosition = CalculateContainerPosition(collectedSunshine.Count - 1);
        
        // 将sunshine移动到容器位置
        sunshine.transform.SetParent(containerTransform);
        sunshine.transform.localPosition = containerPosition;
        
        // 设置sunshine的遮罩交互，使其在tree后面的部分被遮住
        SpriteRenderer sunshineSR = sunshine.GetComponent<SpriteRenderer>();
        SpriteRenderer treeSR = GetComponent<SpriteRenderer>();
        if (sunshineSR != null)
        {
            // 获取tree的sortingOrder，设置sunshine的sortingOrder低于tree
            // 这样sunshine在tree后面，重叠的部分会被tree遮挡，只显示tree部分
            int treeSortingOrder = treeSR != null ? treeSR.sortingOrder : 1;
            sunshineSR.sortingOrder = treeSortingOrder - 1;
            
            // 不设置maskInteraction，让sunshine正常显示
            // 由于sunshine的sortingOrder低于tree，重叠的部分会被tree自然遮挡
            sunshineSR.maskInteraction = SpriteMaskInteraction.None;
        }
        
        // 禁用TrailRenderer（如果存在）
        TrailRenderer trail = sunshine.GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.enabled = false;
        }
    }
    
    /// <summary>
    /// 计算sunshine在容器中的位置（梯形排列）
    /// </summary>
    Vector3 CalculateContainerPosition(int index)
    {
        if (containerTransform == null)
        {
            return Vector3.zero;
        }
        
        // 计算每行可以放多少个sunshine（根据容器宽度动态计算）
        int maxColsPerRow = Mathf.Max(3, Mathf.FloorToInt(containerTopWidth / sunshineSpacing));
        int row = index / maxColsPerRow;
        int col = index % maxColsPerRow;
        
        // 根据行数计算该行的宽度（梯形，从上到下变窄）
        float t = Mathf.Clamp01((float)row / 8f); // 最多8行
        float rowWidth = Mathf.Lerp(containerTopWidth, containerBottomWidth, t);
        
        // 计算该行实际可以放置的列数
        int colsInThisRow = Mathf.Max(1, Mathf.FloorToInt(rowWidth / sunshineSpacing));
        if (col >= colsInThisRow)
        {
            col = colsInThisRow - 1; // 如果超出，放在最后一列
        }
        
        // 计算该行的起始x位置（居中）
        float totalWidth = (colsInThisRow - 1) * sunshineSpacing;
        float startX = -totalWidth / 2f;
        
        // 计算x位置
        float x = startX + col * sunshineSpacing;
        
        // 计算y位置（从下往上排列）
        float y = -containerHeight / 2f + row * sunshineSpacing + sunshineSpacing / 2f;
        
        // 限制在容器范围内
        x = Mathf.Clamp(x, -rowWidth / 2f, rowWidth / 2f);
        y = Mathf.Clamp(y, -containerHeight / 2f, containerHeight / 2f);
        
        return new Vector3(x, y, 0);
    }
}
