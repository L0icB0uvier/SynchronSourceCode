using System;
using GeneralScriptableObjects.Events;
using UnityEngine;
using UnityEngine.Events;

namespace UI.InventoryUI
{
    public class CollectibleUIIcon : MonoBehaviour
    {
        private Animator m_animator;
    
        private static readonly int usable = Animator.StringToHash("Usable");

        public BoolEventChannelSO collectibleUsableChannel;
        
        public UnityEvent onItemBecomeUsable;
        public UnityEvent onItemBecomeUnusable;

        public UnityEvent onIconHidden;

        private bool m_usable;

        private void Awake()
        {
            m_animator = GetComponent<Animator>();
            collectibleUsableChannel.OnEventRaised += CollectibleUsableChanged;
        }

        private void OnDestroy()
        {
            collectibleUsableChannel.OnEventRaised -= CollectibleUsableChanged;
        }

        private void OnEnable()
        {
            m_animator.SetBool(usable, m_usable);
        }

        private void OnDisable()
        {
            Initialize();
        }

        private void CollectibleUsableChanged(bool isUsable)
        {
            m_usable = isUsable;

            if (!gameObject.activeInHierarchy) return;
            
            m_animator.SetBool(usable, isUsable);
            if (isUsable)
            {
                onItemBecomeUsable?.Invoke();
            }

            else
            {
                onItemBecomeUnusable?.Invoke();
            }
        }
    
        public void ShowIcon(bool playAnimation)
        {
            gameObject.SetActive(true);
        
            if (!playAnimation)
            {
                m_animator.Play("CollectibleUIIdle");
            }
        }

        public void HideIcon()
        {
            CollectibleUsableChanged(false);
            
            Hide();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            onIconHidden?.Invoke();
        }

        private void Initialize()
        {
            m_animator.SetBool(usable, false);
        }
    }
}
