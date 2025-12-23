using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class MouseObjectControl : MonoBehaviour
{
    private IDisposable _mouseDownStream;
    private IDisposable _mouseUpStream;
    private IDisposable _longClickStream;
    private IDisposable _longClickUpdateStream;
    private IDisposable _shortClickUpdateStream;
    private IDisposable _zoomUpdateStream;

    private bool _isLongClickTriggered = false;
    private float _clickDuration = 1f; // 클릭 유지 시간 (2초)

    public string targetTag = "TouchObject"; // 클릭을 허용할 태그
    private Dictionary<GameObject, InitObjct> _touchObjects = new Dictionary<GameObject, InitObjct>();

    private GameObject clickedObject;
    private Transform selectedObject;

    private bool _isRotation = false;

    private Quaternion _lastRotation;
    public class InitObjct
    {
        public Vector3 scale;
        public Vector3 position;
        public Quaternion rotation;
    }

    void Start()
    {
        _touchObjects.Clear();

        // 마우스 버튼이 눌렸을 때의 이벤트 스트림
        _mouseDownStream = Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Subscribe(_ => OnMouseDown());

        // 마우스 버튼이 떼어졌을 때의 이벤트 스트림
        _mouseUpStream = Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonUp(0))
            .Subscribe(_ => OnMouseUp());

        // 마우스 휠을 통한 확대/축소 스트림
        _zoomUpdateStream = Observable.EveryUpdate()
            .Where(_ => Input.mouseScrollDelta.y != 0)
            .Subscribe(_ => HandleZoom(Input.mouseScrollDelta.y));
    }

    void OnMouseDown()
    {
        _longClickStream = Observable.Timer(TimeSpan.FromSeconds(_clickDuration))
            .TakeUntil(Observable.EveryUpdate().Where(_ => Input.GetMouseButtonUp(0)))
            .Select(_ => Camera.main != null ? Camera.main.ScreenPointToRay(Input.mousePosition) : default)
            .Select(ray =>
            {
                var hits = Physics.RaycastAll(ray).OrderBy(h => h.distance).ToArray();
                var closestHit = FirstOrHit(hits);
                return Tuple.Create(hits.Length > 0, closestHit);
            }).Where(x =>
            {
                if (!x.Item1 || !x.Item2.collider.CompareTag(targetTag))
                    return false;
                clickedObject = x.Item2.collider.gameObject;
                selectedObject = x.Item2.collider.transform;
                return !_isRotation;

            })
            .Subscribe(
                _ => TriggerLongClick(),
                () =>
                {
                    if (!_isLongClickTriggered)
                    {
                        Debug.Log("2초 미만으로 클릭되었습니다.");
                    }
                });

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            clickedObject = hit.collider.gameObject;
            if (clickedObject.CompareTag(targetTag))
            {
                selectedObject = clickedObject.transform;
                _lastRotation = selectedObject.rotation;
                if (!_touchObjects.ContainsKey(clickedObject))
                {
                    _touchObjects.Add(clickedObject, new InitObjct
                    {
                        scale = selectedObject.localScale,
                        position = selectedObject.position,
                        rotation = selectedObject.rotation
                    });
                }
            }
        }
        HandleShortClick();
    }

    RaycastHit FirstOrHit(RaycastHit[] hits)
    {
        return hits.FirstOrDefault(hit => hit.transform.CompareTag(targetTag));
    }

    void OnMouseUp()
    {
        _longClickStream?.Dispose();
        _longClickUpdateStream?.Dispose();
        _shortClickUpdateStream?.Dispose();

        // if (!_isLongClickTriggered && clickedObject != null && clickedObject.CompareTag(targetTag))
        // {
        //     Debug.Log("2초 미만으로 클릭되었습니다. (OnMouseUp 처리)");
        //     HandleShortClick();
        // }

        _isRotation = false;
        selectedObject = null;
        _isLongClickTriggered = false;
        clickedObject = null;
        
    }

    void TriggerLongClick()
    {
        _isLongClickTriggered = true;

        Debug.Log("2초 동안 클릭을 유지했습니다. Long Click 동작을 실행합니다.");
        _longClickUpdateStream = Observable.EveryUpdate()
            .TakeUntil(Observable.EveryUpdate().Where(_ => Input.GetMouseButtonUp(0)))
            .Subscribe(_ => OnLongClickUpdate());
        HandleLongClick();
    }

    void OnLongClickUpdate()
    {
        Debug.Log("Long Click 업데이트 중...");
        if (clickedObject != null)
        {
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(selectedObject.position).z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            selectedObject.position = worldPosition;
        }
    }

    void HandleLongClick()
    {
        if (clickedObject != null && clickedObject.CompareTag(targetTag))
        {
            // Long Click 처리 로직
        }
    }

    void HandleShortClick()
    {
        if (_isLongClickTriggered)
            return;
        _shortClickUpdateStream = Observable.EveryUpdate()
            .TakeUntil(Observable.EveryUpdate().Where(_ => Input.GetMouseButtonUp(0)))
            .Subscribe(_ => OnShortClickUpdate());
    }

    private void OnShortClickUpdate()
    {
        if (_isLongClickTriggered)
            return;

        // if (!_isRotation && (selectedObject.rotation != _touchObjects[selectedObject.gameObject].rotation))
        //     _isRotation = true;
        Debug.Log("2초 미만으로 클릭된 경우의 처리 로직");
        Vector3 mouseDeltaPosition = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
        if (selectedObject != null)
        {
            
            float rotationX = mouseDeltaPosition.x * 5f; // 회전 속도 조정 가능
            float rotationY = mouseDeltaPosition.y * 5f;
            selectedObject.Rotate(Vector3.up, -rotationX, Space.World);
            selectedObject.Rotate(Vector3.right, rotationY, Space.World);
            // ReSharper disable once RedundantCheckBeforeAssignment
            if (_lastRotation == selectedObject.rotation)
                return;
            _lastRotation = selectedObject.rotation;
            _isRotation = true;
        }
    }

    private void HandleZoom(float zoomDelta)
    {
        if (selectedObject != null)
        {
            selectedObject.localScale += Vector3.one * zoomDelta * 0.1f; // 확대/축소 속도 조정 가능
        }
    }

    public void ResetObject()
    {
        foreach (var touchObject in _touchObjects)
        {
            touchObject.Key.transform.localScale = touchObject.Value.scale;
            touchObject.Key.transform.position = touchObject.Value.position;
            touchObject.Key.transform.rotation = touchObject.Value.rotation;
        }
    }

    void OnDestroy()
    {
        _mouseDownStream?.Dispose();
        _mouseUpStream?.Dispose();
        _longClickStream?.Dispose();
        _shortClickUpdateStream?.Dispose();
        _zoomUpdateStream?.Dispose();
    }
}