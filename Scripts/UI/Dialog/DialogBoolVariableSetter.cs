using System;
using GeneralScriptableObjects;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEngine;

public class DialogBoolVariableSetter : MonoBehaviour
{
    [SerializeField] private string variableName;
    [SerializeField] private BoolVariable value;

    private void OnEnable()
    {
        SetDialogVariable();
    }

    
    [Button]
    public void SetDialogVariable()
    {
        DialogueLua.SetVariable(variableName, value.Value);
    }
}
