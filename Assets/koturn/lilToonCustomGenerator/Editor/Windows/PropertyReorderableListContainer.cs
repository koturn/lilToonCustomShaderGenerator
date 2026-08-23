using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Koturn.LilToonCustomGenerator.Editor.Enums;
using Koturn.LilToonCustomGenerator.Editor.Internals;


namespace Koturn.LilToonCustomGenerator.Editor.Windows
{
    /// <summary>
    /// <see cref="ReorderableListContainer{T}"/> customized for <see cref="ShaderPropertyDefinition"/>.
    /// </summary>
    [System.Runtime.InteropServices.Guid("686b2be4-289e-ebd4-29f1-5694f379dcbb")]
    public sealed class PropertyReorderableListContainer : ReorderableListContainer<ShaderPropertyDefinition>
    {
        /// <summary>
        /// Width padding.
        /// </summary>
        private const float WidthPadding = 2.0f;
        /// <summary>
        /// Height padding.
        /// </summary>
        private const float HeightPadding = 2.0f;
        /// <summary>
        /// Labels for min/max values of Range property.
        /// </summary>
        private static readonly GUIContent[] _rangeMinMaxLabel =
        {
            new GUIContent("Min", "Minimum value of the range"),
            new GUIContent("Max", "Maximum value of the range")
        };
        /// <summary>
        /// Labels for <see cref="Vector4"/> value input filed.
        /// </summary>
        private static readonly GUIContent[] _defaultVectorLabel =
        {
            new GUIContent("X"),
            new GUIContent("Y"),
            new GUIContent("Z"),
            new GUIContent("W"),
        };
        /// <summary>
        /// Label for Property Name.
        /// </summary>
        private static readonly GUIContent _labelPropertyName = new GUIContent("Property Name");
        /// <summary>
        /// "Default" labels.
        /// </summary>
        private static readonly GUIContent _labelDefaultValue = new GUIContent("Default");
        /// <summary>
        /// Duplicate property name list.
        /// </summary>
        private readonly List<string> _duplicatePropertyNameList = new List<string>();
        /// <summary>
        /// Invalid property name list.
        /// </summary>
        private readonly List<string> _invalidPropertyNameList = new List<string>();
        /// <summary>
        /// List of property names with missing arguments.
        /// </summary>
        private readonly List<string> _missingDrawerArgumentPropertyNameList = new List<string>();
        /// <summary>
        /// List of property names with invalid arguments.
        /// </summary>
        private readonly List<string> _invalidDrawerArgumentPropertyNameList = new List<string>();
        /// <summary>
        /// List of property names which is used in lilToon.
        /// </summary>
        private readonly List<string> _usedInlilToonPropertyNameList = new List<string>();
        /// <summary>
        /// <see cref="ReadOnlyCollection{T}"/> of <see cref="_duplicatePropertyNameList"/>.
        /// </summary>
        private readonly ReadOnlyCollection<string> _duplicatePropertyNameCollection;
        /// <summary>
        /// <see cref="ReadOnlyCollection{T}"/> of <see cref="_invalidPropertyNameList"/>.
        /// </summary>
        private readonly ReadOnlyCollection<string> _invalidPropertyNameCollection;
        /// <summary>
        /// <see cref="ReadOnlyCollection{T}"/> of <see cref="_missingDrawerArgumentPropertyNameList"/>.
        /// </summary>
        private readonly ReadOnlyCollection<string> _missingDrawerArgumentPropertyCollection;
        /// <summary>
        /// <see cref="ReadOnlyCollection{T}"/> of <see cref="_invalidDrawerArgumentPropertyNameList"/>.
        /// </summary>
        private readonly ReadOnlyCollection<string> _invalidDrawerArgumentPropertyNameCollection;
        /// <summary>
        /// <see cref="ReadOnlyCollection{T}"/> of <see cref="_usedInlilToonPropertyNameList"/>.
        /// </summary>
        private readonly ReadOnlyCollection<string> _usedInlilToonPropertyNameCollection;


        /// <summary>
        /// Cached array of min/max values of the range property.
        /// </summary>
        private readonly float[] _rangeMinMaxArray = new float[2];
        /// <summary>
        /// Cached array of min/max values of the int range property.
        /// </summary>
        private readonly int[] _rangeIntMinMaxArray = new int[2];
        /// <summary>
        /// Cache array of default vector components.
        /// </summary>
        private readonly float[] _defaultVectorArray = new float[4];


        /// <summary>
        /// Hidden ctor.
        /// </summary>
        [Obsolete("Should not be instanciated directly, Use ScriptableObject.CreateInstance()")]
        private PropertyReorderableListContainer()
        {
            _duplicatePropertyNameCollection = _duplicatePropertyNameList.AsReadOnly();
            _invalidPropertyNameCollection = _invalidPropertyNameList.AsReadOnly();
            _missingDrawerArgumentPropertyCollection = _missingDrawerArgumentPropertyNameList.AsReadOnly();
            _invalidDrawerArgumentPropertyNameCollection = _invalidDrawerArgumentPropertyNameList.AsReadOnly();
            _usedInlilToonPropertyNameCollection = _usedInlilToonPropertyNameList.AsReadOnly();
        }


        /// <summary>
        /// Get duplicate property names.
        /// </summary>
        /// <returns><see cref="ReadOnlyCollection{T}"/> of duplicate property names.</returns>
        public ReadOnlyCollection<string> GetDuplicatePropertyNames()
        {
            var dupNameList = _duplicatePropertyNameList;
            dupNameList.Clear();

            var set = new HashSet<string>();
            foreach (var item in List)
            {
                if (set.Contains(item.Name))
                {
                    dupNameList.Add(item.Name);
                }
                else
                {
                    set.Add(item.Name);
                }
            }

            return _duplicatePropertyNameCollection;
        }

        /// <summary>
        /// Get invalid property names.
        /// </summary>
        /// <returns><see cref="ReadOnlyCollection{T}"/> of invalid property names.</returns>
        public ReadOnlyCollection<string> GetInvalidPropertyNames()
        {
            var invalidNameList = _invalidPropertyNameList;
            invalidNameList.Clear();

            foreach (var item in List)
            {
                if (!RegexProvider.IdentifierRegex.IsMatch(item.Name))
                {
                    invalidNameList.Add(item.Name);
                }
            }

            return _invalidPropertyNameCollection;
        }

        /// <summary>
        /// Get list of property names with missing arguments.
        /// </summary>
        /// <returns><see cref="ReadOnlyCollection{T}"/> of property names with invalid arguments.</returns>
        public ReadOnlyCollection<string> GetMissingDrawerArgumentPropertyNames()
        {
            var missingDrawerArgumentPropertyNameList = _missingDrawerArgumentPropertyNameList;
            missingDrawerArgumentPropertyNameList.Clear();

            foreach (var item in List)
            {
                if (item.DrawerArgumentType == ArgumentType.Required && string.IsNullOrEmpty(item.DrawerArgument))
                {
                    missingDrawerArgumentPropertyNameList.Add(item.Name);
                }
            }

            return _missingDrawerArgumentPropertyCollection;
        }

        /// <summary>
        /// Get list of property names with invalid arguments.
        /// </summary>
        /// <returns><see cref="ReadOnlyCollection{T}"/> of property names with invalid arguments.</returns>
        public ReadOnlyCollection<string> GetInvalidDrawerArgumentPropertyNames()
        {
            var invalidDrawerArgumentPropertyNameList = _invalidDrawerArgumentPropertyNameList;
            invalidDrawerArgumentPropertyNameList.Clear();

            foreach (var item in List)
            {
                if (item.DrawerArgument.Contains('"'))
                {
                    invalidDrawerArgumentPropertyNameList.Add(item.Name);
                }
            }

            return _invalidDrawerArgumentPropertyNameCollection;
        }

        /// <summary>
        /// Get list of property names which is used in lilToon.
        /// </summary>
        /// <returns><see cref="ReadOnlyCollection{T}"/> of property names with invalid arguments.</returns>
        public ReadOnlyCollection<string> GetNamesUsedInLilToon()
        {
            var usedInlilToonPropertyNameList = _usedInlilToonPropertyNameList;
            usedInlilToonPropertyNameList.Clear();

            var nameSet = ShaderPropertyDefinition.LilToonPropertyNameSet;
            foreach (var item in List)
            {
                if (nameSet.Contains(item.Name))
                {
                    usedInlilToonPropertyNameList.Add(item.Name);
                }
            }

            return _usedInlilToonPropertyNameCollection;
        }


        /// <inheritdoc/>
        protected override ReorderableList CreateReorderableList(SerializedObject serializedObject, SerializedProperty serializedProperty)
        {
            return new ReorderableList(serializedObject, serializedProperty, true, true, true, true);
        }


        /// <summary>
        /// Initialize <see cref="ReorderableList"/> instance.
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
        /// <param name="rect"></param>
        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Shader Property Definitions");
        }

        /// <summary>
        /// <para>Callback method for <see cref="ReorderableList.elementHeight"/>.</para>
        /// <para>Returns height of the element of the specified index.</para>
        /// </summary>
        /// <param name="index">Element index. (unused)</param>
        /// <returns>Height of the element of the specified index.</returns>
        private float GetElementHeight(int index)
        {
            return (EditorGUIUtility.singleLineHeight + HeightPadding) * 3.0f;
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

            var line = EditorGUIUtility.singleLineHeight;

            //
            // First line.
            //
            rect.y += HeightPadding;

            var row1 = new Rect(rect.x, rect.y, rect.width, line);
            var nameWidth = row1.width * 0.3f;
            var descWidth = row1.width * 0.7f;

            EditorGUI.PropertyField(
                new Rect(row1.x, row1.y, nameWidth - WidthPadding, line),
                element.FindPropertyRelative(ShaderPropertyDefinition.NameOfName),
                _labelPropertyName);

            var propDescription = element.FindPropertyRelative(ShaderPropertyDefinition.NameOfDescription);
            EditorGUI.PropertyField(
                new Rect(row1.x + nameWidth, row1.y, descWidth, line),
                propDescription);

            //
            // Second line.
            //
            rect.y += line + HeightPadding;
            var row2 = new Rect(rect.x, rect.y, rect.width, line);

            var col1 = row2.width * 0.4f;
            var col2 = row2.width * 0.2f;
            var col3 = row2.width * 0.4f;

            var propPropertyType = element.FindPropertyRelative(ShaderPropertyDefinition.NameOfPropertyType);
            var propUniformType = element.FindPropertyRelative(ShaderPropertyDefinition.NameOfUniformType);
            var propDrawerType = element.FindPropertyRelative(ShaderPropertyDefinition.NameOfDrawerType);
            using (var ccScope = new EditorGUI.ChangeCheckScope())
            {
                if ((ShaderPropertyType)propPropertyType.intValue == ShaderPropertyType.Range)
                {
                    propPropertyType.intValue = EditorGUI.Popup(
                        new Rect(row2.x, row2.y, col1 * 0.5f - WidthPadding, line),
                        "Property type",
                        propPropertyType.intValue,
                        ShaderPropertyDefinition.PropertyTypeSelections);
                }
                else
                {
                    propPropertyType.intValue = EditorGUI.Popup(
                        new Rect(row2.x, row2.y, col1 - WidthPadding, line),
                        "Property type",
                        propPropertyType.intValue,
                        ShaderPropertyDefinition.PropertyTypeSelections);
                }

                if (ccScope.changed)
                {
                    switch ((ShaderPropertyType)propPropertyType.intValue)
                    {
                        case ShaderPropertyType.Float:
                        case ShaderPropertyType.Range:
                            propUniformType.intValue = (int)ShaderVariableType.Float;
                            break;
                        case ShaderPropertyType.Int:
                            propUniformType.intValue = (int)ShaderVariableType.Int;
                            break;
                        case ShaderPropertyType.Vector:
                        case ShaderPropertyType.Color:
                            propUniformType.intValue = (int)ShaderVariableType.Float4;
                            break;
                        case ShaderPropertyType.Texture2D:
                            propUniformType.intValue = (int)ShaderVariableType.Texture2D;
                            break;
                        case ShaderPropertyType.Texture3D:
                            propUniformType.intValue = (int)ShaderVariableType.Texture3D;
                            break;
                        case ShaderPropertyType.TextureCube:
                            propUniformType.intValue = (int)ShaderVariableType.TextureCube;
                            break;
                    }
                    propDrawerType.intValue = (int)DrawerType.None;
                }
            }

            if ((ShaderPropertyType)propPropertyType.intValue == ShaderPropertyType.Range)
            {
                propPropertyType.intValue = EditorGUI.Popup(
                    new Rect(row2.x, row2.y, col1 * 0.5f - WidthPadding, line),
                    "Variable type",
                    propPropertyType.intValue,
                    ShaderPropertyDefinition.PropertyTypeSelections);
                var propRangeMinMax = element.FindPropertyRelative(ShaderPropertyDefinition.NameOfRangeMinMax);
                var rangeMinMax = propRangeMinMax.vector2Value;

                if ((DrawerType)propDrawerType.intValue == DrawerType.IntRange)
                {
                    var rangeIntMinMaxArray = _rangeIntMinMaxArray;
                    rangeIntMinMaxArray[0] = (int)rangeMinMax.x;
                    rangeIntMinMaxArray[1] = (int)rangeMinMax.y;
                    EditorGUI.MultiIntField(
                        new Rect(row2.x + col1 * 0.5f, row2.y, col1 * 0.5f - WidthPadding, line),
                        _rangeMinMaxLabel,
                        rangeIntMinMaxArray);
                    propRangeMinMax.vector2Value = new Vector2(rangeIntMinMaxArray[0], rangeIntMinMaxArray[1]);
                }
                else
                {
                    var rangeMinMaxArray = _rangeMinMaxArray;
                    rangeMinMaxArray[0] = rangeMinMax.x;
                    rangeMinMaxArray[1] = rangeMinMax.y;
                    EditorGUI.MultiFloatField(
                        new Rect(row2.x + col1 * 0.5f, row2.y, col1 * 0.5f - WidthPadding, line),
                        _rangeMinMaxLabel,
                        rangeMinMaxArray);
                    propRangeMinMax.vector2Value = new Vector2(rangeMinMaxArray[0], rangeMinMaxArray[1]);
                }
            }

            var availableTypeNames = ShaderPropertyDefinition.GetSuitableVariableTypeNames((ShaderPropertyType)propPropertyType.intValue);
            var availableTypeIndex = EditorGUI.Popup(
                new Rect(row2.x + col1, row2.y, col2 - WidthPadding, line),
                "Variable type",
                Array.IndexOf(availableTypeNames, ShaderPropertyDefinition.VariableTypeSelections[propUniformType.intValue]),
                availableTypeNames);
            propUniformType.intValue = Array.IndexOf(ShaderPropertyDefinition.VariableTypeSelections, availableTypeNames[availableTypeIndex]);

            var rectDefaultValue = new Rect(row2.x + col1 + col2, row2.y, col3, line);
            switch ((ShaderPropertyType)propPropertyType.intValue)
            {
                case ShaderPropertyType.Float:
                case ShaderPropertyType.Range:
                    EditorGUI.PropertyField(
                        rectDefaultValue,
                        element.FindPropertyRelative(ShaderPropertyDefinition.NameOfDefaultFloat),
                        _labelDefaultValue);
                    break;
                case ShaderPropertyType.Int:
                    EditorGUI.PropertyField(
                        rectDefaultValue,
                        element.FindPropertyRelative(ShaderPropertyDefinition.NameOfDefaultInt));
                    break;
                case ShaderPropertyType.Vector:
                    var propDefaultVector = element.FindPropertyRelative(ShaderPropertyDefinition.NameOfDefaultVector);
                    var defaultVector = propDefaultVector.vector4Value;
                    var defaultVectorArray = _defaultVectorArray;

                    defaultVectorArray[0] = defaultVector.x;
                    defaultVectorArray[1] = defaultVector.y;
                    defaultVectorArray[2] = defaultVector.z;
                    defaultVectorArray[3] = defaultVector.w;

                    EditorGUI.MultiFloatField(
                        rectDefaultValue,
                        _defaultVectorLabel,
                        defaultVectorArray);

                    propDefaultVector.vector4Value = new Vector4(defaultVectorArray[0], defaultVectorArray[1], defaultVectorArray[2], defaultVectorArray[3]);
                    break;
                case ShaderPropertyType.Color:
                    EditorGUI.PropertyField(
                        rectDefaultValue,
                        element.FindPropertyRelative(ShaderPropertyDefinition.NameOfDefaultColor));
                    break;
                case ShaderPropertyType.Texture2D:
                case ShaderPropertyType.Texture3D:
                case ShaderPropertyType.TextureCube:
                    var propDefaultTextureIndex = element.FindPropertyRelative(ShaderPropertyDefinition.NameOfDefaultTextureIndex);
                    propDefaultTextureIndex.intValue = EditorGUI.Popup(
                        rectDefaultValue,
                        "Default",
                        propDefaultTextureIndex.intValue,
                        ShaderPropertyDefinition.DefaultTextureNames);
                    break;
            }

            //
            // Third line.
            //
            rect.y += line + HeightPadding;
            var row3 = new Rect(rect.x, rect.y, rect.width, line);

            col1 = row3.width * 0.3f;
            col2 = row3.width * 0.7f;

            using (var ccScope = new EditorGUI.ChangeCheckScope())
            {
                var drawerSelections = ShaderPropertyDefinition.GetSuitableDrawerSelections((ShaderPropertyType)propPropertyType.intValue);
                var drawerIndex = EditorGUI.Popup(
                    new Rect(row3.x, row3.y, col1 - WidthPadding, line),
                    "Drawer",
                    Array.IndexOf(drawerSelections, ShaderPropertyDefinition.AllDrawerSelections[propDrawerType.intValue]),
                    drawerSelections);
                propDrawerType.intValue = Array.IndexOf(ShaderPropertyDefinition.AllDrawerSelections, drawerSelections[drawerIndex]);

                if (ccScope.changed && propDescription.stringValue.Length == 0)
                {
                    propDescription.stringValue = ShaderPropertyDefinition.GetDefaultDescription((DrawerType)propDrawerType.intValue);
                }
            }

            if (ShaderPropertyDefinition.GetDrawerArgumentType((DrawerType)propDrawerType.intValue) != ArgumentType.NotRequired)
            {
                EditorGUI.PropertyField(
                    new Rect(row3.x + col1, row3.y, col2, line),
                    element.FindPropertyRelative(ShaderPropertyDefinition.NameOfDrawerArgument));
            }
        }

        /// <summary>
        /// <para>Callback method for <see cref="ReorderableList.onAddCallback"/>.</para>
        /// <para>Add new item to <see cref="_shaderPropDefList"/>.</para>
        /// </summary>
        /// <param name="reorderableList">Source <see cref="ReorderableList"/>. (Unused)</param>
        private void OnAdd(ReorderableList reorderableList)
        {
            var propName = "_CustomProperty";
            for (int i = 1; i < 256; i++)
            {
                var isFound = false;
                foreach (var shaderProp in List)
                {
                    if (shaderProp.Name == propName)
                    {
                        isFound = true;
                        break;
                    }
                }
                if (!isFound)
                {
                    break;
                }
                propName = "_CustomProperty" + i;
            }
            List.Add(new ShaderPropertyDefinition(
                propName,
                string.Empty,
                ShaderPropertyType.Float,
                ShaderVariableType.Float));
        }
    }
}
