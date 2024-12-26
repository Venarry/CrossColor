using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LineDataText : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _label;

    public void Init(string text, Color color)
    {
        _label.text = text.ToString();
        _image.color = color;
    }
}