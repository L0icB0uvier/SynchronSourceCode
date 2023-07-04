using System.Collections;
using System.Collections.Generic;
using GeneralScriptableObjects;
using UnityEngine;

public class BoolVariableInitialiser : MonoBehaviour
{
    [SerializeField] private BoolVariable[] boolVariablesToInitialise;
    
    
    void Start()
    {
        foreach (var boolVariable in boolVariablesToInitialise)
        {
            boolVariable.Value = false;
        }
    }

   
}
