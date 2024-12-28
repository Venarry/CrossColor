using System;
using System.Collections.Generic;
using UnityEngine;

public class HealthView : MonoBehaviour
{
    [SerializeField] private Transform _healthParent;
    [SerializeField] private GameObject _heartPrefab;

    private readonly List<GameObject> _hearts = new();
    private HealthModel _healthModel;

    public void Init(HealthModel healthModel)
    {
        _healthModel = healthModel;

        _healthModel.DamageTaken += OnDamageReceive;
        _healthModel.Restored += OnHealthRestore;

        SpawnHearts();
    }

    private void OnHealthRestore()
    {
        foreach (GameObject heart in _hearts)
        {
            Destroy(heart);
        }

        _hearts.Clear();

        SpawnHearts();
    }

    private void OnDamageReceive()
    {
        if (_hearts.Count == 0)
            return;

        Destroy(_hearts[0]);
        _hearts.RemoveAt(0);
    }

    private void SpawnHearts()
    {
        for (int i = 0; i < _healthModel.Value; i++)
        {
            _hearts.Add(Instantiate(_heartPrefab, _healthParent));
        }
    }
}
