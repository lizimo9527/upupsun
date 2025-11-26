using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(Slider))]
[CanEditMultipleObjects]
public class SliderFillEditor : Editor
{
    private SerializedProperty valueProperty;
    private SerializedProperty minValueProperty;
    private SerializedProperty maxValueProperty;
    private SerializedProperty fillRectProperty;
    private SerializedProperty directionProperty;
    
    private Slider slider;
    private RectTransform fillRect;
    private float lastValue = float.MinValue;

    private void OnEnable()
    {
        slider = (Slider)target;
        valueProperty = serializedObject.FindProperty("m_Value");
        minValueProperty = serializedObject.FindProperty("m_MinValue");
        maxValueProperty = serializedObject.FindProperty("m_MaxValue");
        fillRectProperty = serializedObject.FindProperty("m_FillRect");
        directionProperty = serializedObject.FindProperty("m_Direction");
        
        lastValue = slider.value;
        
        // 订阅编辑器更新事件
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnEditorUpdate()
    {
        if (slider == null || slider.fillRect == null)
            return;

        // 检查值是否改变
        if (Mathf.Abs(slider.value - lastValue) > 0.0001f)
        {
            lastValue = slider.value;
            UpdateFillWidth();
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 绘制默认的Inspector
        DrawDefaultInspector();

        // 如果值改变，更新Fill
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(valueProperty);
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            UpdateFillWidth();
            lastValue = slider.value;
            SceneView.RepaintAll();
        }

        // 添加一个按钮来手动更新Fill
        if (GUILayout.Button("更新Fill宽度"))
        {
            UpdateFillWidth();
            SceneView.RepaintAll();
        }
    }

    private void UpdateFillWidth()
    {
        if (slider == null || slider.fillRect == null)
            return;

        fillRect = slider.fillRect;
        RectTransform fillArea = fillRect.parent as RectTransform;
        
        if (fillArea == null)
            return;

        // 计算归一化值 (0-1)
        float normalizedValue = Mathf.InverseLerp(slider.minValue, slider.maxValue, slider.value);
        normalizedValue = Mathf.Clamp01(normalizedValue);
        
        // 获取Fill Area的宽度
        float fillAreaWidth = fillArea.rect.width;
        
        // 根据Slider方向计算Fill宽度
        // Direction: 0 = LeftToRight, 1 = RightToLeft, 2 = BottomToTop, 3 = TopToBottom
        float fillWidth = 0f;
        float fillHeight = 0f;
        
        if (slider.direction == Slider.Direction.LeftToRight || slider.direction == Slider.Direction.RightToLeft)
        {
            fillWidth = fillAreaWidth * normalizedValue;
            fillHeight = fillRect.sizeDelta.y;
        }
        else
        {
            fillWidth = fillRect.sizeDelta.x;
            fillHeight = fillArea.rect.height * normalizedValue;
        }
        
        // 更新Fill的SizeDelta
        Undo.RecordObject(fillRect, "Update Slider Fill Width");
        fillRect.sizeDelta = new Vector2(fillWidth, fillHeight);
        
        // 标记场景为已修改
        if (!Application.isPlaying)
        {
            EditorUtility.SetDirty(fillRect);
            EditorUtility.SetDirty(slider);
        }
    }
}

