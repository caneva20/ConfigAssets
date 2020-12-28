using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace caneva20.ConfigAssets.Editor {
    [CustomEditor(typeof(Defaults))]
    public class DefaultsEditor : UnityEditor.Editor {
        private readonly Regex _resourcesFolder = new Regex("[Rr]esources");
        
        private SerializedProperty _baseDirectory;
        private SerializedProperty _appendNamespaceToFile;
        private SerializedProperty _nameSpaceLength;

        private void OnEnable() {
            _baseDirectory = serializedObject.FindProperty("_baseDirectory");
            _appendNamespaceToFile = serializedObject.FindProperty("_appendNamespaceToFile");
            _nameSpaceLength = serializedObject.FindProperty("_nameSpaceLength");
        }

        public override void OnInspectorGUI() {
            #region Base directory

            using (var scope = new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.LabelField($"{_baseDirectory.displayName}\t Assets\\");
                _baseDirectory.stringValue = EditorGUILayout.TextField(_baseDirectory.stringValue);

                if (GUILayout.Button("...", EditorStyles.miniButton)) {
                    var path = EditorUtility.SaveFolderPanel(_baseDirectory.displayName,
                        Path.Combine(Application.dataPath, _baseDirectory.stringValue),
                        "Resources");

                    path = path.Replace($"{Application.dataPath}/", "");
                    path = path.Replace("/", "\\");

                    _baseDirectory.stringValue = path;
                }
            }
            
            EditorGUILayout.HelpBox("The base directory used to store the configs", MessageType.Info);

            if (!_resourcesFolder.IsMatch(_baseDirectory.stringValue)) {
                EditorGUILayout.HelpBox("Base directory MUST contain a 'Resources' folder or be inside one", MessageType.Error);
            }

            #endregion
            
            Prop(_appendNamespaceToFile, "Should the namespace of the class be appended to the file name of the asset?\n\n" +
                "NOTE: This setting is ignored for classes with no namespace");
            
            if (_appendNamespaceToFile.boolValue) {
                EditorGUI.indentLevel++;
                Prop(_nameSpaceLength, "How many segments of the namespace should be used?\n\n" +
                    "0: None\n-1: The full namespace\n\nNOTE: Segments are the sum of dots(.) in the namespace plus 1, i.e caneva20.ConfigAssets = 2");
                
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private static void Prop(SerializedProperty property, string info) {
            EditorGUILayout.PropertyField(property);

            EditorGUILayout.HelpBox(info, MessageType.Info);
        }
    }
}