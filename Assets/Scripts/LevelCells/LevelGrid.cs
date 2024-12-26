using UnityEngine;

public class LevelGrid
{
    private Vector2[,] _cells;

    public void Generate(int elementsCount, int colomnCount, float cellSize)
    {
        int rowCount = Mathf.CeilToInt((float)elementsCount / colomnCount);
        _cells = new Vector2[rowCount, colomnCount];

        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < colomnCount; j++)
            {
                float cellYPosition = GetPositionOnLine(rowCount, rowCount - i - 1, cellSize);
                float cellXPosition = GetPositionOnLine(colomnCount, j, cellSize);

                _cells[i, j] = new Vector2(cellXPosition, cellYPosition);
            }
        }
    }

    public Vector2 GetPosition(int row, int colomn) =>
        _cells[row, colomn];

    private float GetPositionOnLine(int columnsCount, int currentColumnIndex, float spacing)
    {
        float summaryDistance = (columnsCount - 1) * spacing;
        float centerPosition = summaryDistance - summaryDistance / 2;
        float currentColumnPosition = currentColumnIndex * spacing;

        return currentColumnPosition - centerPosition;
    }
}
