using System.Linq;
using UnityEngine;

namespace Gameplay.Triggers
{
    public class SocketAttractor : MonoBehaviour
    {
        public float distanceThreshold = .1f;
        private PointEffector2D m_pointEffector2D;
        public float forceMagnitude;
        private CircleCollider2D m_collider;

        [SerializeField] private string[] attractedTags = new string[2];
        
        private void Awake()
        {
            m_pointEffector2D = GetComponent<PointEffector2D>();
            m_collider = GetComponent<CircleCollider2D>();
        }
        
        // Start is called before the first frame update
        private void OnTriggerStay2D(Collider2D other)
        {
            if (!IsTagAutorized(other.tag)) return;

            var radius1 = m_collider.radius;
            var radius = radius1 * radius1;
            var sqrDist = (other.gameObject.transform.position - transform.position).sqrMagnitude;

            var factor = sqrDist / radius;
            var force = Mathf.Lerp(-100, forceMagnitude, factor);
            m_pointEffector2D.forceMagnitude = force;
        
            if ((other.gameObject.transform.position - transform.position).sqrMagnitude < distanceThreshold)
            {
                m_pointEffector2D.forceMagnitude = 0;
            }
        }
        
        private bool IsTagAutorized(string tag)
        {
            return attractedTags.Contains(tag);
        }
    }
}
