using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


namespace Koturn.LilToonCustomGenerator.Editor.Windows
{
    /// <summary>
    /// <see cref="ReorderableList"/> container.
    /// </summary>
    [System.Runtime.InteropServices.Guid("f911ca1a-7d7f-96b4-fba6-14601dcd06f6")]
    [Serializable]
    public abstract class ReorderableListContainer<T> : ScriptableObject
    {
        /// <summary>
        /// Offset width per indent level.
        /// </summary>
        public const float IndentOffset = 15.0f;

        /// <summary>
        /// Actual list.
        /// </summary>
        public List<T> List => _list;

        /// <summary>
        /// Actual list.
        /// </summary>
        [SerializeField]
        private List<T> _list = new List<T>();
        /// <summary>
        /// <see cref="ReorderableList"/> instance.
        /// </summary>
        private ReorderableList _reorderableList;
        /// <summary>
        /// <see cref="SerializedObject"/> of this instance.
        /// </summary>
        private SerializedObject _serializedObject;


        /// <summary>
        /// Draw <see cref="ReorderableList"/> and update <see cref="SerializedProperty"/> instance.
        /// </summary>
        public void Draw()
        {
            _serializedObject.Update();

            var rect = EditorGUILayout.GetControlRect(false, _reorderableList.GetHeight());
            var offset = IndentOffset * EditorGUI.indentLevel;
            rect.x += offset;
            rect.width -= offset;
            _reorderableList.DoList(rect);

            _serializedObject.ApplyModifiedProperties();
        }


        /// <summary>
        /// Create <see cref="ReorderableList"/> instance.
        /// </summary>
        /// <param name="serializedObject"><see cref="SerializedObject"/> instance.</param>
        /// <param name="serializedProperty"><see cref="SerializedProperty"/> instance.</param>
        /// <returns>Createed <see cref="ReorderableList"/> instance.</returns>
        protected abstract ReorderableList CreateReorderableList(SerializedObject serializedObject, SerializedProperty serializedProperty);

        /// <summary>
        /// <para>Get self <see cref="ReorderableList"/> instance.</para>
        /// <para>If the instance is not created, create and return it.</para>
        /// </summary>
        /// <returns></returns>
        protected ReorderableList GetReorderableList()
        {
            if (_reorderableList == null)
            {
                var serializedObject = new SerializedObject(this);
                var serializedProperty = serializedObject.FindProperty(nameof(_list));
                _reorderableList = CreateReorderableList(serializedObject, serializedProperty);
                _serializedObject = serializedObject;
            }

            return _reorderableList;
        }
    }
}

