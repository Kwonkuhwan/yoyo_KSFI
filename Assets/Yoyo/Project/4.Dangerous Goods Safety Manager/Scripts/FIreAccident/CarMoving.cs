using System.Collections;
using UnityEngine;
using RJH.DangerousGoods;

public class CarMoving : MonoBehaviour
{
    [SerializeField] private Transform startPos;
    [SerializeField] private Transform carObj;
    [SerializeField] private Transform movePos;
    public float duration = 2.0f;  // 이동하는 데 걸리는 시간 (초)

    private void OnEnable()
    {
        StartCoroutine(MoveObject(carObj, movePos.position, duration));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        carObj.position = startPos.position;
    }

    IEnumerator MoveObject(Transform objectToMove, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = objectToMove.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // 이동 경로의 비율을 계산
            float t = elapsedTime / duration;

            // 오브젝트의 위치를 선형 보간하여 업데이트
            objectToMove.position = Vector3.Lerp(startPosition, targetPosition, t);

            // 경과 시간 업데이트
            elapsedTime += Time.deltaTime;
            yield return null;  // 다음 프레임까지 대기
        }

        // 이동이 끝난 후, 정확하게 목표 위치에 위치시킴
        objectToMove.position = targetPosition;
    }
}
