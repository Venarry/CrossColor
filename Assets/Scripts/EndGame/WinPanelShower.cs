using System.Collections;
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
    [SerializeField] private RawImage _videoImage;

    private readonly float _finalImageFadeDuration = 2f;
    private Color _startWinPanelColor;
    private Color _endWinPanelColor;
    private Color _startFinalImageColor;
    private Color _endFinalImageColor;
    private Coroutine _showingPanelCoroutine;

    private void Awake()
    {
        _winPanel.gameObject.SetActive(false);

        _startWinPanelColor = _winPanel.color;
        _endWinPanelColor = _winPanel.color;
        _startWinPanelColor.a = 0;

        _startFinalImageColor = _finalImage.color;
        _endFinalImageColor = _finalImage.color;
        _startFinalImageColor.a = 0;
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
        _videoPlayer.Stop();

        _finalImage.transform.localPosition = offset;
        _videoPlayer.transform.localPosition = offset;

        HidePanels();
    }

    public void HandleWinGame()
    {
        if(_showingPanelCoroutine != null)
        {
            StopCoroutine(_showingPanelCoroutine);
            _showingPanelCoroutine = null;
        }

        _levelCellsSpawner.IncreaseLevelIndex();
        _showingPanelCoroutine = StartCoroutine(ShowingWinPanel());
    }

    private IEnumerator ShowingWinPanel()
    {
        _winPanel.gameObject.SetActive(true);
        _finalImage.enabled = true;
        _labelsParent.gameObject.SetActive(false);
        _videoImage.enabled = false;

        _winPanel.color = _startWinPanelColor;
        _finalImage.color = _startFinalImageColor;

        for (float timer = 0; timer < _finalImageFadeDuration; timer += Time.deltaTime)
        {
            _finalImage.color = Color.Lerp(_startFinalImageColor, _endFinalImageColor, timer / _finalImageFadeDuration);
            yield return null;
        }

        _videoPlayer.Prepare();

        while(_videoPlayer.isPrepared == false)
        {
            yield return null;
        }

        _videoPlayer.Play();

        _finalImage.enabled = false;
        _videoImage.enabled = true;
        _labelsParent.SetActive(true);

        if(_levelCellsSpawner.IsLastLevel() == true)
        {
            _restartLevelButton.gameObject.SetActive(true);
        }
        else
        {
            _nextLevelButton.gameObject.SetActive(true);
        }

        for (float timer = 0; timer < _finalImageFadeDuration; timer += Time.deltaTime)
        {
            _winPanel.color = Color.Lerp(_startWinPanelColor, _endWinPanelColor, timer / _finalImageFadeDuration);
            yield return null;
        }

        _showingPanelCoroutine = null;
    }

    private void HidePanels()
    {
        _winPanel.gameObject.SetActive(false);
        _finalImage.enabled = false;
        _labelsParent.gameObject.SetActive(false);
        _restartLevelButton.gameObject.SetActive(false);
        _nextLevelButton.gameObject.SetActive(false);
    }
}