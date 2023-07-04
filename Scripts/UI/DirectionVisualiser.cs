using GeneralScriptableObjects;
using UnityEngine;
using Utilities;

public class DirectionVisualiser : MonoBehaviour
{
    [SerializeField] private FloatVariable direction;
    [SerializeField] private FloatVariable distance;

    [SerializeField] private Transform visualiser;
    
    void Update()
    {
        var pos = MathCalculation.GetPointOnEllipse(Vector2.zero, distance.Value, direction.Value);
        visualiser.localPosition = pos;

        visualiser.eulerAngles = new Vector3(0, 0, direction.Value);
    }
}
