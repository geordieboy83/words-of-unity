using UnityEngine;

public class Background : MonoBehaviour {

    public GameObject rightBorder;

	// Use this for initialization
	void Start () {

        float videoAspect = 16 / 9f;
        float screenAspect = Screen.width / (float)Screen.height;
        float shift = (1-screenAspect/videoAspect) * transform.localScale.x / 2;
        //Shift left if screen is narrower than 16/9.
        transform.position += new Vector3(shift, 0, 0);
        //But now we need to shift the right border too!
        if (rightBorder)
        {
            rightBorder.transform.position -= new Vector3(2 * shift, 0, 0);
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}
