using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Game : MonoBehaviour {
    //Game logic class


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

    void Start () {
        //Chooose a random text from the ones provided.
        textIndex = UnityEngine.Random.Range(0, texts.Length);
        //Split on white spaces
        words.AddRange(texts[textIndex].text.Split(null));

        //Background music. Random track to start with, cyclical afterwards.
        myMusic = GetComponent<AudioSource>();
        musicIndex = UnityEngine.Random.Range(0, backgroundMusic.Length);

        gameStart = Time.time;
    }
	

	void Update () {
        if (gameOver) return;

        gameTime = Time.time - gameStart;

        //Check if words currently on screen have finished with...
        bool flag = true;
        foreach(Word w in currentWords)
        {
            if (!w.IsEnded())
            {
                flag = false;
                break;
            }
        }

        //... or if there aren't any words yet.
        flag |= currentWords.Count == 0;
        
        if (flag)
        {
            // Make words.
            foreach (Word w in currentWords) Destroy(w.gameObject);
            currentWords.Clear();
            MakeMultipleWords();
        }

        if (myMusic && !myMusic.isPlaying)
        {
            //Play next music track, if no music is playing.
            myMusic.clip = backgroundMusic[musicIndex++ % backgroundMusic.Length];
            myMusic.Play();
        }

        // Check if game is over
        if (currentWordIndex >= words.Count && currentWords.Count == 0)
        {
            gameOver = true;
            OnGameOver();
        }
	
	}
    
    // Make word
    public bool MakeWord()
    {
        //Next word
        currentWordIndex++;

        if (gameOver||words.Count <= currentWordIndex)
        {
            //No more words
            return false;
        }

        // Place in actual text.
        currentChar += words[currentWordIndex].Length;

        // Word to produce. Ignore non-alphabetic characters around word.
        // If word has hyphens and the like, it will be truncated.
        // Equally if there are more than one words joined with something other than a white space.
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
            //No point if there is just a single letter word!
            return false;
        }

        //Make the actual word
        GameObject wordObject = Instantiate(wordPrefab);
        currentWords.Add(wordObject.GetComponent<Word>());
        currentWords[currentWords.Count-1].word = wordText;
        currentWords[currentWords.Count - 1].letterPrefab = letterPrefabs[UnityEngine.Random.Range(0, letterPrefabs.Length)];
        return true;
    }


    // Secret message decoded. Play narration,  if available.
    void OnGameOver()
    {
        try
        {
            mySpeech.clip = clips[textIndex];
            mySpeech.Play();
        }
        catch { }
        
    }


    // Depending on the length of the words, make words on the screen.
    public void MakeMultipleWords()
    {
        
        // maxTotalLettersOnMultipleWords and letterPrefabs are the constraints:
        //(overall letters on screen and no more words than there are prefabs!
        // We don't want all words to have the same shape, people wouldn't know which letter is whose!
        while (LettersOnScreen() <= maxTotalLettersOnMultipleWords && currentWords.Count <= letterPrefabs.Length)
        {
            //Check next word
            if (currentWordIndex < words.Count - 1)
            {
                // If next word is too big for the screen, stop.
                // Yes, this is before truncating due to non alphabetic characters, but still.
                if (currentWords.Count>=1&&words[currentWordIndex + 1].Length + LettersOnScreen() > maxTotalLettersOnMultipleWords)
                    break;
            }
            
            if (MakeWord())
            {
                //Assign shape and spawn origin
                currentWords[currentWords.Count - 1].letterPrefab = letterPrefabs[(currentWords.Count - 1) % letterPrefabs.Length];
                currentWords[currentWords.Count - 1].originInRatio = wordSpawnPoints[(currentWords.Count - 1) % wordSpawnPoints.Length];

            }
            else if (currentWordIndex>=words.Count)
                // No more words to make. Text is finished.
                return;
            //Enough words made!
            if (currentWords.Count>=letterPrefabs.Length)
                break;

        }
        //If just one word, spawn it in the centre of the screen
        if (currentWords.Count == 1)
            currentWords[0].originInRatio = new Vector2(0.5f, 0.5f);
    }

    //Count letters on screen
    protected int LettersOnScreen()
    {
        int sum = 0;
        foreach (Word w in currentWords)
            sum += w.word.Length;
        return sum;
    }

    //Text completion ratio ([0..1])
    public float Progress()
    {
        try { int wordsCurrentlyNotEnded = 0;
            foreach (Word w in currentWords)
            {
                if (!w.IsEnded()) wordsCurrentlyNotEnded++;
            }
            return (currentWordIndex - wordsCurrentlyNotEnded) / (float)words.Count; }
        catch { return currentWordIndex/(float)words.Count; }
    }

    public float GetTime() { return gameTime; }
    public bool isEnded() { return gameOver;  }
    public bool readyToDestroy() { return gameOver && mySpeech && !mySpeech.isPlaying; }
    public int GetCurrentWord() { return Mathf.Min(currentWordIndex + 1,words.Count); }
    public int GetTotalWords() { return words.Count; }
    public void ForceGameOver() { gameOver = true; try { foreach (Word w in currentWords) if(w)Destroy(w.gameObject); } catch { } }
}
