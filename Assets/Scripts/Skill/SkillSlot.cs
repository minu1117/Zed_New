using System.Collections.Generic;
using UnityEngine;

public class SkillSlot : MonoBehaviour
{
    public List<SkillButtonData> buttonDatas;           // 스킬 버튼마다 할당할 데이터
    public SkillExcutor excutorPrefab;                  // 스킬 실행기
    private Dictionary<string, SkillExcutor> slotDict;  // KeyCode(string)를 Key로 하여 실행기를 찾아 사용하기 위한 Dictionary
    private GameObject slotObj;

    // 초기 설정
    public void Init()
    {
        if (excutorPrefab == null)                              // 실행기 프리팹이 없을 경우 return (실행기를 생성할 수 없어 스킬 사용 불가능)
            return;

        slotDict = new Dictionary<string, SkillExcutor>();
        slotObj = new GameObject("Skill_Slot");                 // 생성할 스킬을 담아둘 오브젝트 생성
        slotObj.transform.SetParent(transform, false);          // 부모 설정

        CreateSkillExcutors();                                  // 스킬 생성기 생성
    }

    public void SetSlotParent(GameObject parent)
    {
        slotObj.transform.SetParent(parent.transform);
    }
    public Dictionary<string, SkillExcutor> GetSlotDict() { return slotDict; }
    public GameObject GetSlotObj() { return slotObj; }
    
    // 스킬 생성기 모두 생성
    private void CreateSkillExcutors()
    {
        if (slotDict == null)
            return;

        foreach (var buttonData in buttonDatas)     // 스킬 버튼 데이터 List 순회
        {
            CreateExcutor(slotObj, buttonData);
        }
    }

    public void CreateExcutor(GameObject parent, SkillButtonData buttonData)
    {
        if (slotDict.ContainsKey(buttonData.keycode))               // 이미 있는 스킬의 생성기는 중복 생성 X
            return;

        var excutor = Instantiate(excutorPrefab);                   // 스킬 실행기 생성
        excutor.transform.SetParent(parent.transform, false);       // 실행기 오브젝트의 부모 설정
        excutor.Init(gameObject, buttonData);                       // 실행기 초기 설정 실행

        if (buttonData.keycode == string.Empty)                     // 버튼 데이터의 KeyCode가 비었을 경우
        {
            buttonData.keycode = GetNotDuplicatedID();              // 슬롯 내부에 있는 스킬들과 겹치지 않는 랜덤 ID 부여
        }

        slotDict.Add(buttonData.keycode, excutor);                  // 스킬 슬롯에 생성기 추가 (이름과 함께)
    }

    // 슬롯 내부에 있는 스킬들과 겹치지 않는 랜덤 ID 추출
    private string GetNotDuplicatedID()
    {
        string randomID = GetRandomID().ToString(); // 랜덤 ID
        while (slotDict.ContainsKey(randomID))      // 슬롯 내부에 같은 ID가 있을 경우 반복
        {
            randomID = GetRandomID().ToString();    // 랜덤 ID 재할당
        }

        return randomID;
    }

    // 랜덤 ID 추출
    private int GetRandomID()
    {
        return Random.Range(0, int.MaxValue);   // 0 ~ int.MaxValue 중 랜덤 값 추출
    }
}
