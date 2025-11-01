using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace ValueSystem.Editor
{
    [CustomPropertyDrawer(typeof(RangeSharedValue), true)]
    public class CustomSharedRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Check if this property has the DisableSharedValueDrawer attribute
            if (fieldInfo.GetCustomAttributes(typeof(DisableSharedValueDrawerAttribute), true).Length > 0)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }
            
            EditorGUI.BeginProperty(position, label, property);

            var valueSo = property.objectReferenceValue as ScriptableObject;
            var totalWidth = position.width;

            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);
            
            // Split the field into two: value display (left) and object field (right)
            totalWidth -= EditorGUIUtility.labelWidth;
            var valueWidth = totalWidth * 0.35f;  // 35% for the value, adjust as needed
            var fieldWidth = totalWidth - valueWidth; // gap between

            var rangeLowRect = new Rect(labelRect.xMax + 2f, position.y, valueWidth / 2f, EditorGUIUtility.singleLineHeight);
            var rangeHighRect = new Rect(rangeLowRect.xMax + 5f, position.y, valueWidth / 2f, EditorGUIUtility.singleLineHeight);
            var fieldRect = new Rect(rangeHighRect.xMax + 5f, position.y, fieldWidth, EditorGUIUtility.singleLineHeight);

            // Draw current value if available
            var currentValue = Vector2.zero;
            if (valueSo != null)
            {
                var type = valueSo.GetType();
                var getMethod = type.GetMethod("GetBase");

                if (getMethod != null)
                {
                    try
                    {
                        currentValue = (Vector2)getMethod.Invoke(valueSo, null);
                    }
                    catch { Debug.LogError("Error calling \"GetBase()\" on " + valueSo); }
                }

                // Edit current value if allowed
                var tempSo = new SerializedObject(valueSo);
                var allowValueEditing = tempSo.FindProperty("allowEditAnywhere").boolValue;
            
                GUI.enabled = allowValueEditing;                
                var newLowValue = EditorGUI.FloatField(rangeLowRect, currentValue.x, EditorStyles.textField);
                var newHighValue = EditorGUI.FloatField(rangeHighRect, currentValue.y, EditorStyles.textField);
                GUI.enabled = true;
                if (allowValueEditing && (!Mathf.Approximately(newLowValue, currentValue.x) || !Mathf.Approximately(newHighValue, currentValue.y)))
                {
                    try
                    {
                        Undo.RecordObject(valueSo, "Edit ValueSO Base");
                        tempSo.FindProperty("range").vector2Value = new Vector2(newLowValue, newHighValue);;
                        tempSo.ApplyModifiedProperties();
                        EditorUtility.SetDirty(valueSo);
                    }
                    catch
                    {
                        tempSo.Dispose();
                        Debug.LogError("Error setting base value on " + valueSo.name);
                    }
                }
            }
            else
            {
                //No assigned SO
                var valueRect = new Rect(labelRect.xMax + 2f, position.y, valueWidth, EditorGUIUtility.singleLineHeight);
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