using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeHandler : MonoBehaviour
{
    private Vector2 _startTouchPosition;
    private Vector2 _currentTouchPosition;
    private bool _isSwiping = false;
    private string _swipeAxis = "";
    private NonogramCell _lastSelectedCell;
    private CellsClickHandler _cellsClickHandler;

    public void Init(CellsClickHandler cellsClickHandler)
    {
        _cellsClickHandler = cellsClickHandler;

        _cellsClickHandler.WrongClicked += DisableTouching;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _startTouchPosition = Input.mousePosition;
            _isSwiping = true;
        }

        float magnitudeForSwipe = 25;
        
        if (Input.GetMouseButton(0) && _isSwiping)
        {
            _currentTouchPosition = Input.mousePosition;
            Vector2 delta = _currentTouchPosition - _startTouchPosition;

            if (_swipeAxis == "" && delta.magnitude >= magnitudeForSwipe)
            {
                if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    _swipeAxis = "x";
                else
                    _swipeAxis = "y";
            }

            if(_swipeAxis == "")
            {
                return;
            }

            if(_swipeAxis == "x")
            {
                _currentTouchPosition.y = _startTouchPosition.y;
            }
            else
            {
                _currentTouchPosition.x = _startTouchPosition.x;
            }

            PointerEventData eventData = new(EventSystem.current);
            eventData.position = _currentTouchPosition;
            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(eventData, results);

            if (results.Count == 0)
            {
                return;
            }

            foreach (RaycastResult hit in results)
            {
                if (hit.gameObject.TryGetComponent(out NonogramCell cell))
                {
                    if (_lastSelectedCell == cell)
                    {
                        return;
                    }
                    _lastSelectedCell = cell;
                    cell.ClickOnCell();
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (_swipeAxis == "")
            {
                PointerEventData eventData = new(EventSystem.current);
                eventData.position = Input.mousePosition;
                List<RaycastResult> results = new();
                EventSystem.current.RaycastAll(eventData, results);

                if (results.Count == 0)
                {
                    return;
                }

                foreach (RaycastResult hit in results)
                {
                    if (hit.gameObject.TryGetComponent(out NonogramCell cell))
                    {
                        if (_lastSelectedCell == cell)
                        {
                            return;
                        }

                        _lastSelectedCell = cell;
                        cell.ClickOnCell();
                    }
                }
            }

            _isSwiping = false;
            _lastSelectedCell = null;
            _swipeAxis = "";
        }
    }

    private void DisableTouching()
    {
        _isSwiping = false;
    }
}