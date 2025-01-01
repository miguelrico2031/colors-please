using UnityEngine;

public class BucketDst : MonoBehaviour
{
    [SerializeField] private BucketsMinigameManager manager;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Paint Drop"))
        {
            RGB255 otherColor = new RGB255(other.GetComponent<SpriteRenderer>().color);
            Destroy(other.gameObject);

            manager.MixColor(otherColor);
        }

    }
}
