using UnityEngine;

public static class Raycast
{
    // 현재 마우스 위치 바닥에 ray를 쏴 위치 반환
    public static Vector3 GetMousePointVec()
    {
        Vector3 point = Input.mousePosition;
        string layerMaskName = "Plane";
        var hit = GetHit(point, layerMaskName);

        if (hit.collider == null)
            return Vector3.zero;

        return hit.point;
    }

    // raycast hit return
    // 인자로 받은 레이어 마스크에 ray가 부딪히지 않았으면 default 값 return
    public static RaycastHit GetHit(Vector3 point, string layerMask)
    {
        var ray = Camera.main.ScreenPointToRay(point);
        RaycastHit hit;
        int mask = LayerMask.GetMask(layerMask);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            return hit;
        }

        return default;
    }

    // raycast hit return
    // 레이어 마스크 상관 없이 ray 사용 (ray가 부딪히지 않으면 default return)
    public static RaycastHit GetHit(Vector3 point)
    {
        var ray = Camera.main.ScreenPointToRay(point);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            return hit;
        }

        return default;
    }
}
