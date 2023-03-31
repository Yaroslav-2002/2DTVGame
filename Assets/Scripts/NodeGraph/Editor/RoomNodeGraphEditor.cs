using System.Collections.Generic;
using GameManager;
using PlasticGui.WorkspaceWindow.CodeReview;
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor.MPE;

namespace NodeGraph.Editor
{
    [CustomEditor(typeof(RoomNodeTypeListSO))]
    [CanEditMultipleObjects]
    public class RoomNodeGraphEditor : EditorWindow
    {
        private GUIStyle _roomNodeStyle;

        private static RoomNodeGraphSO _currentRoomNodeGraph;
        private RoomNodeTypeListSO _roomNodeTypeListSo;

        // Node layout values
        private const float NodeWidth = 160f;
        private const float NodeHeight = 75f;
        private const int NodePadding = 25;
        private const int NodeBorder = 12;
        
        private void OnEnable()
        {
            _roomNodeStyle = new GUIStyle
            {
                normal =
                {
                    background = EditorGUIUtility.Load("node1") as Texture2D,
                    textColor = Color.green
                },
                padding = new RectOffset( NodePadding,NodePadding, NodePadding,NodePadding),
                border = new RectOffset( NodeBorder,NodeBorder, NodeBorder,NodeBorder)
            };

            _roomNodeTypeListSo = GameResources.Instance.RoomNodeTypeList;
        }

        
        [MenuItem("Room node editor", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
        private static void Init()
        {
            GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
        }

        [OnOpenAsset(0)]
        public static bool OnDoubleClickAsset(int instanceID, int line)
        {
            RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;
            if (roomNodeGraph != null)
            {
                Init();

                _currentRoomNodeGraph = roomNodeGraph;

                return true;
            }

            return false;
        }
        
        private void OnGUI()
        {
            if (_currentRoomNodeGraph != null)
            {
                ProcessEvents(Event.current);

                DrawRoomNodes();
            }

            if (GUI.changed)
            {
                Repaint();
            }
        }

        private void DrawRoomNodes()
        {
            foreach (RoomNodeSO roomNode in _currentRoomNodeGraph.roomNodes)
            {
                if (roomNode != null)
                    roomNode.Draw(_roomNodeStyle);
            }

            GUI.changed = true;
        }

        private void ProcessEvents(Event currentEvent)
        {
            ProcessRoomNodeGraphEvents(currentEvent);
        }

        private void ProcessRoomNodeGraphEvents(Event currentEvent)
        {
            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    ProcessMouseDownEvent(currentEvent);
                    break;
            }
        }

        private void ProcessMouseDownEvent(Event currentEvent)
        {
            if (currentEvent.button == 1)
            {
                ShowContextMenu(currentEvent.mousePosition);
            }
        }

        private void ShowContextMenu(Vector2 mousePosition)
        {
            GenericMenu menu = new GenericMenu();
            
            menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
            
            menu.ShowAsContext();
        }

        private void CreateRoomNode(object mousePositionObject)
        {
            CreateRoomNode(mousePositionObject, _roomNodeTypeListSo.typeList.Find(x => x.isNone));
        }

        private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
        {
            Vector2 mousePosition = (Vector2)mousePositionObject;
            RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();
            
            _currentRoomNodeGraph.roomNodes.Add(roomNode);

            roomNode.Initialise(new Rect(mousePosition, new Vector2(NodeWidth, NodeHeight)), _currentRoomNodeGraph,
                roomNodeType);
            
            AssetDatabase.AddObjectToAsset(roomNode, _currentRoomNodeGraph);

            AssetDatabase.SaveAssets();
        }
    }
}
