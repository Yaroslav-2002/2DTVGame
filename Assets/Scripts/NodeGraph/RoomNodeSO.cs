using System;
using UnityEditor;
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
        [HideInInspector] public bool isLeftClickDragging;
        [HideInInspector] public bool isSelected;

        public void Initialise(Rect rect, RoomNodeGraphSO roomNodeGraph, RoomNodeTypeSO roomNodeType)
        {
            this.rect = rect;
            id = Guid.NewGuid().ToString();
            name = "Room Node";
            this.roomNodeGraph = roomNodeGraph;
            this.roomNodeType = roomNodeType;

            roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
        }
        
        public void Draw(GUIStyle roomNodeStyle)
        {
            GUILayout.BeginArea(rect, roomNodeStyle);
            
            EditorGUI.BeginChangeCheck();

            if (parentRoomNodeIDList.Count > 0 || roomNodeType.isEntrance)
            {
                EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
            }
            else
            {
                var selected = roomNodeTypeList.typeList.FindIndex(x => x == roomNodeType);
                var selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypeToDisplay());
                
                roomNodeType = roomNodeTypeList.typeList[selection];
            }
            
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

        public void ProcessEvents(Event currentEvent)
        {
            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    ProcessMouseDownEvent(currentEvent);
                    break;
                case EventType.MouseUp:
                    ProcessMouseUpEvent(currentEvent);
                    break;
                case EventType.MouseDrag:
                    ProcessMouseDragEvent(currentEvent);
                    break;
            }
        }

        private void ProcessMouseDownEvent(Event currentEvent)
        {
            if (currentEvent.button == 0)
            {
                ProcessLeftClickDownEvent();
            }
            else if (currentEvent.button == 1)
            {
                ProcessRightClickDownEvent(currentEvent);
            }
        }

        private void ProcessRightClickDownEvent(Event currentEvent)
        {
           roomNodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
        }

        private void ProcessMouseUpEvent(Event currentEvent)
        {
            if (currentEvent.button == 0)
            {
                ProcessLeftClickUpEvent();
            }
        }
        
        private void ProcessMouseDragEvent(Event currentEvent)
        {
            isLeftClickDragging = true;

            DragNode(currentEvent.delta);
            GUI.changed = true;
        }

        private void DragNode(Vector2 currentEventDelta)
        {
            rect.position += currentEventDelta;
            EditorUtility.SetDirty(this);
        }

        private void ProcessLeftClickUpEvent()
        {
            if (isLeftClickDragging)
            {
                isLeftClickDragging = false;
            }
        }

        private void ProcessLeftClickDownEvent()
        {
            Selection.activeObject = this;

            isSelected = !isSelected;
        }

        public bool AddChildID(string childID)
        {
            childRoomNodeIDList.Add(childID);
            return true;
        }
        
        public bool AddParentID(string childID)
        {
            parentRoomNodeIDList.Add(childID);
            return true;
        }
        
#endif
        
        #endregion
    }
}
