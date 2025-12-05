using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 用于在 game directory 场景中绑定按钮点击事件的脚本
/// </summary>
public class GameDirectoryButtonBinder : MonoBehaviour
{
    [Header("按钮引用")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button noButton1;
    [SerializeField] private Button noButton2;
    [SerializeField] private Button noButton3;
    [SerializeField] private Button noButton4;
    [SerializeField] private Button noButton5;
    [SerializeField] private Button noButton6;

    private SceneLoader sceneLoader;

    private void Awake()
    {
        // 获取 SceneLoader 组件
        sceneLoader = FindObjectOfType<SceneLoader>();
        
        if (sceneLoader == null)
        {
            Debug.LogError("GameDirectoryButtonBinder: 未找到 SceneLoader 组件！");
            return;
        }

        // 如果按钮引用为空，尝试通过名称查找
        if (backButton == null)
        {
            GameObject backBtnObj = GameObject.Find("backButton");
            if (backBtnObj != null)
            {
                backButton = backBtnObj.GetComponent<Button>();
            }
        }

        if (noButton1 == null)
        {
            GameObject noBtn1Obj = GameObject.Find("NoButton1");
            if (noBtn1Obj != null)
            {
                noButton1 = noBtn1Obj.GetComponent<Button>();
            }
        }

        if (noButton2 == null)
        {
            GameObject noBtn2Obj = GameObject.Find("NoButton 2");
            if (noBtn2Obj != null)
            {
                noButton2 = noBtn2Obj.GetComponent<Button>();
            }
        }

        if (noButton3 == null)
        {
            GameObject noBtn3Obj = GameObject.Find("NoButton 3");
            if (noBtn3Obj != null)
            {
                noButton3 = noBtn3Obj.GetComponent<Button>();
            }
        }

        if (noButton4 == null)
        {
            GameObject noBtn4Obj = GameObject.Find("NoButton 4");
            if (noBtn4Obj != null)
            {
                noButton4 = noBtn4Obj.GetComponent<Button>();
            }
        }

        if (noButton5 == null)
        {
            GameObject noBtn5Obj = GameObject.Find("NoButton 5");
            if (noBtn5Obj != null)
            {
                noButton5 = noBtn5Obj.GetComponent<Button>();
            }
        }

        if (noButton6 == null)
        {
            GameObject noBtn6Obj = GameObject.Find("NoButton 6");
            if (noBtn6Obj != null)
            {
                noButton6 = noBtn6Obj.GetComponent<Button>();
            }
        }

        // 绑定按钮事件
        BindButtons();
    }

    private void BindButtons()
    {
        // 绑定 backButton
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(() => {
                Debug.Log("backButton 被点击，准备跳转到 Start 场景");
                sceneLoader.LoadStartScene();
            });
            Debug.Log("backButton 事件已绑定");
        }
        else
        {
            Debug.LogWarning("GameDirectoryButtonBinder: backButton 未找到！");
        }

        // 绑定 NoButton1
        if (noButton1 != null)
        {
            noButton1.onClick.RemoveAllListeners();
            noButton1.onClick.AddListener(() => {
                Debug.Log("NoButton1 被点击，准备跳转到 SampleScene");
                sceneLoader.LoadSampleScene();
            });
            Debug.Log("NoButton1 事件已绑定");
        }
        else
        {
            Debug.LogWarning("GameDirectoryButtonBinder: NoButton1 未找到！");
        }

        // 绑定 NoButton2
        if (noButton2 != null)
        {
            noButton2.onClick.RemoveAllListeners();
            noButton2.onClick.AddListener(() => {
                Debug.Log("NoButton 2 被点击，准备跳转到 No.2 场景");
                sceneLoader.LoadNo2Scene();
            });
            Debug.Log("NoButton 2 事件已绑定");
        }
        else
        {
            Debug.LogWarning("GameDirectoryButtonBinder: NoButton 2 未找到！");
        }

        // 绑定 NoButton3
        if (noButton3 != null)
        {
            noButton3.onClick.RemoveAllListeners();
            noButton3.onClick.AddListener(() => {
                Debug.Log("NoButton 3 被点击，准备跳转到 No.3 场景");
                sceneLoader.LoadNo3Scene();
            });
            Debug.Log("NoButton 3 事件已绑定");
        }
        else
        {
            Debug.LogWarning("GameDirectoryButtonBinder: NoButton 3 未找到！");
        }

        // 绑定 NoButton4
        if (noButton4 != null)
        {
            noButton4.onClick.RemoveAllListeners();
            noButton4.onClick.AddListener(() => {
                Debug.Log("NoButton 4 被点击，准备跳转到 No.4 场景");
                sceneLoader.LoadNo4Scene();
            });
            Debug.Log("NoButton 4 事件已绑定");
        }
        else
        {
            Debug.LogWarning("GameDirectoryButtonBinder: NoButton 4 未找到！");
        }

        // 绑定 NoButton5
        if (noButton5 != null)
        {
            noButton5.onClick.RemoveAllListeners();
            noButton5.onClick.AddListener(() => {
                Debug.Log("NoButton 5 被点击，准备跳转到 No.5 场景");
                sceneLoader.LoadNo5Scene();
            });
            Debug.Log("NoButton 5 事件已绑定");
        }
        else
        {
            Debug.LogWarning("GameDirectoryButtonBinder: NoButton 5 未找到！");
        }

        // 绑定 NoButton6
        if (noButton6 != null)
        {
            noButton6.onClick.RemoveAllListeners();
            noButton6.onClick.AddListener(() => {
                Debug.Log("NoButton 6 被点击，准备跳转到 No.6 场景");
                sceneLoader.LoadNo6Scene();
            });
            Debug.Log("NoButton 6 事件已绑定");
        }
        else
        {
            Debug.LogWarning("GameDirectoryButtonBinder: NoButton 6 未找到！");
        }
    }
}





