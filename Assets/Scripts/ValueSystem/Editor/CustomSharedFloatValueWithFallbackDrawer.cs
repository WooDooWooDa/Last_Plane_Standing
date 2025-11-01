using UnityEditor;
using UnityEngine;

namespace ValueSystem.Editor
{

    [CustomPropertyDrawer(typeof(FloatSharedValueWithFallback))]
    public class SharedValueWithFallbackDrawer : PropertyDrawer
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

            var fallbackProp = property.FindPropertyRelative("_fallbackValue");
            var valueProp = property.FindPropertyRelative("_value");

            var valueSO = valueProp.objectReferenceValue as ScriptableObject;
            var totalWidth = position.width;

            // Label
            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, label);

            // Layout splits
            totalWidth -= EditorGUIUtility.labelWidth;
            var valueWidth = totalWidth * 0.35f;
            var fieldWidth = totalWidth - valueWidth;

            var valueRect = new Rect(labelRect.xMax + 2f, position.y, valueWidth, EditorGUIUtility.singleLineHeight);
            var fieldRect = new Rect(valueRect.xMax + 5f, position.y, fieldWidth, EditorGUIUtility.singleLineHeight);

            // SharedValue assigned
            if (valueSO != null)
            {
                var type = valueSO.GetType();
                var getMethod = type.GetMethod("GetBase");
                string valueText = "";

                if (getMethod != null)
                {
                    try
                    {
                        object currentValue = getMethod.Invoke(valueSO, null);
                        valueText = currentValue != null ? currentValue.ToString() : "null";
                        valueText = valueText.Replace(',', '.');
                    }
                    catch
                    {
                        Debug.LogError("Error calling GetBase() on " + valueSO);
                    }
                }

                var tempSo = new SerializedObject(valueSO);
                var valueField = tempSo.FindProperty("value");
                var allowValueEditing = tempSo.FindProperty("allowEditAnywhere").boolValue;
                
                GUI.enabled = allowValueEditing;
                var newValue = EditorGUI.TextField(valueRect, valueText, EditorStyles.textField);
                GUI.enabled = true;
                if (allowValueEditing && newValue != valueText && valueField != null)
                {
                    try
                    {
                        var fieldType = valueField.propertyType;

                        Undo.RecordObject(valueSO, "Edit ValueSO Base");
                        if (fieldType == SerializedPropertyType.Integer)
                            valueField.intValue = int.Parse(newValue);
                        else if (fieldType == SerializedPropertyType.Float)
                            valueField.floatValue =
                                float.Parse(newValue, System.Globalization.CultureInfo.InvariantCulture);
                        else
                            Debug.LogWarning($"Unsupported type {fieldType} in Value<> drawer.");

                        tempSo.ApplyModifiedProperties();
                        EditorUtility.SetDirty(valueSO);
                    }
                    catch
                    {
                        tempSo.Dispose();
                        Debug.LogError("Error setting base value on " + valueSO);
                    }
                }
            }
            else
            {
                // Fallback active
                EditorGUI.PropertyField(valueRect, fallbackProp, GUIContent.none);
            }
            
            valueProp.objectReferenceValue = EditorGUI.ObjectField(fieldRect, valueSO, typeof(FloatSharedValue), false);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUIUtility.singleLineHeight + 2f;
    }
}