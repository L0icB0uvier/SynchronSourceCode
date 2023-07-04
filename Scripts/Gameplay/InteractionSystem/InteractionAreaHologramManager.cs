using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class InteractionAreaHologramManager : MonoBehaviour
{
   [SerializeField] private Animator animator;
   private static readonly int interacterPresent = Animator.StringToHash("InteracterPresent");
   private static readonly int visible = Animator.StringToHash("Visible");


   public void ConnectionPortConnected()
   {
      animator.SetBool(visible, true);
   }

   public void ConnectionPortDisconnected()
   {
      animator.SetBool(visible, false);
   }

   public void InteracterEnter()
   {
      animator.SetBool(interacterPresent, true);
   }

   public void InteracterExit()
   {
      animator.SetBool(interacterPresent, false);
   }
}
