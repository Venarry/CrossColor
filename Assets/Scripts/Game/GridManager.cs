using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum Colors
{
    Cross,
    Red,
    Blue,
    Green,
    Yellow,
    Black,
    White,
    Gray,
    Orange,
    Purple,
    Pink
}

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public GameObject cellPrefab;
    public int rows = 5;
    public int columns = 5;
    public int maxErrors = 3; 
    private int currentErrors = 0;
    public GameObject[] heartIcons;

    public int[,] levelData =
    {
        {1, 0, 0, 2, 0},
        {0, 1, 0, 2, 2},
        {0, 0, 1, 2, 0},
        {2, 2, 2, 2, 0},
        {0, 0, 0, 0, 1}
    };
    public Dictionary<string, Color> colorMap = new Dictionary<string, Color>
    {
        { "Red", Color.red },
        { "Blue", Color.blue },
        { "Green", Color.green },
        { "Yellow", Color.yellow },
        { "Black", Color.black },
        { "White", Color.white },
        { "Gray", Color.gray },
        { "Orange", new Color(1.0f, 0.5f, 0.0f) },
        { "Purple", new Color(0.5f, 0.0f, 0.5f) },
        { "Pink", new Color(1.0f, 0.4f, 0.7f) }
    };

    public Color[] colors = new Color[] {Color.clear, Color.red, Color.green, Color.gray};

    public LineColorInfo rowHintPrefab; 
    public LineColorInfo columnHintPrefab; 
    public Transform rowHintsParent; 
    public Transform columnHintsParent; 

    private int totalCellsToFill; 
    private int cellsFilledCorrectly; 

    [Header("Lose")]
    public GameObject loseGM;
    public event Action OnLose;

    [Header("Win")]
    public Sprite victorySprite;
    public GameObject winGM; 
    public event Action OnWin; 

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        loseGM.SetActive(false);
        winGM.SetActive(false);
        //GenerateGridWithHints();
    }

    public void GenerateGridWithHints()
    {
        float cellSize = 150f;
        float spacing = 5f;
        float totalWidth = (columns + 1) * cellSize + (columns) * spacing;
        float totalHeight = (rows + 1) * cellSize + (rows) * spacing;
        totalCellsToFill = 0;
        float startX = -totalWidth / 2f + cellSize / 2f;
        float startY = totalHeight / 2f - cellSize / 2f;
        colors = new Color[] { Color.clear, Color.red, Color.green, Color.gray };
        for (int j = 0; j < columns; j++)
        {
            var hint = GetColumnHint(j);
            LineColorInfo hintObject = Instantiate(columnHintPrefab, columnHintsParent);

            float posX = (cellSize + spacing) + startX + j * (cellSize + spacing);
            float posY = startY + spacing;

            hintObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);
            hintObject.Initialize(hint);
        }

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                var cell = Instantiate(cellPrefab, transform);
                var rectTransform = cell.GetComponent<RectTransform>();

                float posX = (cellSize + spacing) + startX + j * (cellSize + spacing);
                float posY = startY - (cellSize + spacing) - (i) * (cellSize + spacing);

                rectTransform.anchoredPosition = new Vector2(posX, posY);
                rectTransform.sizeDelta = new Vector2(cellSize, cellSize);

                var cellScript = cell.GetComponent<Cell>();
                cellScript.SetGridPosition(i, j);
                int colorIndex = levelData[i, j];

                if (colorIndex == 0)
                {
                    cellScript.correctColor = colors[3];
                }
                else
                {
                    cellScript.correctColor = colors[colorIndex];
                }

                cellScript.shouldBeFilled = colorIndex != 0;
                cell.name = $"Cell_{i}_{j}";

                cellScript.shouldBeFilled = colorIndex != 0;

                if (cellScript.shouldBeFilled)
                    totalCellsToFill++; 
            }
        }

        for (int i = 0; i < rows; i++)
        {
            var hint = GetRowHint(i);
            LineColorInfo hintObject = Instantiate(rowHintPrefab, rowHintsParent);
            var rectTransform = hintObject.GetComponent<RectTransform>();
            var textHelp = hintObject.GetComponent<LineColorInfo>();

            float posX = startX;
            float posY = startY - (cellSize + spacing) - i * (cellSize + spacing);

            rectTransform.anchoredPosition = new Vector2(posX, posY);
            textHelp.Initialize(hint);
        }
    }

    Dictionary<int, (int count, Color color)> GetRowHint(int row)
    {
        return GetHintForArray(levelData, row, true);
    }

    Dictionary<int, (int count, Color color)> GetColumnHint(int column)
    {
        return GetHintForArray(levelData, column, false);
    }

    Dictionary<int, (int count, Color color)> GetHintForArray(int[,] data, int index, bool isRow)
    {
        Dictionary<int, (int count, Color color)> colorData = new Dictionary<int, (int count, Color color)>();

        for (int i = 0; i < (isRow ? columns : rows); i++)
        {
            int colorIndex = isRow ? data[index, i] : data[i, index];
            if (colorIndex != 0)
            {
                if (!colorData.ContainsKey(colorIndex))
                    colorData[colorIndex] = (0, colors[colorIndex]);

                colorData[colorIndex] = (colorData[colorIndex].count + 1, colors[colorIndex]);
            }
        }

        return colorData;
    }

    public void OnPlayerError(Cell cell)
    {
        currentErrors++;
        UpdateHeartUI();

        if (currentErrors >= maxErrors)
        {
            OnLose?.Invoke();
            loseGM.SetActive(true);
            return;
        }

        StartCoroutine(ShowError(cell));
    }

    private void UpdateHeartUI()
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            heartIcons[i].SetActive(i < maxErrors - currentErrors);
        }
    }

    private IEnumerator ShowError(Cell cell)
    {
        Color originalColor = cell.image.color;

        cell.image.color = Color.black;
        yield return new WaitForSeconds(1.5f);
        cell.image.color = originalColor;
        cell.isFilledCorrectly = true;
        CheckVictory();
    }

    private void WinGame()
    {
        winGM.SetActive(true); 
    }

    public void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    public void OnCellFilled(Cell cell)
    {
        CheckVictory();
    }

    private bool IsVictory()
    {
        foreach (Transform cellTransform in transform)
        {
            var cell = cellTransform.GetComponent<Cell>();
            if (cell != null && cell.shouldBeFilled && !cell.isFilledCorrectly)
            {
                return false; 
            }
        }
        return true; 
    }

    private void CheckVictory()
    {
        if (IsVictory())
        {
            WinGame();
        }
    }

    
}
