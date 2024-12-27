using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    private ColorsDataSource _colorsDataSource;
    private RectTransform _gridRectTransform;
    private RectTransform _rowParentRectTransform;
    private RectTransform _columnParentRectTransform;
    private LevelLoadData[] _levelsLoadData;
    private LevelData[] _levelsData;
    private int _activeLevelIndex = 0;

    public event Action<NonogramCell[]> Spawned;
    public event Action<string[]> ColorsChanged;
    public event Action<LevelData, Vector3> LevelChanged;

    public void Init(ColorsDataSource colorsDataSource, LevelLoadData[] levelsLoadData, LevelData[] levelsData)
    {
        _colorsDataSource = colorsDataSource;

        _gridRectTransform = _gridLayout.GetComponent<RectTransform>();
        _rowParentRectTransform = _rowsDataParent.GetComponent<RectTransform>();
        _columnParentRectTransform = _columnsDataParent.GetComponent<RectTransform>();

        _activeLevelIndex = 0;
        _levelsLoadData = levelsLoadData;
        _levelsData = levelsData;
    }

    public async Task SpawnLevel()
    {
        if(_activeLevelIndex >= _levelsLoadData.Length)
        {
            return;
        }

        if(_spawnedCells.Count > 0)
        {
            ClearMap();
        }

        LevelLoadData levelData = _levelsLoadData[_activeLevelIndex];

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

        int rowCount = _spawnedCells.Count / columnCount;
        int maxSideLength = Math.Max(columnCount, rowCount);

        float scaleFactor = 1100;
        float sideSize = scaleFactor / maxSideLength;
        Vector2 cellsSize = new(sideSize, sideSize);
        _gridLayout.cellSize = cellsSize;

        for (int i = 0; i < levelData.rows.Count; i++)
        {
            LineData rowData = Instantiate(_rowDataPrefab, _rowsDataParent);
            rowData.Init(levelData.rows[i].data, _colorsDataSource);

            rowData.GetComponent<RectTransform>().sizeDelta = cellsSize;

            _spawnedRowsData.Add(rowData);
        }

        for (int i = 0; i < columnCount; i++)
        {
            LineData columnData = Instantiate(_columnDataPrefab, _columnsDataParent);
            columnData.Init(levelData.columns[i].data, _colorsDataSource);

            columnData.GetComponent<RectTransform>().sizeDelta = cellsSize;

            _spawnedColumnsData.Add(columnData);
        }

        //LayoutRebuilder.MarkLayoutForRebuild(_gridRectTransform);
        //LayoutRebuilder.MarkLayoutForRebuild(_rowParentRectTransform);
        //LayoutRebuilder.MarkLayoutForRebuild(_columnParentRectTransform);
        //LayoutRebuilder.ForceRebuildLayoutImmediate(_gridRectTransform);
        //LayoutRebuilder.ForceRebuildLayoutImmediate(_rowParentRectTransform);
        //LayoutRebuilder.ForceRebuildLayoutImmediate(_columnParentRectTransform);
        //Canvas.ForceUpdateCanvases();

        UpdateContainerSize(_gridRectTransform, _gridLayout);
        UpdateHorizontalLayoutSize(_columnParentRectTransform, _columnParentRectTransform.GetComponent<HorizontalLayoutGroup>());
        UpdateVerticalLayoutSize(_rowParentRectTransform, _rowParentRectTransform.GetComponent<VerticalLayoutGroup>());

        /*while (_gridRectTransform.sizeDelta.x == 0)
        {
            await Task.Yield();
        }*/

        float sizeDivider = 2;
        float xOffset = _rowParentRectTransform.sizeDelta.x / 2;

        Vector2 offset = new(xOffset, 0);

        float rowDataXPosition = -_gridRectTransform.sizeDelta.x / sizeDivider - _rowParentRectTransform.sizeDelta.x / sizeDivider;
        _rowsDataParent.localPosition = new Vector2(rowDataXPosition, 0) + offset;

        float columnDataYPosition = _gridRectTransform.sizeDelta.y / sizeDivider + _columnParentRectTransform.sizeDelta.y / sizeDivider;
        _columnsDataParent.localPosition = new Vector2(0, columnDataYPosition) + offset;

        _gridRectTransform.localPosition = new Vector3(xOffset, 0, 0);

        Spawned?.Invoke(_spawnedCells.ToArray());
        ColorsChanged?.Invoke(colorsStack.ToArray());
        LevelChanged?.Invoke(_levelsData[_activeLevelIndex], offset);

        _activeLevelIndex++;
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

    void UpdateHorizontalLayoutSize(RectTransform container, HorizontalLayoutGroup layoutGroup)
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

    void UpdateVerticalLayoutSize(RectTransform container, VerticalLayoutGroup layoutGroup)
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
}
