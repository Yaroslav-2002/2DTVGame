using System;
using NodeGraph.Utilities;
using UnityEngine;

namespace NodeGraph
{
    [CreateAssetMenu(fileName = "RoomNodeType", menuName = "Scriptable Objects/Dungeon/Room Node Type")]
    public class RoomNodeTypeSO : ScriptableObject
    {
        //public RoomNodeType roomNodeType;
        public string roomNodeTypeName;
        public bool isCorridor;
        public bool isCorridorNS;
        public bool isCorridorEW;
        public bool isEntrance;
        public bool isBossRoom;
        public bool isNone;
        public bool displayInNodeGraphEditor = true;

        #region Validation

#if UNITY_EDITOR
        private void OnValidate()
        {
            Helper.ValidateCheckEmptyString(this, nameof(roomNodeTypeName), roomNodeTypeName);
        }
#endif
        #endregion
    }
}
