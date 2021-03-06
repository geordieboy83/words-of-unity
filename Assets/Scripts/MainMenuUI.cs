﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuUI : MonoBehaviour {
    //Main Menu handler.    

    public GameObject wordPrefab;
    public string[] title;
    public Vector2[] origins;
    protected Word[] words;
    public Image logo;
    public float animationLength = 1;
    public GameObject inGameMenu, howToPlayMenu, creditsMenu;
    public RectTransform buttonsPanel;
    public Vector2 buttonsPanelFrom, buttonsPanelTo;

    void OnEnable()
    {
        //Uses game words, rather than UI assets, for the title.
        words = new Word[title.Length];
        buttonsPanel.anchoredPosition = buttonsPanelFrom;
        logo.transform.localScale = Vector3.zero;
        howToPlayMenu.SetActive(false);
        creditsMenu.SetActive(false);
        inGameMenu.SetActive(false);
        for (int i=0; i<words.Length; i++)
        {
            
            words[i] = Instantiate(wordPrefab).GetComponent<Word>();
            words[i].word = title[i];
            words[i].originInRatio = origins[i];
            words[i].interactable = false;
            
            
        }
        StartCoroutine("OnAppear");
        
    }

    //Animate in
    IEnumerator OnAppear()
    {
        bool flag = false;
        while(!flag)
        {
            flag = true;
            foreach (Word w in words)
                if (!w.isReady)
                    flag = false;
            yield return null;
        }

        yield return new WaitForSeconds(2);

        float start = Time.time;
        while (Time.time < start + animationLength)
        {
            logo.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, (Time.time - start) / animationLength);
            buttonsPanel.anchoredPosition=Vector3.Lerp(buttonsPanelFrom,buttonsPanelTo, (Time.time - start) / animationLength);
            yield return null;
        }
    }

    void Update () {
        if (Input.GetKeyUp(KeyCode.Escape))
            Application.Quit();
	}

    public void OnPlay()
    {
        StartCoroutine("StartGame");
    }

    //Animate Out
    IEnumerator StartGame()
    {
        foreach(Word w in words)
        {
            w.Pop();
            while (!w.IsEnded())
                yield return null;
        }

        float start = Time.time;
        while (Time.time < start + animationLength)
        {
            logo.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, (Time.time - start) / animationLength);
            buttonsPanel.anchoredPosition = Vector3.Lerp(buttonsPanelTo, buttonsPanelFrom, (Time.time - start) / animationLength);
            yield return null;
        }

        foreach (Word w in words)
            Destroy(w.gameObject);
        inGameMenu.SetActive(true);
        gameObject.SetActive(false);
    }

   
}
