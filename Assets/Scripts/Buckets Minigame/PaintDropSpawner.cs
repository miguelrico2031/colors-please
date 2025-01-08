using System.Collections.Generic;
using UnityEngine;

public class PaintDropSpawner : MonoBehaviour
{
    [SerializeField] private GameObject paintDropPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private BucketSrc bucketSrc;
    [SerializeField] private int initAmount;
    [SerializeField] private int poolSize;
    [SerializeField] private int currentColorIndex;
    [SerializeField] private Color currentColor;

    private Queue<GameObject> paintDropPool = new Queue<GameObject>();

    private Color[] colors =
    {
        new RGB255(255, 0, 0).ToColor(),
        new RGB255(255, 255, 0).ToColor(),
        new RGB255(0, 0, 255).ToColor(),
        new RGB255(255, 255, 255).ToColor()
    };

    public static PaintDropSpawner Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }

    private void Start()
    {
        currentColorIndex = 3;
        currentColor = colors[currentColorIndex];

        for (int i = 0; i < poolSize; i++)
        {
            GameObject newPaintDrop = Instantiate(paintDropPrefab);
            newPaintDrop.SetActive(false);
            paintDropPool.Enqueue(newPaintDrop);
        }

        for (int i = 0; i < initAmount; i++)
        {
            SpawnPaintDrop();
        }
    }

    public void SpawnPaintDrop()
    {
        if (paintDropPool.Count > 0)
        {
            GameObject newPaintDrop = paintDropPool.Dequeue();

            if (newPaintDrop == null) return; // Evitar error al acabar el juego

            newPaintDrop.transform.position = spawnPoint.position + new Vector3(
                Random.Range(-0.15f, 0.15f),
                Random.Range(-0.15f, 0.15f),
                0);

            newPaintDrop.GetComponent<SpriteRenderer>().color = currentColor;
            newPaintDrop.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No hay más bolas disponibles en el pool");
            GameObject newPaintDrop = Instantiate(paintDropPrefab);
            newPaintDrop.transform.position = spawnPoint.position + new Vector3(
                Random.Range(-0.1f, 0.1f),
                Random.Range(-0.1f, 0.1f),
                0);

            newPaintDrop.GetComponent<SpriteRenderer>().color = currentColor;
            newPaintDrop.SetActive(true);
        }
    }

    public void ReturnToPool(GameObject paintDrop)
    {
        paintDrop.SetActive(false);
        paintDropPool.Enqueue(paintDrop);
    }

    public void ChangeBucketColor(int colorIndex)
    {
        if (colorIndex >= 0 && colorIndex < colors.Length)
        {
            currentColor = colors[colorIndex];

            bucketSrc.ChangeBallsInTriggerColor(currentColor);

            foreach (GameObject paintDrop in paintDropPool)
            {
                if (paintDrop.activeSelf)
                {
                    paintDrop.GetComponent<SpriteRenderer>().color = currentColor;
                }
            }
        }
        else
        {
            Debug.LogError("Índice de color inválido");
        }
    }
}
