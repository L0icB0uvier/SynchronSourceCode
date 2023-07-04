using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SceneSelectionTextColorChanger : MonoBehaviour
{
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color selectedColor;

    [SerializeField] private TMP_Text tmp;
    

    public void SetDefaultColor()
    {
        tmp.color = defaultColor;
    }

    public void SetSelectedColor()
    {
        tmp.color = selectedColor;
    }
    
}
