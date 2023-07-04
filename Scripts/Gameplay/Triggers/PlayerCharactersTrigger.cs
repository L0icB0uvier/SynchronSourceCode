using System;
using System.Collections.Generic;
using System.Linq;
using Characters.Controls.Controllers.PlayerControllers;
using Characters.Controls.Input.Actions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Gameplay.Triggers
{
    public class PlayerCharactersTrigger : MonoBehaviour
    {
        private readonly List<GameObject> m_playerControllers = new List<GameObject>();

        [FormerlySerializedAs("cameraTriggerType")] [SerializeField] private ECharacterTriggerType _characterTriggerType = ECharacterTriggerType.BothCharacters;
        
        public UnityEvent onCharactersInsideTrigger;
        public UnityEvent onCharactersOutsideTrigger;

        public UnityEvent<EPlayerCharacterType> onCharacterEnteredTrigger;
        public UnityEvent<EPlayerCharacterType> onCharacterExitedTrigger;
        

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Hicks") && !other.CompareTag("Skullface")) return;
            
            CharacterEnteredTrigger(other.gameObject);

            if (CheckTriggerCondition())
            {
                onCharactersInsideTrigger?.Invoke();
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Hicks") && !other.CompareTag("Skullface")) return;
            
            CharacterExitedTrigger(other.gameObject);

            if (!CheckTriggerCondition())
            {
                onCharactersOutsideTrigger?.Invoke();
            }
        }
        
        private bool CheckTriggerCondition()
        {
            switch (_characterTriggerType)
            {
                case ECharacterTriggerType.Hicks: 
                    if (m_playerControllers.Any(x => x.CompareTag("Hicks")))
                    {
                        return true;
                    }
                    break;
                case ECharacterTriggerType.Skullface:
                    if (m_playerControllers.Any(x => x.CompareTag("Skullface")))
                    {
                        return true;
                    }
                    break;
                case ECharacterTriggerType.BothCharacters:
                    if (m_playerControllers.Any(x => x.CompareTag("Hicks")) &&
                        m_playerControllers.Any(x => x.CompareTag("Skullface")))
                    {
                        return true;
                    }
                    break;
                case ECharacterTriggerType.AnyCharacter:
                    if (m_playerControllers.Any(x => x.CompareTag("Hicks")) ||
                        m_playerControllers.Any(x => x.CompareTag("Skullface")))
                    {
                        return true;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return false;
        }

        private void CharacterEnteredTrigger(GameObject characterGo)
        {
            m_playerControllers.Add(characterGo);

            switch (characterGo.tag)
            {
                case "Hicks":
                    onCharacterEnteredTrigger?.Invoke(EPlayerCharacterType.Hicks);
                    break;
                case "Skullface":
                    onCharacterEnteredTrigger?.Invoke(EPlayerCharacterType.Skullface);
                    break;
            }
        } 
        
        private void CharacterExitedTrigger(GameObject characterGo)
        {
            m_playerControllers.Remove(characterGo);
            
            switch (characterGo.tag)
            {
                case "Hicks":
                    onCharacterExitedTrigger?.Invoke(EPlayerCharacterType.Hicks);
                    break;
                case "Skullface":
                    onCharacterExitedTrigger?.Invoke(EPlayerCharacterType.Skullface);
                    break;
            }
        }
    }
    
    public enum ECharacterTriggerType {Hicks, Skullface, BothCharacters, AnyCharacter}
}
