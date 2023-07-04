using System;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Characters.Enemies.Perception.Editor
{
	[CustomEditor(typeof (FieldOfView))]
	public class FieldOfViewEditor : OdinEditor
	{
		private const int CircleResolution = 36;

		private Vector3[] m_pointsLocation;

		private int[] m_segmentIndices;

		private FieldOfView m_fov;

		private Vector3 m_prevPos;

		private float m_previousRadius;
		private float m_previousRatio;

		private float m_radiusX = 1, m_radiusY = 2;
		
		private void Awake()
		{
			m_fov = (FieldOfView)target;

			m_pointsLocation = new Vector3[CircleResolution];
			m_prevPos = m_fov.transform.position;
			m_previousRadius = m_fov.FOVSettings.viewRadius;
			m_previousRatio = FieldOfView.Ratio;
			RecalculatePoints();
		}

		private void RecalculatePoints()
		{
			m_radiusX = m_fov.FOVSettings.viewRadius;
			m_radiusY = FieldOfView.Ratio * m_fov.FOVSettings.viewRadius;

			float ang = 0;

			for (int i = 0; i < CircleResolution; i++)
			{
				float a = ang * Mathf.Deg2Rad;

				var position = m_fov.transform.position;
				float x = position.x + m_radiusX * Mathf.Cos(a);
				float y = position.y - m_radiusY * Mathf.Sin(a);

				m_pointsLocation[i] = new Vector3(x, y, position.z);

				ang += 360f / CircleResolution;
			}

			m_segmentIndices = new int[CircleResolution * 2];
			var prevIndex = m_pointsLocation.Length - 1;
			var pointIndex = 0;
			var segmentIndex = 0;

			for (int i = 0; i < CircleResolution; i++)
			{
				// the index to the start of the line segment
				m_segmentIndices[segmentIndex] = prevIndex;
				segmentIndex++;

				// the index to the end of the line segment
				m_segmentIndices[segmentIndex] = pointIndex;
				segmentIndex++;

				pointIndex++;
				prevIndex = i;
			}
		}
		
		void OnSceneGUI()
		{
			Handles.color = Color.white;
			//Handles.DrawWireArc (fow.transform.position, Vector3.forward, Vector3.up, 360, fow.currentViewRadius);

			if(m_fov.transform.position != m_prevPos)
			{
				RecalculatePoints();
				m_prevPos = m_fov.transform.position;
			}

			if(Math.Abs(m_fov.FOVSettings.viewRadius - m_previousRadius) > .5f)
			{
				RecalculatePoints();
				m_previousRadius = m_fov.FOVSettings.viewRadius;
			}

			if(Math.Abs(FieldOfView.Ratio - m_previousRatio) > .5f)
			{
				RecalculatePoints();
				m_previousRatio = FieldOfView.Ratio;
			}

			Handles.DrawLines(m_pointsLocation, m_segmentIndices);

			float angle1 = m_fov.AIController.LookingDirection + m_fov.FOVSettings.viewAngle / 2;
			if (angle1 < 0)
				angle1 += 360;

			if (angle1 > 360)
				angle1 -= 360;

			float a = angle1 * Mathf.Deg2Rad;

			var position = m_fov.transform.position;
			float xA = position.x + m_radiusX * Mathf.Cos(a);
			float yA = position.y + m_radiusY * Mathf.Sin(a);

			Vector2 viewAngleLineA = new Vector2(xA, yA);

			var angle2 = m_fov.AIController.LookingDirection - m_fov.FOVSettings.viewAngle / 2;
			if (angle2 < 0)
				angle2 += 360;

			if (angle2 > 360)
				angle2 -= 360;

			a = angle2 * Mathf.Deg2Rad;

			float xB = position.x + m_radiusX * Mathf.Cos(a);
			float yB = position.y + m_radiusY * Mathf.Sin(a);
			Vector2 viewAngleLineB = new Vector2(xB, yB);

			Handles.DrawLine (position, viewAngleLineA);
			Handles.DrawLine (position, viewAngleLineB);
		}

	}
}
