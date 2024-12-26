using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Level colors", menuName = "Level/Colors", order = 1)]
public class LevelColorsSO : ScriptableObject
{
    [SerializeField] private List<Color> _colors;

    public Color[] Colors => _colors.ToArray();
}
