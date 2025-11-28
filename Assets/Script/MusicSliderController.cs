using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 音乐滑块控制器，用于调整背景音乐音量
/// </summary>
public class MusicSliderController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider musicSlider; // 音乐滑块（如果为空，会自动查找）
    
    [Header("Settings")]
    [SerializeField] private float minVolume = 0f; // 最小音量
    [SerializeField] private float maxVolume = 1f; // 最大音量
    [SerializeField] private bool initializeOnStart = true; // 是否在Start时初始化滑块值
    
    private void Awake()
    {
        // 如果没有手动指定滑块，尝试自动查找
        if (musicSlider == null)
        {
            musicSlider = GetComponent<Slider>();
            if (musicSlider == null)
            {
                // 尝试查找名为"MusicSlider"的对象
                GameObject sliderObj = GameObject.Find("MusicSlider");
                if (sliderObj != null)
                {
                    musicSlider = sliderObj.GetComponent<Slider>();
                }
            }
        }
    }
    
    private void Start()
    {
        if (musicSlider == null)
        {
            Debug.LogWarning("MusicSliderController: 未找到Slider组件！请在Inspector中设置MusicSlider引用，或确保当前GameObject有Slider组件。");
            return;
        }
        
        // 设置滑块范围
        musicSlider.minValue = minVolume;
        musicSlider.maxValue = maxVolume;
        
        // 初始化滑块值（延迟一帧，确保BGMManager已经初始化）
        if (initializeOnStart)
        {
            // 使用协程延迟初始化，确保BGMManager已经创建
            StartCoroutine(DelayedInitialize());
        }
        
        // 绑定滑块值变化事件
        musicSlider.onValueChanged.RemoveAllListeners();
        musicSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }
    
    /// <summary>
    /// 延迟初始化，确保BGMManager已经创建
    /// </summary>
    private System.Collections.IEnumerator DelayedInitialize()
    {
        // 等待一帧，确保所有Awake和Start都执行完毕
        yield return null;
        
        // 如果BGMManager还没有创建，尝试访问Instance来触发创建
        if (BGMManager.Instance == null)
        {
            Debug.LogWarning("MusicSliderController: BGMManager尚未初始化，将使用PlayerPrefs中的音量值。");
        }
        
        InitializeSliderValue();
    }
    
    /// <summary>
    /// 初始化滑块值（从BGMManager获取当前音量）
    /// </summary>
    private void InitializeSliderValue()
    {
        if (BGMManager.Instance != null)
        {
            float currentVolume = BGMManager.Instance.Volume;
            musicSlider.value = Mathf.Lerp(minVolume, maxVolume, currentVolume);
        }
        else
        {
            // 如果BGMManager不存在，从PlayerPrefs加载
            float savedVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
            musicSlider.value = Mathf.Lerp(minVolume, maxVolume, savedVolume);
        }
    }
    
    /// <summary>
    /// 滑块值变化时的回调
    /// </summary>
    private void OnSliderValueChanged(float value)
    {
        // 将滑块值（minVolume到maxVolume）转换为0-1的音量值
        float normalizedVolume = Mathf.InverseLerp(minVolume, maxVolume, value);
        
        // 更新BGM音量
        // 尝试获取BGMManager实例（如果不存在会自动创建）
        try
        {
            BGMManager bgmManager = BGMManager.Instance;
            if (bgmManager != null)
            {
                bgmManager.Volume = normalizedVolume;
            }
            else
            {
                // 如果BGMManager不存在，直接保存到PlayerPrefs
                // 这样当BGMManager创建时会自动读取这个值
                PlayerPrefs.SetFloat("BGMVolume", normalizedVolume);
                PlayerPrefs.Save();
            }
        }
        catch (System.Exception e)
        {
            // 如果获取实例失败，至少保存到PlayerPrefs
            Debug.LogWarning($"MusicSliderController: 无法访问BGMManager: {e.Message}，将音量保存到PlayerPrefs。");
            PlayerPrefs.SetFloat("BGMVolume", normalizedVolume);
            PlayerPrefs.Save();
        }
    }
    
    /// <summary>
    /// 手动设置滑块值（0-1）
    /// </summary>
    public void SetVolume(float volume)
    {
        if (musicSlider != null)
        {
            float normalizedValue = Mathf.Lerp(minVolume, maxVolume, Mathf.Clamp01(volume));
            musicSlider.value = normalizedValue;
        }
    }
    
    private void OnDestroy()
    {
        // 清理事件监听
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveAllListeners();
        }
    }
}

