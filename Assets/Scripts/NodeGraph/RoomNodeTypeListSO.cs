using System;
using System.Collections.Generic;
using NodeGraph.Utilities;
using UnityEngine;

namespace NodeGraph
{
    [CreateAssetMenu(fileName = "RoomNodeTypeList", menuName = "Scriptable Objects/Dungeon/Room Node Type List")]
    public class RoomNodeTypeListSO : ScriptableObject
    {
        public List<RoomNodeTypeSO> typeList;
        
        #region Validation

#if UNITY_EDITOR
        private void OnValidate()
        {
            Helper.ValidateCheckEnumerableValues(this, nameof(typeList), typeList);
        }
#endif
        #endregion
    }
}
