#if UNITY_EDITOR
using System;
using System.Reflection;
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

            var valueSO = property.objectReferenceValue as ScriptableObject;
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
            var valueText = "";
            if (valueSO != null)
            {
                var type = valueSO.GetType();
                var getMethod = type.GetMethod("Get");

                if (getMethod != null)
                {
                    try
                    {
                        object currentValue = getMethod.Invoke(valueSO, null);
                        valueText = currentValue != null ? currentValue.ToString() : "null";
                        valueText = valueText.Replace(',', '.');
                    }
                    catch { Debug.LogError("Error calling \"Get()\" on " + valueSO); }
                }
            
                var newValue = EditorGUI.TextField(valueRect, valueText, EditorStyles.textField);

                var tempSo = new SerializedObject(valueSO);
                var valueField = tempSo.FindProperty("value");
                if (newValue != valueText && valueField != null)
                {
                    try
                    {
                        var fieldType = valueField.propertyType;

                        Undo.RecordObject(valueSO, "Edit ValueSO Base");
                        if (fieldType == SerializedPropertyType.Integer)
                            valueField.intValue = int.Parse(newValue);
                        else if (fieldType == SerializedPropertyType.Float)
                            valueField.floatValue = float.Parse(newValue, System.Globalization.CultureInfo.InvariantCulture);
                        else
                            Debug.LogWarning($"Unsupported type {fieldType} in Value<> drawer.");
                            
                        tempSo.ApplyModifiedProperties();
                        EditorUtility.SetDirty(valueSO);
                    }
                    catch { Debug.LogError("Error setting base value on " + valueSO); }
                }
            }
            else
            {
                //No assigned SO
                GUI.enabled = false;
                EditorGUI.TextField(valueRect, "No SO");
                GUI.enabled = true;
            }

            // Draw the object reference field
            property.objectReferenceValue = EditorGUI.ObjectField(fieldRect, valueSO, fieldInfo.FieldType, false);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUIUtility.singleLineHeight + 2f;
    }
}
#endif