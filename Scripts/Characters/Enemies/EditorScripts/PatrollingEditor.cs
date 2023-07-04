using System.Collections;
using UnityEngine;
using UnityEditor;

/*
[CustomEditor(typeof(Patrol))]
public class PatrollingEditor : Editor
{
	public string StringToEdit;

	int patrolPointCount = 0;
	int selectedPatrolPointIndex = 0;

	Vector3 unitPosition;

	SerializedObject m_Object;

	SerializedProperty m_PatrolPointCount;
	SerializedProperty m_WaitingTime;
	SerializedProperty m_LookAroundAngle;
	SerializedProperty m_LookAroundSpeed;
	SerializedProperty m_LookAroundDelay;
	SerializedProperty m_PatrolPointBehavior;

	public void OnEnable()
	{
		m_Object = new SerializedObject(target);

		m_PatrolPointCount = m_Object.FindProperty(kArraySizePath);
		m_WaitingTime = m_Object.FindProperty("waitingTime");
		m_LookAroundAngle = m_Object.FindProperty("rotationAngle");
		m_LookAroundSpeed = m_Object.FindProperty("lookAroundSpeed");
		m_LookAroundDelay = m_Object.FindProperty("lookAroundDelay");
		m_PatrolPointBehavior = m_Object.FindProperty("patrolPointReachedBehavior");

		SceneView.onSceneGUIDelegate = DrawSceneInspector;
		if (SceneView.lastActiveSceneView) SceneView.lastActiveSceneView.Repaint();
	}

	private void OnDisable()
	{
		SceneView.onSceneGUIDelegate = null;
	}

	private static string kArraySizePath = "patrolPoints.Array.size";
	private static string kArrayData = "patrolPoints.Array.data[{0}]";

	private Vector2[] GetPatrolPoints()
	{
		var arrayCount = m_PatrolPointCount.intValue;
		var vector2Array = new Vector2[arrayCount];

		for (var i = 0; i < arrayCount; i++)
			vector2Array[i] = m_Object.FindProperty(string.Format(kArrayData, i)).vector2Value;

		return vector2Array;
	}

	private void SetPatrolPoint (int index, Vector2 patrolPoint)
	{
		m_Object.FindProperty(string.Format(kArrayData, index)).vector2Value = patrolPoint;
	}

	private void insertPatrolPoint(int index)
	{
		for (int i = m_PatrolPointCount.intValue - 1; i >= index + 1; i--)
		{
			if (i == index + 1)
				SetPatrolPoint(i, (GetPatrolPointAtIndex(index) + GetPatrolPointAtIndex(index + 1))/2);

			else
				SetPatrolPoint(i, GetPatrolPointAtIndex(i - 1));
		}
	}

	private Vector2 GetPatrolPointAtIndex(int index)
	{
		return m_Object.FindProperty(string.Format(kArrayData, index)).vector2Value;
	}

	private void RemovePatrolPointAtIndex(int index)
	{
		if(m_PatrolPointCount.intValue == 1)
			m_PatrolPointCount.intValue = 0;

		else
		{
			for (int i = index; i < m_PatrolPointCount.intValue - 1; i++)
				SetPatrolPoint(i, GetPatrolPointAtIndex(i + 1));

			m_PatrolPointCount.intValue--;

			if (selectedPatrolPointIndex > 0)
				selectedPatrolPointIndex = selectedPatrolPointIndex - 1;
			else
				selectedPatrolPointIndex = m_PatrolPointCount.intValue - 1;
		}
	}

	public override void OnInspectorGUI()
	{
		m_Object.Update();

		unitPosition = Selection.activeGameObject.transform.position;

		GUILayout.Label("Patrolling Properties", EditorStyles.boldLabel);

		GUILayout.Label("Patrol Points", EditorStyles.boldLabel);
		var patrolPoints = GetPatrolPoints();

		EditorGUI.indentLevel ++;

		for (int i = 0; i < m_PatrolPointCount.intValue; i++)
		{	
			GUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Patrol point " + i, GUILayout.Width(100f));
			var result = EditorGUILayout.Vector2Field(GUIContent.none, patrolPoints[i]);

			if (GUI.changed)
				SetPatrolPoint(i, result);

			var oldEnabled = GUI.enabled;

			if (GUILayout.Button("-", GUILayout.Width(20f)))
				RemovePatrolPointAtIndex(i);

			GUILayout.EndHorizontal();
		}

		if (GUILayout.Button("Add Patrol Point"))
		{
			if(m_PatrolPointCount.intValue == 0)
			{
				m_PatrolPointCount.intValue++;
				SetPatrolPoint(0, unitPosition);
				selectedPatrolPointIndex = 0;
			}

			else if(m_PatrolPointCount.intValue == 1)
			{
				m_PatrolPointCount.intValue++;
				SetPatrolPoint(1, GetPatrolPointAtIndex(0) + new Vector2(2, 0));
				selectedPatrolPointIndex = 1;
			}

			else
			{
				m_PatrolPointCount.intValue++;
				SetPatrolPoint(m_PatrolPointCount.intValue - 1, (GetPatrolPointAtIndex(m_PatrolPointCount.intValue - 1) + GetPatrolPointAtIndex(0)) / 2);
				selectedPatrolPointIndex = m_PatrolPointCount.intValue - 1;
			}
		}

		GUILayout.Space(15f);
		EditorGUI.indentLevel--;
		EditorGUIUtility.labelWidth = EditorGUIUtility.currentViewWidth * .55f;

		EditorGUILayout.PropertyField(m_PatrolPointBehavior);

		EditorGUI.indentLevel++;
		switch (m_PatrolPointBehavior.enumValueIndex)
		{
			case 0:
				break;
			case 1:
				EditorGUILayout.PropertyField(m_WaitingTime);
				break;
			case 2:
				EditorGUILayout.PropertyField(m_LookAroundAngle);
				EditorGUILayout.PropertyField(m_LookAroundSpeed);
				EditorGUILayout.PropertyField(m_LookAroundDelay);
				break;
		}

		EditorGUI.indentLevel--;
		GUILayout.Space(15f);

		m_Object.ApplyModifiedProperties();
	}

	void OnSceneGUI()
	{
		Handles.color = Color.blue;
		patrolPointCount = 0;

		if (m_PatrolPointCount.intValue > 0)
		{
			for (int i = 0; i < m_PatrolPointCount.intValue; i++)
			{
				if (selectedPatrolPointIndex == i)
					Handles.color = Color.green;

				else Handles.color = Color.red;

				if (Handles.Button(GetPatrolPoints()[i], Quaternion.identity, .5f, .5f, Handles.RectangleHandleCap))
					selectedPatrolPointIndex = i;

				patrolPointCount++;

				Handles.DrawSolidArc(new Vector3(GetPatrolPoints()[i].x, GetPatrolPoints()[i].y, 0), new Vector3(0, 0, -1), new Vector3(1f, 1f, 0f), 360, .2f);

				StringToEdit = "Point " + (patrolPointCount - 1);

				GUIStyle style = new GUIStyle();
				style.normal.textColor = Color.white;
				style.fontSize = 15;


				Handles.Label(new Vector3(GetPatrolPoints()[i].x - 1.5f, GetPatrolPoints()[i].y - .5f, 0), StringToEdit, style);
				Handles.color = Color.red;
			}
		}

		if (m_PatrolPointCount.intValue > 0)
			SetPatrolPoint(selectedPatrolPointIndex, Handles.PositionHandle(GetPatrolPoints()[selectedPatrolPointIndex], Quaternion.identity));

		m_Object.ApplyModifiedProperties();
		
	}
	

	void DrawSceneInspector(SceneView scene)
	{
		Handles.BeginGUI();

		float screenHeight = SceneView.currentDrawingSceneView.position.size.y;

		if (m_PatrolPointCount.intValue > 0)
		{
			Vector2 selectedPatrolPoint = GetPatrolPoints()[selectedPatrolPointIndex];

			Vector2 addRemoveButtonSize = new Vector2(70, 20);

			Vector3 addRemoveButtonPos = new Vector3(selectedPatrolPoint.x, selectedPatrolPoint.y - 2.5f, 0);
			Vector3 addRemoveButtonScreenPos = SceneView.lastActiveSceneView.camera.WorldToScreenPoint(addRemoveButtonPos);

			// this prevents the GUI control from being drawn if you aren't looking at it
			if (addRemoveButtonScreenPos.z > 0)
			{
				Vector2 minusButtonPos = new Vector2(addRemoveButtonScreenPos.x - addRemoveButtonSize.x, screenHeight - addRemoveButtonScreenPos.y - addRemoveButtonSize.y);
				Vector2 plusButtonPos = new Vector2(addRemoveButtonScreenPos.x, screenHeight - addRemoveButtonScreenPos.y - addRemoveButtonSize.y);

				if (GUI.Button(new Rect(minusButtonPos, addRemoveButtonSize), "Remove"))
				{
					RemovePatrolPointAtIndex(selectedPatrolPointIndex);
				}

				if (GUI.Button(new Rect(plusButtonPos, addRemoveButtonSize), "Add"))
				{
					if (m_PatrolPointCount.intValue == 1)
					{
						m_PatrolPointCount.intValue++;
						SetPatrolPoint(1, GetPatrolPointAtIndex(0) + new Vector2(2, 0));
						selectedPatrolPointIndex = 1;
					}

					else if (selectedPatrolPointIndex == m_PatrolPointCount.intValue - 1)
					{
						m_PatrolPointCount.intValue++;
						SetPatrolPoint(m_PatrolPointCount.intValue - 1, (GetPatrolPointAtIndex(m_PatrolPointCount.intValue - 1) + GetPatrolPointAtIndex(0)) / 2);
						selectedPatrolPointIndex = selectedPatrolPointIndex + 1;
					}

					else
					{
						m_PatrolPointCount.intValue++;
						insertPatrolPoint(selectedPatrolPointIndex);
						selectedPatrolPointIndex = selectedPatrolPointIndex + 1;
					}
				}
			}

			Vector2 nextPreviousButtonSize = new Vector2(35, 20);
			Vector3 nextPreviousButtonPos = new Vector3(selectedPatrolPoint.x, selectedPatrolPoint.y - 4.5f, 0);
			Vector3 nextPreviousButtonScreenPos = SceneView.lastActiveSceneView.camera.WorldToScreenPoint(nextPreviousButtonPos);

			if (nextPreviousButtonScreenPos.z > 0)
			{
				Vector2 previousButtonPos = new Vector2(nextPreviousButtonScreenPos.x - nextPreviousButtonSize.x, screenHeight - nextPreviousButtonScreenPos.y - nextPreviousButtonSize.y);
				Vector2 nextButtonPos = new Vector2(nextPreviousButtonScreenPos.x, screenHeight - nextPreviousButtonScreenPos.y - nextPreviousButtonSize.y);
				char leftArrow = '\u21E6';
				char rightArrow = '\u21E8';

				if (GUI.Button(new Rect(previousButtonPos, nextPreviousButtonSize), leftArrow.ToString()))
				{
					if (selectedPatrolPointIndex == 0)
						selectedPatrolPointIndex = m_PatrolPointCount.intValue - 1;
					else
						selectedPatrolPointIndex--;
				}

				if (GUI.Button(new Rect(nextButtonPos, nextPreviousButtonSize), rightArrow.ToString()))
				{
					if (selectedPatrolPointIndex == m_PatrolPointCount.intValue - 1)
						selectedPatrolPointIndex = 0;
					else
						selectedPatrolPointIndex++;
				}
			}
		}

		else
		{
			Vector3 unitScreenPosition = SceneView.lastActiveSceneView.camera.WorldToScreenPoint(unitPosition);
			Vector2 buttonPosition = new Vector2(unitScreenPosition.x, screenHeight - unitScreenPosition.y);

			if (GUI.Button(new Rect(buttonPosition, new Vector2(100, 20)), "Start new Path"))
			{
				m_PatrolPointCount.intValue++;
				SetPatrolPoint(0, unitPosition);
				selectedPatrolPointIndex = 0;
			}
		}	

		Handles.EndGUI();
	}
}*/
