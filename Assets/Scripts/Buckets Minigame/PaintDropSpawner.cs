using UnityEngine;

public class PaintDropSpawner : MonoBehaviour
{
    [SerializeField] private GameObject paintDropPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private int initAmount;

    private Color[] colors = { new RGB255(255, 0, 0).ToColor(), new RGB255(255, 255, 0).ToColor(),
        new RGB255(0, 0, 255).ToColor(), new RGB255(255, 255, 255).ToColor() };

    private void Start()
    {
        for(int i = 0; i < initAmount;  i++)
        {
            SpawnPaintDrop();
        }
    }

    private void SpawnPaintDrop()
    {
        Vector3 randomOffset = new Vector3(
        Random.Range(-0.1f, 0.1f),
        Random.Range(-0.1f, 0.1f),
        0);

        GameObject newPaintDrop = Instantiate(paintDropPrefab, spawnPoint.position + randomOffset, Quaternion.identity);
        newPaintDrop.GetComponent<SpriteRenderer>().color = colors[Random.Range(0, colors.Length-1)];
    }
}
