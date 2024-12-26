using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    private Vector2Int gridPosition;
    public Color correctColor;

    public Image image;
    private CellState currentState;

    public bool isFilledCorrectly; // Указывает, правильно ли закрашена ячейка
    public bool shouldBeFilled;   // Указывает, должна ли ячейка быть закрашена
    
    public enum CellState { Empty, Filled, Cross }

    void Awake()
    {
        image = GetComponent<Image>();
        currentState = CellState.Empty;
    }

    public void SetGridPosition(int x, int y)
    {
        gridPosition = new Vector2Int(x, y);
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }

    public void SetColor(Color color)
    {
        currentState = CellState.Filled;
        image.color = color;
        CheckIfFilledCorrectly();
    }

    public void MarkAsCross()
    {
        currentState = CellState.Cross;
        image.color = Color.gray;
        CheckIfFilledCorrectly();
    }

    private void CheckIfFilledCorrectly()
    {
        if (currentState == CellState.Filled)
        {
            isFilledCorrectly = image.color == correctColor;
        }
        else if (currentState == CellState.Cross)
        {
            isFilledCorrectly = !shouldBeFilled;
        }
        else
        {
            isFilledCorrectly = !shouldBeFilled;
        }

        if (isFilledCorrectly)
        {
            GridManager.Instance.OnCellFilled(this);
        }
        else
        {
            if (shouldBeFilled)
            {
                SetColor(correctColor);
            }
            else
            {
                MarkAsCross();
            }
            GridManager.Instance.OnPlayerError(this);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentState == CellState.Filled)
            return;

        if (ColorPicker.Instance.isCrossToolActive)
        {
            MarkAsCross();
        }
        else
        {
            Color selectedColor = ColorPicker.Instance.GetSelectedColor();

            if (selectedColor == Color.clear)
                return;

            SetColor(selectedColor);
        }

        isFilledCorrectly = IsCorrectlyFilled();

        GridManager.Instance.OnCellFilled(this);
    }

    
    public bool IsCorrectlyFilled()
    {
        if (currentState == CellState.Filled)
            return image.color == correctColor; // Проверка цвета

        if (currentState == CellState.Cross)
            return !shouldBeFilled; // Крест допустим только для ячеек, которые не нужно закрашивать

        return !shouldBeFilled; // Пустая ячейка корректна только если она не должна быть закрашена
    }

}
