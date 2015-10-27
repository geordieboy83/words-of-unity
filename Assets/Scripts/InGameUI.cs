using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InGameUI : MonoBehaviour {

    public GameObject gamePrefab;
    public Slider progress;
    protected Game game;
    public float animationLength = 1f;
    protected bool animating = false, destroying = false;
    public Text status, time, words;
    public GameObject mainMenu;

    void OnEnable()
    {
        game = Instantiate(gamePrefab).GetComponentInChildren<Game>();
        destroying = false;
        animating = false;
        status.text="Decoding...";
        progress.value = 0;
        progress.enabled = true;
        words.text = "";
        time.text = "";

    }


	// Update is called once per frame
	void Update () {
        if (!game||destroying)
            return;
        if (!animating && game.Progress() != progress.value)
            StartCoroutine("AnimateSlider", game.Progress());
        time.text="Your time:\n"+game.GetTime().ToString("0.00")+" sec.";
        words.text = "Words:\n" + game.GetCurrentWord() + " of " + game.GetTotalWords();
        if (game.isEnded()&&!destroying)
        {
            progress.gameObject.SetActive(false);
            status.text = "Decoded!";
            if (game.readyToDestroy())
            {
                StartCoroutine("WrapUpAndDestroyGame");
                
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

    IEnumerator WrapUpAndDestroyGame()
    {
        destroying = true;
        float start = Time.time;
        while (Time.time - start <= animationLength) {
            game.myMusic.volume = Mathf.Lerp(1, 0, ((Time.time - start) / animationLength));
            yield return null;
                }
        Destroy(game.gameObject);
        mainMenu.SetActive(true);
        gameObject.SetActive(false);
        yield break;

    }
}
