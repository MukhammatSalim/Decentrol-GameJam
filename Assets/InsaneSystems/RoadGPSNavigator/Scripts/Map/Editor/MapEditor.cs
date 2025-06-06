﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace InsaneSystems.RoadNavigator
{
	public sealed class MapEditor : EditorWindow
	{
		const string navPointTemplatePath = "Unity/NavPoint";
		
		static Transform pointsParent;
		static bool isSomethingChanged;
		static bool selectOnlyPoints = true;
		static bool customMovePoints = false;

		readonly List<NavigatorCrossRoadPoint> selectedNavPoints = new List<NavigatorCrossRoadPoint>();

		NavigatorCrossRoadPoint pointA, pointB;

		Map map;

		List<NavigatorCrossRoadPoint> allPoints = new List<NavigatorCrossRoadPoint>();

		[MenuItem("Window/Insane Systems/Road GPS Navigator/Map Editor")]
		static void CreateEditorWindow()
		{
			var window = (MapEditor)EditorWindow.GetWindow(typeof(MapEditor), false, "Map Editor");
			window.maxSize = new Vector2(512f, 900f);
			window.minSize = new Vector2(380f, 400f);
			window.Show();
			window.Initialize();
		}

		void Initialize() => Refresh();

		void Refresh()
		{
			allPoints = FindObjectsOfType<NavigatorCrossRoadPoint>().ToList();
			Tools.hidden = selectOnlyPoints;
		}

		void OnInspectorUpdate() => Repaint();

		void OnGUI()
		{
			var logo = (Texture2D)Resources.Load("Unity/ISLargeLogo", typeof(Texture2D));
			GUILayout.Label(logo);

			if (Application.isPlaying)
				GUI.enabled = false;

			GUILayout.BeginHorizontal();
			
			EditorGUI.BeginChangeCheck();
			selectOnlyPoints = GUILayout.Toggle(selectOnlyPoints, "Points Selection mode");

			if (EditorGUI.EndChangeCheck())
				Refresh();
			
			if (map && map.MapStorage.enableAlphaFeatures)
				customMovePoints = GUILayout.Toggle(customMovePoints, "(Preview) Custom points Move mode");
			
			GUILayout.EndHorizontal();
			
			selectedNavPoints.Clear();

			if (!pointsParent)
			{
				var pointsParentComponent = FindObjectOfType<NavigatorPointsParent>();

				if (pointsParentComponent)
				{
					pointsParent = pointsParentComponent.transform;
				}
				else
				{
					EditorGUILayout.HelpBox("No points parent found on scene. Are this scene contains Map Prefab?", MessageType.Warning);
					return;
				}
			}

			if (!map)
			{
				map = FindObjectOfType<Map>();

				if (!map)
				{
					EditorGUILayout.HelpBox("No Map component found. Are this scene contains Map Prefab?", MessageType.Warning);
					return;
				}
			}

			if (map.MapMask && map.MapMask.enabled && !Application.isPlaying)
			{
				Debug.Log("[Road GPS Navigator] Map Mask component was disabled, because it can cause problem, when you can't see map image. No actions is needed, this is just inform message. Mask will be always enabled when game runs.");
				map.MapMask.enabled = false;
			}

			var sceneView = SceneView.sceneViews[0] as SceneView;

			if (sceneView && !sceneView.in2DMode)
			{
				EditorGUILayout.HelpBox("We recommend to work with Map in SceneView 2D Mode - it is good to work with UI Canvas.", MessageType.Info);

				if (GUILayout.Button("Enable 2D Mode"))
					sceneView.in2DMode = true;
			}
			
			if (Selection.gameObjects.Length > 0)
			{
				var selected = new List<GameObject>(Selection.gameObjects);
				var selectedNavPointsGameObjects = new List<GameObject>();

				selectedNavPointsGameObjects = selected.FindAll(obj => obj.GetComponent<NavigatorCrossRoadPoint>());

				for (int i = 0; i < selectedNavPointsGameObjects.Count; i++)
					selectedNavPoints.Add(selectedNavPointsGameObjects[i].GetComponent<NavigatorCrossRoadPoint>());
			}

			if (GUILayout.Button("Create point"))
				CreatePoint();

			HandleMainPoints();
			HandleABPoints();
			HandleObstacles();

			if (GUILayout.Button("Refresh (try it if see any problems)"))
				Refresh();

			if (!Application.isPlaying && isSomethingChanged)
			{
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
				isSomethingChanged = false;
			}
		}

		void HandleMainPoints()
		{
			GUI.enabled = selectedNavPoints.Count >= 2;
			
			GUILayout.BeginHorizontal();
			
			if (GUILayout.Button("Connect all selected points"))
			{
				for (int i = 0; i < selectedNavPoints.Count; i++)
				{
					var curPoint = selectedNavPoints[i];
					Undo.RecordObject(curPoint, "Changed Navigation Point settings");

					for (int j = 0; j < selectedNavPoints.Count; j++)
					{
						var otherPoint = selectedNavPoints[j];
						Undo.RecordObject(otherPoint, "Changed Navigation Point settings");

						if (otherPoint != curPoint && !curPoint.connectedPoints.Contains(otherPoint))
							curPoint.connectedPoints.Add(otherPoint);
					}
				}

				isSomethingChanged = true;
				SceneView.RepaintAll();
			}

			if (GUILayout.Button("Unconnect all selected points"))
			{
				for (int i = 0; i < selectedNavPoints.Count; i++)
				{
					var curPoint = selectedNavPoints[i];
					Undo.RecordObject(curPoint, "Changed Navigation Point settings");

					for (int j = 0; j < selectedNavPoints.Count; j++)
					{
						var otherPoint = selectedNavPoints[j];
						Undo.RecordObject(otherPoint, "Changed Navigation Point settings");

						if (curPoint.connectedPoints.Contains(otherPoint))
							curPoint.connectedPoints.Remove(otherPoint);
					}
				}

				isSomethingChanged = true;
				SceneView.RepaintAll();
			}
			
			GUILayout.EndHorizontal();

			if (!isSomethingChanged)
				return;
			
			foreach (var navigationPoint in selectedNavPoints)
				if (navigationPoint)
					EditorUtility.SetDirty(navigationPoint);
		}

		void HandleABPoints()
		{
			GUI.enabled = true;
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			var logo = (Texture2D)Resources.Load("Unity/EditorAB", typeof(Texture2D));
			GUILayout.Label(logo, GUILayout.ExpandWidth(false));
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			GUI.enabled = selectedNavPoints.Count == 2;
			pointA = null;
			pointB = null;

			var richButton = new GUIStyle(GUI.skin.button) { richText = true };

			if (GUI.enabled)
			{
				pointA = selectedNavPoints[0];
				pointB = selectedNavPoints[1];
			}

			if (pointA)
				Undo.RecordObject(pointA, "Changed Navigation Point settings");

			if (pointB)
				Undo.RecordObject(pointB, "Changed Navigation Point settings");

			if (GUILayout.Button("Connect <color=red>A</color> and <color=green>B</color> points", richButton))
			{
				if (!pointA.connectedPoints.Contains(pointB))
					pointA.connectedPoints.Add(pointB);
			
				if (!pointB.connectedPoints.Contains(pointA))
					pointB.connectedPoints.Add(pointA);

				isSomethingChanged = true;
				SceneView.RepaintAll();
			}

			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Connect only from <color=red>A</color> to <color=green>B</color>", richButton))
			{
				if (!pointA.connectedPoints.Contains(pointB))
					pointA.connectedPoints.Add(pointB);

				if (pointB.connectedPoints.Contains(pointA))
					pointB.connectedPoints.Remove(pointA);

				isSomethingChanged = true;
				SceneView.RepaintAll();
			}

			if (GUILayout.Button("Connect only from <color=green>B</color> to <color=red>A</color>", richButton))
			{
				if (pointA.connectedPoints.Contains(pointB))
					pointA.connectedPoints.Remove(pointB);

				if (!pointB.connectedPoints.Contains(pointA))
					pointB.connectedPoints.Add(pointA);

				isSomethingChanged = true;
				SceneView.RepaintAll();
			}
			GUILayout.EndHorizontal();

			if (GUILayout.Button("Create point between <color=red>A</color> and <color=green>B</color>", richButton))
			{
				var point = CreatePoint(false);

				point.transform.position = Vector3.Lerp(pointA.transform.position, pointB.transform.position, 0.5f);

				point.connectedPoints.Add(pointA);
				point.connectedPoints.Add(pointB);

				pointA.connectedPoints.Remove(pointB);
				pointB.connectedPoints.Remove(pointA);

				pointA.connectedPoints.Add(point);
				pointB.connectedPoints.Add(point);

				isSomethingChanged = true;
				SceneView.RepaintAll();
			}

			if (isSomethingChanged && pointA && pointB)
			{
				EditorUtility.SetDirty(pointA);
				EditorUtility.SetDirty(pointB);
			}
		}

		void HandleObstacles()
		{
			GUI.enabled = true;

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Obstacles");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			if (GUILayout.Button("Create obstacle collider"))
			{
				var spawned = new GameObject("Obstacle");
				spawned.AddComponent<Obstacle>();

				var obstaclesParent = pointsParent.Find("Obstacles");

				spawned.transform.parent = obstaclesParent != null ? obstaclesParent : pointsParent;
				spawned.transform.localScale = new Vector3(45, 3, 1);
				spawned.transform.position = spawned.transform.parent.transform.position;

				var selectionObjects = new GameObject[1];
				selectionObjects[0] = spawned;

				Selection.objects = selectionObjects;
				SceneView.lastActiveSceneView.FrameSelected();
			}
		}

		void OnSceneGUI(SceneView sceneView)
		{
			if (selectOnlyPoints)
				HandleSelectOnlyPointsMode(sceneView);

			var pointDrawRadius = 2f;
			
			if (selectedNavPoints.Count == 2 && pointA && pointB)
			{
				var color = Color.red;
				color.a = 0.4f;

				Handles.color = color;
				Handles.DrawSolidDisc(pointA.transform.position, Vector3.forward, pointDrawRadius);

				color = Color.green;
				color.a = 0.35f;

				Handles.color = color;
				Handles.DrawSolidDisc(pointB.transform.position, Vector3.forward, pointDrawRadius);
			}
			else
			{
				Color color = Color.red + Color.green / 2f;
				color.a = 0.4f;
				Handles.color = color;

				for (int i = 0; i < selectedNavPoints.Count; i++)
					if (selectedNavPoints[i] != null)
						Handles.DrawSolidDisc(selectedNavPoints[i].transform.position, Vector3.forward, 2f);
			}

			Handles.color = new Color(1f, 1f, 0.5f, 0.2f);
			
			foreach (var point in allPoints)
				if (point != null && !selectedNavPoints.Contains(point))
					Handles.DrawSolidDisc(point.transform.position, Vector3.forward, pointDrawRadius);
		}

		void HandleSelectOnlyPointsMode(SceneView sceneView)
		{
			DrawSelectOnlyTexts();

			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

			var e = Event.current;

			float scaler = 1f;
			
			#if UNITY_2017_1_OR_NEWER
			scaler = EditorGUIUtility.pixelsPerPoint;
			#endif
			
			var screenHeight = sceneView.camera.pixelRect.height / scaler;
			var mPos = e.mousePosition;
			mPos.y = screenHeight - mPos.y; // Idk why in unity in some places screen y inversed...
			
			if (e.button != 0)
				return;

			if (e.type == EventType.MouseDown)
			{
				//Selection.activeGameObject = null;

				foreach (var point in allPoints)
				{
					var pPos = sceneView.camera.WorldToScreenPoint(point.transform.position) / scaler;
					var dist = Vector2.Distance(mPos, pPos);
					
					if (dist < 10)
					{
						if (e.shift)
						{
							var list = Selection.objects.ToList();
							list.Add(point.gameObject);
							Selection.objects = list.ToArray();
						}
						else
						{
							Selection.activeGameObject = point.gameObject;
						}

						break;
					}
				}

				e.Use(); // Use the event so it doesn't propagate through the editor
			}

			if (customMovePoints && e.type == EventType.MouseDrag)
			{
				var mWorldPos = sceneView.camera.ScreenToWorldPoint(mPos);
				mWorldPos.z = 0;

				if (Selection.activeGameObject &&
				    Selection.activeGameObject.GetComponent<NavigatorCrossRoadPoint>())
				{
					Undo.RecordObject(Selection.activeGameObject.transform, "Move navigation point");
					Selection.activeGameObject.transform.position = mWorldPos;
				}
			}
		}

		void DrawSelectOnlyTexts()
		{
			Handles.BeginGUI();

			var styleText = new GUIStyle { normal = { textColor = Color.yellow } };
			var styleTextSub = new GUIStyle { normal = { textColor = new Color(0.75f, 0.75f, 0.75f) } };
			var styleTextMove = new GUIStyle { normal = { textColor = Color.red } };
			
			GUI.Label(new Rect(10, 10, 256, 32), "Points selection mode enabled", styleText);
			GUI.Label(new Rect(10, 30, 256, 32), "Use Shift for multi-select", styleTextSub);
				
			if (customMovePoints)
				GUI.Label(new Rect(10, 50, 256, 32), "Points Move mode enabled", styleTextMove);

			Handles.EndGUI();
		}

		NavigatorCrossRoadPoint CreatePoint(bool focusCamera = true)
		{
			var pointTpl = Resources.Load(navPointTemplatePath, typeof(GameObject)) as GameObject;

			GameObject spawned;
			NavigatorCrossRoadPoint navPoint = null;
			RectTransform rectTransform = null;

			if (pointTpl)
			{
				spawned = Instantiate(pointTpl, pointsParent);
				spawned.name = "Point";
				navPoint = spawned.GetComponent<NavigatorCrossRoadPoint>();
				rectTransform = spawned.GetComponent<RectTransform>();
			}
			else
			{
				spawned = new GameObject("Point");
				spawned.transform.parent = pointsParent;
			}

			if (!rectTransform)
				rectTransform = spawned.AddComponent<RectTransform>();
			if (!navPoint)
				navPoint = spawned.AddComponent<NavigatorCrossRoadPoint>();

			spawned.transform.position = spawned.transform.parent.transform.position;

			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.zero;

			var selectionObjects = new GameObject[1];
			selectionObjects[0] = spawned;

			Selection.objects = selectionObjects;

			if (focusCamera)
				SceneView.lastActiveSceneView.FrameSelected();

			allPoints.Add(navPoint);
			
			return navPoint;
		}

		void OnFocus()
		{
#if UNITY_2019_1_OR_NEWER
			SceneView.duringSceneGui -= OnSceneGUI;
			SceneView.duringSceneGui += OnSceneGUI;
#else
			SceneView.onSceneGUIDelegate -= OnSceneGUI;
			SceneView.onSceneGUIDelegate += OnSceneGUI;
#endif
		}

		void OnDestroy()
		{
#if UNITY_2019_1_OR_NEWER
			SceneView.duringSceneGui -= OnSceneGUI;
#else
			SceneView.onSceneGUIDelegate -= OnSceneGUI;
#endif
			Tools.hidden = false;
		}
	}
}