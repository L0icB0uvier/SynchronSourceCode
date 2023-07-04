using UnityEngine;

namespace Utilities
{
	public struct MathCalculation
	{
		private const float IsoRatio = .707f;
		public static bool ApproximatelyEqualFloat(float a, float b, float tolerance)
		{
			return (Mathf.Abs(a - b) < tolerance);
		}

		public static bool ApproximatelyEqualVector2(Vector2 a, Vector2 b, float tolerance)
		{
			return ((Mathf.Abs(a.x - b.x) < tolerance) && (Mathf.Abs(a.y - b.y) < tolerance));
		}

		public static bool AreAngleApproximatelyEqual(float angle1, float angle2, float tolerance)
		{
			var deltaAngle = Mathf.Abs(Mathf.DeltaAngle(angle1, angle2));
			return deltaAngle <= tolerance;
		}

		public static Vector2 RadianToVector2(float radian)
		{
			return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
		}

		public static Vector2 RadianToVector2(float radian, float length)
		{
			return RadianToVector2(radian) * length;
		}

		public static Vector2 ConvertAngleToDirection(float degree)
		{
			return RadianToVector2((degree) * Mathf.Deg2Rad);
		}

		public static float ConvertDirectionToAngle(Vector2 direction)
		{
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			angle = Mathf.RoundToInt(angle);

			if (angle < 0)
				angle += 360;

			return angle;
		}

		public static float Remap(float value, float from1, float to1, float from2, float to2)
		{
			return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
		}

		public static float ClampAngle(float angle)
		{
			if (angle < 0)
				angle += 360;

			if (angle > 360)
				angle -= 360;

			return angle;
		}

		public static Vector2 GetPointOnEllipse(Vector2 center, float radiusX, float angle)
		{
			var radiusY = radiusX * IsoRatio;
			var radian = angle * Mathf.Deg2Rad;
			return new Vector2(center.x + radiusX * Mathf.Cos(radian), center.y + radiusY * Mathf.Sin(radian));
		}
		
		public static Vector2 GetPointOnEllipse(Vector2 center, float radiusX, Vector2 direction)
		{
			var radiusY = radiusX * IsoRatio;
			var angle = ConvertDirectionToAngle(direction);
			var radian = angle * Mathf.Deg2Rad;
			return new Vector2(center.x + radiusX * Mathf.Cos(radian), center.y + radiusY * Mathf.Sin(radian));
		}

		public static Vector2 GetMiddlePositionBetween2Points(Vector2 pointA, Vector2 pointB)
		{
			return (pointA + pointB) / 2;
		}
	
		public static Vector2 GetDirectionalVectorBetween2Points(Vector2 pointA, Vector2 pointB)
		{
			return (pointB - pointA).normalized;
		}

		public static float GetAngleBetween2Points(Vector2 pointA, Vector2 pointB)
		{
			return ConvertDirectionToAngle(GetDirectionalVectorBetween2Points(pointA, pointB));
		}

		public static float CalculateTopVelocity(Rigidbody2D rb2d, Vector2 addedForce)
		{
			return  ((addedForce.magnitude / rb2d.drag) - Time.fixedDeltaTime * addedForce.magnitude) / rb2d.mass;
		}
	}
}
