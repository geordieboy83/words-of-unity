using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {

    protected MovieTexture myVideo;

	// Use this for initialization
	void Start () {
	    myVideo= (MovieTexture) GetComponent<Renderer>().material.mainTexture;
        myVideo.loop = true;
        myVideo.Play();
        float videoAspect = myVideo.width / (float)myVideo.height;
        float screenAspect = Screen.width / (float)Screen.height;
        float shift = (1-screenAspect/videoAspect) * transform.localScale.x / 2;
        transform.position += new Vector3(shift, 0, 0);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
