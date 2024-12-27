using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEntryPoint : MonoBehaviour
{
    [SerializeField] private LevelCellsSpawner _levelCellsSpawner;
    [SerializeField] private LevelsDataSource _levelsDataSource;
    [SerializeField] private ColorPicker _colorPicker;

    private async void Awake()
    {
        StreaminAssetsReader streaminAssetsReader = new();
        ColorsDataSource colorsDataSource = new();
        LevelData level = await streaminAssetsReader.ReadAsync<LevelData>("level1.json");

        _levelCellsSpawner.Init(colorsDataSource);
        CellsClickHandler cellsClickHandler = new(_levelCellsSpawner, _colorPicker);
        _colorPicker.Init(_levelCellsSpawner, colorsDataSource);

        _levelCellsSpawner.SpawnLevel(level);
    }
}

[Serializable]
public class LevelData
{
    public List<Line> rows;
    public List<Line> columns;
}

[Serializable]
public class Line
{
    public List<CellData> data;
}

[Serializable]
public class CellData
{
    public int count;
    public string color;
}