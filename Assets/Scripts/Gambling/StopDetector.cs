using UnityEngine;

public class StopDetector : MonoBehaviour
{
    [SerializeField] SlotType slotType;
    [SerializeField] Transform initPos;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        collider.transform.parent.SetPositionAndRotation(initPos.position, transform.rotation);
        GamblingManager.Instance.ColumnStop(collider.GetComponent<SpriteRenderer>().color, slotType);
    }
}
