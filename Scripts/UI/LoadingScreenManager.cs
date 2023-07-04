using GeneralScriptableObjects.Events;
using UnityEngine;

namespace UI
{
    public class LoadingScreenManager : MonoBehaviour
    {
        [SerializeField] private BoolEventChannelSO _onToggleLoadingScreen;

        private CanvasGroup m_canvasGroup;

        private void Awake()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            _onToggleLoadingScreen.OnEventRaised += ToggleLoadingScreen;
        }

        private void OnDisable()
        {
            _onToggleLoadingScreen.OnEventRaised -= ToggleLoadingScreen;
        }

        private void ToggleLoadingScreen(bool enable)
        {
            m_canvasGroup.alpha = enable ? 1 : 0;
        }
    }
}
