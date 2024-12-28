using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;

public class LevelsDataSource : MonoBehaviour
{
    [SerializeField] private LevelData[] _levelsName;

    public LevelData[] Levels => _levelsName.ToArray();
    public string[] LevelsName => _levelsName.Select(c => c.LevelName).ToArray();
}

[Serializable]
public class LevelData
{
    public string LevelName;
    public Sprite Sprite;
    public VideoClip VideoClip;
    public bool IsTutorial = false;
}
