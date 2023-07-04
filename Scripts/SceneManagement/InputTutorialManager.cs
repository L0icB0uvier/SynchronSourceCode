using System;
using GeneralScriptableObjects;
using UnityEngine;

namespace SceneManagement
{
    public class InputTutorialManager : MonoBehaviour
    {
        [SerializeField] private InputTutorialState[] inputTutorialStates;
        [SerializeField] private BoolVariable showTutorial;
        
        private void Start()
        {
            foreach (var inputTutorialState in inputTutorialStates)
            {
                inputTutorialState.inputState.Value = !showTutorial.Value || inputTutorialState.stateValue;
            }
        }
    }

    [Serializable]
    public class InputTutorialState
    {
        public BoolVariable inputState;
        public bool stateValue;
    }
}