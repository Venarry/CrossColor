using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LevelCellsSpawner : MonoBehaviour
{
    private const float CellsSpace = 100;
    private const float CellsOutline = 2;

    [SerializeField] private NonogramCell _nonogramCellPrefab;
    [SerializeField] private LineData _rowDataPrefab;
    [SerializeField] private LineData _columnDataPrefab;
    [SerializeField] private Transform _gridParent;
    [SerializeField] private GridLayoutGroup _gridLayout;
    [SerializeField] private Transform _rowsDataParent;
    [SerializeField] private Transform _columnsDataParent;

    private readonly List<NonogramCell> _spawnedCells = new();
    private ColorsDataSource _colorsDataSource;
    private RectTransform _gridRectTransform;
    private RectTransform _rowParentRectTransform;
    private RectTransform _columnParentRectTransform;
    private LevelData[] _levels;
    private int _activeLevelIndex = 0;

    public event Action<NonogramCell[]> Spawned;
    public event Action<string[]> ColorsChanged;

    public void Init(ColorsDataSource colorsDataSource, LevelData[] levels)
    {
        _colorsDataSource = colorsDataSource;

        _gridRectTransform = _gridLayout.GetComponent<RectTransform>();
        _rowParentRectTransform = _rowsDataParent.GetComponent<RectTransform>();
        _columnParentRectTransform = _columnsDataParent.GetComponent<RectTransform>();

        _activeLevelIndex = 0;
        _levels = levels;
    }

    /*public void SpawnLevel(int[,] levelData, LevelColorsSO levelColors)
    {
        LevelGrid levelGrid = new();
        levelGrid.Generate(levelData.Length, levelData.GetLength(0), CellsSpace - CellsOutline);

        for (int i = 0; i < levelData.GetLength(1); i++)
        {
            for (int j = 0; j < levelData.GetLength(0); j++)
            {
                NonogramCell cell = Instantiate(_nonogramCellPrefab, _gridParent);

                int winColorIndex = levelData[i, j];

                //cell.SetWinCondition(winColorIndex);

                Vector2 position = levelGrid.GetPosition(i, j);
                cell.transform.localPosition = position;

                _spawnedCells.Add(cell);
            }
        }

        //Spawned?.Invoke(_spawnedCells.ToArray());
    }*/

    public async Task SpawnLevel()
    {
        /*LevelGrid levelGrid = new();
        int elementCount = 0;
        int sideSize = 1;

        for (int i = 0; i < levelData.rows.Count; i++)
        {
            for (int j = 0; j < levelData.rows[i].data.Count; j++)
            {
                elementCount += levelData.rows[i].data[j].count;
            }

            elementCount += sideSize;
        }

        int columnCount = 0;

        for (int j = 0; j < levelData.rows[0].data.Count; j++)
        {
            columnCount += levelData.rows[0].data[j].count;
        }

        int firstRowElementsCount = columnCount + sideSize;

        levelGrid.Generate(elementCount + firstRowElementsCount, columnCount + sideSize, CellsSpace - CellsOutline);

        List<string> colorsStack = new();

        for (int i = 0; i < levelData.rows.Count; i++)
        {
            int xPositionIndex = 0;

            for (int j = 0; j < levelData.rows[i].data.Count; j++)
            {
                for (int k = 0; k < levelData.rows[i].data[j].count; k++)
                {
                    string colorKey = levelData.rows[i].data[j].color;
                    Vector2 position = levelGrid.GetPosition(i + sideSize, xPositionIndex + sideSize);
                    xPositionIndex++;

                    NonogramCell cell = Instantiate(_nonogramCellPrefab, _gridParent);
                    cell.SetWinCondition(colorKey);

                    cell.transform.localPosition = position;
                    _spawnedCells.Add(cell);

                    if (colorsStack.Contains(colorKey) == false)
                    {
                        colorsStack.Add(colorKey);
                    }
                }
            }
        }

        
        for (int i = 0; i < levelData.rows.Count; i++)
        {
            LineData rowData = Instantiate(_rowDataPrefab, _gridParent);
            rowData.Init(levelData.rows[i].data.Select(c => c.count).ToArray());

            rowData.transform.localPosition = levelGrid.GetPosition(i + 1, 0);
        }

        for (int i = 0; i < columnCount; i++)
        {
            LineData rowData = Instantiate(_rowDataPrefab, _gridParent);
            //rowData.Init(levelData.rows[i].data.Select(c => c.count).ToArray());

            rowData.transform.localPosition = levelGrid.GetPosition(0, i + 1);
        }*/

        LevelData levelData = _levels[_activeLevelIndex];
        _activeLevelIndex++;

        int columnCount = 0;

        for (int j = 0; j < levelData.rows[0].data.Count; j++)
        {
            columnCount += levelData.rows[0].data[j].count;
        }

        _gridLayout.constraintCount = columnCount;

        List<string> colorsStack = new();

        for (int i = 0; i < levelData.rows.Count; i++)
        {
            for (int j = 0; j < levelData.rows[i].data.Count; j++)
            {
                for (int k = 0; k < levelData.rows[i].data[j].count; k++)
                {
                    string colorKey = levelData.rows[i].data[j].color;

                    NonogramCell cell = Instantiate(_nonogramCellPrefab, _gridLayout.transform);
                    cell.SetWinCondition(colorKey);

                    _spawnedCells.Add(cell);

                    if (colorsStack.Contains(colorKey) == false)
                    {
                        colorsStack.Add(colorKey);
                    }
                }
            }
        }

        Vector2 cellsSize = _gridLayout.cellSize;

        for (int i = 0; i < levelData.rows.Count; i++)
        {
            LineData rowData = Instantiate(_rowDataPrefab, _rowsDataParent);
            rowData.Init(levelData.rows[i].data, _colorsDataSource);

            rowData.GetComponent<RectTransform>().sizeDelta = cellsSize;
        }

        for (int i = 0; i < columnCount; i++)
        {
            LineData rowData = Instantiate(_columnDataPrefab, _columnsDataParent);
            rowData.Init(levelData.columns[i].data, _colorsDataSource);

            rowData.GetComponent<RectTransform>().sizeDelta = cellsSize;
        }

        //LayoutRebuilder.ForceRebuildLayoutImmediate(gridRectTransform);
        //LayoutRebuilder.ForceRebuildLayoutImmediate(rowParentRectTransform);
        //LayoutRebuilder.ForceRebuildLayoutImmediate(columnParentRectTransform);

        while (_gridRectTransform.sizeDelta.x == 0)
        {
            await Task.Yield();
        }

        float sizeDivider = 2;

        float rowDataXPosition = -_gridRectTransform.sizeDelta.x / sizeDivider - _rowParentRectTransform.sizeDelta.x / sizeDivider;
        _rowsDataParent.localPosition = new Vector2(rowDataXPosition, 0);

        float columnDataYPosition = _gridRectTransform.sizeDelta.y / sizeDivider + _columnParentRectTransform.sizeDelta.y / sizeDivider;
        _columnsDataParent.localPosition = new Vector2(0, columnDataYPosition);

        Spawned?.Invoke(_spawnedCells.ToArray());
        ColorsChanged?.Invoke(colorsStack.ToArray());
    }

    public Vector2 GetGridSize() =>
        _gridRectTransform.sizeDelta;
}
