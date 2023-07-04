using UnityEngine;

namespace SavingSystem
{
    public abstract class Respawner : MonoBehaviour
    {
        protected Rigidbody2D rbd2;
        
        protected virtual void Awake()
        {
            rbd2 = GetComponent<Rigidbody2D>();
        }
    }
}