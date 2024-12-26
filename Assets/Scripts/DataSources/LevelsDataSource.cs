using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelsDataSource : MonoBehaviour
{
    [SerializeField] private LevelColorsSO[] _levelsColorData;

    private readonly List<int[,]> _levels = new()
    {
        new int[,] 
        {
            {1, 3, 3, 2, 0},
            {0, 1, 0, 2, 2},
            {0, 0, 1, 2, 0},
            {2, 2, 2, 2, 0},
            {0, 0, 0, 0, 1}
        }
    };

    public int[][,] Levels => _levels.ToArray();

    public LevelColorsSO[] LevelColors => _levelsColorData.ToArray();
}
