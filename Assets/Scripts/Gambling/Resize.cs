using UnityEngine;

public class Resize : MonoBehaviour
{
    [SerializeField] private float resizeFactor;
    [SerializeField] private bool changeMask;
    void Start()
    {
        ResizeFunc();
    }

    private void OnRectTransformDimensionsChange()
    {
        ResizeFunc();
    }

    void ResizeFunc()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer attached to this GameObject.");
            return;
        }

        // Obtén el tamaño del sprite
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        // Obtén las dimensiones del mundo que coinciden con la pantalla
        float worldScreenHeight = Camera.main.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        // Calcula el factor de escala necesario
        Vector3 scale = transform.localScale;
        scale.x = worldScreenWidth / spriteSize.x * resizeFactor;
        scale.y = worldScreenHeight / spriteSize.y * resizeFactor;
        if(scale.x < scale.y)
        {
            scale.y = scale.x;
        }
        else
        {
            scale.x = scale.y;
        }

        // Aplica la escala al sprite
        transform.localScale = scale;
        if(changeMask)
        {
            GameObject.Find("SpriteMask").transform.localScale = transform.localScale;
        }
    }
}
