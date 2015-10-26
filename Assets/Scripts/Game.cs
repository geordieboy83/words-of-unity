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
    protected List<string> words = new List<string>();
    protected bool gameOver = false;
    protected int textIndex = 0, musicIndex = 0;
    protected AudioSource myAudio;

    

	// Use this for initialization
	void Start () {
        textIndex = UnityEngine.Random.Range(0, texts.Length);
        words.AddRange(texts[textIndex].text.Split(null));
        myAudio = GetComponent<AudioSource>();
        musicIndex = UnityEngine.Random.Range(0, backgroundMusic.Length);
    }
	

	// Update is called once per frame
	void Update () {
        if (gameOver) return;
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
        if (myAudio && !myAudio.isPlaying)
        {
            myAudio.clip = backgroundMusic[musicIndex++ % backgroundMusic.Length];
            myAudio.Play();
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
            AudioSource mySpeech = gameObject.AddComponent<AudioSource>();
            mySpeech.loop = false;
            mySpeech.clip = clips[textIndex];
            mySpeech.Play();
        }
        catch { }
        
    }

    public void MakeMultipleWords()
    {
        
        while (LettersOnScreen() <= maxTotalLettersOnMultipleWords && currentWords.Count <= letterPrefabs.Length)
        {
            bool flag=MakeWord();
            if (flag && !gameOver)
            {
                currentWords[currentWords.Count - 1].letterPrefab = letterPrefabs[(currentWords.Count - 1)%letterPrefabs.Length];
                currentWords[currentWords.Count - 1].originInRatio = wordSpawnPoints[(currentWords.Count - 1) % wordSpawnPoints.Length];
                
            }
            if (currentWords.Count>=letterPrefabs.Length)
                return;

        }
    }

    protected int LettersOnScreen()
    {
        int sum = 0;
        foreach (Word w in currentWords)
            sum += w.word.Length;
        return sum;
    }
}
