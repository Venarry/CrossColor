using System.Collections.Generic;
using UnityEngine;

public class LineData : MonoBehaviour
{
    [SerializeField] private Transform _numbersParent;
    [SerializeField] private LineDataText _lineDataTextPrefab;

    private readonly List<LineDataText> _lines = new();
    private bool _disabled = false;

    public void Init(List<CellData> cellsData, ColorsDataSource colorsDataSource)
    {
        foreach (CellData value in cellsData)
        {
            if (colorsDataSource.TryGet(value.color, out Color color))
            {
                LineDataText text = Instantiate(_lineDataTextPrefab, _numbersParent);
                text.Init(value.count.ToString(), color);

                _lines.Add(text);
            }
        }
    }

    public void DisableTextColor()
    {
        if(_disabled == true)
        {
            return;
        }

        foreach (LineDataText line in _lines)
        {
            line.DisableTextColor();
        }

        _disabled = true;
    }
}
