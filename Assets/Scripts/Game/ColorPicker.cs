using System.Collections.Generic;
using UnityEngine;

public class ColorPicker : MonoBehaviour
{
    public static ColorPicker Instance;

    [SerializeField] private ColorPickerButton _colorPickerButtonPrefab;
    [SerializeField] private ColorPickerButton _colorPickerCrossToolButtonPrefab;
    [SerializeField] private Transform _toolsParent;

    private List<ColorPickerButton> _spawnedButtons = new();
    private LevelCellsSpawner _levelCellsSpawner;
    private ColorsDataSource _colorsDataSource;

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

    public void SetColor(string colorKey)
    {
        SelectedColorKey = colorKey;

        SelectedColor = _colorsDataSource.Get(colorKey);
    }

    public void GenerateColorButtons(string[] colorsKeys)
    {
        ClearButtons();

        int startButtonIndex = 0;

        //SpawnButton(Color.white, _colorPickerCrossToolButtonPrefab, startButtonIndex);

        for (int i = 0; i < colorsKeys.Length; i++)
        {
            ColorPickerButton button = SpawnButton(_colorsDataSource.Get(colorsKeys[i]), _colorPickerButtonPrefab, colorsKeys[i]);
            _spawnedButtons.Add(button);
        }

        SetColor(colorsKeys[startButtonIndex]);
    }

    private ColorPickerButton SpawnButton(Color color, ColorPickerButton prefab, string key)
    {
        ColorPickerButton button = Instantiate(prefab, _toolsParent);
        button.Init(this);
        button.SetColor(color, key);

        return button;
    }

    private void ClearButtons()
    {
        if(_spawnedButtons.Count == 0)
        {
            return;
        }

        foreach (ColorPickerButton button in _spawnedButtons)
        {
            Destroy(button.gameObject);
        }

        _spawnedButtons.Clear();
    }
}