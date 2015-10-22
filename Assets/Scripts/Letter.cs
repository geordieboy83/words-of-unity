using UnityEngine;
using System.Collections;

public class Letter : MonoBehaviour {
    public enum LetterState { Appearing, Soaring, Returning};
    public TextMesh myText;
    public MeshRenderer myBubble;
    protected Quaternion textRotation0;
    protected Vector3 bubbleAxis;
    protected Color bubbleColor;
    public float rotationFactor = 20;
    public LetterState myState = LetterState.Appearing;
    protected float shaderAlpha;
    public float animationLength = 1f;

    // Use this for initialization
    void Start () {
        textRotation0 = myText.transform.rotation;
        bubbleAxis = Random.onUnitSphere;
        bubbleColor = new Color(Random.Range(0, 256) / 255f, Random.Range(0, 256) / 255f, Random.Range(0, 256) / 255f,1);
        shaderAlpha = myBubble.material.GetColor("_MKGlowTexColor").a;
        myBubble.material.SetColor("_RimColor", bubbleColor);
        myBubble.material.SetColor("_MKGlowColor", bubbleColor);
        myBubble.material.SetColor("_MKGlowTexColor", new Color(bubbleColor.r, bubbleColor.g, bubbleColor.b, shaderAlpha));
        StartCoroutine("UpdateAppearing");
    }
	
	 IEnumerator UpdateAppearing()
    {
        float start = Time.time;
        while (Time.time < start + animationLength)
        {
             transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, (Time.time - start) / animationLength);
             yield return null;
        }
        myState = LetterState.Soaring;
        StartCoroutine("UpdateSoaring");
        yield break;
    }

    IEnumerator UpdateReturning()
    {
        yield break;
    }

    IEnumerator UpdateSoaring()
    {
        while (true)
        {
            myBubble.GetComponent<Rigidbody>().AddForce(Random.insideUnitSphere * 10.0f);
            yield return new WaitForSeconds(Random.Range(0.5f, 2.5f));
        }

    }

    void Update/*Soaring*/ () {
        Vector3 bubbleRotation = bubbleAxis * ((Time.time * rotationFactor) % 360);
        myBubble.transform.rotation = Quaternion.Euler(bubbleRotation.x,bubbleRotation.y,bubbleRotation.z);
        /*try
        {
            myBubble.material.SetColor("_RimColor", bubbleColor);
            myBubble.material.SetColor("_MKGlowColor", bubbleColor);
            myBubble.material.SetColor("_MKGlowTexColor", new Color(bubbleColor.r, bubbleColor.g, bubbleColor.b, shaderAlpha));

        }
        catch { }*/
	}

    void LateUpdate()
    {
        myText.transform.rotation = textRotation0;
    }

    public void OnMouseDown()
    {
        Debug.Log("OnMouseDown");
    }

    public void OnMouseDrag()
    {
        if (myState != LetterState.Soaring) return;
        Debug.Log("OnMouseDrag");
        myBubble.GetComponent<Rigidbody>().isKinematic = true;
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mouse.x, mouse.y, transform.position.z);
    }

    public void OnMouseEnter()
    {
        Debug.Log("OnMouseEnter");
    }

    public void OnMouseExit()
    {
        Debug.Log("OnMouseExit");
    }

    public void OnMouseUp()
    {
        Debug.Log("OnMouseUp");
    }

    public void OnMouseUpAsButton()
    {
        Debug.Log("OnMouseUpAsButton");
        myBubble.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void OnMouseOver()
    {
        Debug.Log("OnMouseOver");
    }
}
