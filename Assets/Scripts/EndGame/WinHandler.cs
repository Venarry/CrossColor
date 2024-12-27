using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class WinHandler : MonoBehaviour
{
    [SerializeField] private Image _winPanel;
    [SerializeField] private GameObject _labelsParent;
    [SerializeField] private Image _finalImage;
    [SerializeField] private RawImage _videoClip;
    [SerializeField] private LevelCellsSpawner _levelCellsSpawner;

    private readonly float _finalImageFadeDuration = 2f;

    private void Awake()
    {
        _winPanel.gameObject.SetActive(false);
    }

    public void Enable()
    {
        _levelCellsSpawner.Spawned += OnLevelSpawn;
    }

    public void Disable()
    {
        _levelCellsSpawner.Spawned -= OnLevelSpawn;
    }

    private void OnLevelSpawn(NonogramCell[] cells)
    {
        _finalImage.GetComponent<RectTransform>().sizeDelta = _levelCellsSpawner.GetGridSize();
        _videoClip.GetComponent<RectTransform>().sizeDelta = _levelCellsSpawner.GetGridSize();
    }

    public async void Activate()
    {
        _winPanel.gameObject.SetActive(true);
        _finalImage.gameObject.SetActive(true);
        _videoClip.gameObject.SetActive(false);

        Color winPanelColor = _winPanel.color;
        winPanelColor.a = 0;
        _winPanel.color = winPanelColor;

        Color startFinalImageColor = _finalImage.color;
        Color endFinalImageColor = _finalImage.color;
        startFinalImageColor.a = 0;

        _finalImage.color = startFinalImageColor;

        for (float timer = 0; timer < _finalImageFadeDuration; timer += Time.deltaTime)
        {
            _finalImage.color = Color.Lerp(startFinalImageColor, endFinalImageColor, timer / _finalImageFadeDuration);
            await Task.Yield();
        }

        winPanelColor.a = 1;
        _winPanel.color = winPanelColor;

        _videoClip.gameObject.SetActive(true);
        _labelsParent.SetActive(true);
    }
}