using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ColorPicker : MonoBehaviour
{
    private const string KeyCross = "Space";

    [SerializeField] private ColorPickerButton _colorPickerButtonPrefab;
    [SerializeField] private ColorPickerButton _colorPickerCrossToolButtonPrefab;
    [SerializeField] private Transform _toolsParent;

    private List<ColorPickerButton> _spawnedButtons = new();
    private LevelCellsSpawner _levelCellsSpawner;
    private ColorsDataSource _colorsDataSource;

    public Color SelectedColor { get; private set; }
    public bool CrossActive { get; private set; }
    public string SelectedColorKey { get; private set; }

    public void Init(LevelCellsSpawner levelCellsSpawner, ColorsDataSource colorsDataSource)
    {
        _levelCellsSpawner = levelCellsSpawner;
        _colorsDataSource = colorsDataSource;

        _levelCellsSpawner.ColorsChanged += OnColorsChange;
    }

    public Vector2 GetButtonPosition(string colorKey)
    {
        Debug.Log(colorKey);

        foreach (var item in _spawnedButtons)
        {
            Debug.Log(item.ColorKey);
        }
        ColorPickerButton button = _spawnedButtons.FirstOrDefault(c => c.ColorKey == colorKey);

        return button.transform.position;
    }

    private void OnColorsChange(string[] colorsKeys)
    {
        GenerateColorButtons(colorsKeys);
    }

    public void SetColor(string colorKey)
    {
        SelectedColorKey = colorKey;

        if (_colorsDataSource.TryGet(colorKey, out Color color))
        {
            SelectedColor = color;
            CrossActive = false;
        }
        else
        {
            CrossActive = true;
        }
    }

    public void GenerateColorButtons(string[] colorsKeys)
    {
        ClearButtons();

        int startButtonIndex = 0;

        ColorPickerButton crossButton = SpawnButton(Color.white, _colorPickerCrossToolButtonPrefab, KeyCross);
        _spawnedButtons.Add(crossButton);

        for (int i = 0; i < colorsKeys.Length; i++)
        {
            if(_colorsDataSource.TryGet(colorsKeys[i], out Color color))
            {
                ColorPickerButton button = SpawnButton(color, _colorPickerButtonPrefab, colorsKeys[i]);
                _spawnedButtons.Add(button);
            }
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