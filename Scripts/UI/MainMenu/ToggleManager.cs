using System;
using GeneralScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleManager : MonoBehaviour
{
    [SerializeField] private BoolVariable toggleTarget;
    private Toggle m_toggle;

    private void Awake()
    {
        m_toggle = GetComponent<Toggle>();
    }

    private void OnEnable()
    {
        m_toggle.isOn = toggleTarget.Value;
    }
}
