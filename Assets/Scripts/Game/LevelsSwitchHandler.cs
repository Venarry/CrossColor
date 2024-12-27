using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelsSwitchHandler : MonoBehaviour
{
    [SerializeField] private Button _nextLevelButton;
    [SerializeField] private LevelCellsSpawner _levelCellsSpawner;

    private void OnEnable()
    {
        _nextLevelButton.onClick.AddListener(StartNextLevel);
    }

    private async void StartNextLevel()
    {
        await _levelCellsSpawner.SpawnLevel();
    }
}
