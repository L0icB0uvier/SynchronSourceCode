using GeneralScriptableObjects;
using UnityEngine;

public class PowerSocketManager : MonoBehaviour
{
    [SerializeField] private BoolVariable poweringSocket;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnergySocket"))
        {
            poweringSocket.SetValue(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("EnergySocket"))
        {
            poweringSocket.SetValue(false);
        }
    }
}
