using UnityEngine;

public class Shen_DuskSword_Dummy : MonoBehaviour
{
    [SerializeField] private LineRenderer duskSwordLineRenderer;
    public Transform LineRendererTr;
    private Shen shen;

    public void SetShen(Shen shen) { this.shen = shen; }

    public void Update()
    {
        DrawCurvedLine();
    }

    private void DrawCurvedLine()
    {
        if (shen == null)
            return;

        duskSwordLineRenderer.positionCount = 2; // 두 점으로 선을 만듭니다
        duskSwordLineRenderer.SetPosition(0, shen.shotStartTransform.position); // A 오브젝트 위치
        duskSwordLineRenderer.SetPosition(1, LineRendererTr.position);          // B 오브젝트 위치
    }
}
