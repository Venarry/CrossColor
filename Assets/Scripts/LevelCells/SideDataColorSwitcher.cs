public class SideDataColorSwitcher
{
    private LevelCellsSpawner _levelCellsSpawner;
    private NonogramCell[] _cells;
    private LineData[] _rows;
    private LineData[] _columns;
    private int _columnCount;
    private int _rowsCount;

    public SideDataColorSwitcher(LevelCellsSpawner levelCellsSpawner)
    {
        _levelCellsSpawner = levelCellsSpawner;
    }

    public void Enable()
    {
        _levelCellsSpawner.Spawned += OnLevelSpawn;
        _levelCellsSpawner.SideDataSet += OnSideDataSet;
    }

    public void Disable()
    {
        _levelCellsSpawner.Spawned -= OnLevelSpawn;
        _levelCellsSpawner.SideDataSet -= OnSideDataSet;
    }

    private void OnLevelSpawn(NonogramCell[] cells, int rowsCount, int columnCount)
    {
        ClearCells();

        _cells = cells;
        _rowsCount = rowsCount;
        _columnCount = columnCount;

        foreach (NonogramCell cell in cells)
        {
            cell.Clicked += OnCellClick;
        }
    }

    private void OnSideDataSet(LineData[] rows, LineData[] cloumns)
    {
        _rows = rows;
        _columns = cloumns;
    }

    private void OnCellClick(NonogramCell cell)
    {
        TryDisableRow(cell.RowIndex);
        TryDisableColumns(cell.ColumnIndex);
    }

    private void TryDisableRow(int rowIndex)
    {
        for (int i = 0; i < _columnCount; i++)
        {
            int targetIndex = i + (rowIndex * _columnCount);

            if (_cells.Length <= targetIndex)
            {
                continue;
            }

            if (_cells[targetIndex].IsActivated == false)
            {
                return;
            }
        }

        if (_rows.Length <= rowIndex)
            return;

        _rows[rowIndex].DisableTextColor();
    }

    private void TryDisableColumns(int columnIndex)
    {
        for (int i = 0; i < _rowsCount; i++)
        {
            int targetIndex = columnIndex + (i * _rowsCount);

            if(_cells.Length <= targetIndex)
            {
                continue;
            }

            if (_cells[targetIndex].IsActivated == false)
            {
                return;
            }
        }

        if (_columns.Length <= columnIndex)
            return;

        _columns[columnIndex].DisableTextColor();
    }

    private void ClearCells()
    {
        if(_cells == null)
        {
            return;
        }

        if(_cells.Length == 0)
        {
            return;
        }

        foreach (NonogramCell cell in _cells)
        {
            cell.Clicked -= OnCellClick;
        }
    }
}