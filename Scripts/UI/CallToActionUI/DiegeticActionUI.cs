using System;
using System.Collections;
using GeneralScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Utilities;

namespace UI.CallToActionUI
{
    public class DiegeticActionUI : ActionUI
    {
        [FormerlySerializedAs("text1")] public TMP_Text TMP_Component;

        private string m_currentText;

        [SerializeField] private Transform connectionInterfaceTransform;
        
        [SerializeField] private CanvasGroup canvasGroup;
        
        [SerializeField] private RectTransform canvasTransform;
        [SerializeField] private RectTransform iconTransform;
        [SerializeField] private HorizontalLayoutGroup horizontalGroup;

        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private FloatReference lineStartOffset;
        [SerializeField] private FloatReference lineEndOffset;
        
        private bool m_actionVisible;

        [SerializeField] private BoolVariableNotifyChange conversationActive;
        
        private void Awake()
        {
            HideUI();
            conversationActive.onValueChanged += OnConversationStatusChanged;
        }

        private void OnDestroy()
        {
            conversationActive.onValueChanged -= OnConversationStatusChanged;
        }

        private void OnConversationStatusChanged()
        {
            if (conversationActive.Value)
            {
                if (m_actionVisible)
                {
                    canvasGroup.alpha = 0;
                    lineRenderer.gameObject.SetActive(false);
                }
            }

            else
            {
                if (m_actionVisible)
                {
                    canvasGroup.alpha = 1;
                    lineRenderer.gameObject.SetActive(true);
                }
            }
        }

        private IEnumerator SetCanvasLocation(ActionUISettings actionUISetting)
        {
            yield return new WaitForSecondsRealtime(.1f);

            switch (actionUISetting.actionDisplayType)
            {
                case ActionUISettings.EActionDisplayType.CenteredTop:
                    horizontalGroup.reverseArrangement = false;
                    canvasTransform.anchoredPosition = GetDesiredLocation(actionUISetting);
                    break;
                case ActionUISettings.EActionDisplayType.Direction:
                    horizontalGroup.reverseArrangement = actionUISetting.iconOnTheRight;
                    canvasTransform.anchoredPosition = GetDesiredLocation(actionUISetting) + GetIconOffset();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            yield return null;
            
            if (actionUISetting.actionDisplayType == ActionUISettings.EActionDisplayType.Direction)
            {
                SetLinePosition();
            }
            
            ShowUI(actionUISetting.actionDisplayType == ActionUISettings.EActionDisplayType.Direction);
        }

        private void SetLinePosition()
        {
            var interfaceLocalPos = iconTransform.InverseTransformPoint(connectionInterfaceTransform.position);
            var dir = interfaceLocalPos.normalized;
            var endDir = dir * -1;
            
            Vector3[] linePos = new Vector3[2];
            linePos[0] = dir * lineStartOffset;
            linePos[1] =  interfaceLocalPos + endDir * lineEndOffset;

            lineRenderer.SetPositions(linePos);
        }

        private Vector2 GetDesiredLocation(ActionUISettings actionUISetting)
        {
            switch (actionUISetting.actionDisplayType)
            {
                case ActionUISettings.EActionDisplayType.CenteredTop:
                    return Vector2.up * actionUISetting.distanceFromParent;
                case ActionUISettings.EActionDisplayType.Direction:
                    return MathCalculation.ConvertAngleToDirection((int)actionUISetting.direction) * actionUISetting.distanceFromParent;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private Vector2 GetIconOffset()
        {
            float x;
            
            if (horizontalGroup.reverseArrangement)
            {
                var sizeDelta = canvasTransform.sizeDelta;
                x =  -(sizeDelta.x / 2 - (horizontalGroup.padding.right + iconTransform.sizeDelta.x / 2));
            }

            else
            {
                x = canvasTransform.sizeDelta.x / 2 - (horizontalGroup.padding.left + iconTransform.sizeDelta.x / 2);
            }
           
            return new Vector2(x, 0);
        }

        public override void ChangeText(string text, ActionUISettings actionUISettings)
        {
            if (m_currentText == text) return;
        
            m_currentText = text;
            TMP_Component.text = text;

            StartCoroutine(SetCanvasLocation(actionUISettings));

            //TextWriter.AddWriter_Static(text1, m_currentText, timePerCharacter, false, writeReverse, true, OnComplete);
        }

        private void ShowUI(bool showLine)
        {
            StopAllCoroutines();
            
            m_actionVisible = true;

            if (conversationActive.Value) return;
            
            canvasGroup.alpha = 1;
            lineRenderer.gameObject.SetActive(showLine);
        }
    
        public override void HideUI()
        {
            StopAllCoroutines();
            canvasGroup.alpha = 0;
            lineRenderer.gameObject.SetActive(false);
            m_actionVisible = false;
            m_currentText = null;
        }
    }
}