using UnityEngine;

namespace UpupSun.Utilities
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }
        
        [Header("输入绑定")]
        public KeyCode drawLineKey = KeyCode.Mouse0;
        public KeyCode startLightKey = KeyCode.Space;
        public KeyCode resetLightKey = KeyCode.R;
        public KeyCode pauseKey = KeyCode.Escape;
        public KeyCode restartKey = KeyCode.F5;
        
        [Header("触摸设置")]
        public float tapThreshold = 0.1f;
        public float swipeThreshold = 50f;
        private Vector2 touchStartPos;
        private float touchStartTime;
        
        public bool IsDrawing { get; private set; }
        public Vector3 WorldPosition { get; private set; }
        
        private Camera mainCamera;
        private bool isTouchDevice;
        
        public System.Action OnDrawStart;
        public System.Action OnDrawEnd;
        public System.Action OnLightStart;
        public System.Action OnLightReset;
        public System.Action OnPause;
        public System.Action OnRestart;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = null;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }
        
        private void Start()
        {
            InitializeInput();
        }
        
        private void InitializeInput()
        {
            mainCamera = Camera.main;
            isTouchDevice = Input.touchSupported;
            
            Debug.Log($"输入系统初始化完成 - 设备类型: {(isTouchDevice ? "触摸设备" : "鼠标键盘设备")}");
        }
        
        private void Update()
        {
            if (isTouchDevice)
            {
                HandleTouchInput();
            }
            else
            {
                HandleMouseInput();
            }
            
            HandleKeyboardInput();
            UpdateWorldPosition();
        }
        
        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                IsDrawing = true;
                OnDrawStart?.Invoke();
            }
            else if (Input.GetMouseButtonUp(0))
            {
                IsDrawing = false;
                OnDrawEnd?.Invoke();
            }
        }
        
        private void HandleTouchInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchStartPos = touch.position;
                        touchStartTime = Time.time;
                        IsDrawing = true;
                        OnDrawStart?.Invoke();
                        break;
                        
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        IsDrawing = false;
                        OnDrawEnd?.Invoke();
                        
                        // 检测手势
                        DetectGesture(touch);
                        break;
                }
            }
        }
        
        private void DetectGesture(Touch touch)
        {
            float touchDuration = Time.time - touchStartTime;
            Vector2 touchEndPos = touch.position;
            Vector2 swipeVector = touchEndPos - touchStartPos;
            float swipeDistance = swipeVector.magnitude;
            
            // 检测点击
            if (touchDuration < tapThreshold && swipeDistance < 10f)
            {
                HandleTap(touch.position);
            }
            // 检测滑动
            else if (swipeDistance > swipeThreshold)
            {
                HandleSwipe(swipeVector);
            }
        }
        
        private void HandleTap(Vector2 position)
        {
            // 点击可能用于开始光线移动
            OnLightStart?.Invoke();
        }
        
        private void HandleSwipe(Vector2 swipeVector)
        {
            // 水平滑动检测
            if (Mathf.Abs(swipeVector.x) > Mathf.Abs(swipeVector.y))
            {
                if (swipeVector.x > 0)
                {
                    // 向右滑动
                    Debug.Log("向右滑动");
                }
                else
                {
                    // 向左滑动
                    Debug.Log("向左滑动");
                }
            }
            // 垂直滑动检测
            else
            {
                if (swipeVector.y > 0)
                {
                    // 向上滑动
                    Debug.Log("向上滑动");
                }
                else
                {
                    // 向下滑动
                    Debug.Log("向下滑动");
                }
            }
        }
        
        private void HandleKeyboardInput()
        {
            if (Input.GetKeyDown(startLightKey))
            {
                OnLightStart?.Invoke();
            }
            
            if (Input.GetKeyDown(resetLightKey))
            {
                OnLightReset?.Invoke();
            }
            
            if (Input.GetKeyDown(pauseKey))
            {
                OnPause?.Invoke();
            }
            
            if (Input.GetKeyDown(restartKey))
            {
                OnRestart?.Invoke();
            }
        }
        
        private void UpdateWorldPosition()
        {
            Vector3 screenPosition = Input.mousePosition;
            
            if (Input.touchCount > 0)
            {
                screenPosition = Input.GetTouch(0).position;
            }
            
            screenPosition.z = 10f; // 设置合适的Z距离
            WorldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        }
        
        public Vector3 GetWorldPosition(Vector2 screenPosition)
        {
            Vector3 worldPos = new Vector3(screenPosition.x, screenPosition.y, 10f);
            return mainCamera.ScreenToWorldPoint(worldPos);
        }
        
        public bool IsPointerOverUI()
        {
#if UNITY_EDITOR
            return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
#else
            if (Input.touchCount > 0)
            {
                return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
            }
            return false;
#endif
        }
        
        public void SetInputActive(bool active)
        {
            enabled = active;
        }
        
        public void VibrateDevice(long milliseconds = 100)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            Handheld.Vibrate();
#endif
        }
    }
}