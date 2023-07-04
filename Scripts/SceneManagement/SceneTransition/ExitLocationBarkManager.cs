using System;
using System.Collections;
using Characters.Controls.Controllers.PlayerControllers;
using PixelCrushers.DialogueSystem.Wrappers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SceneManagement.SceneTransition
{
    public class ExitLocationBarkManager : MonoBehaviour
    {
        [SerializeField] private ExitLocationBarkSettings settings;

        private DialogueSystemTrigger m_dialogSystemTrigger;
        private Transform m_hicksTransform;
        private Transform m_skullfaceTransform;
        
        private bool m_canPlayBark = true;
        private float m_timeSinceLastBark;

        private bool m_characterInTrigger;
        private EPlayerCharacterType m_currentCharacterType;

        private void Update()
        {
            if (m_canPlayBark != false || !settings.repeatBark) return;
            
            m_timeSinceLastBark += Time.deltaTime;

            if (!(m_timeSinceLastBark > settings.delayBeforeRepeat)) return;
            
            m_canPlayBark = false;

            if (!settings.repeatOnCharacterStay || !m_characterInTrigger) return;
            
            SetUpBarkTrigger(m_currentCharacterType);
            Bark();
        }

        private void Awake()
        {
            m_dialogSystemTrigger = GetComponent<DialogueSystemTrigger>();
            m_hicksTransform = GameObject.FindGameObjectWithTag("Hicks").transform;
            m_skullfaceTransform = GameObject.FindGameObjectWithTag("Skullface").transform;
        }
        
        public void OnCharacterEntered(EPlayerCharacterType characterType)
        {
            m_characterInTrigger = true;
            m_currentCharacterType = characterType;
            
            SetUpBarkTrigger(m_currentCharacterType);
            
            if (m_canPlayBark == false) return;

            StartCoroutine(BarkAfterDelay());
        }

        private void SetUpBarkTrigger(EPlayerCharacterType characterType)
        {
            switch (characterType)
            {
                case EPlayerCharacterType.Hicks:
                    switch (settings.exitBarkType)
                    {
                        case ExitLocationBarkSettings.EExitBarkType.CharacterInTrigger:
                            m_dialogSystemTrigger.barker = m_hicksTransform;
                            m_dialogSystemTrigger.barkConversation = GetBarkConversation(true);
                            break;
                        case ExitLocationBarkSettings.EExitBarkType.OtherCharacter:
                            m_dialogSystemTrigger.barker = m_skullfaceTransform;
                            m_dialogSystemTrigger.barkConversation = GetBarkConversation(false);
                            break;
                        case ExitLocationBarkSettings.EExitBarkType.Random:
                            var random = Random.Range(0, 2);
                            m_dialogSystemTrigger.barker = random == 0 ? m_hicksTransform : m_skullfaceTransform;
                            m_dialogSystemTrigger.barkConversation = GetBarkConversation(random == 0);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case EPlayerCharacterType.Skullface:
                    switch (settings.exitBarkType)
                    {
                        case ExitLocationBarkSettings.EExitBarkType.CharacterInTrigger:
                            m_dialogSystemTrigger.barker = m_skullfaceTransform;
                            m_dialogSystemTrigger.barkConversation = GetBarkConversation(true);
                            break;
                        case ExitLocationBarkSettings.EExitBarkType.OtherCharacter:
                            m_dialogSystemTrigger.barker = m_hicksTransform;
                            m_dialogSystemTrigger.barkConversation = GetBarkConversation(false);
                            break;
                        case ExitLocationBarkSettings.EExitBarkType.Random:
                            var random = Random.Range(0, 1);
                            m_dialogSystemTrigger.barker = random == 0 ? m_skullfaceTransform : m_hicksTransform;
                            m_dialogSystemTrigger.barkConversation = GetBarkConversation(random == 0);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(characterType), characterType, null);
            }
        }

        private string GetBarkConversation(bool insideTrigger)
        {
            switch (insideTrigger)
            {
                case true:
                    switch (settings.playTransitionImpossibleBark)
                    {
                        case true:
                            return "ExitImpossibleBark_InsideTrigger";
                        case false:
                            return "ExitBark_InsideTrigger";
                    }
                case false:
                    switch (settings.playTransitionImpossibleBark)
                    {
                        case true:
                            return "ExitImpossibleBark_OutsideTrigger";
                        case false:
                            return "ExitBark_OutsideTrigger";
                    }
            }
            
        }

        public void OnCharacterExited(EPlayerCharacterType characterType)
        {
            m_characterInTrigger = false;
            StopAllCoroutines();
        }

        private IEnumerator BarkAfterDelay()
        {
            yield return new WaitForSeconds(settings.randomDelay? Random.Range(settings.delayRange[0], settings.delayRange[1]) : settings.delay);
            Bark();
        }

        private void Bark()
        {
            m_canPlayBark = false;
            m_dialogSystemTrigger.OnUse();
            m_timeSinceLastBark = 0;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }
    }
}
