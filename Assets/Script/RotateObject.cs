using UnityEngine;

/// <summary>
/// 简单的旋转组件，可以让对象持续旋转
/// </summary>
public class RotateObject : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 90f; // 旋转速度（度/秒）
    [SerializeField] private bool clockwise = true; // 是否顺时针旋转
    [SerializeField] private bool rotateOnStart = true; // 是否在Start时开始旋转
    [SerializeField] private bool pauseWhenGamePaused = true; // 游戏暂停时是否暂停旋转
    
    private bool isRotating = false;
    
    private void Start()
    {
        if (rotateOnStart)
        {
            StartRotation();
        }
    }
    
    private void Update()
    {
        if (isRotating)
        {
            // 如果游戏暂停，检查Time.timeScale
            if (pauseWhenGamePaused && Time.timeScale == 0f)
            {
                return;
            }
            
            // 计算旋转角度（考虑时间缩放）
            float angle = rotationSpeed * Time.deltaTime;
            if (!clockwise)
            {
                angle = -angle;
            }
            
            // 应用旋转
            transform.Rotate(0, 0, angle);
        }
    }
    
    /// <summary>
    /// 开始旋转
    /// </summary>
    public void StartRotation()
    {
        isRotating = true;
    }
    
    /// <summary>
    /// 停止旋转
    /// </summary>
    public void StopRotation()
    {
        isRotating = false;
    }
    
    /// <summary>
    /// 设置旋转速度
    /// </summary>
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
    }
    
    /// <summary>
    /// 设置旋转方向
    /// </summary>
    public void SetClockwise(bool clockwise)
    {
        this.clockwise = clockwise;
    }
}


