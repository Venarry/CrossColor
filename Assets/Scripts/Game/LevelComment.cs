using TMPro;
using UnityEngine;

public class LevelComment : MonoBehaviour
{
    [SerializeField] private TMP_Text _comment;

    private string[] _comments;
    private int _commentIndex;

    public void SetData(string[] comments)
    {
        gameObject.SetActive(true);

        _comments = comments;
        _commentIndex = 0;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        _comments = null;
    }

    public void ShowNextComment()
    {
        if (_comments == null)
            return;

        if (_commentIndex >= _comments.Length)
            return;

        _comment.text = _comments[_commentIndex];
        _commentIndex++;
    }
}
