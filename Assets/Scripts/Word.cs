using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Word : MonoBehaviour {

    public string word;
    public GameObject letterPrefab;
    public Vector2 originInRatio = new Vector2(0.5f, 0);
    protected bool scattered = false;
    protected List<Vector4> scatteredLetters=new List<Vector4>();

    // Use this for initialization
    IEnumerator Start () {
        if (word == "" || !letterPrefab) yield break;
        Vector3 origin = Camera.main.ScreenToWorldPoint(new Vector3(originInRatio.x * Screen.width, originInRatio.y * Screen.height));
        origin.z = 0;
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
            letter.transform.position =
                //origin; 
                //new Vector3(origin.x + (i - wordCStyle.Length / 2) * width+(wordCStyle.Length % 2==0?0.5f*width:0), origin.y, 0);
                newPosition;


            yield return new WaitForSeconds(l.animationLength / 2);
        }
        
	}

    
	
	// Update is called once per frame
	void Update () {
        
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
            Debug.Log("Horizontal");
            OnCorrect();
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

    public void OnCorrect()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Letter>().Return();
            //.GetComponent<Rigidbody>().isKinematic = true;
        }
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
}
