using System;
using System.Collections;
using System.Collections.Generic;
using Characters.Controls.Controllers.PlayerControllers;
using GeneralScriptableObjects;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UI.InputTutorial
{
    public class InputTutorialTrigger : MonoBehaviour
    {
        [BoxGroup("Hide Condition")][SerializeField] private bool hideAfterDelay;
        [BoxGroup("Hide Condition")][ShowIf("hideAfterDelay")][Indent()] public float delay;
        [BoxGroup("Hide Condition")][SerializeField] private bool disableOnHide = true;
        
        [SerializeField] private UnityEvent onHide;
        
        [SerializeField] private InputIconInfoSO[] inputIconsInfo;
        [SerializeField] private AssetReference InputIconReference;

        [SerializeField] private BoolVariable showTutorialSetting;
        
        private readonly List<InputIcon> m_spawnedInputIcons = new List<InputIcon>();

        private bool m_iconOnScreen;

        private bool m_triggerDisabled;

        public void DisplayInputIcons()
        {
            if (showTutorialSetting.Value == false || m_triggerDisabled || m_iconOnScreen) return;
            
            m_iconOnScreen = true;
            InputIconReference.LoadAssetAsync<GameObject>().Completed += OnAssetReferenceLoaded;
        }
        
        private void WaitAndHide()
        {
            StartCoroutine(HideAfterDelay());
        }

        public void HideInputIcons()
        {
            onHide?.Invoke();

            for (int i = m_spawnedInputIcons.Count - 1; i >= 0; i--)
            {
                Destroy(m_spawnedInputIcons[i].gameObject);
                m_spawnedInputIcons.RemoveAt(i);
            }
            
            m_iconOnScreen = false;
            
            if (disableOnHide) DisableTutorialTrigger();
        }

        public void DisableTutorialTrigger()
        {
            if (m_iconOnScreen) HideInputIcons();
            
            m_triggerDisabled = true;
            gameObject.SetActive(false);
        }

        private void OnAssetReferenceLoaded(AsyncOperationHandle<GameObject> obj)
        {
            GameObject inputIconRef = obj.Result;
            foreach (var inputIconInfo in inputIconsInfo)
            {
                Transform target;
                
                switch (inputIconInfo.aboveCharacter)
                {
                    case EPlayerCharacterType.Hicks:
                        target = GameObject.FindWithTag("Hicks").transform;
                        break;
                    case EPlayerCharacterType.Skullface:
                        target = GameObject.FindWithTag("Skullface").transform;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                var inputIcon = Instantiate(inputIconRef, new Vector3(1000, 1000, 0) , quaternion.identity)
                .GetComponent<InputIcon>();

                inputIcon.SetupIcon(target, inputIconInfo);
                m_spawnedInputIcons.Add(inputIcon);
            }
            
            Addressables.Release(obj);

            if (hideAfterDelay)
            {
                WaitAndHide();
            }
        }
        
        private IEnumerator HideAfterDelay()
        {
            yield return new WaitForSeconds(delay);
            HideInputIcons();
        }
    }
}