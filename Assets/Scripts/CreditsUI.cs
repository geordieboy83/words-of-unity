using UnityEngine;
using System.Collections;

public class CreditsUI : MonoBehaviour {
    // Credits UI handler

    public float animationLength = 30;
    public Vector2 from, to;
    public RectTransform credits;
    public UnityEngine.UI.Button backButton;

    public void OnEnable()
    {
        StartCoroutine("Scroll");
    }
    
    //Scroll text
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

}
