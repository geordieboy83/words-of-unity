using UnityEngine;
using System.Collections;

public class Letter : MonoBehaviour {
    public TextMesh myText;
    public MeshRenderer myBubble;
    protected Quaternion textRotation0;
    protected Vector3 bubbleAxis;
    protected Color bubbleColor;

	// Use this for initialization
	void Start () {
        textRotation0 = myText.transform.rotation;
        bubbleAxis = new Vector3(Random.Range(0, 359f), Random.Range(0, 359f), Random.Range(0, 359f)).normalized;
        bubbleColor = new Color(Random.Range(0, 256) / 255f, Random.Range(0, 256) / 255f, Random.Range(0, 256) / 255f,1);
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 bubbleRotation = bubbleAxis * ((Time.time * 20) % 360);
        myBubble.transform.rotation = Quaternion.Euler(bubbleRotation.x,bubbleRotation.y,bubbleRotation.z);
        try
        {
            myBubble.material.SetColor("_RimColor", bubbleColor);
            myBubble.material.SetColor("_MKGlowColor", bubbleColor);
            float alpha = myBubble.material.GetColor("_MKGlowTexColor").a;
            myBubble.material.SetColor("_MKGlowTexColor", new Color(bubbleColor.r, bubbleColor.g, bubbleColor.b, alpha));

        }
        catch { }
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
        Debug.Log("OnMouseDrag");
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
    }

    public void OnMouseOver()
    {
        Debug.Log("OnMouseOver");
    }
}
