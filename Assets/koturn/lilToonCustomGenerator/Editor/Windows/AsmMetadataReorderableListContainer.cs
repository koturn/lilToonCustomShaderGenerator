using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


namespace Koturn.LilToonCustomGenerator.Editor.Windows
{
    /// <summary>
    /// <see cref="ReorderableListContainer{T}"/> customized for <see cref="KVPair{TKey, TValue}"/>.
    /// </summary>
    [System.Runtime.InteropServices.Guid("9dc1f4b2-9b8e-d9e4-a9e1-e25c1876320b")]
    public sealed class AsmMetadataReorderableListContainer : ReorderableListContainer<KVPair<string, string>>
    {
        /// <summary>
        /// Width margin.
        /// </summary>
        private const float WidthPadding = 2.0f;
        /// <summary>
        /// Height padding.
        /// </summary>
        private const float HeightPadding = 2.0f;


        /// <inheritdoc/>
        protected override ReorderableList CreateReorderableList(SerializedObject serializedObject, SerializedProperty serializedProperty)
        {
            return new ReorderableList(serializedObject, serializedProperty, true, true, true, true);
        }


        /// <summary>
        /// Hidden ctor.
        /// </summary>
        [Obsolete("Should not be instanciated directly, Use ScriptableObject.CreateInstance()")]
        private AsmMetadataReorderableListContainer()
        {
        }


        /// <summary>
        /// Create and initialize <see cref="ReorderableList"/> instance.
        /// </summary>
        private void OnEnable()
        {
            var reorderableList = GetReorderableList();
            reorderableList.drawHeaderCallback = DrawHeader;
            reorderableList.elementHeightCallback = GetElementHeight;
            reorderableList.drawElementCallback = DrawElement;
            reorderableList.onAddCallback = OnAdd;
        }

        /// <summary>
        /// <para>Callback method for <see cref="ReorderableList.drawHeaderCallback"/>.</para>
        /// <para>Draw header of this <see cref="ReorderableList"/>.</para>
        /// </summary>
        /// <param name="rect">Header region.</param>
        private void DrawHeader(Rect rect)
        {
            rect.x -= IndentOffset * EditorGUI.indentLevel;
            rect.width -= IndentOffset * EditorGUI.indentLevel;
            EditorGUI.LabelField(rect, "Metadata");
        }

        /// <summary>
        /// <para>Callback method for <see cref="ReorderableList.elementHeight"/>.</para>
        /// <para>Returns height of the element of the specified index.</para>
        /// </summary>
        /// <param name="index">Element index. (unused)</param>
        /// <returns>Height of the element of the specified index.</returns>
        private float GetElementHeight(int index)
        {
            return EditorGUIUtility.singleLineHeight + HeightPadding;
        }

        /// <summary>
        /// <para>Callback method for <see cref="ReorderableList.drawElementCallback"/>.</para>
        /// <para>Draw single element.</para>
        /// </summary>
        /// <param name="rect">Draw target <see cref="Rect"/>.</param>
        /// <param name="index">Element index.</param>
        /// <param name="isActive">True if the element is active, otherwise false.</param>
        /// <param name="isFocused">True if the element is focused, otherwise false.</param>
        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = GetReorderableList().serializedProperty.GetArrayElementAtIndex(index);

            //
            // First line.
            //
            rect.y += HeightPadding;

            var row = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            var keyWidth = row.width * 0.5f;
            var valueWidth = row.width * 0.5f;

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y, rect.width - keyWidth - WidthPadding, row.height),
                element.FindPropertyRelative(KVPair.NameOfKey),
                new GUIContent("Key"));

            EditorGUI.PropertyField(
                new Rect(rect.x + keyWidth, rect.y, rect.width - keyWidth, row.height),
                element.FindPropertyRelative(KVPair.NameOfValue),
                new GUIContent("Value"));
        }

        /// <summary>
        /// <para>Callback method for <see cref="ReorderableList.onAddCallback"/>.</para>
        /// <para>Add new item to <see cref="_stringList"/>.</para>
        /// </summary>
        /// <param name="reorderableList">Source <see cref="ReorderableList"/>. (Unused)</param>
        private void OnAdd(ReorderableList reorderableList)
        {
            List.Add(KVPair.Create("Key", "Value"));
        }
    }
}
