using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UniRx;
using UnityEditor;
using UnityEngine.EventSystems;

public class ButtonManager : MonoBehaviour //, IPointerDownHandler
{
#region singleton

    private static ButtonManager instance;
    public static ButtonManager Instance { get { return instance; } }

#endregion
    [SerializeField] private GameObject highlightObj;
    private List<Button> _allButtons;
    private List<Button> _buttons;
    public float noTouchDuration = 5f; // n초 동안 터치하지 않을 경우 (기본값: 3초)
    public int maxTouchAttempts = 1; // n번 다른 곳 터치 시 (기본값: 3번)
    public float highlightDuration = 0.5f; // 강조 효과 지속 시간 (기본값: 2초)

    private int _touchAttempts;
    private float _touchTimer;
    private bool _isCounting;
    private List<Button> _targetButtons;
    private List<Image> _targetImages;

    private Dictionary<Button, GameObject> _targetButtonDictionary;

    [SerializeField] private List<Button> disableBtns;
    [SerializeField] private Transform parent;

    private EventSystem _eventSystem;
    private PointerEventData _pointerEventData;
    private Dictionary<Button, GameObject> _highlightedButtons = new Dictionary<Button, GameObject>();
    private Dictionary<Image, GameObject> _highlightedImages = new Dictionary<Image, GameObject>();
    private Queue<GameObject> _highlightImagePool = new Queue<GameObject>();
    [SerializeField] private Transform highlightParent;
    private void Awake()
    {
        instance = this;
        Button[] buttons = Resources.FindObjectsOfTypeAll<Button>();
        List<Button> buttonList = new List<Button>(buttons);
        //return buttonList;
        _eventSystem = EventSystem.current;
        _pointerEventData = new PointerEventData(_eventSystem);
        _allButtons = buttonList;
        InitializeSection();
        DisableAllButtons();
        //AddGlobalTouchListener();
        // foreach (Button button in _allButtons)
        // {
        //     Debug.Log(button.GetComponent<RectTransform>().rect);
        // }

        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 또는 첫 번째 터치
            .Subscribe(_ =>
            {
                // var eventSystem = EventSystem.current;
                // PointerEventData pointerData = new PointerEventData(eventSystem);
                //
                // Debug.Log(eventSystem.currentSelectedGameObject);
                // //eventSystem.currentSelectedGameObject

                OnPointerDownCustom();
            })
            .AddTo(this);
    }

    void Start()
    {
        // 하이라키에서 모든 버튼을 찾아서 배열에 저장

    }

    void Update()
    {
        // if (_isCounting)
        // {
        //     _touchTimer += Time.deltaTime;
        //     if (_touchTimer >= noTouchDuration)
        //     {
        //         OnOtherTouch(_targetButtons.ToArray());
        //         ResetTouchTimer();
        //     }
        // }
        _touchTimer += Time.deltaTime;
        if (_touchTimer >= noTouchDuration)
        {
            OnOtherTouch(_targetButtons.ToArray());
            ResetTouchTimer();
        }
    }

    // 섹션마다 초기화하는 함수
    public void InitializeSection(float? newNoTouchDuration = null, int? newMaxTouchAttempts = null)
    {
        _buttons ??= new List<Button>();
        _targetButtons ??= new List<Button>();
        _targetImages ??= new List<Image>();
        _targetButtonDictionary ??= new Dictionary<Button, GameObject>();
        noTouchDuration = newNoTouchDuration ?? noTouchDuration;
        maxTouchAttempts = newMaxTouchAttempts ?? maxTouchAttempts;
        ResetTouchAttempts();
        ResetTouchTimer();

    }

    public void OnPointerDownCustom()
    {
        if (_eventSystem == null)
        {
            Debug.LogWarning("EventSystem이 활성화되지 않았습니다.");
            return;
        }

        _pointerEventData.position = Input.mousePosition;

        // Raycast 결과 리스트 생성
        var raycastResults = new List<RaycastResult>();
        _eventSystem.RaycastAll(_pointerEventData, raycastResults);

        // Raycast 결과에서 pointerPress 값 확인
        if (raycastResults.Where(raycastResult => null != raycastResult.gameObject.GetComponent<Button>()).Any(raycastResult => raycastResult.gameObject.GetComponent<Button>().interactable))
        {
            return;
        }

        if (raycastResults.Count > 0)
        {
            _pointerEventData.pointerPress = raycastResults[0].gameObject;
            Debug.Log("Pointer Pressed on: " + _pointerEventData.pointerPress.name);
        }
        else
        {
            Debug.Log("No object pressed");
        }

        // if (_pointerEventData.pointerPress.GetComponent<Button>() && !_pointerEventData.pointerPress.GetComponent<Button>().gameObject.activeSelf)
        //     return;
        if (!_pointerEventData.pointerPress || _pointerEventData.pointerPress.GetComponent<Button>() == null 
            ||(_pointerEventData.pointerPress.GetComponent<Button>() && !_pointerEventData.pointerPress.GetComponent<Button>().gameObject.activeSelf))
        {
            ResetTouchTimer();
            OnOtherTouch(_targetButtons.ToArray());
        }
    }


    // 다른 곳을 터치했을 때 호출되는 함수
    public void OnOtherTouch(params Button[] targetButtons)
    {
        _touchAttempts++;
        if (_touchAttempts >= maxTouchAttempts)
        {
            HighlightButton(targetButtons);
            HighlightImage();
            ResetTouchAttempts();
        }
        else
        {
            StartTouchTimer();
        }
    }


    // 모든 버튼을 비활성화하는 함수, 예외 버튼들을 등록할 수 있음
    public void DisableAllButtons()
    {
        Canvas.ForceUpdateCanvases();
        _buttons.Clear();
        foreach (Button button in _allButtons)
        {
            //if (exceptionButtons == null || !exceptionButtons.Contains(button))
            if (disableBtns == null || !disableBtns.Contains(button))
            {
                button.interactable = false;
                ColorBlock colors = button.colors;
                colors.disabledColor = colors.normalColor;
                button.colors = colors;
                //Debug.Log(button.gameObject.name + " is disabled");
                _buttons.Add(button);
            }
        }
    }

    public void NextEnable()
    {
        // bool isEnabled = false;
        // foreach (var btn in _targetButtons)
        // {
        //     if (!btn.Equals(disableBtns[2]))
        //     {
        //         isEnabled = true;
        //     }
        // }
        // if (isEnabled)
        // {
        //     _targetButtons.Add(disableBtns[2]);
        // }
        disableBtns[2].interactable = true;
    }

    public void NextDisable()
    {
        // int index = -1;
        // for (int i =0; i<_targetButtons.Count; ++i)
        // {
        //     if (!_targetButtons[i].Equals(disableBtns[2]))
        //         continue;
        //     index = i;
        //     break;
        // }
        // if (index != -1)
        // {
        //     _targetButtons.RemoveAt(index);
        // }
        disableBtns[2].interactable = false;
    }

    // 모든 버튼을 활성화하는 함수
    public void EnableAllButtons()
    {
        foreach (Button button in _allButtons)
        {
            button.interactable = true;
        }
    }

    // 특정 버튼(들)만 활성화하고 나머지 버튼은 비활성화하는 함수
    public void EnableSpecificButton(params Button[] targetButtons)
    {
        RemoveAllHighlights();
        RemoveHighlightImage();
        _targetButtons.Clear();
        // foreach (var value in _targetButtonDictionary)
        // {
        //     value.Value.SetActive(true);
        // }
        foreach (Button button in _buttons)
        {
            button.interactable = false;
            ColorBlock colors = button.colors;
            colors.disabledColor = colors.normalColor;
            button.colors = colors;
        }
        foreach (Button targetButton in targetButtons)
        {
            targetButton.interactable = true;
            _targetButtons.Add(targetButton);
        }
        _targetButtons.Add(disableBtns[2]);
    }

    public void SetEvaluationButtons()
    {
        _targetButtons.Clear();
        foreach (Button btn in _buttons)
        {
            btn.interactable = true;
        }
        //EnableSpecificButton(_buttons.ToArray());
    }

    public void EnableSpecificButton()
    {
        RemoveAllHighlights();
        RemoveHighlightImage();
        _targetButtons.Clear();
        // foreach (var value in _targetButtonDictionary)
        // {
        //     value.Value.SetActive(true);
        // }
        foreach (Button button in _buttons)
        {
            button.interactable = false;
            ColorBlock colors = button.colors;
            colors.disabledColor = colors.normalColor;
            button.colors = colors;
        }
        _targetButtons.Add(disableBtns[2]);
    }

    public void EnableSpecificButton(List<SwitchButtonCheck> targetButtons)
    {
        RemoveAllHighlights();
        RemoveHighlightImage();
        _targetButtons.Clear();
        // foreach (var value in _targetButtonDictionary)
        // {
        //     value.Value.SetActive(true);
        // }
        foreach (Button button in _buttons)
        {
            button.interactable = false;
            ColorBlock colors = button.colors;
            colors.disabledColor = colors.normalColor;
            button.colors = colors;
        }

        foreach (SwitchButtonCheck targetButton in targetButtons)
        {
            targetButton.btn.interactable = true;
            if (!targetButton.select)
            {
                _targetButtons.Add(targetButton.btn);
            }
        }
        _targetButtons.Add(disableBtns[2]);
    }

    public void EnableSpecificImage(params GameObject[] targetImages)
    {
        if (null == _targetImages)
            return;
        _targetImages.Clear();
        foreach (var targetImage in targetImages)
        {
            _targetImages.Add(targetImage.GetComponent<Image>());
        }
    }

    // 특정 버튼(들)을 강조하는 함수 (버튼 크기와 동일한 이미지 깜빡임 효과 추가)
    public void HighlightButton(params Button[] targetButtons)
    {
        foreach (Button targetButton in targetButtons)
        {
            if (!_highlightedButtons.ContainsKey(targetButton))
            {
                StartCoroutine(BlinkHighlight(targetButton));
            }
        }
    }

    public void HighlightObj()
    {
        HighlightImage();
        if (null == _targetButtons)
            return;
        foreach (var btn in _targetButtons)
        {
            if (!_highlightedButtons.ContainsKey(btn))
            {
                StartCoroutine(BlinkHighlight(btn));
            }
        }
    }

    public void HighlightImage()
    {
        if (null == _targetImages)
            return;
        foreach (Image targetImage in _targetImages)
        {
            if (!_highlightedImages.ContainsKey(targetImage))
            {
                StartCoroutine(BlinkHighlightImage(targetImage));
            }
        }
    }

    private GameObject GetHighlightImageFromPool()
    {
        if (_highlightImagePool.Count > 0)
        {
            return _highlightImagePool.Dequeue();
        }
        else
        {
            var newHighlightImage = Instantiate(highlightObj, highlightParent);
            // newHighlightImage.AddComponent<CanvasRenderer>();
            // newHighlightImage.AddComponent<Image>();
            newHighlightImage?.SetActive(true);
            return newHighlightImage;
        }
        return null;
    }

    private void ReturnHighlightImageToPool(GameObject highlightImage)
    {
        //highlightImage.SetActive(false);
        highlightImage.transform.SetParent(highlightParent);
        _highlightImagePool.Enqueue(highlightImage);
    }

    // 강조 효과를 깜빡이게 하는 코루틴 함수
    private IEnumerator BlinkHighlight(Button targetButton)
    {
        if (!targetButton.interactable)
            yield break;

        if (_highlightedButtons.ContainsKey(targetButton))
            yield break;

        if (!targetButton.gameObject.activeInHierarchy)
            yield break;

        //GameObject highlightImage = new GameObject("HighlightImage");
        //var highlightImage = GetHighlightImageFromPool(); 
        //highlightImage.transform.SetParent(targetButton.transform);
        var highlightImage = GameObject.Instantiate(highlightObj, targetButton.transform);

        Image image = highlightImage.GetComponent<Image>();
        image.rectTransform.sizeDelta = targetButton.GetComponent<RectTransform>().sizeDelta + new Vector2(5,5);;
        //image.color = new Color(0, 0.6f, 0, 0.3f); // 반투명한 노란색

        highlightImage.transform.localPosition = Vector3.zero;
        highlightImage.transform.localScale = Vector3.one;
        _highlightedButtons.Add(targetButton, highlightImage);
        /*
        for (float t = 0; t < highlightDuration; t += 0.5f)
        {
            image.enabled = !image.enabled;
            yield return new WaitForSeconds(0.5f);
        }
        */
        image.enabled = true;
        yield return new WaitForSeconds(highlightDuration);
        if (null != highlightImage)
        {
            image.enabled = false;
            //ReturnHighlightImageToPool(highlightImage);
            Destroy(highlightImage);
        }
        _highlightedButtons.Remove(targetButton);
    }

    private IEnumerator BlinkHighlightImage(Image targetImage)
    {
        if (!targetImage.gameObject.activeSelf)
            yield break;

        if (_highlightedImages.ContainsKey(targetImage))
            yield break;
        
        if (!targetImage.gameObject.activeInHierarchy)
            yield break;

        // GameObject highlightImage = new GameObject("HighlightImage");
        // highlightImage.transform.SetParent(targetImage.transform);
        var highlightImage = GameObject.Instantiate(highlightObj, targetImage.transform);
        // var highlightImage = GetHighlightImageFromPool();
        // highlightImage.transform.SetParent(targetImage.transform);

        
        highlightImage.transform.localPosition = Vector3.zero;
        highlightImage.transform.localScale = Vector3.one;

        Image image = highlightImage.GetComponent<Image>();
        image.rectTransform.sizeDelta = targetImage.GetComponent<RectTransform>().sizeDelta + new Vector2(5,5);
        //image.color = new Color(0, 0.6f, 0, 0.3f); // 반투명한 노란색

        _highlightedImages.Add(targetImage, highlightImage);
        /*
        for (float t = 0; t < highlightDuration; t += 0.5f)
        {
            image.enabled = !image.enabled;
            yield return new WaitForSeconds(0.5f);
        }
        */
        image.enabled = true;
        yield return new WaitForSeconds(highlightDuration);
        if (null != highlightImage)
        {
            image.enabled = false;
            Destroy(highlightImage);
            //ReturnHighlightImageToPool(highlightImage);
        }
        _highlightedImages.Remove(targetImage);
    }

    public void RemoveHighlight(Button targetButton)
    {
        if (!_highlightedButtons.TryGetValue(targetButton, out var button))
            return;
        Destroy(button);
        _highlightedButtons.Remove(targetButton);
    }

    public void RemoveHighlightImage()
    {
        ResetTouchAttempts();
        ResetTouchTimer();
        _targetImages.Clear();
        foreach (var img in _highlightedImages)
        {
            Destroy(img.Value);
        }
        _highlightedImages.Clear();
    }

    public void RemoveAllHighlights()
    {
        ResetTouchAttempts();
        ResetTouchTimer();
        foreach (var button in _highlightedButtons)
        {
            //ReturnHighlightImageToPool(button.Value);
            Destroy(button.Value);
        }
        _highlightedButtons.Clear();
        foreach (var img in _highlightedImages)
        {
            //ReturnHighlightImageToPool(img.Value);
            Destroy(img.Value);
        }
        _highlightedImages.Clear();
    }

    // 터치 타이머 시작
    private void StartTouchTimer()
    {
        _isCounting = true;
        _touchTimer = 0f;
    }

    // 터치 타이머 리셋
    private void ResetTouchTimer()
    {
        _isCounting = false;
        _touchTimer = 0f;
    }

    // 터치 시도 리셋
    private void ResetTouchAttempts()
    {
        _touchAttempts = 0;
    }
}
