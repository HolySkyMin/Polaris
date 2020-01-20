﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

// Last edited 2020-01-18
[System.Serializable]
public class SaveData
{
    public int CharVersion;
    public List<CharacterData> Characters;

    public int Starlight;
    public int[] StoreUpgradeLevel;

    public bool TutorialFinished;
    public int TutorialStep;

    public int ObserveSkyLevel;

    public void Load()
    {
        // 처음 캐릭터 데이터를 일단 읽어옵니다.
        var raw = Resources.Load<TextAsset>("Data/Characters");
        var charGroup = JsonMapper.ToObject<CharacterDataGroup>(raw.text);
        Variables.CharacterVersion = charGroup.Version;
        Variables.isFirst = false;

        Variables.Characters = new Dictionary<int, CharacterData>();
        if (Variables.CharacterVersion > CharVersion) // 만약 현재 세이브 파일의 버전이 낮다면 == 캐릭터 목록이 업데이트 되었었다면...
        {
            // 읽어온 캐릭터 데이터를 통해 Variables.Characters를 구성한 뒤, 이 객체가 가지고 있는 데이터를 반복문으로 적용시킵니다.
            // 적용시킬 때, 캐릭터나 카드의 위치 변경은 이루어지지 않았다고 가정하였습니다.
            foreach (var data in charGroup.Characters)
                Variables.Characters.Add(data.CharNumber, data);
            foreach(var curData in Characters)
            {
                if(Variables.Characters.ContainsKey(curData.CharNumber))
                {
                    Variables.Characters[curData.CharNumber].Observed = curData.Observed;
                    Variables.Characters[curData.CharNumber].Favority = curData.Favority;
                    Variables.Characters[curData.CharNumber].StoryProgress = curData.StoryProgress;
                    Variables.Characters[curData.CharNumber].LastReapDate = curData.LastReapDate;
                }
            }
        }
        else // 그렇지 않았다면...
        {
            // 이 객체가 가지고 있는 데이터를 통해 Variables.Characters를 구성합니다.
            foreach (var data in Characters)
                Variables.Characters.Add(data.CharNumber, data);
        }

        // 기타 변수들을 동기화시켜줍니다.
        Variables.Starlight = Starlight;
        Variables.StoreUpgradeLevel = StoreUpgradeLevel;
        Variables.isTutorialFinished = TutorialFinished;
        Variables.tutState = TutorialStep;
        Variables.ObserveSkyLevel = ObserveSkyLevel;
    }

    public void Create()
    {
        var raw = Resources.Load<TextAsset>("Data/Characters");
        var charGroup = JsonMapper.ToObject<CharacterDataGroup>(raw.text);
        Variables.CharacterVersion = charGroup.Version;

        Variables.Characters = new Dictionary<int, CharacterData>();
        foreach (var data in charGroup.Characters)
            Variables.Characters.Add(data.CharNumber, data);

        Starlight = 0;
        StoreUpgradeLevel = new[] { 1, 1, 1 };
        Variables.Starlight = Starlight;
        Variables.StoreUpgradeLevel = StoreUpgradeLevel;
        Variables.ObserveSkyLevel = -1;
        Variables.isFirst = true;
        Variables.isTutorialFinished = false;
        Variables.tutState = 1;
    }

    public void Save()
    {
        CharVersion = Variables.CharacterVersion;

        Characters = new List<CharacterData>();
        foreach (var item in Variables.Characters)
            Characters.Add(item.Value);

        Starlight = Variables.Starlight;
        StoreUpgradeLevel = Variables.StoreUpgradeLevel;
        ObserveSkyLevel = Variables.ObserveSkyLevel;
        TutorialFinished = Variables.isTutorialFinished;
        TutorialStep = Variables.tutState;
    }
}