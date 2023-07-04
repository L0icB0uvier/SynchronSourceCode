using Characters.Enemies.Weapons.Sentries;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace GeneralScriptableObjects.EnemyDataContainers
{
    [CreateAssetMenu(fileName = "SentryLaserAttackSettings", menuName = "ScriptableObjects/Settings/Enemies/SentryLaserAttackSettings", order = 0)]
    public class LaserAttackSettings : ScriptableObject
    {
        public ELaserAttackType laserType;
        
        [Range(0, 20)] public float laserAttackCooldown;
        
        [MinMaxSlider(0, 50, true)] public Vector2 laserAttackRange;
        
        public Vector2[] originLocation = new Vector2[8];
        
        public float laserStartOffset = 3;
        
        public float laserMovingSpeed = 1;
        
        [FormerlySerializedAs("straightLaserMaxDistance")] [ShowIf("ShowLaserMaxDistance")]
        public float lineLaserMaxDistance = 50;

        [ShowIf("ShowLaserContinueDistance")]
        public float followLaserTargetHitContinueDistance = 5;
        
        public LayerMaskVariable laserObstaclesLayerMask;

        private bool ShowLaserMaxDistance()
        {
            return laserType == ELaserAttackType.Line;
        } 
        
        private bool ShowLaserContinueDistance()
        {
            return laserType == ELaserAttackType.Follow;
        }
    }
}