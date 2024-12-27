using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LineDataText : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _label;

    private Color _textDisabledColor = Color.black;

    public void Init(string text, Color color)
    {
        _label.text = text.ToString();
        _image.color = color;
    }

    public void DisableTextColor()
    {
        _label.color = _textDisabledColor;
    }
}