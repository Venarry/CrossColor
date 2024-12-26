using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NonogramCell : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _image;

    public event Action<NonogramCell> Clicked;

    public Color CellColor => _image.color;
    public string WinColorKey { get; private set; }

    public void SetWinCondition(string colorKey)
    {
        WinColorKey = colorKey;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Clicked?.Invoke(this);
    }

    public void SetColor(Color color)
    {
        _image.color = color;
    }
}
