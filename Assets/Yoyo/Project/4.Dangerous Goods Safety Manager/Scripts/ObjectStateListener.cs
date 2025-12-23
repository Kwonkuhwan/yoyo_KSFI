using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObjectStateListener : MonoBehaviour
{
    // 활성화 시 호출할 이벤트
    public UnityEvent onEnableEvent;

    // 비활성화 시 호출할 이벤트
    public UnityEvent onDisableEvent;

    // 오브젝트가 활성화될 때 호출
    private void OnEnable()
    {
        Debug.Log($"{gameObject.name}이(가) 활성화되었습니다.");
        onEnableEvent?.Invoke();
    }

    // 오브젝트가 비활성화될 때 호출
    private void OnDisable()
    {
        Debug.Log($"{gameObject.name}이(가) 비활성화되었습니다.");
        onDisableEvent?.Invoke();
    }
}
