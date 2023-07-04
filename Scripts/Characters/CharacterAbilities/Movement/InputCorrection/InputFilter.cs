using GeneralEnums;
using GeneralScriptableObjects.Events;
using UnityEngine;

namespace Characters.CharacterAbilities.Movement.InputCorrection
{
    public abstract class InputFilter : MonoBehaviour, IInputFilter
    {
        public Transform ObjectTransform => transform;
        public bool IsOutOfRange { get; private set; }
        public EIsometricCardinal4DiagonalDirection OutOfRangeDirection { get; set; }
        
        [SerializeField] private OutOfRangeReachedEventChannel characterExitControlRangeChannel;
        [SerializeField] private VoidEventChannelSO characterEnterControlRangeChannel;
        

        private void Awake()
        {
            characterExitControlRangeChannel.OnEventRaised += ExitControlRangeChannel;
            characterEnterControlRangeChannel.onEventRaised += EnterControlRangeChannel;
        }

        private void OnDestroy()
        {
            characterExitControlRangeChannel.OnEventRaised -= ExitControlRangeChannel;
            characterEnterControlRangeChannel.onEventRaised -= EnterControlRangeChannel;
        }

        private void ExitControlRangeChannel(EIsometricCardinal4DiagonalDirection direction)
        {
            IsOutOfRange = true;
            OutOfRangeDirection = direction;
        }

        private void EnterControlRangeChannel()
        {
            IsOutOfRange = false;
        }

        public Vector2 CorrectInput(Vector2 input)
        {
            if (IsDirectionLocked(EIsometricCardinal4DiagonalDirection.NorthWest))
            {
                if (input.x < 0)
                {
                    input.x = 0;
                }

                if (input.y > 0)
                {
                    input.y = 0;
                }
            }

            if (IsDirectionLocked(EIsometricCardinal4DiagonalDirection.SouthWest))
            {
                if (input.x < 0)
                {
                    input.x = 0;
                }

                if (input.y < 0)
                {
                    input.y = 0;
                }
            }

            if (IsDirectionLocked(EIsometricCardinal4DiagonalDirection.NorthEast))
            {
                if (input.x > 0)
                {
                    input.x = 0;
                }

                if (input.y > 0)
                {
                    input.y = 0;
                }
            }

            if (IsDirectionLocked(EIsometricCardinal4DiagonalDirection.SouthEast))
            {
                if (input.x > 0)
                {
                    input.x = 0;
                }

                if (input.y < 0)
                {
                    input.y = 0;
                }
            }

            return input;
        }

        public abstract bool IsDirectionLocked(EIsometricCardinal4DiagonalDirection direction);
    }
}