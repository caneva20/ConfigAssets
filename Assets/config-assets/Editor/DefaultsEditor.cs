using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace me.caneva20.ConfigAssets.Editor {
    [CustomEditor(typeof(Defaults))]
    public class DefaultsEditor : UnityEditor.Editor {
        private readonly Regex _resourcesFolder = new Regex("[Rr]esources");
        private readonly Regex _editorFolder = new Regex("[Ee]ditor");

        private SerializedProperty _baseDirectory;
        private SerializedProperty _codeGenDirectory;
        private SerializedProperty _appendNamespaceToFile;
        private SerializedProperty _nameSpaceLength;
        private SerializedProperty _loggingLevel;

        private void OnEnable() {
            _baseDirectory = serializedObject.FindProperty("_baseDirectory");
            _codeGenDirectory = serializedObject.FindProperty("_codeGenDirectory");
            _appendNamespaceToFile = serializedObject.FindProperty("_appendNamespaceToFile");
            _nameSpaceLength = serializedObject.FindProperty("_nameSpaceLength");
            _loggingLevel = serializedObject.FindProperty("_loggingLevel");
        }

        public override void OnInspectorGUI() {
            #region Logging

            EditorGUILayout.PropertyField(_loggingLevel);

            #endregion

            #region Base directory

            DirectorySelector(_baseDirectory, "Resources", "This directory is used to store the .asset files");

            if (!_resourcesFolder.IsMatch(_baseDirectory.stringValue)) {
                EditorGUILayout.HelpBox("This directory MUST contain a 'Resources' folder or be inside one",
                    MessageType.Error);
            }

            #endregion

            Spacing();

            #region Code gen directory

            DirectorySelector(_codeGenDirectory, "Editor", "This directory is used to store auto-generated files");

            if (_editorFolder.IsMatch(_codeGenDirectory.stringValue)) {
                EditorGUILayout.HelpBox("This directory MUST NOT contain an 'Editor' folder", MessageType.Error);
            }

            #endregion

            Spacing();

            Prop(_appendNamespaceToFile,
                "Should the namespace of the class be appended to the file name of the .asset?\n\n" +
                "NOTE: This setting is ignored for classes with no namespace");

            if (_appendNamespaceToFile.boolValue) {
                using (new EditorGUILayout.VerticalScope("box")) {
                    EditorGUI.indentLevel++;
                    Prop(_nameSpaceLength,
                        "How many segments of the namespace should be used?\n\n" +
                        "0: None\n-1: The full namespace\n\nNOTE: Segments are the sum of dots(.) in the namespace plus 1, i.e caneva20.ConfigAssets = 2");

                    EditorGUI.indentLevel--;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static void DirectorySelector(
            SerializedProperty property,
            string defaultDirectory,
            string description
        ) {
            EditorGUILayout.HelpBox(description, MessageType.None);

            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.LabelField($"{property.displayName}", "Assets\\");
                property.stringValue = EditorGUILayout.TextField(property.stringValue);

                if (GUILayout.Button("...", EditorStyles.miniButton, GUILayout.Width(32))) {
                    var path = EditorUtility.SaveFolderPanel(property.displayName,
                        Path.Combine(Application.dataPath, property.stringValue), defaultDirectory);

                    path = path.Replace($"{Application.dataPath}/", "");
                    path = path.Replace("/", "\\");

                    property.stringValue = path;
                }
            }
        }

        private static void Prop(SerializedProperty property, string description) {
            EditorGUILayout.HelpBox(description, MessageType.None);

            EditorGUILayout.PropertyField(property);
        }

        public static void Spacing() {
            EditorGUILayout.Space(4);

            using (new EditorGUILayout.VerticalScope("box")) {
                EditorGUILayout.Space(0);
            }

            EditorGUILayout.Space(4);
        }
    }
}
