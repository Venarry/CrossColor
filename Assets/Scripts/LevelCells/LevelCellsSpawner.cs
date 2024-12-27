using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

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
    private LevelLoadData[] _levelsLoadData;
    private LevelData[] _levelsData;
    private int _activeLevelIndex = 0;

    public event Action<NonogramCell[]> Spawned;
    public event Action<string[]> ColorsChanged;
    public event Action<LevelData> LevelChanged;

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
        LevelChanged?.Invoke(_levelsData[_activeLevelIndex]);

        _activeLevelIndex++;
    }

    public Vector2 GetGridSize() =>
        _gridRectTransform.sizeDelta;
}
