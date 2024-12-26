using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LineColorInfo : MonoBehaviour
{
    public Image[] images;
    public TextMeshProUGUI[] Texts; 
    public RectTransform RectTransform;
    public void Initialize(Dictionary<int, (int count, Color color)> colorData)
    {
        for (int i = 0; i < images.Length; i++)
        {
            if (i < Texts.Length)
                Texts[i].text = "";

            images[i].color = Color.clear;
            images[i].gameObject.SetActive(false);
        }

        int index = 0;

        foreach (KeyValuePair<int, (int count, Color color)> kvp in colorData)
        {
            if (index >= images.Length || index >= Texts.Length)
                break;

            int colorID = kvp.Key;
            int count = kvp.Value.count;
            Color color = kvp.Value.color;

            images[index].color = color;
            images[index].gameObject.SetActive(true);
            Texts[index].text = count.ToString();

            index++;
        }
    }
}
