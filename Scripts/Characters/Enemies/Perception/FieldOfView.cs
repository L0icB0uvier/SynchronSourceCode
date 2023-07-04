using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Characters.Controls.Controllers.AIControllers.Enemies;
using Characters.Controls.Controllers.PlayerControllers;
using Gameplay.EnergySystem.EnergyProduction;
using Gameplay.PoweredObjects.ControlledPoweredObjects;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using Plugins.Custom_2D_Colliders.Scripts;
using SceneManagement.LevelManagement;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace Characters.Enemies.Perception
{
	[RequireComponent(typeof(EllipseCollider2D))]
	public class FieldOfView : MonoBehaviour
	{
		[FormerlySerializedAs("m_AIController")] [SerializeField][FoldoutGroup("References")]
		private EnemyAIController aiController;
		public EnemyAIController AIController => aiController;

		[SerializeField] private BoolVariable hicksStealthModeActive;
		
		[FormerlySerializedAs("m_EllipseCollider")] [SerializeField][FoldoutGroup("References")]
		private EllipseCollider2D ellipseCollider;

		[SerializeField]
		private Collider2D m_fovCollider;

		[SerializeField] private FOVSettings fovSettings;
		public FOVSettings FOVSettings => fovSettings;
		
		private float m_currentViewRadius;

		private float CurrentViewAngle { get; set; }
		
		[FoldoutGroup("Layer Masks")]
		public ContactFilter2D targetsContactFilter;

		[FoldoutGroup("Layer Masks")]
		public LayerMask obstacleMask;

		[FormerlySerializedAs("CoverObstacleMask")] [FoldoutGroup("Layer Masks")]
		public LayerMask coverObstacleMask;

		//[FoldoutGroup("FOV Settings")]
		//public float AngleVariationAmount = 20;
		[FoldoutGroup("Debug", 4)]
		public bool fovActive = true;

		private readonly List<GameObject> m_visibleTargets = new List<GameObject>();
		
		[FoldoutGroup("Debug")]
		public List<DistractingMachine> m_visibleMonitoredSystems = new List<DistractingMachine>();

		[FormerlySerializedAs("m_TargetsAcquired")] [SerializeField][ReadOnly][FoldoutGroup("Debug")]
		private List<GameObject> targetsAcquired = new List<GameObject>();
		public List<GameObject> TargetsAcquired => targetsAcquired;

		[FormerlySerializedAs("m_TargetsInAcquisition")] [SerializeField][ReadOnly][FoldoutGroup("Debug")]
		private List<AcquisitionInfo> targetsInAcquisition = new List<AcquisitionInfo>();

		[FormerlySerializedAs("m_SeeingSomething")] [SerializeField][ReadOnly][FoldoutGroup("Debug")]
		private bool seeingSomething;
		public bool SeeingSomething => seeingSomething;

		[FormerlySerializedAs("m_AnomalyLocation")] [SerializeField][ReadOnly][FoldoutGroup("Debug")]
		private Vector2 anomalyLocation;
		public Vector2 AnomalyLocation => anomalyLocation;

		private float m_radiusX = 1, m_radiusY = 2;

		private const float IsoRatio = 0.707f;
		public static float Ratio => IsoRatio;

		[FoldoutGroup("Debug")]
		public bool displayFOVMesh;
		
		[FoldoutGroup("FOVVisual")] public FOVVisualRequest[] fovVisualRequests = new FOVVisualRequest[2];

		private float m_meshOffsetDistanceY;

		[HideInInspector]
		public Vector3[] vertices;

		[HideInInspector]
		public int[] triangles;

		private int m_stepCount;
	
		private readonly List<ViewPointInfo> m_viewPoints = new List<ViewPointInfo>();
	
		private readonly RaycastHit2D[] m_viewCastHit = new RaycastHit2D[1];
		private readonly RaycastHit2D[] m_obstacleHit = new RaycastHit2D[1];
		private readonly RaycastHit2D[] m_coverHit = new RaycastHit2D[1];

		private readonly List<Collider2D> m_targetColliders = new List<Collider2D>();

		private bool m_quitingApp;
		private static readonly int color = Shader.PropertyToID("_Color");
		private static readonly int aboveVoidColor = Shader.PropertyToID("_AboveVoidColor");

		[SerializeField] private VoidEventChannelSO alertStartChannel;
		[SerializeField] private VoidEventChannelSO alertEndChannel;
		
		private void Reset()
		{
			m_currentViewRadius = fovSettings.viewRadius;
			ellipseCollider.ChangeRadius(m_currentViewRadius);
		}
		
		private void OnBecameVisible()
		{
			fovActive = true;
			displayFOVMesh = true;
		}

		private void OnBecameInvisible()
		{
			fovActive = false;
			displayFOVMesh = false;
		}

		private void OnApplicationQuit()
		{
			m_quitingApp = true;
			StopAllCoroutines();
		}
		
		private void Awake()
		{
			aiController = transform.root.GetComponentInChildren<EnemyAIController>();
			ellipseCollider = GetComponent<EllipseCollider2D>();
			m_fovCollider = GetComponent<Collider2D>();
			
			InitializeToSettingValues();
			InitializeMeshRenderer();
		}

		private void InitializeMeshRenderer()
		{
			foreach (var t in fovVisualRequests)
			{
				t.meshInfo.mesh = t.meshInfo.meshFilter.mesh;
				t.meshInfo.meshRenderer.sortingLayerName = "Perception";
			}
		}

		private void InitializeToSettingValues()
		{
			m_currentViewRadius = fovSettings.viewRadius;
			CurrentViewAngle = fovSettings.viewAngle;

			ellipseCollider.ratio = IsoRatio;
			ellipseCollider.radius = m_currentViewRadius;

			m_radiusX = m_currentViewRadius;
			m_radiusY = IsoRatio * m_currentViewRadius;

			m_meshOffsetDistanceY = fovSettings.meshOffsetDistance * IsoRatio;
			
			ChangeFOVColor();
		}

		private void OnEnable()
		{
			alertStartChannel.onEventRaised += SwitchToAlertFOV;
			alertEndChannel.onEventRaised += SwitchToDefaultFOV;
		}
		
		private void OnDisable()
		{
			if (m_quitingApp) return;
			
			alertStartChannel.onEventRaised -= SwitchToAlertFOV;
			alertEndChannel.onEventRaised -= SwitchToDefaultFOV;

			InitializeFOV();
		}
		
		private void Start()
		{
			ChangeFOVColor(0);
		}

		public void InitializeFOV()
		{
			RemoveAcquiredTargets();
			targetsInAcquisition.Clear();
			seeingSomething = false;
			InitializeToSettingValues();
		}

		private void RemoveAcquiredTargets()
		{
			for (var i = targetsAcquired.Count - 1; i >= 0; i--)
			{
				TargetLost(targetsAcquired[i]);
			}

			seeingSomething = false;
			targetsAcquired.Clear();
		}

		private void SwitchToDefaultFOV()
		{
			ChangeFOVColor(0);
			StartCoroutine(SetDefaultFOV());
		}

		private void SwitchToAlertFOV()
		{
			ChangeFOVColor(1);
			StartCoroutine(SetAlertFOV());
		}
		
		private void FixedUpdate()
		{
			if (!fovActive) return;
		
			UpdateVision();
		
			ManageAcquisition();
		}
		
		private void LateUpdate()
		{
			if (!displayFOVMesh) return;
		
			foreach (var request in fovVisualRequests)
			{
				MakeMesh(request);
			}
		}

		private void ManageAcquisition()
		{
			if (m_visibleTargets.Count == 0 && targetsAcquired.Count > 0)
			{
				for (var i = targetsAcquired.Count - 1; i >= 0; i--)
				{
					TargetLost(targetsAcquired[i]);
				}

				for (var i = targetsInAcquisition.Count - 1; i >= 0; i--)
				{
					UpdateAcquisitionProcess(targetsInAcquisition[i], false);
				}
			}

			else
			{
				//Remove all acquired targets that are not currently visible
				for (var i = targetsAcquired.Count - 1; i >= 0; i--)
				{
					if (!m_visibleTargets.Contains(targetsAcquired[i]))
					{
						TargetLost(targetsAcquired[i]);
					}
				}

				//Update acquisition processes
				for (var i = targetsInAcquisition.Count - 1; i >= 0; i--)
				{
					UpdateAcquisitionProcess(targetsInAcquisition[i],
						m_visibleTargets.Contains(targetsInAcquisition[i].targetInAcquisition));
				}

				targetsAcquired.OrderBy(x => (x.transform.position - transform.position).sqrMagnitude);
			}

			if (targetsInAcquisition.Count > 0 && AIController.CurrentArea.AreaStatus != EAreaStatus.Alert)
			{
				ChangeFOVColor(targetsInAcquisition.Max(x => x.acquisitionFactor));
			}
		}
		
		private void ChangeFOVColor(float i = 0)
		{
			foreach (var request in fovVisualRequests)
			{
				request.meshInfo.meshRenderer.material.SetColor(color, Color.Lerp(fovSettings.defaultColor, fovSettings.targetAcquiredColor, i));
				request.meshInfo.meshRenderer.material.SetColor(aboveVoidColor, Color.Lerp(fovSettings.aboveVoidDefaultColor, fovSettings.aboveVoidTargetAcquiredColor, i));
				request.meshInfo.meshRenderer.sortingOrder = Mathf.RoundToInt((1 - i) * 10);
			}
		}

		private IEnumerator SetAlertFOV()
		{
			float time = 0;
			while (time < fovSettings.fovChangeTime)
			{
				time += Time.deltaTime;
				var t = Mathf.Clamp01(time / fovSettings.fovChangeTime);
				m_currentViewRadius = Mathf.Lerp(fovSettings.viewRadius, fovSettings.alertViewRadius, t);
				RadiusChanged();
			
				CurrentViewAngle = Mathf.Lerp(fovSettings.viewAngle, fovSettings.alertViewAngle, t);
				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator SetDefaultFOV()
		{
		
			float time = 0;
			while (time < fovSettings.fovChangeTime)
			{
				time += Time.deltaTime;
				var t = Mathf.Clamp01(time / fovSettings.fovChangeTime);
				m_currentViewRadius = Mathf.Lerp(fovSettings.alertViewRadius, fovSettings.viewRadius,t);
				RadiusChanged();
			
				CurrentViewAngle = Mathf.Lerp(fovSettings.alertViewAngle, fovSettings.viewAngle, t);
				yield return new WaitForEndOfFrame();
			}
		}

		private void RadiusChanged()
		{
			m_radiusX = m_currentViewRadius;
			m_radiusY = m_currentViewRadius * IsoRatio;
		
			ellipseCollider.ChangeRadius(m_currentViewRadius);
		}
		
		//----------------------------------------------- Find target ---------------------------------------------//
		private void UpdateVision()
		{
			//CorrectViewAngle();

			m_visibleTargets.Clear();
			m_visibleMonitoredSystems.Clear();
			m_targetColliders.Clear();

			if(m_fovCollider.OverlapCollider(targetsContactFilter, m_targetColliders) == 0) return;

			if (m_targetColliders.Count == 0) return;
		
			foreach(var target in m_targetColliders)
			{
				switch (target.tag)
				{
					case "Hicks":
						if (IsCharacterVisible(target.gameObject))
						{
							m_visibleTargets.Add(target.gameObject);
						}
						break;
				
					case "Skullface":
						if (IsCharacterVisible(target.gameObject))
						{
							m_visibleTargets.Add(target.gameObject);
						}
						break;
				
					case "MonitoredSystem":
						var controlledElement = target.gameObject.GetComponent<DistractingMachine>();
						if (IsObjectVisible(controlledElement.transform, controlledElement.DetectionCollider.points))
						{
							if (m_visibleMonitoredSystems.Any(x => x.gameObject == target.gameObject)) continue;
							m_visibleMonitoredSystems.Add(controlledElement);
						}
						break;
				}
			}
		}

		private bool IsCharacterVisible(GameObject target)
		{
			Vector2 vectorToTarget = target.transform.position - transform.position;
			var angleToTarget = MathCalculation.ConvertDirectionToAngle(vectorToTarget.normalized);
					
			if (Math.Abs(CurrentViewAngle - 360) < .5f && CheckSightClear(target, vectorToTarget, angleToTarget)) return true;
		
			var deltaAngleToTarget = Mathf.Abs(Mathf.DeltaAngle(angleToTarget, aiController.LookingDirection));

			var leftAngle = MathCalculation.ClampAngle(AIController.LookingDirection + CurrentViewAngle / 2) * Mathf.Deg2Rad;

			var position = transform.position;
			var viewLeftLimitLocation = new Vector2(position.x + m_radiusX * Mathf.Cos(leftAngle), position.y + m_radiusY * Mathf.Sin(leftAngle));
			var leftFOVLimit = viewLeftLimitLocation - (Vector2)position;

			var deltaAngleToFOVLimit = Mathf.Abs(Mathf.DeltaAngle(aiController.LookingDirection, MathCalculation.ConvertDirectionToAngle(leftFOVLimit.normalized)));

			return deltaAngleToTarget < deltaAngleToFOVLimit && CheckSightClear(target.gameObject, vectorToTarget, angleToTarget);
		}

		private bool IsObjectVisible(Transform objectTransform, Vector2[] colliderPoints)
		{
			Vector2 position = transform.position;
		
			foreach (var point in colliderPoints)
			{
				Vector2 p = objectTransform.TransformPoint(point);
				Vector2 vectorToTarget = p - position;
				var angleToTarget = MathCalculation.ConvertDirectionToAngle(vectorToTarget.normalized);

				if (Math.Abs(CurrentViewAngle - 360) < .5f)
				{
					if(Physics2D.LinecastNonAlloc(p, position, m_obstacleHit, obstacleMask) == 0 || 
					   m_obstacleHit[0].transform.IsChildOf(objectTransform)) return true;
					continue;
				}
		
				var deltaAngleToTarget = Mathf.Abs(Mathf.DeltaAngle(angleToTarget, aiController.LookingDirection));

				var leftAngle = MathCalculation.ClampAngle(AIController.LookingDirection + CurrentViewAngle / 2) * Mathf.Deg2Rad;
			
				var viewLeftLimitLocation = new Vector2(position.x + m_radiusX * Mathf.Cos(leftAngle), position.y + m_radiusY * Mathf.Sin(leftAngle));
				var leftFOVLimit = viewLeftLimitLocation - position;

				var deltaAngleToFOVLimit = Mathf.Abs(Mathf.DeltaAngle(aiController.LookingDirection, MathCalculation.ConvertDirectionToAngle(leftFOVLimit.normalized)));

				if (deltaAngleToTarget > deltaAngleToFOVLimit) continue;
			
				if(Physics2D.LinecastNonAlloc(p, position, m_obstacleHit, obstacleMask) == 0 || 
				   m_obstacleHit[0].transform.IsChildOf(objectTransform)) return true;
			}

			return false;
		}

		private bool CheckSightClear(GameObject target, Vector2 vectorToTarget, float angleToTarget)
		{
			var distanceToTarget = vectorToTarget.sqrMagnitude;
			Vector2 position2D = transform.position;

			var a = angleToTarget * Mathf.Deg2Rad;

			var x = position2D.x + m_radiusX * Mathf.Cos(a);
			var y = position2D.y + m_radiusY * Mathf.Sin(a);

			var fovLimitLocation = new Vector2(x, y);
			var distanceToFOVMaxPoint = (fovLimitLocation - position2D).sqrMagnitude;

			if (IsTargetVisible(target.transform))
			{
				if (target.CompareTag("MonitoredSystem"))
				{
					return true;
				}
			
				if (targetsAcquired.Contains(target.gameObject)) return true;
			
				var targetDistanceFactor = 1 - (distanceToTarget / distanceToFOVMaxPoint);	
		
				var detectionDistanceFactor = MathCalculation.Remap(targetDistanceFactor,0, 1, fovSettings.distanceMinFactor, 1);
				var index = targetsInAcquisition.FindIndex(v => v.targetInAcquisition == target.gameObject);

				if (index >= 0)
				{
					targetsInAcquisition[index].distanceFactor = detectionDistanceFactor;
				}

				else
				{
					var newAcquisition = new AcquisitionInfo(target.gameObject) {distanceFactor = detectionDistanceFactor};
					targetsInAcquisition.Add(newAcquisition);
				}

				return true;
			}

			return false;
		}

		private bool IsTargetVisible(Transform target)
		{
			Vector2 position2D = transform.position;

			switch (target.tag)
			{
				case "Hicks":

					//Return false if their is an obstacle
					if (Physics2D.LinecastNonAlloc(position2D, target.position, m_obstacleHit, obstacleMask) > 0) return false;
				
					//Return true if there is no cover
					if (Physics2D.LinecastNonAlloc(position2D, target.position, m_coverHit, coverObstacleMask) == 0) return true;
				
					//Is player in stealthMode
					return !hicksStealthModeActive.Value;
			
				case "Skullface":
					return Physics2D.LinecastNonAlloc(position2D, target.position, m_obstacleHit, obstacleMask) == 0 && 
					       Physics2D.LinecastNonAlloc(position2D, target.position, m_coverHit, coverObstacleMask) == 0;
			
				case "MonitoredSystem":
					return Physics2D.LinecastNonAlloc(position2D, target.position, m_obstacleHit, obstacleMask) == 0 || m_obstacleHit[0].transform.IsChildOf(target);
				default:
					return false;
			}		
		}

		/// <summary>
		/// Will correct the angle size based on the direction facing in order to look more realistic with the isometric angle.
		/// </summary>
		//private void CorrectViewAngle()
		//{
		//	float directionModifier = Mathf.Abs((Mathf.Sin(m_AIController.LookingDirection * Mathf.Deg2Rad)));
		//	float angleCorrection = AngleVariationAmount * directionModifier;
		//	currentViewAngle = normalViewAngle + (angleCorrection);
		//}

		//Used in custom editor
		public Vector2 DirFromAngle(float angleInDegrees, bool angleIsGlobal) {
			if (!angleIsGlobal)
			{
				angleInDegrees += aiController.LookingDirection;
			}
			return new Vector2(Mathf.Cos((90 + angleInDegrees) * Mathf.Deg2Rad), Mathf.Sin((90 + angleInDegrees) * Mathf.Deg2Rad));
		}

		private void UpdateAcquisitionProcess(AcquisitionInfo acquisition, bool increment)
		{
			if (increment)
			{
				acquisition.acquisitionTime += Time.fixedDeltaTime * acquisition.distanceFactor;

				UpdateAcquisitionFactor(acquisition);

				if (acquisition.acquisitionFactor >= fovSettings.investigateFactor && acquisition.acquisitionFactor < 1)
				{
					seeingSomething = true;
					anomalyLocation = acquisition.targetInAcquisition.transform.position;
				}

				if (Math.Abs(acquisition.acquisitionFactor - 1) > .05f) return;
			
				OnTargetAcquired(acquisition);
				targetsInAcquisition.Remove(acquisition);
			}

			else
			{

				acquisition.acquisitionTime -= Time.fixedDeltaTime;

				UpdateAcquisitionFactor(acquisition);

				if (acquisition.acquisitionFactor <= fovSettings.investigateFactor)
				{
					seeingSomething = false;
				}

				if (acquisition.acquisitionFactor == 0)
				{
					OnAcquisitionFailed(acquisition);
				}
			}
		}

		private void UpdateAcquisitionFactor(AcquisitionInfo acquisition)
		{
			switch (AIController.CurrentArea.AreaStatus)
			{
				case EAreaStatus.Normal:
					acquisition.acquisitionFactor = Mathf.Clamp01(acquisition.acquisitionTime / fovSettings.defaultAcquisitionTime);
					break;

				case EAreaStatus.Vigilance:
					acquisition.acquisitionFactor = Mathf.Clamp01(acquisition.acquisitionTime / fovSettings.vigilanceAcquisitionTime);
					break;

				case EAreaStatus.Alert:
					acquisition.acquisitionFactor = Mathf.Clamp01(acquisition.acquisitionTime / fovSettings.alertAcquisitionTime);
					break;
			
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void OnTargetAcquired(AcquisitionInfo acquisition)
		{
			seeingSomething = false;
			targetsAcquired.Add(acquisition.targetInAcquisition);
		
			if (acquisition.targetInAcquisition.CompareTag("MonitoredSystem")) return;
		
			AIController.CurrentArea.IntruderVisualAcquired(acquisition.targetInAcquisition.GetComponent<PlayerController>(), transform.root.gameObject);
		}

		private void OnAcquisitionFailed(AcquisitionInfo acquisition)
		{
			seeingSomething = false;
			targetsInAcquisition.Remove(acquisition);
		}

		private void TargetLost(GameObject target)
		{
			AIController.CurrentArea.IntruderVisualLost(target.GetComponent<PlayerController>(), transform.root.gameObject);
			targetsAcquired.Remove(target);
		}
		
		private void MakeMesh(FOVVisualRequest request)
		{
			m_stepCount = Mathf.RoundToInt(CurrentViewAngle * fovSettings.meshRes);
			var stepAngle = CurrentViewAngle / m_stepCount;

			m_viewPoints.Clear();
			var oldViewCast = new ViewCastInfo();

			for (var i = 0; i <= m_stepCount; i++)
			{
				var angle = MathCalculation.ClampAngle((AIController.LookingDirection) - (CurrentViewAngle / 2) + (stepAngle * i));

				var newViewCast = ViewCast(angle, request.fieldVisualMask);

				if (newViewCast.hitDistanceBelowThreshold)
				{
					oldViewCast = newViewCast;
					continue;
				}

				if (i > 0 && i < m_stepCount - 1)
				{
					var edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > fovSettings.edgeDstThreshold;
				
					if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && edgeDstThresholdExceeded))
					{
						var edge = FindEdge(oldViewCast, newViewCast, request.fieldVisualMask);
						if (edge.pointA != Vector3.zero)
						{
							m_viewPoints.Add(new ViewPointInfo(edge.pointAAngle, edge.pointA, edge.pointADistanceFactor));
						}
						if (edge.pointB != Vector3.zero)
						{
							m_viewPoints.Add(new ViewPointInfo(edge.pointBAngle, edge.pointB, edge.pointBDistanceFactor));
						}
					}
				}

				m_viewPoints.Add(new ViewPointInfo(angle, newViewCast.point, newViewCast.dstFactor) );

				oldViewCast = newViewCast;
			}

			if (m_viewPoints.Count == 0)
			{
				return;
			}
		
			var vertexCount = m_viewPoints.Count * 2;

			vertices = new Vector3[vertexCount];
			triangles = new int[(vertexCount - 2) * 3];

			var uvs = new Vector2[vertices.Length];
		
			for (var i = 0; i <= vertexCount - 1; i++)
			{
				if(i % 2 == 0)
				{
					var position = transform.position;

					var a = m_viewPoints[i/2].viewPointAngle * Mathf.Deg2Rad;

					var x = position.x + fovSettings.meshOffsetDistance * Mathf.Cos(a);
					var y = position.y + m_meshOffsetDistanceY * Mathf.Sin(a);

					var fovStartPoint = new Vector2(x, y);

					vertices[i] = transform.InverseTransformPoint(fovStartPoint);

					uvs[i] = new Vector2(0, 0);
				}

				else
				{
					vertices[i] = transform.InverseTransformPoint(m_viewPoints[(i-1) / 2].viewPointPos);
					uvs[i] = new Vector2(m_viewPoints[(i-1) / 2].viewPointDstFactor, m_viewPoints[(i-1) / 2].viewPointDstFactor);
				}

				if (i >= vertexCount - 2) continue;
			
				if ((i * 3) % 6 == 0)
				{
					triangles[(i * 3)] = (i * 3) / 3;
					triangles[(i * 3) + 1] = ((i * 3) / 3) + 1;
					triangles[(i * 3) + 2] = ((i * 3) / 3) + 2;
				}

				else
				{
					triangles[(i * 3)] = triangles[(i * 3) - 1] + 1;
					triangles[(i * 3) + 1] = triangles[(i * 3)] - 1;
					triangles[(i * 3) + 2] = triangles[(i * 3)] - 2;
				}
			}

		
			{
				request.meshInfo.mesh.Clear();

				request.meshInfo.mesh.SetVertices(vertices);
				request.meshInfo.mesh.SetTriangles(triangles, 0);
				request.meshInfo.mesh.RecalculateNormals();
				request.meshInfo.mesh.uv = uvs;
			}
		}

		private EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast, LayerMask fieldVisualMask)
		{
			var minAngle = minViewCast.angle;
			var maxAngle = maxViewCast.angle;
			Vector2 minPoint = Vector2.zero;
			Vector2 maxPoint = Vector2.zero;
			float minPointDistanceFactor = 0;
			float maxPointDistanceFactor = 0; 
		
			Vector2 pos2D = transform.position;
		
			for (var i = 0; i < fovSettings.edgeResolveIterations; i++)
			{
				var angle = Mathf.LerpAngle(minAngle, maxAngle, .5f);
				var newViewCast = ViewCast(angle, fieldVisualMask);

				var edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > fovSettings.edgeDstThreshold;
				if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
				{
					minAngle = angle;
					minPoint = newViewCast.point;
				
					minPointDistanceFactor = FindViewPointDistanceFactor(minAngle, minPoint);
				}
				else
				{
					maxAngle = angle;
					maxPoint = newViewCast.point;
				

					maxPointDistanceFactor = FindViewPointDistanceFactor(maxAngle, maxPoint);
				}
			}

			return new EdgeInfo(minAngle, minPoint, minPointDistanceFactor,maxAngle, maxPoint, maxPointDistanceFactor);
		}
	
	
		private readonly struct EdgeInfo
		{
			public readonly float pointAAngle;
			public readonly Vector3 pointA;
			public readonly float pointADistanceFactor;
			public readonly float pointBAngle;
			public readonly Vector3 pointB;
			public readonly float pointBDistanceFactor;


			public EdgeInfo(float pointAAngle, Vector3 pointA, float pointADistanceFactor, float pointBAngle, Vector3 pointB, float pointBDistanceFactor)
			{
				this.pointAAngle = pointAAngle;
				this.pointA = pointA;
				this.pointADistanceFactor = pointADistanceFactor;
				this.pointBAngle = pointBAngle;
				this.pointB = pointB;
				this.pointBDistanceFactor = pointBDistanceFactor;
			}
		}

		private ViewCastInfo ViewCast(float globalAngle, LayerMask visualLayerMask)
		{
			Vector2 pos2D = transform.position;

			var a = globalAngle * Mathf.Deg2Rad;
		
			var x = pos2D.x + m_radiusX * Mathf.Cos(a);
			var y = pos2D.y + m_radiusY * Mathf.Sin(a);
		
			var maxPoint = new Vector2(pos2D.x + m_radiusX * Mathf.Cos(a), pos2D.y + m_radiusY * Mathf.Sin(a));
		
			if (Physics2D.LinecastNonAlloc(pos2D, maxPoint, m_viewCastHit, visualLayerMask) == 0)
			{
				return new ViewCastInfo(false, false, maxPoint, Vector2.Distance(pos2D, maxPoint), 1, globalAngle);
			}

			if (m_viewCastHit[0].distance * m_viewCastHit[0].distance <= (new Vector2(pos2D.x + fovSettings.meshOffsetDistance * Mathf.Cos(a), pos2D.y + m_meshOffsetDistanceY * Mathf.Sin(a)) - pos2D).sqrMagnitude)
			{
				return new ViewCastInfo(true, true, m_viewCastHit[0].point, m_viewCastHit[0].distance, 0, globalAngle);;
			}
		
			return new ViewCastInfo(true, false, m_viewCastHit[0].point, m_viewCastHit[0].distance, FindViewPointDistanceFactor(globalAngle, m_viewCastHit[0].point), globalAngle);
		}

		private readonly struct ViewCastInfo
		{
			public readonly bool hit;
			public readonly bool hitDistanceBelowThreshold;
			public readonly Vector3 point;
			public readonly float dst;
			public readonly float dstFactor;
			public readonly float angle;

			public ViewCastInfo(bool hit, bool hitDistanceBelowThreshold, Vector3 point, float dst, float dstFactor, float angle)
			{
				this.hit = hit;
				this.hitDistanceBelowThreshold = hitDistanceBelowThreshold;
				this.point = point;
				this.dst = dst;
				this.dstFactor = dstFactor;
				this.angle = angle;
			}
		}

		[Serializable]
		public readonly struct ViewPointInfo
		{
			public readonly float viewPointAngle;
			public readonly Vector2 viewPointPos;
			public readonly float viewPointDstFactor;

			public ViewPointInfo(float viewPointAngle, Vector2 viewPointPos, float viewPointDstFactor)
			{
				this.viewPointAngle = viewPointAngle;
				this.viewPointPos = viewPointPos;
				this.viewPointDstFactor = viewPointDstFactor;
			}
		}

		private float FindViewPointDistanceFactor(float angle, Vector2 point)
		{
			Vector2 pos2D = transform.position;
		
			var a = angle * Mathf.Deg2Rad;
		
			var fovStart = new Vector2(pos2D.x + fovSettings.meshOffsetDistance * Mathf.Cos(a), pos2D.y + m_meshOffsetDistanceY * Mathf.Sin(a));
			var fovEnd = new Vector2(pos2D.x + m_radiusX * Mathf.Cos(a), pos2D.y + m_radiusY * Mathf.Sin(a));
		
			return (point - fovStart).magnitude / (fovEnd - fovStart).magnitude;
		}


		[Serializable]
		public class AcquisitionInfo
		{
			public GameObject targetInAcquisition;
			public float acquisitionTime;
			public float distanceFactor;
			public float acquisitionFactor;

			public AcquisitionInfo(GameObject target)
			{
				targetInAcquisition = target;
			}
		}

		[Serializable]
		public class MeshInfo
		{
			public MeshRenderer meshRenderer;
			public MeshFilter meshFilter;
			[HideInInspector]
			public Mesh mesh;
		}

		[Serializable]
		public class FOVVisualRequest
		{
			public LayerMask fieldVisualMask;
			public MeshInfo meshInfo = new MeshInfo();
		}
	}
}




