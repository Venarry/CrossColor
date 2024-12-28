using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

public class LevelsDataSource : MonoBehaviour
{
    [SerializeField] private LevelData[] _levelsName;

    public LevelData[] Levels => _levelsName.ToArray();
    public string[] LevelsName => _levelsName.Select(c => c.LevelFile.name).ToArray();
}

[Serializable]
public class LevelData
{
    public DefaultAsset LevelFile;
    public Sprite Sprite;
    public VideoClip VideoClip;
    public bool IsTutorial = false;
}
