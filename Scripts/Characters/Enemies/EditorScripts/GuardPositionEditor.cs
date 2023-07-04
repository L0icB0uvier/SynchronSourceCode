using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/*
[CustomEditor(typeof(GuardPosition))]
public class GuardPositionEditor : Editor
{
	SerializedObject m_Object;

	SerializedProperty m_PositionToGuard;
	SerializedProperty m_WatchDirection;
	SerializedProperty m_LookAround;
	SerializedProperty m_RightRotationAngle;
	SerializedProperty m_LeftRotationAngle;
	SerializedProperty m_LookAroundSpeed;
	SerializedProperty m_LookAroundDelay;
	SerializedProperty m_DelayBetweenLookAround;


	public void OnEnable()
	{
		m_Object = new SerializedObject(target);

		m_PositionToGuard = m_Object.FindProperty("positionToGuard");
		m_WatchDirection = m_Object.FindProperty("watchDirection");
		m_LookAround = m_Object.FindProperty("lookAround");
		m_RightRotationAngle = m_Object.FindProperty("rightRotationAngle");
		m_LeftRotationAngle = m_Object.FindProperty("leftRotationAngle");
		m_LookAroundSpeed = m_Object.FindProperty("lookAroundSpeed");
		m_LookAroundDelay = m_Object.FindProperty("lookAroundDelay");
		m_DelayBetweenLookAround = m_Object.FindProperty("delayBetweenLookAround");
	}

	private void OnDisable()
	{
		SceneView.onSceneGUIDelegate = null;
	}
	
	private void OnSceneGUI()
	{
		Handles.color = Color.green;

		Handles.DrawSolidArc(new Vector3(m_PositionToGuard.vector2Value.x, m_PositionToGuard.vector2Value.y, 0), new Vector3(0, 0, -1), new Vector3(1f, 1f, 0f), 360, .2f);

		m_PositionToGuard.vector2Value = Handles.PositionHandle(m_PositionToGuard.vector2Value, Quaternion.identity);

		GUIStyle style = new GUIStyle();
		style.normal.textColor = Color.white;
		style.fontSize = 15;

		Handles.Label(m_PositionToGuard.vector2Value + new Vector2(2f, 2f), "Position to guard", style);

		Handles.color = Color.black;
		Handles.ArrowHandleCap(1, m_PositionToGuard.vector2Value, Quaternion.LookRotation(new Vector3(Mathf.Cos((90 + m_WatchDirection.floatValue) * Mathf.Deg2Rad), Mathf.Sin((90 + m_WatchDirection.floatValue) * Mathf.Deg2Rad), 0)), 3, EventType.Repaint);

		m_Object.ApplyModifiedProperties();
	}

	public override void OnInspectorGUI()
	{
		m_Object.Update();

		EditorGUILayout.PropertyField(m_PositionToGuard);
		EditorGUILayout.PropertyField(m_WatchDirection);

		GUILayout.Space(15);

		EditorGUILayout.PropertyField(m_LookAround);

		if (m_LookAround.boolValue)
		{
			EditorGUI.indentLevel++;

			EditorGUILayout.PropertyField(m_RightRotationAngle);
			EditorGUILayout.PropertyField(m_LeftRotationAngle);
			EditorGUILayout.PropertyField(m_LookAroundSpeed);
			EditorGUILayout.PropertyField(m_LookAroundDelay);
			EditorGUILayout.PropertyField(m_DelayBetweenLookAround);
		}


		m_Object.ApplyModifiedProperties();
	}

}*/
