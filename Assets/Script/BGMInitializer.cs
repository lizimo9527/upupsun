using UnityEngine;

/// <summary>
/// BGM初始化器，在Start场景中初始化背景音乐
/// 可以附加到Start场景中的任意GameObject上
/// </summary>
public class BGMInitializer : MonoBehaviour
{
    [Header("BGM Settings")]
    [SerializeField] private AudioClip bgmClip; // 背景音乐音频片段（如果为空，会尝试从Resources加载）
    [SerializeField] private bool playOnStart = true; // 是否在Start时自动播放
    
    private void Start()
    {
        InitializeBGM();
    }
    
    /// <summary>
    /// 初始化BGM管理器
    /// </summary>
    private void InitializeBGM()
    {
        // 获取或创建BGMManager实例
        BGMManager bgmManager = BGMManager.Instance;
        
        // 如果指定了音频片段，设置它
        if (bgmClip != null)
        {
            bgmManager.SetBGMClip(bgmClip);
        }
        else
        {
            // 尝试从Resources文件夹加载
            AudioClip defaultBGM = Resources.Load<AudioClip>("Bgm/G大调小步舞曲");
            if (defaultBGM != null)
            {
                bgmManager.SetBGMClip(defaultBGM);
            }
            else
            {
                Debug.LogWarning("BGMInitializer: 未找到BGM音频片段。请在Inspector中设置BGM Clip，或确保Resources/Bgm/G大调小步舞曲.ogg存在。");
            }
        }
        
        // 如果需要，开始播放
        if (playOnStart)
        {
            bgmManager.PlayBGM();
        }
    }
}


