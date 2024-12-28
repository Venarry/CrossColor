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
    public TextAsset[] LevelsFiles => _levelsName.Select(c => c.LevelFile).ToArray();
}

[Serializable]
public class LevelData
{
    public string Name;
    public TextAsset LevelFile;
    public Sprite Sprite;
    public VideoClip VideoClip;
    public bool IsTutorial = false;
    [TextArea()]
    public string[] Comments;
}
