using UnityEngine;

namespace UI.CallToActionUI
{
    public abstract class ActionUI : MonoBehaviour
    {
        public abstract void ChangeText(string text, ActionUISettings actionUISettings);

        public abstract void HideUI();
    }
}