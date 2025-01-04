using UnityEngine;

public enum SlotType { Red, Green, Blue }

public class SlotController : MonoBehaviour
{
      
    [SerializeField] float speed;
    Rigidbody2D rb;
    [SerializeField] SlotType slotType;
    bool firstOne = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + Vector2.down * speed * Time.fixedDeltaTime);
        if(rb.position.y <= GamblingManager.Instance.botEdge)
        {
            rb.position = new Vector2(rb.position.x, GamblingManager.Instance.topEdge + GamblingManager.Instance.yDistance);
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            float c, newC;
            switch(slotType)
            {
                case SlotType.Red:
                    c = spriteRenderer.color.r;
                    newC = ((c * 255 + GamblingManager.Instance.nSlots) % 255f) / 255f;
                    spriteRenderer.color = new Color(newC, 0, 0);
                    break;
                case SlotType.Green:
                    c = spriteRenderer.color.g;
                    newC = ((c * 255 + GamblingManager.Instance.nSlots) % 255f) / 255f;
                    spriteRenderer.color = new Color(0, newC, 0);
                    break;
                case SlotType.Blue:
                    c = spriteRenderer.color.b;
                    newC = ((c * 255 + GamblingManager.Instance.nSlots) % 255f) / 255f;
                    spriteRenderer.color = new Color(0, 0, newC);
                    break;
            }
        }
    }
}
