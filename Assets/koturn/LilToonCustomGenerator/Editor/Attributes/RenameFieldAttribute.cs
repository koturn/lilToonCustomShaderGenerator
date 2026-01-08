using UnityEditor;
using UnityEngine;


namespace Koturn.LilToonCustomGenerator.Attributes
{
    public class RenameFieldAttribute : PropertyAttribute
    {
        /// <summary>
        /// Name for showing on inspector.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// <para>Ctor</para>
        /// <para>Initialize <see cref="Name"/>.</para>
        /// </summary>
        /// <param name="name">Name for showing on inspector.</param>
        public RenameFieldAttribute(string name) => Name = name;

#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(RenameFieldAttribute))]
        class FieldNameDrawer : PropertyDrawer
        {
            /// <summary>
            /// If <see cref="attribute"/> is <see cref="RenameFieldAttribute"/>, overwrite text of the <paramref name="label"/>.
            /// </summary>
            /// <param name="position">Rectangle on the screen to use for the property GUI.</param>
            /// <param name="property">The SerializedProperty to make the custom GUI for.</param>
            /// <param name="label">The label of this property.</param>
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var path = property.propertyPath.Split('.');
                if (!(path.Length > 1 && path[1] == "Array") && attribute is RenameFieldAttribute fieldName)
                {
                    label.text = fieldName.Name;
                }
                EditorGUI.PropertyField(position, property, label, true);
            }
        }
#endif
    }
}
