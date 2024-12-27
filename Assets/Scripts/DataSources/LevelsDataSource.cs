using System.Linq;
using UnityEngine;

public class LevelsDataSource : MonoBehaviour
{
    [SerializeField] private string[] _levelsName;

    public string[] LevelsName => _levelsName.ToArray();
}
