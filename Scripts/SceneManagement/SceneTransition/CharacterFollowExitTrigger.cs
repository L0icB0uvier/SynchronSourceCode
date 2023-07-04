using Characters.Controls.Controllers.PlayerControllers;
using GeneralScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

public class CharacterFollowExitTrigger : MonoBehaviour
{
    [SerializeField] private BoolVariable skullfaceFollowing;
    [SerializeField] private FloatVariable transitionMaxDistance;
    
    private Transform m_hicksTransform;
    private Transform m_skullfaceTransform;

    private bool m_waitForSkullface;

    [SerializeField] private UnityEvent onStartExitTransition;
    
    private void Awake()
    {
        m_hicksTransform = GameObject.FindWithTag("Hicks").transform;
        m_skullfaceTransform = GameObject.FindWithTag("Skullface").transform;
    }

    public void OnCharacterEnter(EPlayerCharacterType characterType)
    {
        if (characterType == EPlayerCharacterType.Hicks && skullfaceFollowing)
        {
            if (CanTransit())
            {
                StartExitTransition();
            }

            else
            {
                m_waitForSkullface = true;
            }
        }
    }

    public void OnCharacterExited(EPlayerCharacterType characterType)
    {
        if (m_waitForSkullface && characterType == EPlayerCharacterType.Hicks)
        {
            m_waitForSkullface = false;
        }
    }

    private void FixedUpdate()
    {
        if (m_waitForSkullface && CanTransit())
        {
            StartExitTransition();
        }
    }

    private void StartExitTransition()
    {
        onStartExitTransition?.Invoke();
        m_waitForSkullface = false;
    }

    private bool CanTransit()
    {
        return (m_hicksTransform.position - m_skullfaceTransform.position).sqrMagnitude < transitionMaxDistance.Value
            * transitionMaxDistance.Value;
    }
}
