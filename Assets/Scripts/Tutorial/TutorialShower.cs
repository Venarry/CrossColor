using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

public class TutorialShower : MonoBehaviour
{
    [SerializeField] private LevelCellsSpawner _levelCellsSpawner;
    [SerializeField] private ColorPicker _colorPicker;
    [SerializeField] private GameObject _fingerPrefab;
    [SerializeField] private Transform _fingerParent;
    [SerializeField] private LevelComment _levelComment;

    private readonly Dictionary<string, List<NonogramCell>> _cellsByColor = new();
    private readonly List<Coroutine> _fingersCoroutine = new();
    private readonly List<GameObject> _fingers = new();
    private ColorsDataSource _colorsDataSource;

    public string ActiveTutorialColor { get; private set; }

    public void Init(ColorsDataSource colorsDataSource)
    {
        _colorsDataSource = colorsDataSource;
    }

    public void Enable()
    {
        _levelCellsSpawner.Spawned += OnLevelSpawn;
        _levelCellsSpawner.LevelIncreased += OnLevelIncrease;
    }

    private void OnLevelIncrease()
    {
        foreach (Coroutine fingerCoroutine in _fingersCoroutine)
        {
            if(fingerCoroutine != null)
            {
                StopCoroutine(fingerCoroutine);
            }
        }

        _fingersCoroutine.Clear();

        foreach (GameObject finger in _fingers)
        {
            Destroy(finger);
        }

        _fingers.Clear();

        _cellsByColor.Clear();
    }

    public void Disable()
    {
        _levelCellsSpawner.Spawned -= OnLevelSpawn;
        _levelCellsSpawner.LevelIncreased -= OnLevelIncrease;
    }

    private void OnLevelSpawn(NonogramCell[] cells, int rowCount, int columnsCount, LevelData levelData)
    {
        if (levelData.Comments.Length > 0)
        {
            _levelComment.SetData(levelData.Comments);
        }
        else
        {
            _levelComment.Hide();
        }

        if (_levelCellsSpawner.IsTutorial == false)
            return;

        foreach (NonogramCell cell in cells)
        {
            string colorKey = cell.WinColorKey;

            if (_cellsByColor.ContainsKey(colorKey) == true)
            {
                _cellsByColor[colorKey].Add(cell);
            }
            else
            {
                _cellsByColor.Add(colorKey, new());
                _cellsByColor[colorKey].Add(cell);
            }
        }

        ShowNextStage();
    }

    private IEnumerator ShowFingerCellsClickTutorial(string colorKey, NonogramCell[] cellsByColor)
    {
        int activeCellIndex = 0;
        int cellsCount = cellsByColor.Length;
        float towardSpeed = 350f;
        float lerpSpeed = 10f;

        GameObject finger = Instantiate(_fingerPrefab, _colorPicker.GetButtonPosition(colorKey), Quaternion.identity, _fingerParent);

        if (_colorsDataSource.TryGet(colorKey, out Color color) == true)
        {
            finger.GetComponent<Image>().color = color;
        }

        _fingers.Add(finger);

        while (cellsByColor.Where(c => c.IsActivated == false).Count() > 0)
        {
            finger.transform.position = Vector3.MoveTowards(
                finger.transform.position,
                cellsByColor[activeCellIndex].transform.position,
                towardSpeed * Time.deltaTime);

            if (Vector3.Distance(finger.transform.position, cellsByColor[activeCellIndex].transform.position) < 0.1f)
            {
                activeCellIndex = GetNextIndex(activeCellIndex, cellsCount, out bool reseted);

                if (reseted == true)
                {
                    while (Vector3.Distance(finger.transform.position, cellsByColor[activeCellIndex].transform.position) > 0.1f)
                    {
                        finger.transform.position = Vector3
                            .Lerp(finger.transform.position, cellsByColor[activeCellIndex].transform.position, lerpSpeed * Time.deltaTime);

                        yield return null;
                    }
                }
            }

            yield return null;
        }

        Destroy(finger);
        _fingers.Remove(finger);

        _cellsByColor.Remove(colorKey);
        ShowNextStage();
    }

    private void ShowNextStage()
    {
        if (_cellsByColor.Count == 0)
            return;

        KeyValuePair<string, List<NonogramCell>> cell = _cellsByColor.First();

        _levelComment.ShowNextComment();
        ActiveTutorialColor = cell.Key;
        Coroutine coroutine = StartCoroutine(ShowFingerCellsClickTutorial(cell.Key, cell.Value.ToArray()));
        _fingersCoroutine.Add(coroutine);
    }

    private int GetNextIndex(int currentIndex, int maxCount, out bool reseted)
    {
        currentIndex++;
        reseted = false;

        if (currentIndex >= maxCount)
        {
            currentIndex = 0;
            reseted = true;
        }

        return currentIndex;
    }
}
