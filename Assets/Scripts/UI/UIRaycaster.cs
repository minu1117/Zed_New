using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UIRaycaster
{
    // 현재 마우스 위치 ray 쏘기, ray를 UI가 맞은 결과 return
    public static List<RaycastResult> GetHit(Canvas canvas)
    {
        var evSystem = canvas.GetComponent<EventSystem>();
        var raycaster = canvas.GetComponent<GraphicRaycaster>();

        PointerEventData pointerEventData = new PointerEventData(evSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        return results;
    }

    // UI가 ray에 맞았는 지 확인
    // false == 해당 위치에 UI 없음
    public static bool IsHit(Canvas canvas)
    {
        return GetHit(canvas).Count > 0;
    }
}
