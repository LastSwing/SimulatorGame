 using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 using UnityEditor;
 using UnityEditor.UI;

[CustomEditor(typeof(UICircle), true)]
[CanEditMultipleObjects]
public class UICircleInspector : RawImageEditor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        UICircle circle = target as UICircle;
        circle.segments = Mathf.Clamp(EditorGUILayout.IntField("UICircle多边形", circle.segments), 3, 360);//设置边数的最小于最大值（3-360）
    }
}