using TMPro;
using UnityEngine;

public class LineData : MonoBehaviour
{
    [SerializeField] private Transform _numbersParent;
    [SerializeField] private TMP_Text _textPrefab;

    public void Init(int[] values)
    {
        foreach (int value in values)
        {
            TMP_Text text = Instantiate(_textPrefab, _numbersParent);
            text.text = value.ToString();
        }
    }
}