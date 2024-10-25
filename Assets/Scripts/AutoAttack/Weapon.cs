using UnityEngine;

public class Weapon : MonoBehaviour, IDamageable
{
    public WeaponData data;                 // 무기 데이터
    public TrailRenderer trailRenderer;     // 무기를 따라다닐 TrailRenderer
    private bool isReady = false;           // 무기 준비 상태 (공격 가능, 불가능)
    private Collider coll;                  // 무기 Collider

    private ChampBase champ;                // 무기 소지자

    public void Awake()
    {
        coll = GetComponent<Collider>();
        coll.enabled = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!isReady)   // 준비가 되지 않았을 경우 return
            return;

        if (other.gameObject.tag == champ.tag)  // 같은 태그일 시 return (팀킬 방지)
            return;

        if (other.TryGetComponent(out ChampBase champion)) // 부딪힌 오브젝트에서 ChampBase 추출 성공 시
        {
            DealDamage(champion, data.damage);             // 데미지 부여
        }
    }

    public void SetChamp(ChampBase champion) { champ = champion; }

    public void SetActiveTrailRenderer(bool active)
    {
        trailRenderer.gameObject.SetActive(active);
    }

    // 무기 준비 완료
    public void OnReady()
    {
        isReady = true;         // 준비 상태 변경 (완료)
        coll.enabled = true;    // Collider 활성화 (부딪힐 수 있게)

        if (data.useClips == null || data.useClips.Count == 0)          // 시전 사운드가 없을 시 return
            return;

        int randomIndex = Random.Range(0, data.useClips.Count);         // 랜덤 인덱스 (평타 준비 완료 사운드)
        SoundManager.Instance.PlayOneShot(data.useClips[randomIndex]);  // 사운드 매니저에서 재생
    }

    // 무기 준비 해제
    public void OnFinished()
    {
        isReady = false;        // 준비 상태 변경 (미완료)
        coll.enabled = false;   // Collider 비활성화 (부딪히지 않게)
        //Debug.Log("Finishe");
    }

    public void SetDamage(float dmg)
    {
        data.damage = dmg;
    }

    // 타겟에 데미지 부여
    public void DealDamage(ChampBase target, float damage)
    {
        target.OnDamage(damage);

        if (data.attackClips == null || data.attackClips.Count == 0)        // 때릴 때 나올 사운드가 없을 경우 return
            return;

        int randomIndex = Random.Range(0, data.attackClips.Count);          // 랜덤 인덱스 (때린 사운드 클립)
        SoundManager.Instance.PlayOneShot(data.attackClips[randomIndex]);   // 사운드 매니저에서 재생
    }
}
