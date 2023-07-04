using Sirenix.OdinInspector;
using UnityEngine;

namespace Characters.Enemies.Perception
{
    [CreateAssetMenu(fileName = "FOVVisualSetting", menuName = "ScriptableObjects/Settings/FOVVisualSetting", order = 0)]
    public class FOVSettings : ScriptableObject
    {
        [FoldoutGroup("FOV Settings")][Range(3, 200)] public float viewRadius = 30;
	
        [FoldoutGroup("FOV Settings")][Range(3, 200)] public float alertViewRadius = 40;
        
        [FoldoutGroup("FOV Settings")] [Range(10, 360)] public float viewAngle = 70;

        [FoldoutGroup("FOV Settings")][Range(3, 200)] public float alertViewAngle = 80;

        [FoldoutGroup("FOV Settings")][Tooltip("How fast fov changes from normal to alert mode and reverse")] public float fovChangeTime;
        
        [FoldoutGroup("Acquisition Settings")] public float defaultAcquisitionTime = 1;
        
        [FoldoutGroup("Acquisition Settings")] public float vigilanceAcquisitionTime = .5f;
        
        [FoldoutGroup("Acquisition Settings")] public float alertAcquisitionTime = .25f;
        
	    [FoldoutGroup("Acquisition Settings")][Range(0, 1)] public float distanceMinFactor = .5f;
		
        [FoldoutGroup("Acquisition Settings")][Range(0, 1)] public float investigateFactor = .25f;
        
        [FoldoutGroup("FOVVisual")]
        [FoldoutGroup("FOVVisual/Color")] public Color defaultColor;
	
        [FoldoutGroup("FOVVisual/Color")] public Color aboveVoidDefaultColor;
	
        [FoldoutGroup("FOVVisual/Color")]public Color targetAcquiredColor;
	
        [FoldoutGroup("FOVVisual/Color")] public Color aboveVoidTargetAcquiredColor;
        
        [FoldoutGroup("FOVVisual/MeshSettings")] public float meshRes = .1f;

        [FoldoutGroup("FOVVisual/MeshSettings")] public int edgeResolveIterations;

        [FoldoutGroup("FOVVisual/MeshSettings")] public float edgeDstThreshold;
	
        [FoldoutGroup("FOVVisual/MeshSettings")] public float meshOffsetDistance = 3;
    }
}