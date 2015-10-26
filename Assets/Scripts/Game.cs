using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Game : MonoBehaviour {

    public TextAsset[] texts;
    public AudioClip[] clips;
    public AudioClip[] backgroundMusic;
    public GameObject wordPrefab;
    public GameObject[] letterPrefabs;
    protected List<Word> currentWords=new List<Word>();
    public int maxTotalLettersOnMultipleWords = 9;
    public Vector2[] wordSpawnPoints;
    protected int currentWordIndex = -1;
    protected int currentChar = 0;
    protected List<string> words = new List<string>();
    protected bool gameOver = false;
    protected int textIndex = 0, musicIndex = 0;
    public AudioSource myMusic, mySpeech;
    protected float gameTime, gameStart;

    

	// Use this for initialization
	void Start () {
        textIndex = UnityEngine.Random.Range(0, texts.Length);
        words.AddRange(texts[textIndex].text.Split(null));
        myMusic = GetComponent<AudioSource>();
        musicIndex = UnityEngine.Random.Range(0, backgroundMusic.Length);
        gameStart = Time.time;
    }
	

	// Update is called once per frame
	void Update () {
        if (gameOver) return;
        gameTime = Time.time - gameStart;
        bool flag = true;
        foreach(Word w in currentWords)
        {
            if (!w.IsEnded())
            {
                flag = false;
                break;
            }
        }
        flag |= currentWords.Count == 0;
        if (flag)
        {
            foreach (Word w in currentWords) Destroy(w.gameObject);
            currentWords.Clear();
            MakeMultipleWords();
        }
        if (myMusic && !myMusic.isPlaying)
        {
            myMusic.clip = backgroundMusic[musicIndex++ % backgroundMusic.Length];
            myMusic.Play();
        }
	
	}
    

    public bool MakeWord()
    {
        currentWordIndex++;
        if (words.Count <= currentWordIndex&&currentWords.Count==0)
        {
            gameOver = true;
            OnGameOver();
            return false;
        }

        currentChar += words[currentWordIndex].Length;
        string wordText = "";
        for(int i=0; i < words[currentWordIndex].Length; i++)
        {
            if (!Char.IsLetter(words[currentWordIndex][i]))
            {
                if (wordText.Length == 0) continue;
                else break;
            }
            else
            {
                wordText += words[currentWordIndex][i];
            }
        }
        if (wordText.Length <= 1)
        {
            return false;
        }
        GameObject wordObject = Instantiate(wordPrefab);
        currentWords.Add(wordObject.GetComponent<Word>());
        currentWords[currentWords.Count-1].word = wordText;
        currentWords[currentWords.Count - 1].letterPrefab = letterPrefabs[UnityEngine.Random.Range(0, letterPrefabs.Length)];
        return true;
    }

    void OnGameOver()
    {
        try
        {
            mySpeech.clip = clips[textIndex];
            mySpeech.Play();
        }
        catch { }
        
    }

    public void MakeMultipleWords()
    {
        
        while (LettersOnScreen() <= maxTotalLettersOnMultipleWords && currentWords.Count <= letterPrefabs.Length)
        {
            if (currentWordIndex < words.Count - 1)
            {
                if (currentWords.Count>=1&&words[currentWordIndex + 1].Length + LettersOnScreen() > maxTotalLettersOnMultipleWords)
                    break;
            }
            bool flag=MakeWord();
            if (flag && !gameOver)
            {
                currentWords[currentWords.Count - 1].letterPrefab = letterPrefabs[(currentWords.Count - 1)%letterPrefabs.Length];
                currentWords[currentWords.Count - 1].originInRatio = wordSpawnPoints[(currentWords.Count - 1) % wordSpawnPoints.Length];
                
            }
            if (currentWords.Count>=letterPrefabs.Length)
                break;

        }
        if (currentWords.Count == 1)
            currentWords[0].originInRatio = new Vector2(0.5f, 0.5f);
    }

    protected int LettersOnScreen()
    {
        int sum = 0;
        foreach (Word w in currentWords)
            sum += w.word.Length;
        return sum;
    }

    public float Progress()
    {
        try { int wordsCurrentlyNotEnded = 0;
            foreach (Word w in currentWords)
            {
                if (!w.IsEnded()) wordsCurrentlyNotEnded++;
            }
            return (currentWordIndex - wordsCurrentlyNotEnded) / (float)words.Count; }
        catch { return currentWordIndex/(float)words.Count; }
        //return currentChar / (float)texts[textIndex].text.Length;
    }

    public float GetTime() { return gameTime; }
    public bool isEnded() { return gameOver;  }
    public bool readyToDestroy() { return gameOver && mySpeech && !mySpeech.isPlaying; }
}
