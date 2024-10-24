using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Canvas의 CanvasMode 설정
public enum CanvasMode
{
    WorldSpace,
    Overlay,
}

// Slider가 HP, MP 중 어떤 것을 조정할 지 설정
public enum SliderMode
{
    HP,
    MP,
}

public enum UsedSlider
{
    Player,
    Normal,
    Boss,
}

public class StatusController : MonoBehaviour
{
    public Slider slider;
    public Canvas canvas;
    public ChampBase champ;         // HP 또는 MP 값을 조정할 캐릭터
    public TextMeshProUGUI tmp;
    public CanvasMode canvasMode;
    public SliderMode sliderMode;
    public UsedSlider usedSlider;
    private CharacterData data;
    private Camera cam;

    public Slider shieldSlider;
    public bool isShield;
    private float maxShieldValue;

    private void Awake()
    {
        switch (usedSlider)
        {
            case UsedSlider.Player:
                champ = Zed.Instance;           // 플레이어 할당
                champ.SetStatusController(sliderMode, this);
                break;
            case UsedSlider.Boss:
                
                break;
            default:
                break;
        }

        ChangeCanvasMode(canvasMode);               // 캔버스 모드 변경
        data = champ.data;                          // 조정할 데이터 할당

        SetMaxValue();      // 현재 체력 or 현재 마나를 최대 값으로 변경
    }

    // 모드에 따라 현재 체력 or 마나 설정
    public void SetCurrentValue()
    {
        switch (sliderMode)
        {
            case SliderMode.HP:
                SetCurrentHp();
                break;
            case SliderMode.MP:
                SetCurrentMp();
                break;
        }
    }

    // 모드에 따라 현재 체력 or 마나를 최대 값으로 설정
    public void SetMaxValue()
    {
        switch (sliderMode)
        {
            case SliderMode.HP:
                SetMaxHp();
                break;
            case SliderMode.MP:
                SetMaxMp();
                break;
        }
    }

    // 현재 체력 변경
    public void SetCurrentHp()
    {
        data.currentHp = Math.Clamp(data.currentHp, 0, data.maxHp);
        champ.data.currentHp = data.currentHp;
        slider.value = data.currentHp / data.maxHp;
        SetText($"{data.currentHp} / {data.maxHp}");
    }

    // 최대 체력으로 설정
    public void SetMaxHp()
    {
        data.currentHp = data.maxHp;
        SetCurrentHp();
        SetText($"{data.currentHp} / {data.maxHp}");
    }

    // 현재 마나 변경
    public void SetCurrentMp()
    {
        data.currentMp = Math.Clamp(data.currentMp, 0, data.maxMp);
        champ.data.currentMp = data.currentMp;
        slider.value = data.currentMp / data.maxMp;
        SetText($"{data.currentMp} / {data.maxMp}");
    }

    // 최대 마나로 설정
    public void SetMaxMp()
    {
        data.currentMp = data.maxMp;
        SetCurrentMp();
        SetText($"{data.currentMp} / {data.maxMp}");
    }

    public void SetShield(float shieldValue)
    {
        if (shieldSlider == null)
            return;

        if (shieldValue <= 0)
            return;

        isShield = true;

        maxShieldValue = shieldValue;
        shieldSlider.value = shieldValue / maxShieldValue;

        SetText($"{data.currentHp} / {data.maxHp} ({maxShieldValue})");
        shieldSlider.gameObject.SetActive(true);
    }

    public float HitShield(float damage)
    {
        if (shieldSlider == null)
            return 0;

        var currentShield = shieldSlider.value * maxShieldValue;
        shieldSlider.value = (currentShield - damage) / maxShieldValue;

        if (currentShield - damage <= 0)
        {
            isShield = false;
            shieldSlider.value = 0;
            shieldSlider.gameObject.SetActive(false);

            SetText($"{data.currentHp} / {data.maxHp}");
            return Mathf.Abs(currentShield - damage);
            //data.currentHp -= Mathf.Abs(shield - damage);
            //SetText($"{data.currentHp} / {data.maxHp})");
        }
        else
        {
            SetText($"{data.currentHp} / {data.maxHp} ({shieldSlider.value * maxShieldValue})");
            return 0;
        }
    }

    private void SetText(string text)
    {
        tmp.text = text;
    }

    public void Update()
    {
        LookCamera();
    }

    // UI가 카메라를 바라보게 해줌
    // 3D 화면 상 캐릭터 머리 위에 슬라이더 UI가 위치하기 때문
    private void LookCamera()
    {
        if (canvasMode != CanvasMode.WorldSpace || cam == null)         // CanvasMode가 WorldSpace가 아니거나 카메라가 없을 경우 return
            return;

        slider.gameObject.transform.rotation = cam.transform.rotation;  // Slider의 회전 값을 카메라의 회전 값으로 변경
    }

    // CanvasMode에 따라 화면에 어떻게 보여줄 지 설정
    private void ChangeCanvasMode(CanvasMode mode)
    {
        switch (mode)
        {
            case CanvasMode.WorldSpace: // 3D 화면에 보일 UI
                cam = Camera.main;                              // 카메라 가져오기
                canvas.renderMode = RenderMode.WorldSpace;      // canvas의 renderMode 변경 -> WorldSpace
                canvas.worldCamera = cam;                       // canvas의 worldCamera 설정

                // Slider 오브젝트 위치 조정 -> 해당 스크립트가 붙어 있는 오브젝트의 Collider보다 조금 위로
                slider.gameObject.transform.position = gameObject.transform.position + Vector3.up * (gameObject.GetComponent<Collider>().bounds.size.y);
                break;
            case CanvasMode.Overlay:   // 2D 화면에 보일 UI
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                break;
        }
    }
}
