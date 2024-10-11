using System.Collections;
using UnityEngine;

public class Effect : MonoBehaviour
{
    [SerializeField] protected ParticleSystem particle; // 사용할 파티클
    private Vector3 startPos;                           // 파티클 시작 위치

    protected virtual void Awake()
    {
        ResetParticle();    // 파티클 초기화
    }

    // 파티클 사용
    public virtual void Use()
    {
        ResetParticle();                        // 파티클 초기화
        particle.Play();                        // 파티클 시작
        StartCoroutine(CheckParticleAlive());   // 파티클 활성화 여부 확인 코루틴 시작
    }

    public virtual void Stop()
    {
        particle.Stop();
    }

    public void ResetParticle()
    {
        particle.Stop();
        particle.Clear();
    }

    public void SetStartPos(Vector3 pos)
    {
        startPos = pos;
        particle.transform.position = startPos;
    }

    // 파티클 활성화 여부 확인 코루틴
    private IEnumerator CheckParticleAlive()
    {
        yield return new WaitUntil(() => particle.IsAlive(true) == false);  // 파티클이 비활성화 될 때 까지 대기
        gameObject.SetActive(false);    // 게임오브젝트 비활성화
    }
}
