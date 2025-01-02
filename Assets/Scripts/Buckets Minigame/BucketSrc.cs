using UnityEngine;

public class BucketSrc : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Paint Drop"))
        {
            PaintDropSpawner.Instance.SpawnPaintDrop();
        }
    }

    public void ChangeBallsInTriggerColor(Color newColor)
    {
        Collider2D[] collidersInTrigger = Physics2D.OverlapBoxAll(
            transform.position,
            GetComponent<BoxCollider2D>().size,
            0f);

        foreach (Collider2D collider in collidersInTrigger)
        {
            if (collider.CompareTag("Paint Drop"))
            {
                collider.GetComponent<SpriteRenderer>().color = newColor;
            }
        }
    }
}
