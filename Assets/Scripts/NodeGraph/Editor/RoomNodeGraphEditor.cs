using System;
using System.Linq;
using System.Xml;
using GameManager;
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;

namespace NodeGraph.Editor
{
    [CustomEditor(typeof(RoomNodeTypeListSO))]
    [CanEditMultipleObjects]
    public class RoomNodeGraphEditor : EditorWindow
    {
        private GUIStyle _roomNodeStyle;
        private GUIStyle _roomNodeSelectedStyle;
        private RoomNodeTypeListSO _roomNodeTypeListSo;
        private RoomNodeSO _currentRoomNode;
        private static RoomNodeGraphSO _currentRoomNodeGraph;
        private const float ConnectingLineArrowSize = 6f;
        private const float ConnectingLineWidth = 3f;

        // Node layout values
        private const float NodeWidth = 160f;
        private const float NodeHeight = 75f;
        private const int NodePadding = 25;
        private const int NodeBorder = 12;
        
        private void OnEnable()
        {
            Selection.selectionChanged += InspectorSelectionChanged;
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
            
            _roomNodeSelectedStyle = new GUIStyle
            {
                normal =
                {
                    background = EditorGUIUtility.Load("node1 on") as Texture2D,
                    textColor = Color.green
                },
                padding = new RectOffset( NodePadding,NodePadding, NodePadding,NodePadding),
                border = new RectOffset( NodeBorder,NodeBorder, NodeBorder,NodeBorder)
            };

            _roomNodeTypeListSo = GameResources.Instance.roomNodeTypeList;
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= InspectorSelectionChanged;
        }

        private void InspectorSelectionChanged()
        {
            RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;

            if (roomNodeGraph != null)
            {
                _currentRoomNodeGraph = roomNodeGraph;
                GUI.changed = true;
            }
            
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
                DrawDraggedLine();
                
                ProcessEvents(Event.current);

                DrawRoomNodeConnections();
                
                DrawRoomNodes();
            }

            if (GUI.changed)
            {
                Repaint();
            }
        }

        private void DrawRoomNodeConnections()
        {
            foreach (var roomNode in _currentRoomNodeGraph.roomNodes)
            {
                if (roomNode.childRoomNodeIDList.Count > 0)
                {
                    foreach (var childNodeID in roomNode.childRoomNodeIDList)
                    {
                        if (_currentRoomNodeGraph.RoomNodeDictionary.ContainsKey(childNodeID))
                        {
                            DrawConnectionLine(roomNode, _currentRoomNodeGraph.RoomNodeDictionary[childNodeID]);

                            GUI.changed = true;
                        }
                    }
                }
            }
        }

        private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
        {
            var startPos = parentRoomNode.rect.center;
            var endPos = childRoomNode.rect.center;

            var midPos = (endPos + startPos) / 2;
            
            var direction = endPos - startPos;

            var arrowTailPoint1 = midPos - new Vector2(-direction.y, direction.x).normalized * ConnectingLineArrowSize;
            var arrowTailPoint2 = midPos + new Vector2(-direction.y, direction.x).normalized * ConnectingLineArrowSize;

            var arrowHeadPoint = midPos + direction.normalized * ConnectingLineArrowSize;
            
            Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, ConnectingLineWidth);
            Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, ConnectingLineWidth);
            
            Handles.DrawBezier(startPos, endPos, startPos, endPos, Color.white, null, ConnectingLineWidth);

            GUI.changed = true;
        }

        private void DrawDraggedLine()
        {
            if (_currentRoomNodeGraph.linePosition != Vector2.zero)
            {
                Handles.DrawBezier(_currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, _currentRoomNodeGraph.linePosition,
                    _currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center, _currentRoomNodeGraph.linePosition, Color.white, null, ConnectingLineWidth);
            }
        }

        private void DrawRoomNodes()
        {
            foreach (RoomNodeSO roomNode in _currentRoomNodeGraph.roomNodes)
            {
                if (roomNode == null) continue;
                roomNode.Draw(roomNode.isSelected ? _roomNodeSelectedStyle : _roomNodeStyle);
            }

            GUI.changed = true;
        }

        private void ProcessEvents(Event currentEvent)
        {
            
            if(_currentRoomNode == null || _currentRoomNode.isLeftClickDragging == false)
                _currentRoomNode = IsMouseOverRoomNode(currentEvent);
            
            Debug.Log($"Current room node: {_currentRoomNode}");
            if (_currentRoomNode == null || _currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
            {
                ProcessRoomNodeGraphEvents(currentEvent);
            }
            else
            {
                _currentRoomNode.ProcessEvents(currentEvent);
            }
        }

        private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
        {
            return _currentRoomNodeGraph.roomNodes.FirstOrDefault(roomNode => roomNode.rect.Contains(currentEvent.mousePosition));
        }

        private void ProcessRoomNodeGraphEvents(Event currentEvent)
        {
            switch (currentEvent.type)
            {
                case EventType.MouseDown:
                    ProcessMouseDownEvent(currentEvent);
                    break;
                case EventType.MouseDrag:
                    ProcessMouseDragEvent(currentEvent);
                    break;
                case EventType.MouseUp:
                    ProcessMouseUpEvent(currentEvent);
                    break;
            }
        }

        private void ProcessMouseUpEvent(Event currentEvent)
        {
            if (currentEvent.button == 1 && _currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
            {

                var roomNode = IsMouseOverRoomNode(currentEvent);

                if (roomNode != null)
                {
                    _currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildID(roomNode.id);
                    roomNode.AddParentID(_currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
                ClearLineDrag();
            }
        }

        private void ClearLineDrag()
        {
            _currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
            _currentRoomNodeGraph.linePosition = Vector2.zero;
            GUI.changed = true;
        }

        private void ProcessMouseDragEvent(Event currentEvent)
        {
            if (currentEvent.button == 1)
            {
                ProcessRightMouseDragEvent(currentEvent);
            }
        }

        private void ProcessRightMouseDragEvent(Event currentEvent)
        {
            if (_currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
            {
                DragConnectingLine(currentEvent.delta);
                GUI.changed = true;
            }
        }

        public void DragConnectingLine(Vector2 currentEventDelta)
        {
            _currentRoomNodeGraph.linePosition += currentEventDelta;
        }

        private void ProcessMouseDownEvent(Event currentEvent)
        {
            if (currentEvent.button == 1)
            {
                ShowContextMenu(currentEvent.mousePosition);
            }

            if (currentEvent.button == 1)
            {
                ClearLineDrag();
                ClearAllSelectedRoomNodes();
            }
        }

        private void ClearAllSelectedRoomNodes()
        {
            foreach (var roomNode in _currentRoomNodeGraph.roomNodes)
            {
                if (roomNode.isSelected)
                {
                    roomNode.isSelected = false;
                    GUI.changed = true;
                }
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
            if (_currentRoomNodeGraph.roomNodes.Count == 0)
            {
                CreateRoomNode(new Vector2(200f, 200f), _roomNodeTypeListSo.typeList.Find(x => x.isEntrance));
            }
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

            _currentRoomNodeGraph.OnValidate();
        }
    }
}
