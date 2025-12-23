using UnityEngine;
using Cinemachine;

[ExecuteAlways] // 에디터 모드에서도 업데이트
public class SmoothPathFromChildren : MonoBehaviour
{
    public CinemachineSmoothPath smoothPath;

    private void Update()
    {
        if (smoothPath == null)
        {
            Debug.LogWarning("Cinemachine SmoothPath를 할당하세요!");
            return;
        }

        // 자식 오브젝트 가져오기
        Transform[] childTransforms = GetComponentsInChildren<Transform>();
        // 부모 자신 제외
        childTransforms = System.Array.FindAll(childTransforms, t => t != transform);

        // Waypoint 리스트 초기화
        var waypoints = new CinemachineSmoothPath.Waypoint[childTransforms.Length];

        // 자식 위치로 Waypoint 설정
        for (int i = 0; i < childTransforms.Length; i++)
        {
            waypoints[i] = new CinemachineSmoothPath.Waypoint
            {
                position = childTransforms[i].localPosition, // SmoothPath는 로컬 포지션 사용
                roll = 0f // 기본 설정 (필요시 수정 가능)
            };
        }

        // SmoothPath에 Waypoint 적용
        smoothPath.m_Waypoints = waypoints;
    }
}
