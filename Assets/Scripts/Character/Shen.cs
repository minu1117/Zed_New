using DG.Tweening;
using UnityEngine;

public class Shen : BossEnemy
{
    [SerializeField] private Weapon duskSword;
    [SerializeField] private GameObject duskSwordHand;
    [SerializeField] private Shen_DuskSword_Dummy duskSwordDummy;
    [SerializeField] private SkillButtonData duskSwordSkill;
    [SerializeField] private float duskSwordMoveDuration;
    private Shen_DuskSword_Dummy createdDuskswordDummy;

    public Transform lineLendererTransform;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Init()
    {
        base.Init();

        AddPattern(duskSwordSkill, patternDict);
        SortPattern();

        createdDuskswordDummy = Instantiate(duskSwordDummy);
        createdDuskswordDummy.SetShen(this);

        createdDuskswordDummy.transform.position = new Vector3(shotStartTransform.position.x, createdDuskswordDummy.transform.position.y, shotStartTransform.position.z);
        createdDuskswordDummy.SetInitPosY(createdDuskswordDummy.transform.position.y);
        duskSword.SetDamage(duskSwordSkill.skill.data.damage);
        duskSword.SetChamp(this);

        ResetEnemy();
    }

    public override void ResetEnemy()
    {
        createdDuskswordDummy.gameObject.SetActive(true);
        duskSword.OnFinished();
        duskSword.gameObject.SetActive(false);
    }

    public override void Update()
    {
        MoveAnimation();
        StateBehavior();
    }

    public override void OnDead()
    {
        duskSword.OnFinished();
        duskSword.gameObject.SetActive(false);
        createdDuskswordDummy.gameObject.SetActive(false);
        base.OnDead();
    }

    /********************************************** Animation Event **********************************************/
    public void OnDuskSword()
    {
        var attackClip = createdDuskswordDummy.GetAudioClip(ShenDuskSwordAudio.Attack);
        SoundManager.Instance.PlayOneShot(attackClip);

        var randomWeapon = weapons[Random.Range(0, weapons.Count)];
        var attackVoice = randomWeapon.data.voiceClips[Random.Range(0, randomWeapon.data.voiceClips.Count)];
        SoundManager.Instance.PlayOneShot(attackVoice);

        duskSword.OnReady();
    }

    public void FinishedDuskSword()
    {
        duskSword.OnFinished();
    }

    public void ActivationDuskSword()
    {
        duskSword.gameObject.SetActive(true);
    }

    public void DeactivationDuskSword()
    {
        duskSword.gameObject.SetActive(false);
    }

    public void CallDuskSword()
    {
        transform.LookAt(player.transform.position);

        var useClip = createdDuskswordDummy.GetAudioClip(ShenDuskSwordAudio.Use);
        var voiceClip = createdDuskswordDummy.GetAudioClip(ShenDuskSwordAudio.Voice);
        SoundManager.Instance.PlayOneShot(useClip);
        SoundManager.Instance.PlayOneShot(voiceClip);

        var pos = new Vector3(transform.position.x, createdDuskswordDummy.transform.position.y, transform.position.z);
        createdDuskswordDummy.transform.DOMove(pos, duskSwordMoveDuration)
                                       .OnComplete(OnNext);
    }

    private void OnNext()
    {
        var audioClip = createdDuskswordDummy.GetAudioClip(ShenDuskSwordAudio.Catch);
        SoundManager.Instance.PlayOneShot(audioClip);

        animationController.StartNextMotion();
    }
}
