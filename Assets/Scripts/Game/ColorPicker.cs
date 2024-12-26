using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    public static ColorPicker Instance;

    [SerializeField] private ColorPickerButton _colorPickerButtonPrefab;
    [SerializeField] private ColorPickerButton _colorPickerCrossToolButtonPrefab;
    [SerializeField] private Transform _toolsParent;

    private LevelCellsSpawner _levelCellsSpawner;
    private ColorsDataSource _colorsDataSource;

    public Colors? selectedColor = Colors.Red;
    public bool isCrossToolActive;

    public GameObject buttonPrefab; // Префаб кнопки для выбора цвета
    public Transform toolsParent; // Родитель для кнопок инструментов

    public Color SelectedColor { get; private set; }
    public string SelectedColorKey { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Init(LevelCellsSpawner levelCellsSpawner, ColorsDataSource colorsDataSource)
    {
        _levelCellsSpawner = levelCellsSpawner;
        _colorsDataSource = colorsDataSource;

        _levelCellsSpawner.ColorsChanged += OnColorsChange;
    }

    private void OnColorsChange(string[] colorsKeys)
    {
        GenerateColorButtons(colorsKeys);
    }

    /*public void SetColor(int colorIndex)
    {
        selectedColor = (Colors)colorIndex;
        isCrossToolActive = false; 
    }*/

    public void SetColor(string colorKey)
    {
        SelectedColorKey = colorKey;

        SelectedColor = _colorsDataSource.Get(colorKey);
    }

    // Активирует инструмент "крест"
    public void ActivateCrossTool()
    {
        selectedColor = null;
        isCrossToolActive = true;
    }
    
    public Color GetSelectedColor()
    {
        if (selectedColor.HasValue)
        {
            string colorName = selectedColor.Value.ToString();
            if (GridManager.Instance.colorMap.TryGetValue(colorName, out Color color))
            {
                return color;
            }
        }

        return Color.clear;
    }

    public void GenerateColorButtons(string[] colorsKeys)
    {
        int startButtonIndex = 0;

        //SpawnButton(Color.white, _colorPickerCrossToolButtonPrefab, startButtonIndex);

        for (int i = 0; i < colorsKeys.Length; i++)
        {
            SpawnButton(_colorsDataSource.Get(colorsKeys[i]), _colorPickerButtonPrefab, colorsKeys[i]);
        }

        SetColor(colorsKeys[startButtonIndex]);
    }

    private void SpawnButton(Color color, ColorPickerButton prefab, string key)
    {
        ColorPickerButton button = Instantiate(prefab, _toolsParent);
        button.Init(this);
        button.SetColor(color, key);
    }
}