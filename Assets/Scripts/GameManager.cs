﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using LitJson;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    const int MAX_STARLIGHT = 10000000;
    const int MAX_MEMORIAL_PIECE = 100;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        if (Instance != null)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        Initialize();
    }

    // Internal JSON-based data load
    public void Initialize()
    {
        var valueRaw = Resources.Load<TextAsset>("Data/Values");
        Variables.values = JsonMapper.ToObject<Values>(valueRaw.text);

        var constelRaw = Resources.Load<TextAsset>("Data/Constels");
        var constelGroup = JsonMapper.ToObject(constelRaw.text);

        Variables.Constels = new Dictionary<string, ConstelData>();
        Variables.ConstelGroupName = new Dictionary<int, string>();
        foreach (JsonData data in constelGroup["constels"])
        {
            var index = (int)data["groupIndex"];
            foreach(JsonData constel in data["groupItems"])
            {
                var newConstel = new ConstelData((string)constel["key"], (string)constel["name"], index);
                Variables.Constels.Add(newConstel.InternalName, newConstel);
            }

            Variables.ConstelGroupName.Add(index, (string) data["groupName"]);
        }
    }

    #region Game Data Save/Load
    public void CreateGame()
    {
        DeleteGame();
        SaveData.Load();
    }

    public void SaveGame()
    {
        SaveData.Save();
    }

    public void DeleteGame()
    {
        SaveData.Delete();
        if(File.Exists(Application.persistentDataPath + "/obs_status"))
            File.Delete(Application.persistentDataPath + "/obs_status");
    }
    #endregion

    #region Favority Control Function
    
    /// <summary>
    /// 친밀도를 체크하는 함수입니다.
    /// 직접적으로는 친밀도 레벨을 반환하며 (1 ~ 6), 간접적으로 현재 친밀도 진행 정도와 다음 레벨까지의 요구 친밀도를 반환합니다.
    /// </summary>
    /// <param name="charNumber">캐릭터 번호입니다.</param>
    /// <param name="cardIndex">캐릭터 내부 인덱스입니다. (Cards 배열)</param>
    /// <param name="progress"></param>
    /// <param name="required"></param>
    /// <returns></returns>
    public int CheckFavority(int charNumber, out int progress, out int required)
    {
        var favority = Variables.Characters[charNumber].Favority;
        int cnt = 0;
        for (; cnt < Variables.values.MaxFavorityLevel; cnt++)
        {
            if (favority < Variables.FavorityThreshold[cnt])
                break;
        }
        if(cnt >= Variables.values.MaxFavorityLevel)
        {
            progress = 0;
            required = -1;
        }
        else
        {
            progress = favority - (cnt > 0 ? Variables.FavorityThreshold[cnt - 1] : 0);
            required = Variables.FavorityThreshold[cnt] - (cnt > 0 ? Variables.FavorityThreshold[cnt - 1] : 0);
        }
        return cnt + 1;
    }

    public int CheckAfterFavority(int charNumber, float deltaFav, out float progress, out int required)
    {
        var favority = Variables.Characters[charNumber].Favority;
        int cnt = 0;
        for (; cnt < Variables.values.MaxFavorityLevel; cnt++)
        {
            if (favority + deltaFav < Variables.FavorityThreshold[cnt])
                break;
        }
        if (cnt >= Variables.values.MaxFavorityLevel)
        {
            progress = favority + deltaFav - Variables.FavorityThreshold[cnt - 1];
            required = -1;
        }
        else
        {
            progress = favority + deltaFav - (cnt > 0 ? Variables.FavorityThreshold[cnt - 1] : 0);
            required = Variables.FavorityThreshold[cnt] - (cnt > 0 ? Variables.FavorityThreshold[cnt - 1] : 0);
        }
        return cnt + 1;
    }

    public int IncreaseFavority(int charNumber, int increment)
    {
        var actual = Variables.Characters[charNumber].Favority + increment > Variables.values.MaxFavorityValue
            ? Variables.values.MaxFavorityValue - Variables.Characters[charNumber].Favority
            : increment;

        Variables.Characters[charNumber].Favority += actual;
        return actual;
    }
    
    #endregion
    
    #region Money Control Functions

    public int GetCurrentMoney(MoneyType type)
    {
        switch (type)
        {
            case MoneyType.Starlight:
                return Variables.Starlight;
            case MoneyType.MemorialPiece:
                return Variables.MemorialPiece;
        }

        return 0;
    }

    public bool MoneyPayable(MoneyType type, int price)
    {
        switch (type)
        {
            case MoneyType.Starlight:
                return Variables.Starlight - price >= 0;
            case MoneyType.MemorialPiece:
                return Variables.MemorialPiece - price >= 0;
        }

        return false;
    }

    public bool PayMoney(MoneyType type, int price)
    {
        if (!MoneyPayable(type, price))
            return false;
        
        switch (type)
        {
            case MoneyType.Starlight:
                Variables.Starlight -= price;
                break;
            case MoneyType.MemorialPiece:
                Variables.MemorialPiece -= price;
                break;
        }
        return true;
    }

    public void IncreaseMoney(MoneyType type, int value)
    {
        switch (type)
        {
            case MoneyType.Starlight:
                Variables.Starlight = Mathf.Clamp(Variables.Starlight + value, 0, MAX_STARLIGHT);
                break;
            case MoneyType.MemorialPiece:
                Variables.MemorialPiece = Mathf.Clamp(Variables.MemorialPiece + value, 0, MAX_MEMORIAL_PIECE);
                break;
        }
    }
    
    #endregion

    public bool CheckGameEnd()
    {
        var result = true;
        foreach (var character in Variables.Characters.Values)
        {
            if (character.CharNumber != 1 && character.Favority < Variables.values.MaxFavorityValue)
            {
                result = false;
                break;
            }
        }

        return result;
    }
}