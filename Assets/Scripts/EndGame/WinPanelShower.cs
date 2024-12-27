using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class WinPanelShower : MonoBehaviour
{
    [SerializeField] private LevelCellsSpawner _levelCellsSpawner;
    [SerializeField] private Image _winPanel;
    [SerializeField] private GameObject _labelsParent;
    [SerializeField] private Button _nextLevelButton;
    [SerializeField] private Button _restartLevelButton;
    [SerializeField] private Image _finalImage;
    [SerializeField] private VideoPlayer _videoPlayer;

    private readonly float _finalImageFadeDuration = 2f;

    private void Awake()
    {
        _winPanel.gameObject.SetActive(false);
    }

    public void Enable()
    {
        _levelCellsSpawner.LevelChanged += OnLevelSpawn;
    }

    public void Disable()
    {
        _levelCellsSpawner.LevelChanged -= OnLevelSpawn;
    }

    private void OnLevelSpawn(LevelData levelData, Vector3 offset)
    {
        _finalImage.GetComponent<RectTransform>().sizeDelta = _levelCellsSpawner.GetGridSize();
        _videoPlayer.GetComponent<RectTransform>().sizeDelta = _levelCellsSpawner.GetGridSize();

        _finalImage.sprite = levelData.Sprite;
        _videoPlayer.clip = levelData.VideoClip;

        _finalImage.transform.localPosition = offset;
        _videoPlayer.transform.localPosition = offset;

        HidePanels();
    }

    public async void ShowWinPanel()
    {
        _winPanel.gameObject.SetActive(true);
        _finalImage.gameObject.SetActive(true);
        _videoPlayer.gameObject.SetActive(false);
        _labelsParent.gameObject.SetActive(false);

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

        _videoPlayer.gameObject.SetActive(true);
        _labelsParent.SetActive(true);

        if(_levelCellsSpawner.IsLastLevel == true)
        {
            _restartLevelButton.gameObject.SetActive(true);
        }
        else
        {
            _nextLevelButton.gameObject.SetActive(true);
        }
    }

    private void HidePanels()
    {
        _videoPlayer.gameObject.SetActive(false);
        _winPanel.gameObject.SetActive(false);
        _finalImage.gameObject.SetActive(false);
        _labelsParent.gameObject.SetActive(false);
        _restartLevelButton.gameObject.SetActive(false);
        _nextLevelButton.gameObject.SetActive(false);
    }
}