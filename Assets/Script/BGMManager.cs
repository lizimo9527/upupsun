using UnityEngine;

/// <summary>
/// 全局背景音乐管理器，使用单例模式，确保在所有场景中持续播放
/// </summary>
public class BGMManager : MonoBehaviour
{
    private static BGMManager _instance;
    
    [Header("BGM Settings")]
    [SerializeField] private AudioClip bgmClip; // 背景音乐音频片段
    [SerializeField] private AudioSource audioSource; // 音频源组件
    [SerializeField] private float defaultVolume = 0.5f; // 默认音量（0-1）
    
    private const string VOLUME_KEY = "BGMVolume"; // PlayerPrefs中保存音量的键
    
    /// <summary>
    /// 单例实例
    /// </summary>
    public static BGMManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 如果实例不存在，尝试在场景中查找
                _instance = FindObjectOfType<BGMManager>();
                
                // 如果仍然不存在，创建一个新的
                if (_instance == null)
                {
                    GameObject bgmObject = new GameObject("BGMPlayer");
                    _instance = bgmObject.AddComponent<BGMManager>();
                    _instance.audioSource = bgmObject.AddComponent<AudioSource>();
                    DontDestroyOnLoad(bgmObject);
                }
            }
            return _instance;
        }
    }
    
    /// <summary>
    /// 当前音量（0-1）
    /// </summary>
    public float Volume
    {
        get
        {
            if (audioSource != null)
            {
                return audioSource.volume;
            }
            return defaultVolume;
        }
        set
        {
            float clampedVolume = Mathf.Clamp01(value);
            if (audioSource != null)
            {
                audioSource.volume = clampedVolume;
            }
            // 保存音量设置到PlayerPrefs
            PlayerPrefs.SetFloat(VOLUME_KEY, clampedVolume);
            PlayerPrefs.Save();
        }
    }
    
    private void Awake()
    {
        // 确保只有一个实例存在
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSource();
        }
        else if (_instance != this)
        {
            // 如果已经存在实例，销毁当前对象
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        // 如果还没有播放，开始播放
        if (audioSource != null && !audioSource.isPlaying && bgmClip != null)
        {
            PlayBGM();
        }
    }
    
    /// <summary>
    /// 初始化音频源组件
    /// </summary>
    private void InitializeAudioSource()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        // 配置音频源
        audioSource.loop = true; // 循环播放
        audioSource.playOnAwake = false; // 不在Awake时自动播放，由Start控制
        
        // 从PlayerPrefs加载保存的音量设置
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, defaultVolume);
        audioSource.volume = Mathf.Clamp01(savedVolume);
        
        // 如果还没有设置音频片段，尝试从Resources加载
        if (bgmClip == null)
        {
            // 尝试加载BGM资源（如果放在Resources文件夹中）
            bgmClip = Resources.Load<AudioClip>("Bgm/G大调小步舞曲");
        }
    }
    
    /// <summary>
    /// 播放背景音乐
    /// </summary>
    public void PlayBGM()
    {
        if (audioSource == null)
        {
            InitializeAudioSource();
        }
        
        if (bgmClip == null)
        {
            Debug.LogWarning("BGM音频片段未设置！请在BGMManager组件中设置BGM Clip。");
            return;
        }
        
        if (audioSource.clip != bgmClip || !audioSource.isPlaying)
        {
            audioSource.clip = bgmClip;
            audioSource.Play();
        }
    }
    
    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBGM()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
    
    /// <summary>
    /// 暂停背景音乐
    /// </summary>
    public void PauseBGM()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }
    
    /// <summary>
    /// 恢复背景音乐
    /// </summary>
    public void ResumeBGM()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.UnPause();
        }
    }
    
    /// <summary>
    /// 设置BGM音频片段（可在Inspector中设置，或通过代码动态设置）
    /// </summary>
    public void SetBGMClip(AudioClip clip)
    {
        bgmClip = clip;
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}


