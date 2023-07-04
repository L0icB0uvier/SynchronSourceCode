using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gameplay.InteractionSystem.Interactables;
using Gameplay.PoweredObjects.ControlledPoweredObjects;
using Pathfinding;
using UnityEngine;

public static class PathfindingUtilities
{
   static List<MovingPoweredSystemInteractable> accessibleSwitches = new List<MovingPoweredSystemInteractable>();

   private static List<Transform> accessibleEntrances = new List<Transform>();

   public static MovingPoweredSystemInteractable FindAccessibleInteractable(MovingPoweredSystem movingPoweredSystem, Vector2 unitPosition,
    GraphMask graphMask)
   {
      var nn = new NNConstraint();
      nn.graphMask = graphMask;
      nn.constrainWalkability = true;
      nn.walkable = true;

      var unitNode = AstarPath.active.GetNearest(unitPosition, nn).node;

      accessibleSwitches.Clear();
      
      foreach (var s in movingPoweredSystem.ControlledSystemInteractables)
      {
         var switchNode = AstarPath.active.GetNearest(s.transform.position, nn).node;
         if (unitNode.Area == switchNode.Area) accessibleSwitches.Add(s);
      }

      if (accessibleSwitches.Count == 0) return null;

      if (accessibleSwitches.Count > 1) accessibleSwitches = accessibleSwitches.OrderBy(x => ((Vector2)x.transform.position - unitPosition)
         .sqrMagnitude).ToList();
      
      return accessibleSwitches[0];
   }

   public static Transform FindAccessibleEntrance(PathBlockingMovingPoweredSystem movingPoweredSystem, Vector2 unitPosition,
      GraphMask graphMask)
   {
      var nn = new NNConstraint();
      nn.graphMask = graphMask;
      nn.constrainWalkability = true;
      nn.walkable = true;

      accessibleEntrances.Clear();

      var unitNode = AstarPath.active.GetNearest(unitPosition, nn).node;
      
      foreach (var e in movingPoweredSystem.elementEntrances)
      {
         var entranceNode = AstarPath.active.GetNearest(e.position, nn).node;
         if (unitNode.Area == entranceNode.Area) accessibleEntrances.Add(e);
      }
      
      if (accessibleEntrances.Count == 0) return null;
      
      if (accessibleEntrances.Count > 1) accessibleEntrances = accessibleEntrances.OrderBy(x => ((Vector2)x.position - unitPosition)
         .sqrMagnitude).ToList();
      
      return accessibleEntrances[0];
   }
   
   public static Vector2 GetNearestNavigableNode(Vector2 pos, GraphMask graphMask, uint area)
   {
      var nn = new NNConstraint
      {
         constrainWalkability = true,
         walkable = true,
         graphMask = graphMask,
         constrainArea = true,
         area = (int)area
      };

      var node = AstarPath.active.GetNearest(pos, nn);
      var nodePos = node.position;
      return nodePos;
   } 
   public static GraphNode GetNearestNavigableNode(Vector2 pos, GraphMask graphMask)
   {
      var nn = new NNConstraint
      {
         constrainWalkability = true,
         walkable = true,
         graphMask = graphMask,
      };

      var node = AstarPath.active.GetNearest(pos, nn).node;
      return node;
   }

   public static bool IsGraphAreaEqual(Vector2 start, Vector2 end, GraphMask graphMask)
   {
      var nn = new NNConstraint
      {
         constrainWalkability = true,
         walkable = true,
         graphMask = graphMask,
      };
      
      var n1 = AstarPath.active.GetNearest(start, nn).node;
      var n2 = AstarPath.active.GetNearest(end, nn).node;

      return n1.Area == n2.Area;
   }

   public enum EGraphType
   {
      Main,
      Area
   }
}
