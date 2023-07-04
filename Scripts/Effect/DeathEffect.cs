using Cinemachine;
using GeneralScriptableObjects;
using GeneralScriptableObjects.Events;
using UnityEngine;
using UnityEngine.Events;

public class DeathEffect : MonoBehaviour
{
    [SerializeField] private FloatVariable deathSlowMotionTimeScale;
    [SerializeField] private SpriteRenderer deathEffectSpriteRenderer;
    [SerializeField] private SpriteRenderer hicksSpriteRenderer;
    
    [SerializeField] private Vector2EventChannelSO deathCameraEventChannel;
    [SerializeField] private ShowGameOverEventChannel showGameOverScreenChannel;
    [SerializeField] private VoidEventChannelSO gameOverChannel;
    [SerializeField] private FloatEventChannelSO turnOffMusicChannel;
    [SerializeField] private ChangeCameraBlendUpdateMethodEventChannel changeCameraBlendUpdateMethod;
    
    [SerializeField] private UnityEvent onDeathEffect;
    
    public void OnDeath()
    {
         Time.timeScale = deathSlowMotionTimeScale.Value;
         deathEffectSpriteRenderer.sprite = hicksSpriteRenderer.sprite;
         gameObject.SetActive(true);
         hicksSpriteRenderer.gameObject.SetActive(false);
         //changeCameraBlendUpdateMethod.RaiseEvent(CinemachineBrain.BrainUpdateMethod.LateUpdate);
         //deathCameraEventChannel.RaiseEvent(transform.root.position);
         turnOffMusicChannel.RaiseEvent(0);
         onDeathEffect?.Invoke();
    }

    //Called by animation event
    public void OnDeathEffectComplete()
    {
        showGameOverScreenChannel.RaiseEvent(hicksSpriteRenderer.sprite, true);
        gameOverChannel.RaiseEvent();
        //changeCameraBlendUpdateMethod.RaiseEvent(CinemachineBrain.BrainUpdateMethod.FixedUpdate);
        gameObject.SetActive(false);
        hicksSpriteRenderer.gameObject.SetActive(true);
        Time.timeScale = 1;
    }
}
