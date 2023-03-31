using System;
using UnityEditor;
using System;
using System.Collections.Generic;
using GameManager;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeGraph
{
    [Serializable]
    public class RoomNodeSO : ScriptableObject
    {
        [HideInInspector] public string id;
        [HideInInspector] public List<string> parentRoomNodeIDList = new List<string>();
        [HideInInspector] public List<string> childRoomNodeIDList = new List<string>();
        [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
        [FormerlySerializedAs("roomNodeTypeSO")] public RoomNodeTypeSO roomNodeType;
        [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

        #region Editor Code

#if UNITY_EDITOR
        [HideInInspector] public Rect rect;

        public void Initialise(Rect rect, RoomNodeGraphSO roomNodeGraph, RoomNodeTypeSO roomNodeType)
        {
            this.rect = rect;
            this.id = Guid.NewGuid().ToString();
            this.name = "Room Node";
            roomNodeGraph = roomNodeGraph;
            roomNodeType = roomNodeType;

            roomNodeTypeList = GameResources.Instance.RoomNodeTypeList;
        }
        
        public void Draw(GUIStyle roomNodeStyle)
        {
            GUILayout.BeginArea(rect, roomNodeStyle);
            
            EditorGUI.BeginChangeCheck();

            int selected = roomNodeTypeList.typeList.FindIndex(x => x == roomNodeType);
            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypeToDisplay());

            roomNodeType = roomNodeTypeList.typeList[selection];
            
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(this);
                
                GUILayout.EndArea();
            }
            GUILayout.EndArea();
        }

        private string[] GetRoomNodeTypeToDisplay()
        {
            string[] roomArray = new string[roomNodeTypeList.typeList.Count];

            for (int i = 0; i < roomNodeTypeList.typeList.Count; i++)
            {
                if (roomNodeTypeList.typeList[i].displayInNodeGraphEditor)
                {
                    roomArray[i] = roomNodeTypeList.typeList[i].roomNodeTypeName;
                }
            }

            return roomArray;
        }
#endif
        
        #endregion
    }
}
