using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NonogramCell : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image _mainColorImage;
    [SerializeField] private Image _image;
    [SerializeField] private Image _cross;

    private readonly float _worngColorFadeDuration = 1f;
    private readonly Color _wrongColor = Color.red;

    private Coroutine _activeWrongFading;

    public event Action<NonogramCell> Clicked;

    public string WinColorKey { get; private set; }
    public bool IsActivated { get; private set; } = false;
    public int RowIndex { get; private set; }
    public int ColumnIndex { get; private set; }

    public void SetIndex(int i, int j)
    {
        RowIndex = i;
        ColumnIndex = j;
    }

    public void SetWinCondition(string colorKey)
    {
        WinColorKey = colorKey;
    }

    public void ClickOnCell()
    {
        Clicked?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Clicked?.Invoke(this);
    }

    public void Activate(Color color)
    {
        StopWrongFading();
        _image.color = color;
        StartCoroutine(ShowCellColor());

        IsActivated = true;
    }

    public void Cross()
    {
        StopWrongFading();
        _cross.gameObject.SetActive(true);
        IsActivated = true;
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
            _mainColorImage.color = Color.white;
        }
    }

    private IEnumerator FadingWrongColor()
    {
        Color startColor = _mainColorImage.color;
        float oneStepDuration = _worngColorFadeDuration / 2;

        for (float i = 0; i < oneStepDuration; i += Time.deltaTime)
        {
            _mainColorImage.color = Color.Lerp(startColor, _wrongColor, i / oneStepDuration);
            yield return null;
        }

        for (float i = 0; i < oneStepDuration; i += Time.deltaTime)
        {
            _mainColorImage.color = Color.Lerp(_wrongColor, startColor, i / oneStepDuration);
            yield return null;
        }

        _mainColorImage.color = startColor;
        _activeWrongFading = null;
    }

    private IEnumerator ShowCellColor()
    {
        float scaleTransitDuration = 1f;
        Vector3 startScale = _image.transform.localScale;

        for (float i = 0; i < scaleTransitDuration; i += Time.deltaTime)
        {
            _image.transform.localScale = Vector3.Lerp(_image.transform.localScale, Vector3.one, i / scaleTransitDuration);

            yield return null;
        }
    }
}
