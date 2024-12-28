using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelCellsSpawner : MonoBehaviour
{
    [SerializeField] private NonogramCell _nonogramCellPrefab;
    [SerializeField] private LineData _rowDataPrefab;
    [SerializeField] private LineData _columnDataPrefab;
    [SerializeField] private Transform _gridParent;
    [SerializeField] private GridLayoutGroup _gridLayout;
    [SerializeField] private Transform _rowsDataParent;
    [SerializeField] private Transform _columnsDataParent;

    private readonly List<NonogramCell> _spawnedCells = new();
    private readonly List<LineData> _spawnedRowsData = new();
    private readonly List<LineData> _spawnedColumnsData = new();

    private RectTransform _gridRectTransform;
    private RectTransform _rowParentRectTransform;
    private RectTransform _columnParentRectTransform;

    private ColorsDataSource _colorsDataSource;
    private List<LevelLoadData> _levelsLoadData = new();
    private List<LevelLoadData> _tutorialLevelsLoadData = new();
    private List<LevelData> _levelsData = new();
    private List<LevelData> _tutorialLevelsData = new();
    private int _activeLevelIndex = 0;
    private bool _tutorialPassed = false;

    public event Action<NonogramCell[], int, int> Spawned;
    public event Action<LineData[], LineData[]> SideDataSet;
    public event Action<string[]> ColorsChanged;
    public event Action<LevelData, Vector3> LevelChanged;
    public event Action LevelIncreased;

    public bool IsTutorial => _tutorialPassed == false;

    public void Init(ColorsDataSource colorsDataSource, LevelLoadData[] levelsLoadData, LevelData[] levelsData)
    {
        _gridRectTransform = _gridLayout.GetComponent<RectTransform>();
        _rowParentRectTransform = _rowsDataParent.GetComponent<RectTransform>();
        _columnParentRectTransform = _columnsDataParent.GetComponent<RectTransform>();

        ResetLevels();
        _colorsDataSource = colorsDataSource;

        for (int i = 0; i < levelsData.Length; i++)
        {
            if (levelsData[i].IsTutorial == true)
            {
                _tutorialLevelsData.Add(levelsData[i]);
                _tutorialLevelsLoadData.Add(levelsLoadData[i]);
            }
            else
            {
                _levelsData.Add(levelsData[i]);
                _levelsLoadData.Add(levelsLoadData[i]);
            }
        }
    }

    public void ResetLevels()
    {
        _activeLevelIndex = 0;
    }

    public bool IsLastLevel()
    {
        if(_tutorialPassed == true)
        {
            return _activeLevelIndex >= _levelsLoadData.Count;
        }
        else
        {
            return _activeLevelIndex >= _tutorialLevelsLoadData.Count;
        }
    }

    public bool TrySpawnLevel()
    {
        if(IsLastLevel() == true)
        {
            return false;
        }

        if(_spawnedCells.Count > 0)
        {
            ClearMap();
        }

        LevelLoadData levelLoadData;
        LevelData levelData;

        if(_tutorialPassed == true)
        {
            levelLoadData = _levelsLoadData[_activeLevelIndex];
            levelData = _levelsData[_activeLevelIndex];
        }
        else
        {
            levelLoadData = _tutorialLevelsLoadData[_activeLevelIndex];
            levelData = _tutorialLevelsData[_activeLevelIndex];
        }

        int columnCount = 0;

        for (int j = 0; j < levelLoadData.rows[0].data.Count; j++)
        {
            columnCount += levelLoadData.rows[0].data[j].count;
        }

        _gridLayout.constraintCount = columnCount;

        List<string> colorsStack = new();

        for (int i = 0; i < levelLoadData.rows.Count; i++)
        {
            int columnIndex = 0;

            for (int j = 0; j < levelLoadData.rows[i].data.Count; j++)
            {
                for (int k = 0; k < levelLoadData.rows[i].data[j].count; k++)
                {
                    string colorKey = levelLoadData.rows[i].data[j].color;

                    NonogramCell cell = Instantiate(_nonogramCellPrefab, _gridLayout.transform);
                    cell.SetIndex(i, columnIndex);
                    cell.SetWinCondition(colorKey);

                    if(levelLoadData.rows[i].data[j].filled == true)
                    {
                        if(_colorsDataSource.TryGet(colorKey, out Color color) == true)
                        {
                            cell.Activate(color);
                        }
                        else
                        {
                            cell.Cross();
                        }
                    }

                    cell.gameObject.name = $"Cell {i} {columnIndex}";
                    columnIndex++;

                    _spawnedCells.Add(cell);

                    if (colorsStack.Contains(colorKey) == false)
                    {
                        colorsStack.Add(colorKey);
                    }
                }
            }
        }

        for (int i = 0; i < levelLoadData.columns.Count; i++)
        {
            int rowIndex = 0;

            for (int j = 0; j < levelLoadData.columns[i].data.Count; j++)
            {
                for (int k = 0; k < levelLoadData.columns[i].data[j].count; k++)
                {
                    string colorKey = levelLoadData.columns[i].data[j].color;

                    if (levelLoadData.columns[i].data[j].filled == true)
                    {
                        NonogramCell cell = _spawnedCells.FirstOrDefault(c => c.RowIndex == rowIndex && c.ColumnIndex == i);

                        if (_colorsDataSource.TryGet(colorKey, out Color color) == true)
                        {
                            cell.Activate(color);
                        }
                        else
                        {
                            cell.Cross();
                        }
                    }

                    rowIndex++;
                }
            }
        }

        int rowCount = _spawnedCells.Count / columnCount;
        int maxSideLength = Math.Max(columnCount, rowCount);

        float scaleFactor = 1100;
        float sideSize = scaleFactor / maxSideLength;
        Vector2 cellsSize = new(sideSize, sideSize);
        _gridLayout.cellSize = cellsSize;

        for (int i = 0; i < levelLoadData.rows.Count; i++)
        {
            LineData rowData = Instantiate(_rowDataPrefab, _rowsDataParent);
            rowData.Init(levelLoadData.rows[i].data, _colorsDataSource);

            rowData.GetComponent<RectTransform>().sizeDelta = cellsSize;

            _spawnedRowsData.Add(rowData);
        }

        int rowCountForSpawnColumnsData = 1;

        if(levelLoadData.rows.Count > rowCountForSpawnColumnsData)
        {
            for (int i = 0; i < columnCount; i++)
            {
                LineData columnData = Instantiate(_columnDataPrefab, _columnsDataParent);
                columnData.Init(levelLoadData.columns[i].data, _colorsDataSource);

                columnData.GetComponent<RectTransform>().sizeDelta = cellsSize;

                _spawnedColumnsData.Add(columnData);
            }
        }

        UpdateContainerSize(_gridRectTransform, _gridLayout);
        UpdateHorizontalLayoutSize(_columnParentRectTransform, _columnParentRectTransform.GetComponent<HorizontalLayoutGroup>());
        UpdateVerticalLayoutSize(_rowParentRectTransform, _rowParentRectTransform.GetComponent<VerticalLayoutGroup>());

        float sizeDivider = 2;
        float xOffset = _rowParentRectTransform.sizeDelta.x / 2;

        Vector2 offset = new(xOffset, 0);

        float rowDataXPosition = -_gridRectTransform.sizeDelta.x / sizeDivider - _rowParentRectTransform.sizeDelta.x / sizeDivider;
        _rowsDataParent.localPosition = new Vector2(rowDataXPosition, 0) + offset;

        float columnDataYPosition = _gridRectTransform.sizeDelta.y / sizeDivider + _columnParentRectTransform.sizeDelta.y / sizeDivider;
        _columnsDataParent.localPosition = new Vector2(0, columnDataYPosition) + offset;

        _gridRectTransform.localPosition = new Vector3(xOffset, 0, 0);

        ColorsChanged?.Invoke(colorsStack.ToArray());
        SideDataSet?.Invoke(_spawnedRowsData.ToArray(), _spawnedColumnsData.ToArray());
        LevelChanged?.Invoke(levelData, offset);
        Spawned?.Invoke(_spawnedCells.ToArray(), rowCount, columnCount);

        return true;
    }

    public Vector2 GetGridSize() =>
        _gridRectTransform.sizeDelta;

    private void ClearMap()
    {
        foreach (NonogramCell cell in _spawnedCells)
        {
            DestroyImmediate(cell.gameObject);
        }

        foreach (LineData lineData in _spawnedRowsData)
        {
            DestroyImmediate(lineData.gameObject);
        }

        foreach (LineData lineData in _spawnedColumnsData)
        {
            DestroyImmediate(lineData.gameObject);
        }

        _spawnedCells.Clear();
        _spawnedRowsData.Clear();
        _spawnedColumnsData.Clear();
    }

    private void UpdateContainerSize(RectTransform container, GridLayoutGroup layoutGroup)
    {
        RectOffset padding = layoutGroup.padding;
        Vector2 cellSize = layoutGroup.cellSize;
        Vector2 spacing = layoutGroup.spacing;

        int childCount = container.childCount;

        int columnCount = Mathf.Max(1, layoutGroup.constraintCount);
        int rowCount = Mathf.CeilToInt((float)childCount / columnCount);

        float newWidth = padding.left + padding.right + columnCount * cellSize.x + (columnCount - 1) * spacing.x;
        float newHeight = padding.top + padding.bottom + rowCount * cellSize.y + (rowCount - 1) * spacing.y;

        container.sizeDelta = new Vector2(newWidth, newHeight);
    }

    private void UpdateHorizontalLayoutSize(RectTransform container, HorizontalLayoutGroup layoutGroup)
    {
        RectOffset padding = layoutGroup.padding;
        float spacing = layoutGroup.spacing;

        float totalWidth = padding.left + padding.right;
        float maxHeight = 0f;

        foreach (RectTransform child in container)
        {
            if (!child.gameObject.activeSelf) continue;

            totalWidth += child.rect.width + spacing;
            maxHeight = Mathf.Max(maxHeight, child.rect.height);
        }

        totalWidth -= spacing;

        container.sizeDelta = new Vector2(totalWidth, padding.top + padding.bottom + maxHeight);
    }

    private void UpdateVerticalLayoutSize(RectTransform container, VerticalLayoutGroup layoutGroup)
    {
        RectOffset padding = layoutGroup.padding;
        float spacing = layoutGroup.spacing;

        float totalHeight = padding.top + padding.bottom;
        float maxWidth = 0f;

        foreach (RectTransform child in container)
        {
            if (!child.gameObject.activeSelf) continue;

            totalHeight += child.rect.height + spacing;
            maxWidth = Mathf.Max(maxWidth, child.rect.width);
        }

        totalHeight -= spacing;

        container.sizeDelta = new Vector2(padding.left + padding.right + maxWidth, totalHeight);
    }

    public void IncreaseLevelIndex()
    {
        _activeLevelIndex++;

        if (_tutorialPassed == false && IsLastLevel())
        {
            _tutorialPassed = true;
            ResetLevels();
        }

        LevelIncreased?.Invoke();
    }
}
