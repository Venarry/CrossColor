using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NonogramCell : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _image;
    private readonly float _worngColorFadeDuration = 1f;
    private readonly Color _wrongColor = Color.red;

    private Coroutine _activeWrongFading;

    public event Action<NonogramCell> Clicked;

    public Color CellColor => _image.color;
    public string WinColorKey { get; private set; }
    public bool IsActive { get; private set; } = false;

    public void SetWinCondition(string colorKey)
    {
        WinColorKey = colorKey;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Clicked?.Invoke(this);
    }

    public void ActiveCell(Color color)
    {
        StopWrongFading();
        _image.color = color;
        IsActive = true;
    }

    public void EnableWrongColor()
    {
        StopWrongFading();
        _activeWrongFading = StartCoroutine(FadingWrongColor());
    }

    private void StopWrongFading()
    {
        if (_activeWrongFading != null)
        {
            StopCoroutine(_activeWrongFading);
            _image.color = Color.white;
        }
    }

    private IEnumerator FadingWrongColor()
    {
        Color startColor = _image.color;
        float oneStepDuration = _worngColorFadeDuration / 2;

        for (float i = 0; i < oneStepDuration; i += Time.deltaTime)
        {
            _image.color = Color.Lerp(startColor, _wrongColor, i / oneStepDuration);
            yield return null;
        }

        for (float i = 0; i < oneStepDuration; i += Time.deltaTime)
        {
            _image.color = Color.Lerp(_wrongColor, startColor, i / oneStepDuration);
            yield return null;
        }

        _image.color = startColor;
        _activeWrongFading = null;
    }
}
