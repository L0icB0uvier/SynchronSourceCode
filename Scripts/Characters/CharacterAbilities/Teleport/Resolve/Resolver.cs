using UnityEngine;

namespace Characters.CharacterAbilities.Teleport.Resolve
{
    public abstract class Resolver : MonoBehaviour, IResolver
    {
        protected Animator resolveAnimator;
        private SpriteRenderer m_resolveSprite;
        private static readonly int dissolvePower = Shader.PropertyToID("_DissolvePower");
        protected static readonly int LookingDirection = Animator.StringToHash("Direction");

        protected virtual void Awake()
        {
            resolveAnimator = GetComponent<Animator>();
            m_resolveSprite = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            Hide();
        }

        public void Initialise(Vector2 resolvePos)
        {
            transform.position = resolvePos;
            resolveAnimator.gameObject.SetActive(true);
            SetAnimatorParameters();
            m_resolveSprite.material.SetFloat(dissolvePower, 0);
            gameObject.SetActive(true);
        }

        protected abstract void SetAnimatorParameters();

        public void UpdateResolveValue(float value)
        {
            m_resolveSprite.material.SetFloat(dissolvePower, value);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}