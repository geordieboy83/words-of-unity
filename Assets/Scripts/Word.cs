using UnityEngine;
using System.Collections;

public class Word : MonoBehaviour {

    public string word;
    public GameObject letterPrefab;
    public Vector2 originInRatio = new Vector2(0.5f, 0);
    protected bool scattered = false;

	// Use this for initialization
	IEnumerator Start () {
        if (word == "" || !letterPrefab) yield break;
        char[] wordCStyle = word.ToCharArray();
        Vector3 origin = Camera.main.ScreenToWorldPoint(new Vector3(originInRatio.x * Screen.width, originInRatio.y * Screen.height));
        origin.z = 0;
        for (int i =0; i<wordCStyle.Length; i++)
        {
            GameObject letter = Instantiate(letterPrefab);
            Letter l = letter.GetComponentInChildren<Letter>();
            l.myText.text = wordCStyle[i].ToString();
            letter.transform.parent = transform;
            float width = l.myBubble.bounds.size.x;
            Vector3 newPosition=
                origin + 
                (wordCStyle.Length * width * 1.5f / (2 * Mathf.PI)) * 
                    new Vector3(Mathf.Sin(i * 2*Mathf.PI / wordCStyle.Length), Mathf.Cos(i * 2 * Mathf.PI / wordCStyle.Length), 0);
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
        bool flagX = true, flagYTopDown = true, flagYBottomUp=true;
        for(int i=1; i<transform.childCount; i++)
        {
            Vector3 pos1 = transform.GetChild(i).position;
            Vector3 pos0 = transform.GetChild(i - 1).position;
            if (pos1.x <= pos0.x) flagX = false;
            if (pos1.y <= pos0.y) flagYBottomUp = false;
            if (pos1.y >= pos0.y) flagYTopDown = false;
            if (!flagX && !flagYTopDown && !flagYBottomUp)
            {
                scattered = true;
                Debug.Log("Scattered");
                return;
            }
        }
        if ((flagX || flagYBottomUp || flagYTopDown)&&scattered)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<Rigidbody>().isKinematic = true;
            }
        }
	}
}
