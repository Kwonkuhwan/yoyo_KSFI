using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class TouchControl_2 : MonoBehaviour
{
    private IDisposable _touchDownStream;
    private IDisposable _touchUpStream;
    private IDisposable _longClickStream;
    private IDisposable _longClickUpdateStream;
    private IDisposable _shortClickUpdateStream;
    private IDisposable _pinchUpdateStream;

    private bool _isLongClickTriggered = false;
    private float _clickDuration = 1f; // 클릭 유지 시간 (2초)

    private Quaternion _lastRotation;
    private bool _isRotation = false;

    public string targetTag = "TouchObject"; // 클릭을 허용할 태그
    private Dictionary<GameObject, InitObjct> _touchObjects = new Dictionary<GameObject, InitObjct>();

    private GameObject clickedObject;
    private Transform selectedObject;

    public class InitObjct
    {
        public Vector3 scale;
        public Vector3 position;
        public Quaternion rotation;
    }

    void Start()
    {
        _touchObjects.Clear();

        // 터치가 시작될 때의 이벤트 스트림
        _touchDownStream = Observable.EveryUpdate()
            .Where(_ => Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            .Subscribe(_ => OnTouchDown());

        // 터치가 종료될 때의 이벤트 스트림
        _touchUpStream = Observable.EveryUpdate()
            .Where(_ => Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            .Subscribe(_ => OnTouchUp());

        // 핀치 제스처(두 손가락으로 확대/축소)를 위한 스트림
        _pinchUpdateStream = Observable.EveryUpdate()
            .Where(_ => Input.touchCount == 2)
            .Subscribe(_ => HandlePinchGesture());
    }

    void OnTouchDown()
    {
        _longClickStream = Observable.Timer(TimeSpan.FromSeconds(_clickDuration))
            .TakeUntil(Observable.EveryUpdate().Where(_ => Input.touchCount == 0 || Input.GetTouch(0).phase == TouchPhase.Ended))
            .Select(_ => Camera.main != null ? Camera.main.ScreenPointToRay(Input.GetTouch(0).position) : default)
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

        var ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
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

    void OnTouchUp()
    {
        _longClickStream?.Dispose();
        _longClickUpdateStream?.Dispose();
        _shortClickUpdateStream?.Dispose();

        // if (!_isLongClickTriggered && clickedObject != null && clickedObject.CompareTag(targetTag))
        // {
        //     Debug.Log("2초 미만으로 클릭되었습니다. (OnTouchUp 처리)");
        //     HandleShortClick();
        // }

        selectedObject = null;
        _isLongClickTriggered = false;
        clickedObject = null;
        _isRotation = false;
    }

    void TriggerLongClick()
    {
        _isLongClickTriggered = true;

        Debug.Log("2초 동안 클릭을 유지했습니다. Long Click 동작을 실행합니다.");
        _longClickUpdateStream = Observable.EveryUpdate()
            .TakeUntil(Observable.EveryUpdate().Where(_ => Input.touchCount == 0 || Input.GetTouch(0).phase == TouchPhase.Ended))
            .Subscribe(_ => OnLongClickUpdate());
        HandleLongClick();
    }

    void OnLongClickUpdate()
    {
        Debug.Log("Long Click 업데이트 중...");
        if (clickedObject != null)
        {
            Vector3 touchPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y, Camera.main.WorldToScreenPoint(selectedObject.position).z);
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(touchPosition);
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
        if (_isLongClickTriggered || 2 <=Input.touchCount)
            return;
        _shortClickUpdateStream = Observable.EveryUpdate()
            .TakeUntil(Observable.EveryUpdate().Where(_ => Input.touchCount == 0 || Input.GetTouch(0).phase == TouchPhase.Ended))
            .Subscribe(_ => OnShortClickUpdate());
    }

    private void OnShortClickUpdate()
    {
        if (_isLongClickTriggered || 2 <=Input.touchCount)
            return;
        Debug.Log("2초 미만으로 클릭된 경우의 처리 로직");
        Vector3 touchDeltaPosition = new Vector3(Input.GetTouch(0).deltaPosition.x, Input.GetTouch(0).deltaPosition.y, 0);
        if (selectedObject != null)
        {
            float rotationX = touchDeltaPosition.x * 0.5f; // 회전 속도 조정 가능
            float rotationY = touchDeltaPosition.y * 0.5f;
            selectedObject.Rotate(Vector3.up, -rotationX, Space.World);
            selectedObject.Rotate(Vector3.right, rotationY, Space.World);
            if (_lastRotation == selectedObject.rotation)
                return;
            _lastRotation = selectedObject.rotation;
            _isRotation = true;
        }
    }

    private void HandlePinchGesture()
    {
        _shortClickUpdateStream?.Dispose();
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

        float difference = currentMagnitude - prevMagnitude;

        if (selectedObject != null)
        {
            selectedObject.localScale += Vector3.one * difference * 0.01f; // 확대/축소 속도 조정 가능
            if (2 < selectedObject.localScale.x)
            {
                selectedObject.localScale = Vector3.one * 2f;
            }
            if (0.5 > selectedObject.localScale.x)
            {
                selectedObject.localScale = Vector3.one * 0.5f;
            }
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
        _touchDownStream?.Dispose();
        _touchUpStream?.Dispose();
        _longClickStream?.Dispose();
        _shortClickUpdateStream?.Dispose();
        _pinchUpdateStream?.Dispose();
    }
}
// Raycast를 통해 클릭된 오브젝트 감지
        // Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // RaycastHit hit;
        // if (Physics.Raycast(ray, out hit))
        // {
        //     clickedObject = hit.collider.gameObject;
        //     if (clickedObject.CompareTag(targetTag))
        //     {
        //         selectedObject = clickedObject.transform;
        //         // 2초 후에 Long Click을 처리하기 위한 타이머 설정
        //         _longClickStream = Observable.Timer(TimeSpan.FromSeconds(_clickDuration))
        //             .TakeUntil(Observable.EveryUpdate().Where(_ => Input.GetMouseButtonUp(0)))
        //             .Select(_ => Camera.main != null ? Camera.main.ScreenPointToRay(Input.mousePosition) : default)
        //             .Select(ray =>
        //             {
        //                 var hits = Physics.RaycastAll(ray).OrderBy(h => h.distance).ToArray();
        //                 var closestHit = FirstOrHit(hits);
        //                 // var isHit = Physics.Raycast(ray, out var result, 10f);
        //                 return Tuple.Create(hits.Length > 0, closestHit);
        //             }).Where(x =>
        //             {
        //                 if (!x.Item1 || !x.Item2.collider.CompareTag(targetTag))
        //                     return false;
        //                 clickedObject = x.Item2.collider.gameObject;
        //                 selectedObject = x.Item2.collider.transform;
        //                 return true;
        //
        //             })
        //             .Subscribe(
        //                 _ => TriggerLongClick(),
        //                 () => {
        //                     // 2초가 되기 전에 클릭이 해제된 경우 처리
        //                     if (!_isLongClickTriggered)
        //                     {
        //                         Debug.Log("2초 미만으로 클릭되었습니다.");
        //                         HandleShortClick();
        //                     }
        //                 });
        //     }
        // }