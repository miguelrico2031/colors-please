using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridMinigame : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject cellPrefab;       // Prefab para cada celda
    public Transform gridParent;       // Contenedor para la cuadrícula
    public GameObject cursor;          // Cursor que se mueve
    public TextMeshProUGUI selectedText; // Texto (TextMeshPro) donde se muestra el código hexadecimal seleccionado
    public Image targetImage;          // Imagen donde se aplica el color objetivo
    public RectTransform canvasRect;   // RectTransform del Canvas principal

    [Header("Grid Settings")]
    public int rows = 4;               // Filas de la cuadrícula
    public int columns = 4;            // Columnas de la cuadrícula
    public float cellSize = 50f;       // Tamaño de cada celda (en píxeles)
    public float spacing = 5f;         // Espacio entre celdas
    public float accelerationFactor = 3000f; // Factor de aceleración por inclinación
    public float friction = 2.6f;             // Fricción para reducir la velocidad

    private Vector2 cursorPosition;         // Posición actual del cursor en pantalla
    private Vector2 cursorVelocity;         // Velocidad actual del cursor
    private GameObject[,] gridCells;        // Matriz para almacenar las celdas
    private string selectedHex = "#";       // Código hexadecimal construido por el jugador
    private RGB255 targetColor;             // Color objetivo
    private RGB255 playerColor;             // Color introducido por el jugador

    void Start()
    {
        // Generar un color objetivo aleatorio
        targetColor = RGB255.Random();
        Debug.Log($"Color objetivo generado: {targetColor}");

        // Aplicar el color objetivo a la imagen
        targetImage.color = targetColor.ToColor();

        // Crear la cuadrícula dinámica
        CreateHexGrid();

        // Inicializar el cursor en el centro
        cursorPosition = new Vector2(Screen.width / 2, Screen.height / 2);
        UpdateCursorPosition();

        // Configurar giroscopio
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }
        else
        {
            Debug.LogWarning("El dispositivo no soporta giroscopio.");
        }
    }

    void Update()
    {
        // Obtener inclinación del móvil
        Vector2 tilt = GetTilt();

        // Calcular aceleración basada en la inclinación
        Vector2 acceleration = tilt * accelerationFactor * Time.deltaTime;

        // Actualizar la velocidad del cursor
        cursorVelocity += acceleration;

        // Aplicar fricción para reducir la velocidad gradualmente
        cursorVelocity = Vector2.Lerp(cursorVelocity, Vector2.zero, friction * Time.deltaTime);

        // Actualizar la posición del cursor
        cursorPosition += cursorVelocity * Time.deltaTime;

        // Limitar el cursor dentro de los bordes de la pantalla
        cursorPosition.x = Mathf.Clamp(cursorPosition.x, 0, Screen.width);
        cursorPosition.y = Mathf.Clamp(cursorPosition.y, 0, Screen.height);

        // Actualizar la posición del cursor en el Canvas
        UpdateCursorPosition();

        // Detectar tap para seleccionar la casilla actual
        if (Input.GetMouseButtonDown(0))
        {
            SelectCharacter();
        }
    }

    Vector2 GetTilt()
    {
        if (SystemInfo.supportsGyroscope)
        {
            // Obtener inclinación del móvil en X e Y
            return new Vector2(Input.gyro.gravity.x, Input.gyro.gravity.y);
        }
        else
        {
            return Vector2.zero;
        }
    }

    void UpdateCursorPosition()
    {
        // Convertir coordenadas de pantalla a coordenadas locales del Canvas
        RectTransform cursorRect = cursor.GetComponent<RectTransform>();
        Vector2 canvasPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, cursorPosition, null, out canvasPos);
        cursorRect.anchoredPosition = canvasPos;
    }

    void CreateHexGrid()
    {
        string[] hexChars = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
        gridCells = new GameObject[rows, columns];
        int index = 0;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Instanciar la celda y configurar su posición
                GameObject cell = Instantiate(cellPrefab, gridParent);
                cell.transform.localPosition = new Vector3(
                    j * (cellSize + spacing),
                    -i * (cellSize + spacing),
                    0
                );

                // Configurar el carácter de la celda
                TextMeshProUGUI cellText = cell.GetComponentInChildren<TextMeshProUGUI>();
                cellText.text = hexChars[index % hexChars.Length];

                // Guardar la celda en la matriz
                gridCells[i, j] = cell;
                index++;
            }
        }
    }

    void SelectCharacter()
    {
        // Obtener la celda bajo el cursor
        Vector2 localCursor;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, cursorPosition, null, out localCursor);

        int x = Mathf.Clamp(Mathf.FloorToInt(localCursor.x / (cellSize + spacing)), 0, columns - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(-localCursor.y / (cellSize + spacing)), 0, rows - 1);

        GameObject cell = gridCells[y, x];
        string character = cell.GetComponentInChildren<TextMeshProUGUI>().text;

        // Agregar el carácter al código hexadecimal
        selectedHex += character;
        selectedText.text = selectedHex;

        // Verificar si el código está completo
        if (selectedHex.Length == 7)
        {
            Debug.Log($"Código hexadecimal seleccionado: {selectedHex}");

            // Convertir a RGB255
            playerColor = new RGB255(ColorUtility.TryParseHtmlString(selectedHex, out Color color) ? color : Color.black);

            // Finalizar el minijuego
            FinishMinigame();
        }
    }

    void FinishMinigame()
    {
        // Llamar al servicio con el color objetivo y el color del jugador
        ServiceLocator.Get<IDayService>().FinishMinigame(targetColor, playerColor);
    }
}
