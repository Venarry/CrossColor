using System.Collections.Generic;
using UnityEngine;

public class ColorsDataSource
{
    private readonly Dictionary<string, Color> _colorMap = new()
    {
        { "Red", Color.red },
        { "Blue", Color.blue },
        { "Green", Color.green },
        { "Yellow", Color.yellow },
        { "Black", Color.black },
        { "White", Color.white },
        { "Gray", Color.gray },
        { "Orange", new Color(1.0f, 0.5f, 0.0f) },
        { "Purple", new Color(0.5f, 0.0f, 0.5f) },
        { "Pink", new Color(1.0f, 0.4f, 0.7f) }
    };

    public Color Get(string key) => _colorMap[key];
}
