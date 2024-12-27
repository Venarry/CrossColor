
using UnityEngine;

public class CellsClickHandler
{
    private readonly LevelCellsSpawner _levelCellsSpawner;
    private readonly ColorPicker _colorPicker;

    private NonogramCell[] _activeCells;

    public CellsClickHandler(
        LevelCellsSpawner levelCellsSpawner,
        ColorPicker colorPicker)
    {
        _levelCellsSpawner = levelCellsSpawner;
        _colorPicker = colorPicker;

        _levelCellsSpawner.Spawned += OnCellsSpawn;
    }

    private void OnCellsSpawn(NonogramCell[] cells)
    {
        _activeCells = new NonogramCell[cells.Length];

        for (int i = 0; i < cells.Length; i++)
        {
            _activeCells[i] = cells[i];
            _activeCells[i].Clicked += OnCellClick;
        }
    }

    private void OnCellClick(NonogramCell cell)
    {
        if(_colorPicker.SelectedColorKey == cell.WinColorKey)
        {
            cell.ActiveCell(_colorPicker.SelectedColor);
        }
        else
        {
            cell.EnableWrongColor();
        }
    }
}
