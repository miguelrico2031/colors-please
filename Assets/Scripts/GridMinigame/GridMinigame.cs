using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;


public class GridMinigame : MonoBehaviour
{
    [Header("UI")]
    public GameObject cellPrefab;
    public Transform gridParent;
    public GameObject cursor;
    public TextMeshProUGUI selectedText;
    public TextMeshProUGUI titleText;
    public Image targetImage;
    public RectTransform canvasRect;

    [Header("Grid")]
    public int rows = 4;
    public int columns = 4;
    public float accelerationFactor = 3000f;
    public float friction = 2.6f;

    private Vector2 cursorPosition;
    private Vector2 cursorVelocity;
    private GameObject[,] gridCells;
    private string selectedHex = "#";
    private RGB255 targetColor;
    private RGB255 playerColor;

    public GameObject clickArea;
    //private bool isReady = false;

    void Start()
    {
        clickArea.GetComponent<ClickAreaScript>().pointerDown += SelectCharacter;

        targetColor = RGB255.Random();
        //Debug.Log($"color: {targetColor}");
        targetImage.color = targetColor.ToColor();

        CreateHexGrid();
        PositionTextsAndGrid();
        cursorPosition = new Vector2(Screen.width / 2, Screen.height / 2);
        UpdateCursorPosition();

        /* EL INPUT ANTIGUO 
         * if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }
        else
        {
            Debug.LogWarning("el movil no tiene giroscopio");
        }*/

        if (!SystemInfo.supportsGyroscope)
        {
            throw new System.Exception("Tu dispositivo no tiene giroscopio");
        }

        GetComponent<PlayerInput>().actions["Gyro"].Enable();

        //StartCoroutine(InitializeInput());
    }

    /*
    IEnumerator InitializeInput()
    {
        // Esperar a que se inicialice
        yield return null;
        Canvas.ForceUpdateCanvases();
        isReady = true;
        Debug.Log("Input listo.");
    }*/

    void Update()
    {
        //if (!isReady) return;
        Vector2 tilt = new Vector2(0,0);

        if (GravitySensor.current != null)
        {
            InputSystem.EnableDevice(GravitySensor.current);
            tilt = new Vector2(GravitySensor.current.gravity.ReadValue().x, GravitySensor.current.gravity.ReadValue().y);
        }

        Vector2 acceleration = tilt * accelerationFactor * Time.deltaTime; // * la aceleracion y el deltatime
        cursorVelocity += acceleration; // se aplica al cursor
        cursorVelocity = Vector2.Lerp(cursorVelocity, Vector2.zero, friction * Time.deltaTime); 
        cursorPosition += cursorVelocity * Time.deltaTime;

        // para que el cursor no se salga de la pantalla
        cursorPosition.x = Mathf.Clamp(cursorPosition.x, 0, Screen.width);
        cursorPosition.y = Mathf.Clamp(cursorPosition.y, 0, Screen.height);

        UpdateCursorPosition();
    }

    void PositionTextsAndGrid()
    {
        float canvasHeight = canvasRect.rect.height;
        float canvasWidth = canvasRect.rect.width;

        // proporciones de los 3 elementos en pantalla: Titulo, grid de casillas y marcador de color
        float titleHeightRatio = 0.10f;
        float markerHeightRatio = 0.10f;
        float gridHeightRatio = 0.80f;

        float titleSectionStart = canvasHeight / 2;
        float titleSectionEnd = canvasHeight / 2 - canvasHeight * titleHeightRatio;
        float markerSectionStart = -canvasHeight / 2 + canvasHeight * markerHeightRatio;
        float markerSectionEnd = -canvasHeight / 2;
        float gridHeight = canvasHeight * gridHeightRatio;

        // TITULO
        float titleY = (titleSectionStart + titleSectionEnd) / 2;
        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, titleY);
        titleRect.sizeDelta = new Vector2(canvasWidth * 0.9f, titleRect.sizeDelta.y);

        // MARCADOR
        float markerY = (markerSectionStart + markerSectionEnd) / 2;
        RectTransform markerRect = selectedText.GetComponent<RectTransform>();
        markerRect.anchoredPosition = new Vector2(0, markerY);

        // GRID
        RectTransform gridRect = gridParent.GetComponent<RectTransform>();
        gridRect.sizeDelta = new Vector2(canvasWidth, gridHeight);
        gridRect.anchoredPosition = new Vector2(0, (titleSectionEnd + markerSectionStart) / 2); // Centrado entre título y marcador

    }



    void UpdateCursorPosition()
    {
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

        float containerWidth = canvasRect.rect.width;
        float cellWidth = containerWidth / columns;
        float cellHeight = cellWidth;
        float cellSize = Mathf.Min(cellWidth, cellHeight);
        float spacingX = (containerWidth - (cellSize * columns)) / (columns - 1);
        float startY = (rows - 1) * (cellSize + spacingX) / 2f;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject cell = Instantiate(cellPrefab, gridParent);
                Vector3 cellPosition = new Vector3(
                    -containerWidth / 2f + cellSize / 2f + j * (cellSize + spacingX),
                    startY - i * (cellSize + spacingX),
                    0
                );
                RectTransform cellRect = cell.GetComponent<RectTransform>();
                cellRect.sizeDelta = new Vector2(cellSize, cellSize);
                cellRect.anchoredPosition = cellPosition;
                TextMeshProUGUI cellText = cell.GetComponentInChildren<TextMeshProUGUI>();
                cellText.text = hexChars[index % hexChars.Length];
                gridCells[i, j] = cell;
                index++;
            }
        }
    }

    void SelectCharacter()
    {
        Vector2 localCursor;
        RectTransform gridRect = gridParent.GetComponent<RectTransform>();
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(gridRect, cursorPosition, null, out localCursor))
        {
            Debug.LogWarning("Error en el paso de coordenadas del cursor");
            return;
        }


        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GameObject cell = gridCells[row, column];
                RectTransform cellRect = cell.GetComponent<RectTransform>();
                if (RectTransformUtility.RectangleContainsScreenPoint(cellRect, cursorPosition, null))
                {
                    string character = cell.GetComponentInChildren<TextMeshProUGUI>().text;
                    selectedHex += character;
                    selectedText.text = selectedHex;
                    //Debug.Log($"Caracter: {character}, posicion: [{row}, {column}]");
                    ServiceLocator.Get<IMusicService>().PlaySound("aceptar2");

                    if (selectedHex.Length == 7)
                    {
                        //Debug.Log($"Codigo: {selectedHex}");
                        playerColor = new RGB255(ColorUtility.TryParseHtmlString(selectedHex, out Color color) ? color : Color.black);
                        FinishMinigame();
                    }
                    return;
                }
            }
        }
        Debug.LogWarning("el cursor no esta en ninguna celda");
    }

    private void OnDisable()
    {
        clickArea.GetComponent<ClickAreaScript>().pointerDown -= SelectCharacter;
    }

    void FinishMinigame()
    {
        ServiceLocator.Get<IDayService>().FinishMinigame(targetColor, playerColor);
    }
}
