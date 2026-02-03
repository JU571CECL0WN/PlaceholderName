using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    private DoorBehavior door;

    private void Awake()
    {
        door = GetComponentInParent<DoorBehavior>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (door == null) return;
        door.HandleTriggerEnter(collider);
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (door == null) return;
        door.HandleTriggerExit(collider);
    }
}