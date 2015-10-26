using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Word : MonoBehaviour {

    public string word;
    public GameObject letterPrefab;
    public Vector2 originInRatio = new Vector2(0.5f, 0);
    protected bool scattered = false;
    protected List<Vector4> scatteredLetters=new List<Vector4>();
    public float freezeLengthPerLevel= 0.5f;
    public int maxWordLength = 15;
    public AudioClip[] startSFX, popSFX;
    public bool isReady = false;

    // Use this for initialization
    IEnumerator Start () {
        if (word == "" || !letterPrefab) yield break;
        //word=word.Substring(0, maxWordLength);
        Vector3 origin = Camera.main.ScreenToWorldPoint(new Vector3(originInRatio.x * Screen.width, originInRatio.y * Screen.height));
        origin.z = 0;
        List<Vector3> positionCircular = new List<Vector3>(), positionHorizontal = new List<Vector3>();

        for (int i =0; i<word.Length; i++)
        {
            GameObject letter = Instantiate(letterPrefab);
            Letter l = letter.GetComponentInChildren<Letter>();
            l.myWord = this;
            l.myText.text = word[i].ToString();
            letter.transform.parent = transform;
            float width = l.myBubble.bounds.size.x;
            Vector3 newPosition=
                origin + 
                (word.Length * width * 1.5f / (2 * Mathf.PI)) * 
                    new Vector3(Mathf.Sin(i * 2*Mathf.PI / word.Length), Mathf.Cos(i * 2 * Mathf.PI / word.Length), 0);
            newPosition.z = 0;
            positionCircular.Add(new Vector3(newPosition.x, newPosition.y, i));
            positionHorizontal.Add(
                new Vector3(
                    origin.x + (i - word.Length / 2) * width + (word.Length % 2 == 0 ? 0.5f * width : 0),
                    word.Length * width * 1.5f / (2 * Mathf.PI),
                    i));

            if (i == 0)
            {
                CorrectScale(positionCircular[0], false);
                CorrectScale(positionHorizontal[0], true);                
            }
            positionCircular[i].Scale(transform.localScale);
            positionHorizontal[i].Scale(transform.localScale);
            newPosition.Scale(transform.localScale);

            letter.transform.position =
                //origin; 
                //new Vector3(origin.x + (i - wordCStyle.Length / 2) * width+(wordCStyle.Length % 2==0?0.5f*width:0), origin.y, 0);
                newPosition;

            l.freezeSeconds = freezeLengthPerLevel *word.Length;
            try
            {
                l.SFXAppear = startSFX[Random.Range(0, startSFX.Length)];
                if (i == word.Length - 1)
                    l.SFXPop = popSFX[0];
                else
                    l.SFXPop = popSFX[Random.Range(1, popSFX.Length)];
            }
            catch { }

            for(int j=i-1; j>=0; j--)
            {
                l.returnTrail.Add(positionCircular[j]);
                
            }
            if (i > word.Length/2)
                l.returnTrail.RemoveAt(l.returnTrail.Count-1);
            
            l.returnTrail.Add(positionHorizontal[i]);
            yield return new WaitForSeconds(l.animationLength / 2);
        }

        while (transform.GetChild(transform.childCount - 1).GetComponent<Letter>().myState == Letter.LetterState.Appearing)
        {
            yield return null;
        }

        

        isReady = true;
	}

    
	
	// Update is called once per frame
	void Update () {
        
	}

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
                //Vector3 oldScale = transform.localScale;
                //float start = Time.time;
                transform.localScale = newScale;// Vector3.Lerp(oldScale, newScale, Time.time - start);
            }
        }
        else
        {
            if (max.y < bound.y)
            {
                Vector3 newScale = transform.localScale * (max.y - min.y) / (2 * (Mathf.Abs(Mathf.Abs(bound.y) - Mathf.Abs(max.y))) + (max.y - min.y));
                //Vector3 oldScale = transform.localScale;
                //float start = Time.time;
                transform.localScale = newScale;// Vector3.Lerp(oldScale, newScale, Time.time - start);
            }
        }

    }


    public void Assess()
    {
        scatteredLetters.Clear();
        for(int i=0; i<transform.childCount; i++)
        {
            scatteredLetters.Add(
                new Vector4(
                    transform.GetChild(i).position.x,
                    transform.GetChild(i).position.y,
                    i,
                    (int)transform.GetChild(i).GetComponent<Letter>().myText.text[0]));
        }

        scatteredLetters.Sort(CompareHorizontal);
        if (Check())
        {
            //Debug.Log("Horizontal");
            //OnCorrect();
            StartCoroutine("OnCorrect");
            return;
        }
        /*scatteredLetters.Sort(CompareTopDown);
        if (Check())
        {
            Debug.Log("TopDown");
            OnCorrect();
            return;
        }
        scatteredLetters.Sort(CompareBottomUp);
        if (Check())
        {
            Debug.Log("BottomUp");
            OnCorrect();
            return;
        }*/
    }

    public IEnumerator OnCorrect()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Letter>().Return();
            //.GetComponent<Rigidbody>().isKinematic = true;
        }
        while (transform.GetChild(transform.childCount - 1).GetComponent<Letter>().myState != Letter.LetterState.TheEnd)
        {           
            yield return null;
        }

        yield return new WaitForSeconds(2);
        for (int i = 0; i < transform.childCount-1; i++)
        {
            Letter l = transform.GetChild(i).GetComponent<Letter>();
            l.Pop();
            yield return new WaitForSeconds(l.animationLength / 2);
            //.GetComponent<Rigidbody>().isKinematic = true;
        }
        Letter finalL= transform.GetChild(transform.childCount-1).GetComponent<Letter>();
        yield return new WaitForSeconds(2*finalL.animationLength);
        finalL.Pop();
    }

    public bool Check()
    {
        //string s = "";
        for (int i = 0; i < scatteredLetters.Count; i++)
            //s += (char)scatteredLetters[i].w;
            if ((char)scatteredLetters[i].w != word[i])
                return false;
        return true;//s == word;
    }

    public int CompareHorizontal(Vector4 x, Vector4 y)
    {
        if (x.x < y.x) return -1;
        if (x.x > y.x) return 1;
        return 0;
    }

    public int CompareTopDown(Vector4 x, Vector4 y)
    {
        if (x.y > y.y) return -1;
        if (x.y < y.y) return 1;
        return 0;
    }

    public int CompareBottomUp(Vector4 x, Vector4 y)
    {
        if (x.y < y.y) return -1;
        if (x.y > y.y) return 1;
        return 0;
    }

    public bool IsEnded()
    {
        Letter l = transform.GetChild(transform.childCount - 1).GetComponent<Letter>();
        return l.IsEnded();//l.myState==Letter.LetterState.TheEnd&&!l.myBubble.enabled;
    }
}
