﻿using System;
using System.Linq;

public class CellsClickHandler
{
    private readonly LevelCellsSpawner _levelCellsSpawner;
    private readonly TutorialShower _tutorialShower;
    private readonly ColorPicker _colorPicker;
    private readonly HealthModel _healthModel;
    private readonly WinPanelShower _winHandler;

    private NonogramCell[] _activeCells;

    public CellsClickHandler(
        LevelCellsSpawner levelCellsSpawner,
        ColorPicker colorPicker,
        HealthModel healthModel,
        WinPanelShower winHandler,
        TutorialShower tutorialShower)
    {
        _levelCellsSpawner = levelCellsSpawner;
        _colorPicker = colorPicker;
        _healthModel = healthModel;
        _winHandler = winHandler;
        _tutorialShower = tutorialShower;
    }

    public event Action WrongClicked;

    public void Enable()
    {
        _levelCellsSpawner.Spawned += OnCellsSpawn;
    }

    public void Disable()
    {
        _levelCellsSpawner.Spawned -= OnCellsSpawn;
    }

    private void OnCellsSpawn(NonogramCell[] cells, int rows, int columns, LevelData _)
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
        if (cell.IsActivated == true)
        {
            return;
        }

        if(_colorPicker.SelectedColorKey == cell.WinColorKey)
        {
            if(_levelCellsSpawner.IsTutorial == true && _tutorialShower.ActiveTutorialColor != cell.WinColorKey)
            {
                return;
            }

            if(_colorPicker.CrossActive == true)
            {
                cell.Cross();
            }
            else
            {
                cell.Activate(_colorPicker.SelectedColor);
            }

            TryWinGame();
        }
        else 
        {
            if (_levelCellsSpawner.IsTutorial == false)
            {
                _healthModel.TakeDamage();
            }

            cell.EnableWrongColor();
            WrongClicked?.Invoke();
        }
    }

    private void TryWinGame()
    {
        NonogramCell nonActivatedCell = _activeCells.FirstOrDefault(c => c.IsActivated == false);

        if (nonActivatedCell != null)
        {
            return;
        }

        _winHandler.HandleWinGame();
    }
}
