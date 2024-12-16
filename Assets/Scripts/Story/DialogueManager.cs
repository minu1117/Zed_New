using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 텍스트가 나올 때 어떻게 나올 지 선택하는 enum
public enum TypingType
{
    None = -1,

    TypingSpeed,    // 일정한 스피드로 텍스트가 한 글자씩 나옴 (시간 관계 없이)
    RevealTime,     // 설정한 시간 안에 모든 텍스트가 나옴

    Count,
}

// 대화 데이터 구조체
// CSV에 작성한 데이터를 가져올 때 사용
public struct TalkData
{
    public string name;
    public string talk;
    public string address;
    public bool isEnabled;
    public bool shake;
    public bool bounce;
}

public class DialogueManager : Singleton<DialogueManager>
{
    //public Dictionary<string, List<TalkData>> allTalkData;  // CSV에서 가져온 모든 대화 정보를 저장할 곳
    public Dictionary<string, Dictionary<string, List<TalkData>>> allTalkData;  // CSV에서 가져온 모든 대화 정보를 저장할 곳
    public List<string> csvFileNames;
    private Coroutine messageCoroutine;
    private List<TalkData> currentTalkData;                 // 현재 진행중인 대화 전체의 데이터
    private string currentText;                             // 출력할 대사
    private int currentTalkIndex;                           // 현재 대화에서 어떤 대사를 가져올 지 선택하는 인덱스

    public float typingSpeed;                               // 텍스트가 나오는 속도
    public float textRevealTime;                            // 모든 텍스트가 나와야 할 시간

    //public string csvFileName;                              // 대화를 가져올 CSV 파일의 이름

    public TextMeshProUGUI dialogueTMP;                     // 대사 칸
    public TextMeshProUGUI nameTMP;                         // 캐릭터 이름이 나올 칸
    public GameObject dialoguePanel;                        // 대사 창
    public float blinkTime;                                 // 대사 창이 투명해지는 시간
    public GameObject opacityPanel;                         // 투명 전체 창 (마우스 입력 방지용)
    private WaitForSeconds blinkWait;                       // 대사 창 투명 시간 캐싱용
    
    private string eventNameColumnStr;                      // CSV의 이벤트 이름 열
    private string characterNameColumnStr;                  // CSV의 캐릭터 이름 열
    private string textColumnStr;                           // CSV의 대사 열
    private string addressColumnStr;                        // CSV의 캐릭터 이미지 주소 열
    private string enabledColumnStr;                        // CSV의 이미지를 출력 여부 열
    private string shakeColumnStr;                          // CSV의 이미지 흔들림 여부 열
    private string bounceColumnStr;                         // CSV의 이미지 점프 여부 열

    private string endStr;                                  // CSV에서 쓴 대화가 끝난 후 다음 열에 작성할 텍스트 (대화 끝)
    private string lineSplitStr;                            // CSV에서 쓴 들여쓰기 텍스트 (텍스트 불러올 때 들여쓰기 확인)
    private string emptySellStr;                            // CSV에서 쓴 빈 공간에 적을 텍스트 (텍스트 불러올 때 스킵용)
    private string addressWhiteSpaceStr;                    // CSV에서 쓴 캐릭터 이미지 주소의 띄어쓰기 대신 사용한 문자 (띄어쓰기 시 오동작 할 위험이 있어 해당 문자로 사용)
    private string addressNormalImageStr;                   // CSV에서 쓴 캐릭터 이미지 주소의 평상시 버전 이미지를 가져오기 위한 것
    private Dictionary<string, Sprite> currentTalkImages;   // 현재 대화에서 사용될 이미지들

    public bool isTalking = false;                          // 대화 여부 확인용
    private TypingType currentTypingType;                   // 현재 텍스트가 어떤 방식으로 나와야 하는지 설정하는 타입

    public CharacterImageController leftCharacterImage;     // 좌측에 위치할 캐릭터 이미지
    public CharacterImageController rightCharacterImage;    // 우측에 위치할 캐릭터 이미지

    private List<string> zedNameStrList;                    // 주인공 이름 저장용
    private CharacterMoveController character;              // 대화를 진행중인 캐릭터 저장용

    protected override void Awake()
    {
        base.Awake();

        eventNameColumnStr = "EventName";                   // CSV의 이벤트 이름 열 텍스트
        characterNameColumnStr = "CharacterName";           // CSV의 캐릭터 이름 열 텍스트
        textColumnStr = "Text";                             // CSV의 대사 열 텍스트
        addressColumnStr = "Address";                       // CSV의 캐릭터 이미지 주소 열 텍스트
        endStr = "End";                                     // CSV에서 쓴 대화가 끝난 후 다음 열에 작성할 텍스트
        lineSplitStr = "\\n";                               // CSV에서 쓴 들여쓰기 텍스트
        emptySellStr = "-";                                 // CSV에서 쓴 빈 공간에 적을 텍스트
        addressWhiteSpaceStr = "_";                         // CSV에서 쓴 캐릭터 이미지 주소의 띄어쓰기 대신 사용한 문자
        addressNormalImageStr = "normal";                   // CSV에서 쓴 캐릭터 이미지 주소의 평상시 버전 이미지를 가져오기 위한 것
        enabledColumnStr = "IsEnabled";                     // CSV의 이미지를 출력 여부 열 텍스트
        shakeColumnStr = "Shake";                           // CSV의 이미지 흔들림 여부 열 텍스트
        bounceColumnStr = "Bounce";                         // CSV의 이미지 점프 여부 열 텍스트

        currentTalkImages = new Dictionary<string, Sprite>();
        blinkWait = new WaitForSeconds(blinkTime);

        zedNameStrList = new List<string>(){"제드", "우산", "고보스"};
        ReadAllText();  // 모든 대화 읽어오기
    }

    private void ReadAllText()
    {
        allTalkData = new Dictionary<string, Dictionary<string, List<TalkData>>>();

        string currentEventName = string.Empty;
        List<List<Dictionary<string, object>>> csvList = new();
        foreach (var fileName in csvFileNames)
        {
            csvList.Add(CSVReader.Read(fileName));
        }

        int currentFileIndex = 0;
        foreach (var csv in csvList)
        {
            string currentFileName = csvFileNames[currentFileIndex];
            currentFileIndex++;

            for (int i = 0; i < csv.Count; i++)
            {
                string eventName = csv[i][eventNameColumnStr].ToString(); // 이벤트 이름

                // 이벤트 이름이 endStr이거나, null 또는 empty일 경우 다음 열로 이동
                if (eventName == endStr || string.IsNullOrEmpty(eventName))
                    continue;

                // 겹치는 EventName 키가 없고, 이벤트 이름이 emptySellStr이 아닐 경우
                // 이벤트 별로 구분하여 대화를 Dictionary에 저장
                if (!allTalkData.ContainsKey(eventName) && eventName != emptySellStr)
                {
                    currentEventName = eventName;
                    var newTalkData = new List<TalkData>();
                    if (allTalkData.ContainsKey(currentFileName))
                    {
                        allTalkData[currentFileName].Add(currentEventName, newTalkData);
                    }
                    else
                    {
                        var newData = new Dictionary<string, List<TalkData>>
                        {
                            { currentEventName, newTalkData }
                        };
                        allTalkData.Add(currentFileName, newData);
                    }
                }

                // 대화 데이터 생성
                var data = new TalkData
                {
                    name = csv[i][characterNameColumnStr].ToString(), // 캐릭터 이름
                    talk = csv[i][textColumnStr].ToString(),          // 대사
                    address = csv[i][addressColumnStr].ToString(),    // 캐릭터 이미지
                    isEnabled = csv[i][enabledColumnStr].ToString() != string.Empty ? Convert.ToBoolean(csv[i][enabledColumnStr]) : true,   // 이미지 활성화 여부. 비어있지 않으면 가져오고, 비어있으면 true
                    shake = csv[i][shakeColumnStr].ToString() != string.Empty ? Convert.ToBoolean(csv[i][shakeColumnStr]) : false,          // 흔들림 여부. 비어있지 않으면 가져오고, 비어있으면 false
                    bounce = csv[i][bounceColumnStr].ToString() != string.Empty ? Convert.ToBoolean(csv[i][bounceColumnStr]) : false        // 점프 여부. 비어있지 않으면 가져오고, 비어있으면 false
                };

                // 저장한 대화에서 개행 처리를 해야 할 경우
                if (data.talk.Contains(lineSplitStr))
                {
                    // Line Split 문자가 있는 구간마다 문자 분리
                    // 분리된 문자를 개행 처리 한 후 데이터에 추가
                    string[] lineSplit = data.talk.Split(lineSplitStr);
                    data.talk = string.Empty;
                    for (int j = 0; j < lineSplit.Length; j++)
                    {
                        // 마지막 줄이 아닐 경우에만 개행
                        string splitStr = lineSplit[j];
                        if (j < lineSplit.Length - 1)
                        {
                            splitStr = $"{splitStr}{Environment.NewLine}";
                        }

                        data.talk += splitStr;
                    }
                }

                allTalkData[currentFileName][currentEventName].Add(data);
            }
        }
    }

    private void ChangeActiveDialoguePanel()
    {
        dialoguePanel.SetActive(!dialoguePanel.activeSelf);
    }

    // 첫 번째로 매칭되는 이름의 대화 데이터 가져오기
    private TalkData FindFirstMatchingElement(List<TalkData> dataList, List<string> strList)
    {
        return dataList.FirstOrDefault(element => strList.Contains(element.name));
    }

    // 첫 번째로 매칭되지 않는 이름의 대화 데이터 가져오기
    private TalkData FindFirstNotMatchingElement(List<TalkData> dataList, List<string> strList)
    {
        return dataList.FirstOrDefault(element => !strList.Contains(element.name));
    }

    // 캐릭터 이미지 주소 string 안의 캐릭터 이름 string 가져오기
    private string SetDefalutAddress(string address)
    {
        if (address == null || address == emptySellStr || address == string.Empty)
            return string.Empty;

        string[] parts = address.Split(addressWhiteSpaceStr);
        return parts[0];
    }

    // 이미지 적용
    private async Task ApplyImage(TalkData data, Image image)
    {
        if (data.name == null || data.name == string.Empty || data.name == emptySellStr || !data.isEnabled)
        {
            image.gameObject.SetActive(false);
            return;
        }
        else
        {
            await AddressableManager.ApplyImage(data.address, image);
        }
    }

    // 이름이 있는지 확인
    private bool IsValidName(string name)
    {
        return name != null && name != emptySellStr && name != string.Empty;
    }

    // 현재 대화에 사용할 데이터들 미리 담아두기
    public async void SetCurrentTalk(string csvFileName, string eventName)
    {
        if (!allTalkData.ContainsKey(csvFileName) || allTalkData[csvFileName].Count == 0)
            return;

        if (!allTalkData[csvFileName].ContainsKey(eventName) || allTalkData[csvFileName][eventName].Count == 0)
            return;

        leftCharacterImage.gameObject.SetActive(true);
        rightCharacterImage.gameObject.SetActive(true);

        currentTalkIndex = 0;
        currentTalkData = allTalkData[csvFileName][eventName];

        TalkData firstZedData = FindFirstMatchingElement(currentTalkData, zedNameStrList);      // 대화 중, 주인공의 첫 번째 대사 데이터 가져오기
        TalkData firstOtherData = FindFirstNotMatchingElement(currentTalkData, zedNameStrList); // 대화 중, 다른 캐릭터의 첫 번째 대사 데이터 가져오기
        string zedName = firstZedData.name;
        string otherName = firstOtherData.name;

        // 첫 시작 이미지는 일반 표정의 이미지로 변경
        //if (IsValidName(zedName))
        //{
        //    firstZedData.address = $"{SetDefalutAddress(firstZedData.address)}{addressWhiteSpaceStr}{addressNormalImageStr}";
        //}
        //if (IsValidName(otherName))
        //{
        //    firstOtherData.address = $"{SetDefalutAddress(firstOtherData.address)}{addressWhiteSpaceStr}{addressNormalImageStr}";
        //}

        // TalkData가 할당 되지 않았을 경우 캐릭터가 없는 것으로 판단, 이미지 오브젝트 비활성화
        leftCharacterImage.SetActive(false);
        rightCharacterImage.SetActive(false);
        await ApplyImage(firstZedData, leftCharacterImage.image);
        await ApplyImage(firstOtherData, rightCharacterImage.image);

        //if (firstZedData.address == emptySellStr || firstZedData.address == string.Empty)
        //{
        //    leftCharacterImage.SetActive(false);
        //}
        //if (firstOtherData.address == emptySellStr || firstOtherData.address == string.Empty)
        //{
        //    rightCharacterImage.SetActive(false);
        //}

        // 이름이 유효하지 않을 경우 return
        if (!IsValidName(zedName) && !IsValidName(otherName))
            return;

        // 사용할 이미지들의 Address 모두 담아두기
        List<string> addresses = new List<string>();
        foreach (var data in currentTalkData)
        {
            if (data.isEnabled == false || data.address == emptySellStr)
                continue;

            addresses.Add(data.address);
        }

        // Dictionary에 로딩된 Sprite들의 Address를 Key로 하고, Sprite를 같이 저장
        // Address로 저장된 Image를 찾아 사용하려는 목적
        currentTalkImages = await AddressableManager.LoadSpritesToDictionary(addresses);
    }

    // 대화 진행
    public void GetMessage(TypingType type)
    {
        // 현재 대화를 불러오지 못했거나, 대화가 없다면 대화를 중지하고 return
        if (currentTalkData == null || currentTalkData.Count == 0)
        {
            opacityPanel.SetActive(false);
            isTalking = false;
            character.StartMove();
            Zed.Instance.SetAttackUse(true);
            return;
        }

        // 출력중인 대사 코루틴이 있을 때 출력 코루틴 중지, 전체 대사로 덮어쓴 후 return
        if (messageCoroutine != null)
        {
            StopCoroutine(messageCoroutine);
            messageCoroutine = null;
            dialogueTMP.text = currentText;
            return;
        }

        // 대화가 끝났을 경우 모두 초기화 한 후 return
        if (currentTalkData.Count == currentTalkIndex)
        {
            opacityPanel.SetActive(false);
            currentTalkIndex = 0;
            isTalking = false;
            dialogueTMP.text = string.Empty;
            dialoguePanel.SetActive(false);
            character.StartMove();
            Zed.Instance.SetAttackUse(true);
            currentTalkImages.Clear();
            return;
        }

        // 현재 대화가 시작점일 경우 대화 창 Active 변경 (비활성화 -> 활성화)
        if (currentTalkIndex == 0)
        {
            opacityPanel.SetActive(true);
            ChangeActiveDialoguePanel();
        }

        var currentDialogue = currentTalkData[currentTalkIndex];
        var message = currentDialogue.talk;
        currentText = message;
        currentTalkIndex++;

        if (currentDialogue.name != string.Empty || currentDialogue.name != emptySellStr)
            nameTMP.text = currentDialogue.name;
        else
            nameTMP.text = string.Empty;

        // 현재 대사의 캐릭터 이름 비교
        // 현재 대사에 주인공 이름이 있을 경우 isZed = true (주인공이 대사 치는 중)
        bool isZed = false;
        foreach (var zedName in zedNameStrList)
        {
            if (currentDialogue.name == zedName)
            {
                isZed = true;
                break;
            }
        }

        if (currentDialogue.name != string.Empty && currentDialogue.name != emptySellStr)
        {
            // 주인공 대사 활성화 시, 왼쪽 이미지(주인공 자리)의 sprite 변경, 오른쪽 이미지의 색상 변경 (그림자 진 느낌의 색상으로)
            // 주인공 대사가 아닐 때, 위의 주석과 반대로 실행 (왼쪽 그림자 색상, 오른쪽 원본 색상)
            if (isZed)
            {
                ContorollCharacterImage(leftCharacterImage, rightCharacterImage, currentDialogue);
            }
            else
            {
                ContorollCharacterImage(rightCharacterImage, leftCharacterImage, currentDialogue);
            }
        }
        else
        {
            leftCharacterImage.SetActive(false);
            rightCharacterImage.SetActive(false);
        }

        // 대사 시작 코루틴 실행
        messageCoroutine = StartCoroutine(CoDoText(dialogueTMP, message, type));
    }

    // 캐릭터 이미지 색상 변경, 흔들림, 점프
    private void ContorollCharacterImage(CharacterImageController image, CharacterImageController counterpartImage, TalkData currentDialogue)
    {
        image.SetImage(FindImage(currentDialogue.address));
        image.AdjustImageColor(counterpartImage.image);
        image.SetActive(currentDialogue.isEnabled);

        if (currentDialogue.shake)
            image.Shake();

        if (currentDialogue.bounce)
            image.JumpVertically();
    }

    // 대사 시작 코루틴
    private IEnumerator CoDoText(TextMeshProUGUI text, string endValue, TypingType type)
    {
        string tempString = string.Empty;
        WaitForSeconds timer = null;

        // 타입에 따라 텍스트 출력 대기 시간 조절
        switch (type)
        {
            case TypingType.TypingSpeed:
                timer = new WaitForSeconds(typingSpeed);
                break;
            case TypingType.RevealTime:
                timer = new WaitForSeconds(textRevealTime / endValue.Length);
                break;
            default:
                break;
        }

        // 타이머가 할당되지 않았을 경우 return
        if (timer == null)
            yield break;

        // 대사 시작 (한 글자씩 출력)
        text.text = string.Empty;
        for (int i = 0; i < endValue.Length; i++)
        {
            tempString += endValue[i];
            text.text = tempString;

            yield return timer;
        }

        messageCoroutine = null;
    }

    // 대화 창 투명(비활성화) -> 활성화 조절
    private IEnumerator CoBlinkDialoguePanel()
    {
        ChangeActiveDialoguePanel();    // 투명

        yield return blinkWait;         // 대기

        ChangeActiveDialoguePanel();    // 활성화
    }

    public void SetTypingType(TypingType type)
    {
        currentTypingType = type;
    }

    // 캐릭터 이미지 찾기
    private Sprite FindImage(string address)
    {
        if (currentTalkImages == null || currentTalkImages.Count == 0 || address == emptySellStr)
            return null;

        return currentTalkImages[address];
    }

    public string GetRandomEvent(string csvFileName)
    {
        if (csvFileName == string.Empty)
            return string.Empty;

        if (!allTalkData.ContainsKey(csvFileName) || allTalkData[csvFileName].Count == 0)
            return string.Empty;

        var data = allTalkData[csvFileName];
        List<string> keys = new();
        foreach (var talk in data)
        {
            keys.Add(talk.Key);
        }

        int randomIndex = UnityEngine.Random.Range(0, keys.Count);
        return keys[randomIndex];
    }

    public void SetCharacter(CharacterMoveController character)
    {
        this.character = character;
    }

    public void Update()
    {
        if (isTalking)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                GetMessage(currentTypingType);
            }
        }
    }
}
