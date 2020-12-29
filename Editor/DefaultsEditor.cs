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

        private void OnEnable() {
            _baseDirectory = serializedObject.FindProperty("_baseDirectory");
            _codeGenDirectory = serializedObject.FindProperty("_codeGenDirectory");
            _appendNamespaceToFile = serializedObject.FindProperty("_appendNamespaceToFile");
            _nameSpaceLength = serializedObject.FindProperty("_nameSpaceLength");
        }

        public override void OnInspectorGUI() {
            #region Base directory

            DirectorySelector(_baseDirectory, "Resources", "The base directory used to store the configs");

            if (!_resourcesFolder.IsMatch(_baseDirectory.stringValue)) {
                EditorGUILayout.HelpBox(
                    "Base directory MUST contain a 'Resources' folder or be inside one",
                    MessageType.Error);
            }

            #endregion

            #region Code gen directory

            if (!_editorFolder.IsMatch(_codeGenDirectory.stringValue)) {
                EditorGUILayout.HelpBox(
                    "CodeGen directory MUST contain an 'Editor' folder or be inside one",
                    MessageType.Error);
            }
            
            DirectorySelector(_codeGenDirectory, "Editor",
                "The directory used to save auto-generated files");

            #endregion
            

            Prop(_appendNamespaceToFile,
                "Should the namespace of the class be appended to the file name of the asset?\n\n" +
                "NOTE: This setting is ignored for classes with no namespace");

            if (_appendNamespaceToFile.boolValue) {
                EditorGUI.indentLevel++;
                Prop(_nameSpaceLength,
                    "How many segments of the namespace should be used?\n\n" +
                    "0: None\n-1: The full namespace\n\nNOTE: Segments are the sum of dots(.) in the namespace plus 1, i.e caneva20.ConfigAssets = 2");

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static void DirectorySelector(SerializedProperty property, string defaultDirectory, string info) {
            EditorGUILayout.HelpBox(info, MessageType.Info);
            
            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.LabelField($"{property.displayName}\t Assets\\");
                property.stringValue = EditorGUILayout.TextField(property.stringValue);

                if (GUILayout.Button("...", EditorStyles.miniButton)) {
                    var path = EditorUtility.SaveFolderPanel(property.displayName,
                        Path.Combine(Application.dataPath, property.stringValue), defaultDirectory);

                    path = path.Replace($"{Application.dataPath}/", "");
                    path = path.Replace("/", "\\");

                    property.stringValue = path;
                }
            }
        }

        private static void Prop(SerializedProperty property, string info) {
            EditorGUILayout.PropertyField(property);

            EditorGUILayout.HelpBox(info, MessageType.Info);
        }
    }
}
