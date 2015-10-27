using UnityEngine;
using System.Collections;

public class CreditsUI : MonoBehaviour {

    public float animationLength = 30;
    public Vector2 from, to;
    public RectTransform credits;
    public UnityEngine.UI.Button backButton;

    public void OnEnable()
    {
        StartCoroutine("Scroll");
    }

    IEnumerator Scroll()
    {
        float start = Time.time;
        while (Time.time - start < animationLength)
        {
            credits.anchoredPosition = Vector2.Lerp(from, to, (Time.time - start) / animationLength);
            yield return null;
        }
        backButton.onClick.Invoke();
    } 

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
