using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemSelectedObjectSetter : MonoBehaviour
{
    [SerializeField] private GameObject objectSelected;
    [SerializeField] private bool setOnEnabled = true;

    private void OnEnable()
    {
        if (setOnEnabled)
        {
            SetObjectSelected();
        }
    }

    private void SetObjectSelected()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(objectSelected);
    }
}
