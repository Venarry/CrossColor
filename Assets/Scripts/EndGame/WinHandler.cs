using UnityEngine;

public class WinHandler
{
    private readonly GameObject _winPanel;

    public WinHandler(GameObject winPanel)
    {
        _winPanel = winPanel;
        _winPanel.SetActive(false);
    }

    public void Activate()
    {
        _winPanel.SetActive(true);
    }
}