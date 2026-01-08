using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Koturn.LilToonCustomGenerator.Json;


namespace Koturn.LilToonCustomGenerator.Windows
{
    public class LilToonCustomGeneratorWindow : EditorWindow
    {
        private static readonly string[] _newLineSelections = {
            "LF",
            "CR",
            "CR + LF"
        };

        /// <summary>
        /// <see cref="SerializedObject"/> of this instance.
        /// </summary>
        private SerializedObject _serializedObject;

        private string _shaderName = "Test";
        private string _namespace = "lilToonCustom.Editor";
        private string _inspectorName = "CustomInspector";
        private NewLineType _newLineType;
        [SerializeField]
        private List<ShaderPropertyDefinition> _shaderPropDefList = new List<ShaderPropertyDefinition>();

        private ReorderableList _reorderableList;

        private bool _shouldGenerateVersionDetectionHeader = false;
        private bool _shouldGenerateLangTsv = true;
        private bool _shouldGenerateConvertMenu = true;
        private bool _shouldGenerateCacheClearMenu = true;
        private bool _shouldGenerateAssemblyInfo = true;

        private string _assemblyTitle;

        private string _lastExportDirectoryPath;

        private void OnEnable()
        {
            _serializedObject = new SerializedObject(this);
            _reorderableList = CreateReorderableList(_serializedObject, _serializedObject.FindProperty("_shaderPropDefList"));
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                _shaderName = EditorGUILayout.TextField("Shader name", _shaderName);
                _namespace = EditorGUILayout.TextField("Inspector Namespace", _namespace);
            }

            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
                _newLineType = (NewLineType)EditorGUILayout.Popup("New Line Code", (int)_newLineType, _newLineSelections);
                _shouldGenerateVersionDetectionHeader = EditorGUILayout.ToggleLeft("Generate Version Detection Header", _shouldGenerateVersionDetectionHeader);
                _shouldGenerateLangTsv = EditorGUILayout.ToggleLeft("Generate Language File", _shouldGenerateLangTsv);
                _shouldGenerateConvertMenu = EditorGUILayout.ToggleLeft("Generate Convert Menu", _shouldGenerateConvertMenu);
                _shouldGenerateCacheClearMenu = EditorGUILayout.ToggleLeft("Generate Cache Clear Menu", _shouldGenerateCacheClearMenu);
                _shouldGenerateAssemblyInfo = EditorGUILayout.ToggleLeft("Generate AssemblyInfo.cs", _shouldGenerateAssemblyInfo);
                if (_shouldGenerateAssemblyInfo)
                {
                    using (new EditorGUI.IndentLevelScope())
                    using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                    {
                        _assemblyTitle = EditorGUILayout.TextField("Title", _assemblyTitle);
                    }
                }
            }
            _serializedObject.Update();

            _reorderableList.DoLayoutList();
            _serializedObject.ApplyModifiedProperties();


            if (GUILayout.Button("Generate Custom Shader"))
            {
                var exportDirPath = EditorUtility.SaveFolderPanel(
                    "Select export directory",
                    Directory.Exists(_lastExportDirectoryPath) ? _lastExportDirectoryPath : Application.dataPath,
                    string.Empty);
                if (string.IsNullOrEmpty(exportDirPath))
                {
                    return;
                }

                _lastExportDirectoryPath = exportDirPath;

                Debug.LogFormat("Export dir: {0}", exportDirPath);

                Generate(AbsPathToAssetPath(exportDirPath));
            }
        }

        private ReorderableList CreateReorderableList(SerializedObject serializedObject, SerializedProperty serializedProperty)
        {
            var reorderableList = new ReorderableList(
                serializedObject,
                serializedProperty,
                draggable: true,
                displayHeader: true,
                displayAddButton: true,
                displayRemoveButton: true);

            reorderableList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Shader Property Definitions");
            };

            reorderableList.elementHeightCallback = index =>
            {
                return EditorGUIUtility.singleLineHeight * 2 + 8;
            };

            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = serializedProperty.GetArrayElementAtIndex(index);

                float line = EditorGUIUtility.singleLineHeight;
                float padding = 2.0f;

                rect.y += padding;

                // ===== 1行目 =====
                var row1 = new Rect(rect.x, rect.y, rect.width, line);

                float nameWidth = row1.width * 0.3f;
                float descWidth = row1.width * 0.7f;

                EditorGUI.PropertyField(
                    new Rect(row1.x, row1.y, nameWidth - 2, line),
                    element.FindPropertyRelative("Name"),
                    new GUIContent("Property Name"));

                EditorGUI.PropertyField(
                    new Rect(row1.x + nameWidth, row1.y, descWidth, line),
                    element.FindPropertyRelative("Description"));

                // ===== 2行目 =====
                rect.y += line + padding;
                var row2 = new Rect(rect.x, rect.y, rect.width, line);

                float col1 = row2.width * 0.3f;
                float col2 = row2.width * 0.3f;
                float col3 = row2.width * 0.4f;

                EditorGUI.PropertyField(
                    new Rect(row2.x, row2.y, col1 - 2, line),
                    element.FindPropertyRelative("PropertyType"));

                EditorGUI.PropertyField(
                    new Rect(row2.x + col1, row2.y, col2 - 2, line),
                    element.FindPropertyRelative("UniformType"));

                EditorGUI.PropertyField(
                    new Rect(row2.x + col1 + col2, row2.y, col3, line),
                    element.FindPropertyRelative("DefaultValue"));
            };

            reorderableList.onAddCallback = l =>
            {
                _shaderPropDefList.Add(new ShaderPropertyDefinition(
                    "NewProperty",
                    "",
                    ShaderPropertyType.Float,
                    "0",
                    UniformVariableType.Float));
            };

            return reorderableList;
        }

        private void Generate(string dstDirAssetPath)
        {
            var jsonPath = AssetDatabase.GUIDToAssetPath("407d2dc27f05f774d9ca8d53fdef2047");
            var jsonRoot = DeserializeJson(jsonPath);
            var templateEngine = CreateTemplateEngine();

            var shaderDirAssetPath = dstDirAssetPath + "/" + "Shaders";
            AssetDatabase.ImportAsset(shaderDirAssetPath);
            var guidShaderDir = AssetDatabase.AssetPathToGUID(shaderDirAssetPath);
            if (guidShaderDir.Length != 0)
            {
                templateEngine.AddTag("GUID_SHADER_DIR", guidShaderDir);
            }

            foreach (var config in jsonRoot.configList)
            {
                if (config.name != "koturn")
                {
                    continue;
                }

                Debug.LogFormat("Generate files from {0}", config.name);

                var langCustomIndex = -1;
                var index = 0;
                foreach (var tfc in config.templates)
                {
                    if (tfc.destination == "Editor/lang_custom.tsv")
                    {
                        langCustomIndex = index;
                        break;
                    }
                    index++;
                }
                if (langCustomIndex != -1)
                {
                    var tfcLangCustom = config.templates[langCustomIndex];
                    config.templates.RemoveAt(langCustomIndex);

                    var dstFilePath = dstDirAssetPath + "/" + templateEngine.Replace(tfcLangCustom.destination);
                    var path = AssetDatabase.GUIDToAssetPath(tfcLangCustom.guid);

                    Directory.CreateDirectory(Path.GetDirectoryName(dstFilePath));

                    Debug.LogFormat("  {0} -> {1}", path, dstFilePath);

                    templateEngine.ExpandTemplate(path, dstFilePath);

                    AssetDatabase.ImportAsset(dstFilePath);
                    var guidlangCustom = AssetDatabase.AssetPathToGUID(dstFilePath);
                    Debug.LogFormat("{0}: {1}", path, dstFilePath, guidlangCustom);
                    if (guidlangCustom.Length == 0)
                    {
                        return;
                    }
                    templateEngine.AddTag("GUID_LANG_CUSTOM", guidlangCustom);
                }

                foreach (var tfc in config.templates)
                {
                    var dstFilePath = dstDirAssetPath + "/" + templateEngine.Replace(tfc.destination);
                    var path = AssetDatabase.GUIDToAssetPath(tfc.guid);

                    Debug.LogFormat("  {0} -> {1}", path, dstFilePath);

                    Directory.CreateDirectory(Path.GetDirectoryName(dstFilePath));

                    templateEngine.ExpandTemplate(path, dstFilePath);
                }
            }

            AssetDatabase.Refresh();
        }

        private static JsonRoot DeserializeJson(string filePath)
        {
            var jsonRoot = JsonRoot.LoadFromJsonFile(filePath);

            var nameConfigDict = new Dictionary<string, TemplateConfig>();
            var inheritList = new List<TemplateConfig>();
            foreach (var config in jsonRoot.configList)
            {
                nameConfigDict.Add(config.name, config);
                if (config.basedOn != null)
                {
                    inheritList.Add(config);
                }
            }

            // Resolve "basedOn".
            var visitSet = new HashSet<string>();
            foreach (var config in inheritList)
            {
                var dstSet = new HashSet<string>();
                foreach (var tfc in config.templates)
                {
                    dstSet.Add(tfc.destination);
                }

                visitSet.Clear();
                visitSet.Add(config.name);

                var parentConfig = config;
                while (parentConfig.basedOn != null)
                {
                    if (visitSet.Contains(parentConfig.basedOn))
                    {
                        throw new InvalidOperationException("Circular definition detected: " + config.name);
                    }

                    parentConfig = nameConfigDict[parentConfig.basedOn];
                    foreach (var tfc in parentConfig.templates)
                    {
                        if (!dstSet.Contains(tfc.destination))
                        {
                            Console.WriteLine("Add {0}", tfc.destination);
                            config.templates.Add(tfc);
                            dstSet.Add(tfc.destination);
                        }
                    }
                }
            }

            return jsonRoot;
        }

        private TemplateEngine CreateTemplateEngine()
        {
            var shaderPropDefList = _shaderPropDefList;
            var materialPropNames = new string[shaderPropDefList.Count];
            var langTags = new string[shaderPropDefList.Count];

            var index = 0;
            foreach (var shaderProp in _shaderPropDefList)
            {
                materialPropNames[index] = Regex.Replace(shaderProp.Name, @"_*(\w)(\w*)", m => "_" + m.Groups[1].Value.ToLower() + m.Groups[2].Value);
                langTags[index] = Regex.Replace(shaderProp.Name, @"_*(\w)(\w*)", m => "s" + m.Groups[1].Value + m.Groups[2].Value);
                index++;
            }

            var templateEngine = new TemplateEngine();
            templateEngine.AddTag("SHADER_NAME", _shaderName);
            templateEngine.AddTag("NAMESPACE", _namespace);
            templateEngine.AddTag("INSPECTOR_NAME", _inspectorName);
            templateEngine.AddTag("AUTHOR_NAME", "koturn");
            templateEngine.AddTag("VERSION", "1.0.0.0");
            templateEngine.AddTag("YEAR", DateTime.Now.Year.ToString());
            templateEngine.AddTag("CUSTOM_SHADER_TITLE", "Special shader");

            var sb = new StringBuilder();

            index = 0;
            foreach (var shaderProp in _shaderPropDefList)
            {
                sb.AppendLine("/// <summary>")
                    .AppendFormat("/// <see cref=\"MaterialProperty\" of \"{0}\".", shaderProp.Name).AppendLine()
                    .AppendLine("/// </summary>")
                    .AppendFormat("private MaterialProperty {0};", materialPropNames[index])
                    .AppendLine();
                index++;
            }
            templateEngine.AddTag("DECLARE_MATERIAL_PROPERTIES", sb.ToString());

            sb.Clear();
            index = 0;
            foreach (var shaderProp in _shaderPropDefList)
            {
                sb.AppendFormat("{0} = FindProperty(\"{1}\", props);", materialPropNames[index], shaderProp.Name)
                    .AppendLine();
                index++;
            }
            templateEngine.AddTag("INITIALIZE_MATERIAL_PROPERTIES", sb.ToString());

            if (_shouldGenerateLangTsv)
            {
                sb.Clear();
                index = 0;
                foreach (var shaderProp in _shaderPropDefList)
                {
                    sb.AppendFormat(
                        "m_MaterialEditor.ShaderProperty({0}, GetLoc(\"{1}\"));",
                        materialPropNames[index],
                        langTags[index]).AppendLine();
                    index++;
                }
                templateEngine.AddTag("DRAW_MATERIAL_PROPERTIES", sb.ToString());

                sb.Clear();
                index = 0;
                foreach (var shaderProp in _shaderPropDefList)
                {
                    sb.AppendFormat("{0}\t{1}\t{1}\t{1}\t{1}\t{1}", langTags[index], shaderProp.Description.Length == 0 ? langTags[index] : shaderProp.Description)
                        .AppendLine();
                    index++;
                }
                templateEngine.AddTag("LANGUAGE_FILE_CONTENT", sb.ToString());
            }
            else
            {
                sb.Clear();
                index = 0;
                foreach (var shaderProp in _shaderPropDefList)
                {
                    sb.AppendFormat(
                        "m_MaterialEditor.ShaderProperty({0}, {0}.displayName);",
                        materialPropNames[index]).AppendLine();
                    index++;
                }
                templateEngine.AddTag("DRAW_MATERIAL_PROPERTIES", sb.ToString());
            }

            sb.Clear();
            if (_shouldGenerateLangTsv)
            {
                index = 0;
                foreach (var shaderProp in _shaderPropDefList)
                {
                    sb.AppendFormat(
                        "{0} (\"{1}\", {2}) = {3}",
                        shaderProp.Name,
                        langTags[index],
                        shaderProp.PropertyType.ToString(),
                        shaderProp.DefaultValue).AppendLine();
                    index++;
                }
            }
            else
            {
                foreach (var shaderProp in _shaderPropDefList)
                {
                    sb.AppendFormat(
                        "{0} (\"{1}\", {2}) = {3}",
                        shaderProp.Name,
                        shaderProp.Description,
                        shaderProp.PropertyType.ToString(),
                        shaderProp.DefaultValue).AppendLine();
                }
            }
            templateEngine.AddTag("DECLARE_CUSTOM_PROPERTIES", sb.ToString());

            sb.Clear();
            index = 0;
            foreach (var shaderProp in _shaderPropDefList)
            {
                if (index > 0)
                {
                    sb.Append(@" \").AppendLine();
                }
                sb.AppendFormat(@"{0} {1};", "float", shaderProp.Name);
                index++;
            }
            sb.AppendLine();
            templateEngine.AddTag("DECLARE_UNIFORM_VARIABLES", sb.ToString());
            sb.Clear();

            // DECLARE_TEXTURE_VARIABLES
            // V2F_MEMBERS
            // V2F_MEMBERS_VER140_SHADOWCASTER

            if (_shouldGenerateConvertMenu)
            {
                templateEngine.AddTag("SHOULD_GENERATE_CONVERT_MENU", "true");
            }
            if (_shouldGenerateCacheClearMenu)
            {
                templateEngine.AddTag("SHOULD_GENERATE_REFRESH_MENU", "true");
            }
            if (_shouldGenerateVersionDetectionHeader)
            {
                templateEngine.AddTag("SHOULD_GENERATE_VERSION_DEF_FILE", "true");
            }

            return templateEngine;
        }

        [MenuItem("Tools/lilToon Custom Generator")]
        private static void OpenWindow()
        {
            EditorWindow.GetWindow<LilToonCustomGeneratorWindow>("lilToon Modifier");
        }


        /// <summary>
        /// パスの区切り文字を正規化します。
        /// </summary>
        /// <remarks>
        /// - バックスラッシュ(\)をスラッシュ(/)に変換
        /// - 連続する区切り文字(// や \\\ など)を単一のスラッシュ(/)に変換
        /// </remarks>
        /// <param name="path">正規化するパス文字列</param>
        /// <returns>
        /// 正規化されたパス文字列。
        /// 入力がnullまたは空文字の場合は、入力値をそのまま返します。
        /// </returns>
        /// <example>
        /// 以下のパスはすべて同じ文字列に正規化されます:
        /// <code>
        /// "Assets\\Path"  -> "Assets/Path"
        /// "Assets/Path"   -> "Assets/Path"
        /// "Assets//Path" -> "Assets/Path"
        /// </code>
        /// </example>
        public static string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path)) return path;

            // すべての区切り文字を'/'に変換
            path = path.Replace('\\', '/');

            // 連続する'/'を単一の'/'に置換（複数回の置換で///なども対応）
            while (path.Contains("//"))
            {
                path = path.Replace("//", "/");
            }

            return path;
        }

        /// <summary>
        /// Windowsのフルパスを Unity の Asset パスに変換します
        /// </summary>
        /// <param name="absPath">
        /// 変換元のパス
        /// - Windowsフルパス (例: "C:/Projects/MyGame/Assets/Textures/image.png")
        /// - アセットパス (例: "Assets/Textures/image.png")
        /// </param>
        /// <returns>Unityのアセットパス (例: "Assets/Textures/image.png")</returns>
        public static string AbsPathToAssetPath(string absPath)
        {
            if (string.IsNullOrEmpty(absPath)) return null;

            // パスを正規化
            absPath = NormalizePath(absPath);

            // すでにAssetsから始まる場合は、正規化したパスをそのまま返す
            if (absPath.StartsWith("Assets/", System.StringComparison.OrdinalIgnoreCase))
            {
                return absPath;
            }

            // Application.dataPath は "プロジェクトパス/Assets" を返す
            string projectPath = NormalizePath(Application.dataPath);
            // "Assets" フォルダまでのパスを取得
            string assetsBasePath = projectPath.Substring(0, projectPath.Length - "Assets".Length);

            // フルパスから相対パスに変換
            if (absPath.StartsWith(assetsBasePath, System.StringComparison.OrdinalIgnoreCase))
            {
                return absPath.Substring(assetsBasePath.Length);
            }

            Debug.LogError("Invalid path: The specified path is not within the Unity project.");
            return null;
        }

        /// <summary>
        /// Unity の Asset パスを Windows のフルパスに変換します
        /// </summary>
        /// <param name="assetPath">
        /// 変換元のパス
        /// - アセットパス (例: "Assets/Textures/image.png")
        /// - Windowsフルパス (例: "C:/Projects/MyGame/Assets/Textures/image.png")
        /// </param>
        /// <returns>Windowsのフルパス (例: "C:/Projects/MyGame/Assets/Textures/image.png")</returns>
        public static string AssetPathToAbsPath(string assetPath)
        {
            if (string.IsNullOrEmpty(assetPath)) return null;

            // 入力パスを正規化
            assetPath = NormalizePath(assetPath);

            // すでにフルパスの場合は、正規化したパスをそのまま返す
            string projectPath = NormalizePath(Application.dataPath);
            string projectRoot = projectPath.Substring(0, projectPath.Length - "Assets".Length);
            if (assetPath.StartsWith(projectRoot, System.StringComparison.OrdinalIgnoreCase))
            {
                return assetPath;
            }

            // アセットパスが "Assets/" で始まっていない場合はエラー
            if (!assetPath.StartsWith("Assets/", System.StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogError("Invalid asset path: The path must start with 'Assets/'");
                return null;
            }

            // プロジェクトルートパスとアセットパスを結合して正規化
            return NormalizePath(Path.Combine(projectRoot, assetPath));
        }
    }
}
