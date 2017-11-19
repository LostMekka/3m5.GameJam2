using UnityEngine;

public class Pickup : MonoBehaviour {

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 10);
    }
}
