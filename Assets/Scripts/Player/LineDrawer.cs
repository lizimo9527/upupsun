using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace UpupSun.Player
{
    public class LineDrawer : MonoBehaviour
    {
        [Header("画线设置")]
        public LineRenderer lineRenderer;
        public float lineWidth = 0.1f;
        public Material lineMaterial;
        public Color lineColor = Color.white;
        
        [Header("物理参数")]
        public float minDistanceBetweenPoints = 0.1f;
        public LayerMask drawingLayer;
        
        private List<Vector3> linePoints = new List<Vector3>();
        private bool isDrawing = false;
        private Camera mainCamera;
        
        private void Start()
        {
            mainCamera = Camera.main;
            SetupLineRenderer();
        }
        
        private void SetupLineRenderer()
        {
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            }
            
            lineRenderer.material = lineMaterial ? lineMaterial : CreateDefaultMaterial();
            lineRenderer.color = lineColor;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.positionCount = 0;
            lineRenderer.useWorldSpace = true;
        }
        
        private Material CreateDefaultMaterial()
        {
            Material mat = new Material(Shader.Find("Sprites/Default"));
            mat.color = lineColor;
            return mat;
        }
        
        private void Update()
        {
            HandleInput();
        }
        
        private void HandleInput()
        {
#if UNITY_EDITOR
            HandleMouseInput();
#elif UNITY_ANDROID || UNITY_IOS
            HandleTouchInput();
#endif
        }
        
        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartDrawing();
            }
            else if (Input.GetMouseButton(0) && isDrawing)
            {
                ContinueDrawing();
            }
            else if (Input.GetMouseButtonUp(0) && isDrawing)
            {
                StopDrawing();
            }
        }
        
        private void HandleTouchInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                
                if (touch.phase == TouchPhase.Began)
                {
                    StartDrawing();
                }
                else if (touch.phase == TouchPhase.Moved && isDrawing)
                {
                    ContinueDrawing();
                }
                else if (touch.phase == TouchPhase.Ended && isDrawing)
                {
                    StopDrawing();
                }
            }
        }
        
        private void StartDrawing()
        {
            isDrawing = true;
            linePoints.Clear();
            
            Vector3 startPos = GetWorldPosition();
            if (IsValidDrawingPosition(startPos))
            {
                linePoints.Add(startPos);
                UpdateLineRenderer();
            }
        }
        
        private void ContinueDrawing()
        {
            Vector3 currentPos = GetWorldPosition();
            if (IsValidDrawingPosition(currentPos))
            {
                if (linePoints.Count == 0 || Vector3.Distance(currentPos, linePoints.Last()) >= minDistanceBetweenPoints)
                {
                    linePoints.Add(currentPos);
                    UpdateLineRenderer();
                }
            }
        }
        
        private void StopDrawing()
        {
            isDrawing = false;
            OnLineComplete();
        }
        
        private Vector3 GetWorldPosition()
        {
            Vector3 screenPos = Input.mousePosition;
            screenPos.z = 10f; // 距离摄像机的距离
            return mainCamera.ScreenToWorldPoint(screenPos);
        }
        
        private bool IsValidDrawingPosition(Vector3 position)
        {
            // 可以在这里添加碰撞检测或其他验证逻辑
            return true;
        }
        
        private void UpdateLineRenderer()
        {
            lineRenderer.positionCount = linePoints.Count;
            lineRenderer.SetPositions(linePoints.ToArray());
        }
        
        private void OnLineComplete()
        {
            Debug.Log($"画线完成，共 {linePoints.Count} 个点");
            // 这里可以触发光线跟随逻辑
        }
        
        public List<Vector3> GetLinePoints()
        {
            return new List<Vector3>(linePoints);
        }
        
        public void ClearLine()
        {
            linePoints.Clear();
            lineRenderer.positionCount = 0;
        }
        
        public bool IsLineDrawn()
        {
            return linePoints.Count > 1;
        }
    }
}