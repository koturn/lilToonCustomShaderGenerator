using System;
using UnityEditor;
using UnityEngine;


namespace Koturn.LilToonCustomGenerator.Drawers
{
    [CustomPropertyDrawer(typeof(ShaderPropertyDefinition))]
    public class ShaderPropertyDefinitionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Debug.Log("OnGUI");
            // List用に1つのプロパティであることを示すためPropertyScopeで囲む
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                // 0指定だとReorderableListのドラッグと被るのでLineHeightを指定
                position.height = EditorGUIUtility.singleLineHeight;

                var actionTypeRect = new Rect(position)
                {
                    y = position.y
                };

                // using (var hScope = new EditorGUILayout.HorizontalScope(GUI.skin.box))
                // {
                // }

                var propName = property.FindPropertyRelative("Name");
                propName.stringValue = EditorGUILayout.TextField("Property Name", propName.stringValue);
            }
        }

        // public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        // {
        //     var height = EditorGUIUtility.singleLineHeight;

        //     var actionTypeProperty = property.FindPropertyRelative("actionType");
        //     switch ((EventActionType)actionTypeProperty.enumValueIndex) {
        //         case EventActionType.Talk:
        //             height = 130f;
        //             break;
        //         case EventActionType.CharacterAction:
        //             height = 70f;
        //             break;
        //     }

        //     return height;
        // }
    }
}
