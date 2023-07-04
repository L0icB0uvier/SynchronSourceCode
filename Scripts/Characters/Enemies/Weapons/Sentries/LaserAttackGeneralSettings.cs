using UnityEngine;

namespace Characters.Enemies.Weapons.Sentries
{
    [CreateAssetMenu(fileName = "LaserAttackGeneralSettings", menuName = "ScriptableObjects/Settings/Enemies/LaserAttackGeneralSettings", order = 0)]
    public class LaserAttackGeneralSettings : ScriptableObject
    {
        public LayerMask groundLayerMask;
        public LayerMask obstaclesLayerMask;
        public LayerMask coverLayerMask;

        public float trailStartAlpha;
        public float trailEndAlpha;
        public Color trailStartColor;
        public Color trailEndColor;

        public float collisionRadius;
    }
}