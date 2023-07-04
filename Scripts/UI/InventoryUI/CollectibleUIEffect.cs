using System.Collections;
using Gameplay.Collectibles;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.InventoryUI
{
    public class CollectibleUIEffect : MonoBehaviour
    {
        [SerializeField] private Image m_spriteRenderer;

        private UnityAction<PickableItem> m_collectibleAcquiredAnimationCompleteCallback;
        private UnityAction<ItemContainer> m_collectibleStoredAnimationCompleteCallback;

        private ItemContainer m_item;
    
        [SerializeField] private RectTransform iconTransform;
        [SerializeField] private float moveToCenterTime = .5f;
        [SerializeField] private float moveToFinalPosTime = .5f;
        [SerializeField] private float waitAtCenter = .5f;
    
        [SerializeField] private float ItemContainerScale = 0;
        [SerializeField] private float centerScale = 3;
        [SerializeField] private float InventoryUIScale = 1;

        [SerializeField] private UnityEvent onItemAcquired;
        [SerializeField] private UnityEvent onItemUsed;
    

        private void Start()
        {
            iconTransform.gameObject.SetActive(false);
        }

        public void PlayCollectibleAcquiredAnimation(ItemContainer item, Vector2 itemStartPos, Vector2 inventoryIconScreenPos, 
            UnityAction<PickableItem> callback)
        {
            m_item = item;
            m_spriteRenderer.sprite = item.ContainerObjectType.collectibleIcon;
            m_collectibleAcquiredAnimationCompleteCallback = callback;

            StartCoroutine(AnimateCollectibleAcquired(itemStartPos, inventoryIconScreenPos));
        }
    
        public void PlayCollectibleUsed(ItemContainer item, Vector2 inventoryIconScreenPos, Vector2 itemEndPos, 
            UnityAction<ItemContainer> callback)
        {
            m_item = item;
            m_spriteRenderer.sprite = item.ContainerObjectType.collectibleIcon;
            m_collectibleStoredAnimationCompleteCallback = callback;
        
            StartCoroutine(AnimateCollectibleUsed(inventoryIconScreenPos, itemEndPos));
        }

        private IEnumerator AnimateCollectibleAcquired(Vector2 startPos, Vector2 iconPos)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, startPos, null, out var localStartPos);  
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, iconPos, null, out var localIconPos);

            iconTransform.anchoredPosition = localStartPos;
            iconTransform.gameObject.SetActive(true);
            onItemAcquired?.Invoke();

            yield return StartCoroutine(MoveIcon(localStartPos, Vector2.zero, ItemContainerScale, centerScale, moveToCenterTime));

            yield return new WaitForSeconds(waitAtCenter);
            onItemUsed?.Invoke();
        
            yield return StartCoroutine(MoveIcon(Vector2.zero, localIconPos, centerScale, InventoryUIScale, moveToFinalPosTime));
        
            iconTransform.gameObject.SetActive(false);
        
            m_collectibleAcquiredAnimationCompleteCallback?.Invoke(m_item.ContainerObjectType);
            m_item = null;
        }

        private IEnumerator MoveIcon(Vector2 startPos, Vector2 endPos, float startScale, float endScale, float time)
        {
            float t = 0;

            while (iconTransform.anchoredPosition != endPos)
            {
                t += Time.deltaTime;
                float f = Mathf.Clamp01(t / time);
                iconTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, f);
                Vector3 _startScale = new Vector3(startScale, startScale, 1);
                Vector3 _endScale = new Vector3(endScale, endScale, 1);
            
                iconTransform.localScale = Vector3.Lerp(_startScale, _endScale, f);
                yield return null;
            }
        }
    
        private IEnumerator AnimateCollectibleUsed(Vector2 iconPos, Vector2 endPos)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, iconPos, null, out var localIconPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, endPos, null, out var localEndPos);
        
            iconTransform.anchoredPosition = iconPos;
            iconTransform.gameObject.SetActive(true);
        
            onItemUsed?.Invoke();
        
            yield return StartCoroutine(MoveIcon(localIconPos, Vector2.zero, InventoryUIScale, centerScale, moveToCenterTime));

            yield return new WaitForSeconds(waitAtCenter);
        
            onItemAcquired?.Invoke();
        
            yield return StartCoroutine(MoveIcon(Vector2.zero, localEndPos, centerScale, ItemContainerScale, moveToFinalPosTime));
            iconTransform.gameObject.SetActive(true);
        
            m_collectibleStoredAnimationCompleteCallback?.Invoke(m_item);
            m_item = null;
        }
    }
}
