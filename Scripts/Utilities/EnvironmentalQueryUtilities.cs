using UnityEngine;

namespace Utilities
{
    public static class EnvironmentalQueryUtilities
    {
        private static EnvironmentalQueriesData data;
        
        public static bool IsSightBlockedByObstacle(Vector2 pointA, Vector2 pointB)
        {
            if(!data) SetData();

            return Physics2D.Linecast(pointA, pointB, data.obstacleLayerMask);
        } 
        
        public static bool IsSightBlockedByCoverObstacle(Vector2 pointA, Vector2 pointB)
        {
            if(!data) SetData();

            return Physics2D.Linecast(pointA, pointB, data.coverLayerMask);
        }

        public static bool IsOnGround(Vector2 point)
        {
            if(!data) SetData();

            return Physics2D.OverlapPoint(point, data.groundLayerMask);
        }

        public static bool IsOnGround(Vector2 point, float radius)
        {
            if(!data) SetData();

            return Physics2D.OverlapCircle(point, radius, data.groundLayerMask);
        }
        
        public static bool IsInsideObstacle(Vector2 point)
        {
            if(!data) SetData();

            return Physics2D.OverlapPoint(point, data.obstacleLayerMask);
        }

        public static bool IsInsideJammer(Vector2 point)
        {
            if(!data) SetData();

            var groundCollider = Physics2D.OverlapPoint(point, data.groundLayerMask);

            return groundCollider && groundCollider.CompareTag("Jammer");
        }

        private static void SetData()
        {
            data = Resources.Load<EnvironmentalQueriesData>("Utilities/EnvironmentalQueriesData");
        }
    }
}