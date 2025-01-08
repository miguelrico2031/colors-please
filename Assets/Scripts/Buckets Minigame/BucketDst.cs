using UnityEngine;

public class BucketDst : MonoBehaviour
{

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Paint Drop"))
        {
            RGB255 otherColor = new RGB255(other.GetComponent<SpriteRenderer>().color);
            //Destroy(other.gameObject);
            PaintDropSpawner.Instance.ReturnToPool(other.gameObject);

            BucketsMinigameManager.Instance.MixColor(otherColor);
        }

    }
}
