using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeGraph
{
    [CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]
    public class RoomNodeGraphSO : ScriptableObject
    {
        [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;
        [HideInInspector] public List<RoomNodeSO> roomNodes = new List<RoomNodeSO>();
        public Dictionary<string, RoomNodeSO> RoomNodeDictionary = new Dictionary<string, RoomNodeSO>();
        private void Awake()
        {
            LoadRoomNodeFromDict();
        }

        private void LoadRoomNodeFromDict()
        {
            RoomNodeDictionary.Clear();

            foreach (var node in roomNodes)
            {
                RoomNodeDictionary[node.id] = node;
            }
        }

        public RoomNodeSO GetRoomNode(string roomNodeID)
        {
            if (RoomNodeDictionary.TryGetValue(roomNodeID, out RoomNodeSO roomNode))
            {
                return roomNode;
            }
#if UNITY_EDITOR
            Debug.Log($"There is no such {roomNodeID} in RoomNodeDictionary");
#endif
            return null;
        }
        
#if UNITY_EDITOR
        [HideInInspector] public RoomNodeSO roomNodeToDrawLineFrom = null;
        [HideInInspector] public Vector2 linePosition;


        public void OnValidate()
        {
            LoadRoomNodeFromDict();
        }

        public void SetNodeToDrawConnectionLineFrom(RoomNodeSO node, Vector2 position)
        {
            roomNodeToDrawLineFrom = node;
            linePosition = position;
        }
#endif
    }
}
