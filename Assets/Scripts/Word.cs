﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Word : MonoBehaviour {

    public string word;
    public GameObject letterPrefab;
    public Vector2 originInRatio = new Vector2(0.5f, 0);
    protected bool scattered = false;
    protected List<Vector4> scatteredLetters=new List<Vector4>();
    public float freezeLengthPerLevel= 0.5f;
    public AudioClip[] startSFX, popSFX;
    public bool isReady = false;
    public bool interactable = true;

    // Use this for initialization
    IEnumerator Start () {
        if (word == "" || !letterPrefab) yield break;

        //Spawn origin from screen ratio to world coordinates
        Vector3 origin = Camera.main.ScreenToWorldPoint(new Vector3(originInRatio.x * Screen.width, originInRatio.y * Screen.height));
        origin.z = 0;

        //Circular and horizontal arrangements of the word.
        List<Vector3> positionCircular = new List<Vector3>(), positionHorizontal = new List<Vector3>();

        for (int i =0; i<word.Length; i++)
        {
            GameObject letter = Instantiate(letterPrefab);
            Letter l = letter.GetComponentInChildren<Letter>();
            l.myWord = this;
            l.myText.text = word[i].ToString();
            letter.transform.parent = transform;
            float width = l.myBubble.bounds.size.x;
            
            //Letter position on a circle, such that all of the word's letters fit the circumference
            Vector3 newPosition=
                origin + 
                (word.Length * width * 1.5f / (2 * Mathf.PI)) * 
                    new Vector3(Mathf.Sin(i * 2*Mathf.PI / word.Length), Mathf.Cos(i * 2 * Mathf.PI / word.Length), 0);
            newPosition.z = 0;            
            positionCircular.Add(new Vector3(newPosition.x, newPosition.y, i));

            //Letter position on a straight line, whose centre point is the same as the first letter of the word in the circular arrangement
            // or, if the word has an even number of letters, the two central ones are left and right of said centre point.
            positionHorizontal.Add(
                new Vector3(
                    origin.x + (i - word.Length / 2) * width + (word.Length % 2 == 0 ? 0.5f * width : 0),
                    word.Length * width * 1.5f / (2 * Mathf.PI),
                    i));

            
            if (i == 0)
            {
                //Check if word will fit the screen and scale accordingly. Just once.
                CorrectScale(positionCircular[0], false);
                CorrectScale(positionHorizontal[0], true);                
            }
            //If scaled, every position now must be scaled.
            positionCircular[i].Scale(transform.localScale);
            positionHorizontal[i].Scale(transform.localScale);
            newPosition.Scale(transform.localScale);

            // Position letter in circular arrangement
            letter.transform.position = newPosition;

            // Letter freeze duration
            l.freezeSeconds = freezeLengthPerLevel *word.Length;

            // Play letter appearance sound effect
            try
            {
                l.SFXAppear = startSFX[Random.Range(0, startSFX.Length)];
                if (i == word.Length - 1)
                    l.SFXPop = popSFX[0];
                else
                    l.SFXPop = popSFX[Random.Range(1, popSFX.Length)];
            }
            catch { }

            // Create trail for final animation.
            // First move letter to the top of the circle...
            for(int j=i-1; j>=0; j--)
            {
                l.returnTrail.Add(positionCircular[j]);
                
            }
            // ... (but only if it's in the first half of the word does it go the whole way up)...
            if (i > word.Length/2)
                l.returnTrail.RemoveAt(l.returnTrail.Count-1);
            
            // ...and then add its final position on the line.
            l.returnTrail.Add(positionHorizontal[i]);

            // Spawn letters with a delay between.
            yield return new WaitForSeconds(l.animationLength / 2);
        }

        //Wait for all the letters to spawn.
        while (transform.GetChild(transform.childCount - 1).GetComponent<Letter>().myState == Letter.LetterState.Appearing)
        {
            yield return null;
        }        

        isReady = true;
        // Play end animation if word is not playable (e.g. in Main Menu)
        if (!interactable) StartCoroutine("OnCorrect");
	}

    //Scale word down. Useful if you spawn a really long word that won't fit the screen!
    //The calculations assume one single word, spawned in the centre of the screen.
    void CorrectScale(Vector3 bound, bool isAxisX)
    {
        Vector3 min = Camera.main.ScreenToWorldPoint(new Vector3(0, 0));
        Vector3 max = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height));
        //Horizontal fit 
        if (isAxisX)
        {
            if (min.x > bound.x)
            {
                Vector3 newScale = transform.localScale * (max.x - min.x) / (2 * (Mathf.Abs(Mathf.Abs(bound.x) - Mathf.Abs(min.x))) + (max.x - min.x));
                transform.localScale = newScale;
            }
        }
        else
        {
            if (max.y < bound.y)
            {
                Vector3 newScale = transform.localScale * (max.y - min.y) / (2 * (Mathf.Abs(Mathf.Abs(bound.y) - Mathf.Abs(max.y))) + (max.y - min.y));
                transform.localScale = newScale;
            }
        }

    }


    //Check word correctness
    public void Assess()
    {
        scatteredLetters.Clear();
        //Gather the letters,...
        for(int i=0; i<transform.childCount; i++)
        {
            scatteredLetters.Add(
                new Vector4(
                    transform.GetChild(i).position.x,
                    transform.GetChild(i).position.y,
                    i,
                    (int)transform.GetChild(i).GetComponent<Letter>().myText.text[0]));
        }
        //... sort them, ...
        scatteredLetters.Sort(CompareHorizontal);

        //... check if they are in correct order.
        if (Check())
        {
            StartCoroutine("OnCorrect");
        }
    }

    // Animation of word in correct order.
    public IEnumerator OnCorrect()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Letter>().Return();
        }
        if (!interactable) yield break;
        while (transform.GetChild(transform.childCount - 1).GetComponent<Letter>().myState != Letter.LetterState.TheEnd)
        {           
            yield return null;
        }

        yield return new WaitForSeconds(2);
        Pop();
    }

    // True if letters spell the proper word when they are horizontally sorted
    public bool Check()
    {
        for (int i = 0; i < scatteredLetters.Count; i++)
            if ((char)scatteredLetters[i].w != word[i])
                return false;
        return true;
    }

    // Sort horizontally.
    public int CompareHorizontal(Vector4 x, Vector4 y)
    {
        if (x.x < y.x) return -1;
        if (x.x > y.x) return 1;
        return 0;
    }

    // Sort top down - not used.
    public int CompareTopDown(Vector4 x, Vector4 y)
    {
        if (x.y > y.y) return -1;
        if (x.y < y.y) return 1;
        return 0;
    }

    // Sort bottom - up. Not used.
    public int CompareBottomUp(Vector4 x, Vector4 y)
    {
        if (x.y < y.y) return -1;
        if (x.y > y.y) return 1;
        return 0;
    }

    // Ready to destroy if even the last letter of the word is ready to destroy.
    public bool IsEnded()
    {
        Letter l = transform.GetChild(transform.childCount - 1).GetComponent<Letter>();
        return l.IsEnded();
    }

    public void Pop()
    {
        StartCoroutine("OnPop");
    }

    // Animate popping of the letter bubbles
    IEnumerator OnPop()
    {
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Letter l = transform.GetChild(i).GetComponent<Letter>();
            l.Pop();
            yield return new WaitForSeconds(l.animationLength / 2);
        }
        Letter finalL = transform.GetChild(transform.childCount - 1).GetComponent<Letter>();
        yield return new WaitForSeconds(2 * finalL.animationLength);
        finalL.Pop();
    }
}
