using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InGameUI : MonoBehaviour {

    public GameObject gamePrefab;
    public Slider progress;
    protected Game game;
    public float animationLength = 1f;
    protected bool animating = false;
    public Text status, time, words;

    void OnEnable()
    {
        game = Instantiate(gamePrefab).GetComponentInChildren<Game>();
    }


	// Use this for initialization
	void Start () {
        //game = FindObjectOfType<Game>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!game)
            return;
        if (!animating && game.Progress() != progress.value)
            StartCoroutine("AnimateSlider", game.Progress());
        time.text="Your time: "+game.GetTime().ToString("0.00")+" sec.";
        if (game.isEnded())
        {
            progress.gameObject.SetActive(false);
            status.text = "Decoded!";
            if (game.readyToDestroy())
            {
                Destroy(game.gameObject);
                
            }
        }
    }

    IEnumerator AnimateSlider(float newValue)
    {
        float start = Time.time;
        float oldValue = progress.value;
        animating = true;
        while (Time.time - start <= animationLength)
        {
            progress.value = Mathf.Lerp(oldValue, newValue, ((Time.time - start) / animationLength));
            yield return null;
        }
        animating = false;
    }
}
