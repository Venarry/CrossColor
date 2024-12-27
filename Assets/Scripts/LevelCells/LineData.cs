using System.Collections.Generic;
using UnityEngine;

public class LineData : MonoBehaviour
{
    [SerializeField] private Transform _numbersParent;
    [SerializeField] private LineDataText _lineDataTextPrefab;

    private readonly List<LineDataText> _lines = new();

    public void Init(List<CellData> cellsData, ColorsDataSource colorsDataSource)
    {
        foreach (CellData value in cellsData)
        {
            LineDataText text = Instantiate(_lineDataTextPrefab, _numbersParent);
            text.Init(value.count.ToString(), colorsDataSource.Get(value.color));

            _lines.Add(text);
        }
    }

    public void DisableTextColor()
    {
        foreach (LineDataText line in _lines)
        {
            line.DisableTextColor();
        }
    }
}
