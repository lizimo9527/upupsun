using UnityEngine;

/// <summary>
/// 在 TutorialPath 上做“画线”动画，并驱动 pencil 沿线移动。
/// 不影响游戏逻辑，只做视觉引导。
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class TutorialPathAnimator : MonoBehaviour
{
    [Header("基础引用")]
    [Tooltip("已经布好点位的 TutorialPath LineRenderer")]
    public LineRenderer pathRenderer;

    [Tooltip("沿线移动的铅笔/画笔图标（Sprite 或 UI Image 所在的 Transform）")]
    public Transform pencil;

    [Tooltip("整个画线动画持续时间（秒）")]
    public float drawDuration = 2f;

    [Tooltip("开始前的延迟时间（秒）")]
    public float startDelay = 0.5f;

    [Header("行为设置")]
    [Tooltip("是否循环播放教程动画（玩家未操作时）")]
    public bool loop = true;

    [Tooltip("检测玩家点击后是否自动关闭教程")]
    public bool hideOnPlayerInput = true;

    [Tooltip("教程根节点（通常是 TutorialCanvas），用于整体隐藏")]
    public GameObject tutorialRoot;

    // 原始路径点
    private Vector3[] _points;
    private int _pointCount;
    private float _timer;
    private bool _playing;
    private bool _finishedOnce;

    private void Awake()
    {
        if (pathRenderer == null)
        {
            pathRenderer = GetComponent<LineRenderer>();
        }

        if (pathRenderer == null)
        {
            Debug.LogWarning("TutorialPathAnimator: 未找到 LineRenderer。");
            enabled = false;
            return;
        }

        _pointCount = pathRenderer.positionCount;
        if (_pointCount < 2)
        {
            Debug.LogWarning("TutorialPathAnimator: 路径点数量不足（至少需要 2 个点）。");
            enabled = false;
            return;
        }

        _points = new Vector3[_pointCount];
        pathRenderer.GetPositions(_points);

        // 动画开始前先清空线（只留起点）
        pathRenderer.positionCount = 1;
        pathRenderer.SetPosition(0, _points[0]);

        // 初始 pencil 位置
        if (pencil != null)
        {
            pencil.position = _points[0];
        }
    }

    private void OnEnable()
    {
        _timer = 0f;
        _playing = true;
        _finishedOnce = false;
    }

    private void Update()
    {
        // 玩家主动操作：关闭教程
        if (hideOnPlayerInput && Input.GetMouseButtonDown(0))
        {
            HideTutorial();
            return;
        }

        if (!_playing)
        {
            return;
        }

        _timer += Time.deltaTime;

        // 等待起始延迟
        if (_timer < startDelay)
        {
            return;
        }

        float t = Mathf.Clamp01((_timer - startDelay) / Mathf.Max(0.0001f, drawDuration));

        // 根据 t 显示对应数量的点（至少 2 个）
        int visibleCount = Mathf.Clamp(
            Mathf.RoundToInt(t * (_pointCount - 1)) + 1,
            2,
            _pointCount
        );

        pathRenderer.positionCount = visibleCount;
        for (int i = 0; i < visibleCount; i++)
        {
            pathRenderer.SetPosition(i, _points[i]);
        }

        // 驱动 pencil 沿着当前最后一段移动
        if (pencil != null)
        {
            // 当前所在的线段索引
            float floatIndex = t * (_pointCount - 1);
            int idx = Mathf.Clamp(Mathf.FloorToInt(floatIndex), 0, _pointCount - 2);
            float segT = Mathf.Clamp01(floatIndex - idx);

            Vector3 a = _points[idx];
            Vector3 b = _points[idx + 1];
            pencil.position = Vector3.Lerp(a, b, segT);
        }

        // 动画结束
        if (Mathf.Approximately(t, 1f))
        {
            if (!_finishedOnce)
            {
                _finishedOnce = true;
                OnAnimationFinished();
            }
        }
    }

    private void OnAnimationFinished()
    {
        if (loop)
        {
            // 重置并再次播放
            _timer = 0f;
            pathRenderer.positionCount = 1;
            pathRenderer.SetPosition(0, _points[0]);
            if (pencil != null)
            {
                pencil.position = _points[0];
            }
        }
        else
        {
            _playing = false;
            // 不再循环时，可以选择保留整条线或淡出，这里保留不处理
        }
    }

    /// <summary>
    /// 外部调用：完成教程或玩家操作后隐藏。
    /// </summary>
    public void HideTutorial()
    {
        _playing = false;
        if (tutorialRoot != null)
        {
            tutorialRoot.SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}








