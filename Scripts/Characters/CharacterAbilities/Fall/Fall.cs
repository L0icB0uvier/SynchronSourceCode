using System.Collections;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Characters.CharacterAbilities.Fall
{
    public class Fall : MonoBehaviour
    {
        private bool m_falling;
        private bool m_passedUnderGround;
        
        [SerializeField] private Rigidbody2D m_rb2d;
        [SerializeField] private SpriteRenderer m_spriteRenderer;
        [SerializeField] private float fallGravityScale = 2;

        [SerializeField] private string defaultSortingLayer = "Main";
        [SerializeField] private string fallSortingLayer = "UnderGround";
        
        [SerializeField] private int aboveCliffSortingOrder;
        [SerializeField] private int underCliffSortingOrder;
        [SerializeField] private LayerMask groundLayerMask;

        [SerializeField] private float changeOrderDelay = .3f;
        [SerializeField] private FloatVariable timeBeforeGameOver;

        [SerializeField] private VoidEventChannelSO gameOverChannel;
        [SerializeField] private ShowGameOverEventChannel showGameOverScreenEventChannel;
        
        
        private readonly Collider2D[] m_hit = new Collider2D[1];

        public UnityEvent onStartFalling;
        public UnityEvent onStopFalling;

        [SerializeField] private VoidEventChannelSO onStartFallingChannel;
        
        
        private void Awake()
        {
            m_rb2d = GetComponent<Rigidbody2D>();
        }
        
        public void StartFalling()
        {
            if (m_falling) return;
            
            m_rb2d.velocity = Vector2.zero;
            m_rb2d.drag = 0;
            m_rb2d.gravityScale = fallGravityScale;
            m_spriteRenderer.sortingLayerName = fallSortingLayer;
            m_spriteRenderer.sortingOrder = aboveCliffSortingOrder;
            onStartFalling?.Invoke();
            onStartFallingChannel.RaiseEvent();
            m_falling = true;
            m_passedUnderGround = false;
            StartCoroutine(WaitForGameOver());
        }

        private IEnumerator WaitForGameOver()
        {
            yield return new WaitForSecondsRealtime(timeBeforeGameOver.Value);
            onStopFalling?.Invoke();
            gameOverChannel.RaiseEvent();
            showGameOverScreenEventChannel.RaiseEvent(null, false);
        }

        private void Update()
        {
            if (!m_falling || m_passedUnderGround) return;
            
            if (Physics2D.OverlapCircleNonAlloc(transform.position, 0.5f, m_hit, groundLayerMask) > 0)
            {
                StartCoroutine(ChangeSpriteOrderAfterDelay());
            }
        }

        private IEnumerator ChangeSpriteOrderAfterDelay()
        {
            m_passedUnderGround = true;
            yield return new WaitForSeconds(changeOrderDelay);
            ChangeSpriteSortingOrder();
        }
        
        private void ChangeSpriteSortingOrder()
        {
            m_spriteRenderer.sortingOrder = underCliffSortingOrder;
        }
        
        public void DisableFalling()
        {
            m_rb2d.gravityScale = 0;
            m_falling = false;
            m_rb2d.velocity = Vector2.zero;
            m_spriteRenderer.sortingLayerName = defaultSortingLayer;
        }
    }
}