using NodeGraph;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameManager
{
    public class GameResources : MonoBehaviour
    {
        private static GameResources _instance;
        
        public static GameResources Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<GameResources>("GameResources");
                }
                return _instance;
            }
        }

        #region Header DUNGEON

        [FormerlySerializedAs("RoomNodeTypeList")]
        [Space(10)]
        [Header("Dungeon")]

        #endregion
        #region Tooltip

        [Tooltip("Populate with the dungeon RoomNodeTypeListSO")]

        #endregion
        
        public RoomNodeTypeListSO roomNodeTypeList;
    }
}
