using System;
using Characters.Controls.Controllers.PlayerControllers;
using GeneralScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.InputTutorial
{
    public class InputIcon : MonoBehaviour
    {
        [SerializeField] private TMP_Text prefixText;
        [SerializeField] private TMP_Text suffixText;

        [SerializeField] private Image image;

        [SerializeField] private Color aboveHicksColor;
        [SerializeField] private Color aboveSkullfaceColor;

        [SerializeField] private FloatVariable YOffset;

        [SerializeField] private RectTransform rectTransform;

        private UnityEngine.Camera m_cam;
        
        private Transform m_followTarget;
        private InputIconInfoSO m_currentInputIconInfo;

        private void Awake()
        {
            m_cam = UnityEngine.Camera.main;
        }

        private void FixedUpdate()
        {
            if (m_followTarget == null) return;

            SetWindowPosition();
        }

        private void SetWindowPosition()
        {
            var verticalExtent = m_cam.orthographicSize;
            var horizontalExtent = verticalExtent * Screen.width / Screen.height;

            var camPosition = m_cam.transform.position;
            var windowSizeDelta = rectTransform.sizeDelta;
            
            var maxX = camPosition.x + horizontalExtent - (windowSizeDelta.x / 2);
            var minX = camPosition.x - horizontalExtent + windowSizeDelta.x/2;
            var maxY = camPosition.y + verticalExtent - windowSizeDelta.y/2;
            var minY = camPosition.y - verticalExtent + windowSizeDelta.y/2;

            var targetPosition = m_followTarget.position;

            var xPos = Mathf.Clamp(targetPosition.x, minX, maxX);
            var yPos = Mathf.Clamp(targetPosition.y + YOffset.Value, minY, maxY);

            transform.position = new Vector3(xPos, yPos, 0);
        }

        public void SetupIcon(Transform target, InputIconInfoSO inputIconInfo)
        {
            m_currentInputIconInfo = inputIconInfo;
            m_followTarget = target;

            if (m_currentInputIconInfo.showPrefixText)
            {
                prefixText.text = m_currentInputIconInfo.prefixText;
                prefixText.gameObject.SetActive(true);
            }

            else
            {
                prefixText.gameObject.SetActive(false);
            }
            
            image.sprite = inputIconInfo.iconSprite;

            if (m_currentInputIconInfo.showActionName)
            {
                suffixText.text = m_currentInputIconInfo.actionText;
                suffixText.gameObject.SetActive(true);
            }

            else
            {
                suffixText.gameObject.SetActive(false);
            }
            
            SetColor(m_currentInputIconInfo.aboveCharacter);
        }

        private void SetColor(EPlayerCharacterType characterType)
        {
            switch (characterType)
            {
                case EPlayerCharacterType.Hicks:
                    prefixText.color = aboveHicksColor;
                    suffixText.color = aboveHicksColor;
                    image.color = aboveHicksColor;
                    break;
                case EPlayerCharacterType.Skullface:
                    prefixText.color = aboveSkullfaceColor;
                    suffixText.color = aboveSkullfaceColor;
                    image.color = aboveSkullfaceColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(characterType), characterType, null);
            }
            
        }
    }
}
