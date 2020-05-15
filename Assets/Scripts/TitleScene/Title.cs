﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Title : MonoBehaviour {

    public TextMesh TitleText;
    public TextMesh TitleSubText;
    public TextMesh TouchText;

    private float speed;
    private SpriteRenderer TitleLogo;
    private SpriteRenderer TouchToScreen;
    private bool Finish;
    private bool Stop;
    private bool SceneChanging;
    float TextEffect;

    void Awake()
    {
        // TitleLogo = GameObject.Find("TitleLogo").GetComponent<SpriteRenderer>();
        // TouchToScreen = GameObject.Find("TouchToScreen").GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        speed = -1.0f; TextEffect = 0.0f;
        Finish = false;
        Stop = false;
        SceneChanging = false;
        TitleText.color = new Color(1, 1, 1, 0);
        TitleSubText.color = new Color(1, 1, 1, 0);
        TouchText.color = new Color(1, 1, 1, 0);

        SoundManager.Play(SoundType.BgmTitle);
    }

    void Update () {
        if (!Stop&!Finish)
        {
            if (Camera.main.transform.position.y < 0.0f)
            {
                Camera.main.transform.position = new Vector3(0.0f, 0.0f, -10.0f);
                Stop = true; 
            }

            if (Camera.main.transform.position.y > 0.0f)
            {
                Camera.main.transform.Translate(0.0f, speed * Time.deltaTime, 0.0f);
            }
        }
        else
        {
            if (Finish)
            {
                Camera.main.transform.position = new Vector3(0.0f, 0.0f, -10.0f);
                TitleText.color = new Color(1, 1, 1, 1);
                TitleSubText.color = new Color(1, 1, 1, 1);
                TouchText.color = new Color(1, 1, 1, 1);
            }
            else if (TitleText.color.a < 1)
            {
                TextEffect += 1.0f * Time.deltaTime;
                TitleText.color = new Color(1, 1, 1, TextEffect);
                TitleSubText.color = new Color(1, 1, 1, TextEffect);
                TouchText.color = new Color(1, 1, 1, TextEffect);
            }   
            else
            {
                Finish = true;
            }
        }
        //TODO : 씬 바꾸는 임시 코드 개선
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(!Finish)
            {
                SkipAnimation();
            }
            else
            {
                Application.Quit();
            }
        }
    }

    public void SkipAnimation()
    {
        if (SceneChanging) return;

        if (!Finish) Finish = true;
        else
        {
            SceneChanging = true;
            StartCoroutine(ChangeScene());
        }
    }

    IEnumerator ChangeScene() {
        SoundManager.Play(SoundType.ClickImportant);
        yield return new WaitForSeconds(0.5f);
        if(Variables.HasSave)
        {
            SaveData.Load();

            if (!Variables.TutorialFinished)
            {
                GameManager.Instance.CreateGame();
                SceneChanger.Instance.ChangeScene("Prologue");
            }
            else
                SceneChanger.Instance.ChangeScene("MainScene");
        }
        else
        {
            GameManager.Instance.CreateGame();
            SceneChanger.Instance.ChangeScene("Prologue");
        }
    }

    // Debug Function
    public void DeleteSave()
    {
        GameManager.Instance.DeleteGame();
        Debug.Log("Save deleted.");
    }

    // Debug Function
    public void BlackSheepWall()
    {
        GameManager.Instance.CreateGame();

        foreach(var data in Variables.Characters)
        {
            if (data.Value.Observable)
                data.Value.Observed = true;
        }
        GameManager.Instance.SaveGame();

        StartCoroutine(ChangeScene());
    }
}
