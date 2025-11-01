#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ValueSystem.Editor
{
    [CustomPropertyDrawer(typeof(SharedValue<>), true)]
    public class SharedValueDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var valueSo = property.objectReferenceValue as ScriptableObject;
            var totalWidth = position.width;

            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);
            
            // Split the field into two: value display (left) and object field (right)
            totalWidth -= EditorGUIUtility.labelWidth;
            var valueWidth = totalWidth * 0.35f;  // 35% for the value, adjust as needed
            var fieldWidth = totalWidth - valueWidth; // gap between

            var valueRect = new Rect(labelRect.xMax + 2f, position.y, valueWidth, EditorGUIUtility.singleLineHeight);
            var fieldRect = new Rect(valueRect.xMax + 5f, position.y, fieldWidth, EditorGUIUtility.singleLineHeight);

            // Draw current value if available
            var currentValue = 0f;
            if (valueSo != null)
            {
                var type = valueSo.GetType();
                var getMethod = type.GetMethod("GetBase");

                if (getMethod != null)
                {
                    try
                    {
                        currentValue = (float)getMethod.Invoke(valueSo, null);
                    }
                    catch { Debug.LogError("Error calling \"Get()\" on " + valueSo.name); }
                }
                
                // Edit current value if allowed
                var tempSo = new SerializedObject(valueSo);
                var allowValueEditing = tempSo.FindProperty("allowEditAnywhere").boolValue;
            
                GUI.enabled = allowValueEditing;
                var newValue = EditorGUI.FloatField(valueRect, currentValue, EditorStyles.textField);
                GUI.enabled = true;
                if (allowValueEditing && !Mathf.Approximately(newValue, currentValue))
                {
                    try
                    {
                        Undo.RecordObject(valueSo, "Edit ValueSO Base");
                        tempSo.FindProperty("value").floatValue = newValue;
                        tempSo.ApplyModifiedProperties();
                        EditorUtility.SetDirty(valueSo);
                    }
                    catch
                    {
                        tempSo.Dispose();
                        Debug.LogError("Error setting base value on " + valueSo);
                    }
                }
            }
            else
            {
                //No assigned SO
                GUI.enabled = false;
                GUI.Button(valueRect, "No Value");
                GUI.enabled = true;
            }

            // Draw the object reference field
            property.objectReferenceValue = EditorGUI.ObjectField(fieldRect, valueSo, fieldInfo.FieldType, false);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUIUtility.singleLineHeight + 2f;
    }
}
#endif