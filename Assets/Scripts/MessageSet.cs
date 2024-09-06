using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageSet : MonoBehaviour 
{
    public static MessageSet Now { get; private set; }
    
    [Header("Money Spend Ask")]
    public GameObject moneySpendAskPanel;
    public Text targetLabel;
    public GameObject starlightPanel;
    public Text starlightLabel;
    public GameObject memorialPanel;
    public Text memorialLabel;
    [Header("No Money Alert")]
    public GameObject noMoneyAlertPanel;
    public Text noMoneyLabel;

    int result; // Normally, 0 is set to True
    bool hasResult;

    void Awake()
    {
        Now = this;
    }

    public IEnumerator ShowMoneySpendAsk(string text, int starlightCost, int memorialCost, Action<bool> afterResult)
    {
        hasResult = false;
        starlightPanel.SetActive(false);
        memorialPanel.SetActive(false);

        string moneyTypeString = "";
        if (starlightCost > 0 && memorialCost > 0)
            moneyTypeString = "별빛과 기억의 조각을 사용하여";
        else if (starlightCost > 0)
            moneyTypeString = "별빛을 사용하여";
        else if (memorialCost > 0)
            moneyTypeString = "기억의 조각을 사용하여";

        targetLabel.text = moneyTypeString + Environment.NewLine + text;

        if (starlightCost > 0)
        {
            starlightPanel.SetActive(true);
            starlightLabel.text = starlightCost.ToString();
        }
        if (memorialCost > 0)
        {
            memorialPanel.SetActive(true);
            memorialLabel.text = memorialCost.ToString();
        }
        moneySpendAskPanel.SetActive(true);
        yield return new WaitUntil(() => hasResult);
        moneySpendAskPanel.SetActive(false);

        var boolres = result == 0 ? true : false;
        afterResult(boolres);
    }

    public IEnumerator ShowNoMoneyAlert(MoneyType type)
    {
        hasResult = false;

        switch (type)
        {
            case MoneyType.Starlight:
                noMoneyLabel.text = "별빛이 부족합니다!";
                break;
            case MoneyType.MemorialPiece:
                noMoneyLabel.text = "기억의 조각이 부족합니다!";
                break;
        }
        noMoneyAlertPanel.SetActive(true);
        yield return new WaitUntil(() => hasResult);
        noMoneyAlertPanel.SetActive(false);
    }

    public void SetResult(int index)
    {
        hasResult = true;
        result = index;
    }
}
