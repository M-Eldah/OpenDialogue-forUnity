using DSystem.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DSystem.Inspector
{
    [CustomEditor(typeof(DialogueHandeler))]
    public class DialogueInspector : Editor
    {
        private DialogueHandeler Dialogue;

        public override void OnInspectorGUI()
        {
            Dialogue = (target as DialogueHandeler);

            //The Dialogue
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Choose Dialogue", "Which dialogue is this NPC going to own?"));
            Undo.RecordObject(target, "StartNodeChanged");
            //The Dialogue index refers to the index of the dialogue index in the dialouge name list
            Dialogue.Dindex = EditorGUILayout.Popup(Dialogue.Dindex, Dialoguelist().ToArray());
            string dialogue = "";
            try
            {
                dialogue = Dialoguelist().ToArray()[Dialogue.Dindex];
            }
            catch (Exception)
            {
                dialogue = "";
            }
            Dialogue.Dname = dialogue;
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Open Dialogue Editor"))
            {
                DialogueSystemWindow.Dname = $"{Dialogue.Dname}.json";
                DialogueSystemWindow.Open();
            }
            if (GUILayout.Button("LoadDialogue"))
            {
                Dialogue.LoadData(Dialogue.Dname);

            }
            GUILayout.Label(new GUIContent("Preload dialogue to save up on processing time, needs to be redone every time you edit the dialogue"));
            if (GUILayout.Button("ClearData"))
            {

                Dialogue.Cleardata();

            }
            Undo.RecordObject(target, "StartNodeChanged");

            Undo.RecordObject(target, "Actor change");
            SerializedProperty tileProperty = serializedObject.FindProperty("actors");
            EditorGUILayout.PropertyField(tileProperty, includeChildren: true);
            if (tileProperty.hasChildren)
            {
                serializedObject.ApplyModifiedProperties();
            }
            Undo.RecordObject(target, "ChangeStartNode");
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Override start node", "change the startNode of the dialogue keep at -1 if don't want to change it"));
            Dialogue.ORSNode = EditorGUILayout.IntField(Dialogue.ORSNode);
            EditorGUILayout.EndHorizontal();
            
        }

        public List<string> Dialoguelist()
        {
            List<string> DialogueList = new List<string>();
            DirectoryInfo di = new DirectoryInfo("Assets/OpenDialogue/Resources/DialoguesData");
            FileSystemInfo[] files = di.GetFileSystemInfos();
            var orderedFiles = files.OrderBy(f => f.CreationTimeUtc);
            foreach (FileSystemInfo d in orderedFiles.ToArray())
            {
                if (d.Extension == ".json")
                {
                    DialogueList.Add(d.Name.Split(".")[0]);
                }
            }
            return DialogueList;
        }
    }
}