using System.Collections.Generic;
using UnityEngine;

public class LineData : MonoBehaviour
{
    [SerializeField] private Transform _numbersParent;
    [SerializeField] private LineDataText _lineDataTextPrefab;

    public void Init(List<CellData> cellsData, ColorsDataSource colorsDataSource) //int[] values
    {
        foreach (CellData value in cellsData)
        {
            LineDataText text = Instantiate(_lineDataTextPrefab, _numbersParent);
            text.Init(value.count.ToString(), colorsDataSource.Get(value.color));
        }
    }
}
