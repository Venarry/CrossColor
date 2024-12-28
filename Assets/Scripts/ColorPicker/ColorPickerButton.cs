using System;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickerButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _buttonColor;

    private ColorPicker _colorPicker;
    private string _colorKey;

    private void OnEnable()
    {
        _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        _colorPicker?.SetColor(_colorKey);
    }

    public void Init(ColorPicker colorPicker)
    {
        _colorPicker = colorPicker;
    }

    public void SetColor(Color color, string colorKey)
    {
        _buttonColor.color = color;
        _colorKey = colorKey;
    }
}
