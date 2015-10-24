﻿using UnityEngine;
using System.Collections;

public class Letter : MonoBehaviour {
    public enum LetterState { Appearing, Soaring, Frozen, Returning};
    public Word myWord;
    public TextMesh myText;
    public MeshRenderer myBubble;
    protected Quaternion textRotation0;
    protected Vector3 bubbleAxis;
    protected Color bubbleColor;
    public float rotationFactor = 20;
    public LetterState myState = LetterState.Appearing;
    protected float shaderAlpha;
    public float animationLength = 1f;
    protected Vector3 spawnPosition;
    public float freezeSeconds = 5f;

    // Use this for initialization
    void Start () {
        textRotation0 = myText.transform.rotation;
        bubbleAxis = Random.onUnitSphere;
        bubbleColor = new Color(Random.Range(0, 256) / 255f, Random.Range(0, 256) / 255f, Random.Range(0, 256) / 255f,1);
        shaderAlpha = myBubble.material.GetColor("_MKGlowTexColor").a;
        myBubble.material.SetColor("_RimColor", bubbleColor);
        myBubble.material.SetColor("_MKGlowColor", bubbleColor);
        myBubble.material.SetColor("_MKGlowTexColor", new Color(bubbleColor.r, bubbleColor.g, bubbleColor.b, shaderAlpha));
        spawnPosition = transform.position;
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

        float start = Time.time;
        float rimPower = myBubble.material.GetFloat("_RimPower");
        while (Time.time < start + animationLength)
        {
            myBubble.material.SetFloat("_RimPower", Mathf.Lerp(rimPower,0.5f, (Time.time - start) / animationLength));
            yield return null;
        }
        start = Time.time;
        Vector3 endPosition = transform.position;
        while (Time.time < start + animationLength)
        {
            transform.position = Vector3.Lerp(endPosition, spawnPosition, (Time.time - start) / animationLength);           
            yield return null;
        }

        yield break;
    }

    IEnumerator UpdateSoaring()
    {
        while (true)
        {
            if (myState != LetterState.Soaring) yield return null;
            myBubble.GetComponent<Rigidbody>().AddForce(Random.insideUnitCircle * 10.0f);
            yield return new WaitForSeconds(Random.Range(0.5f, 2.5f));
        }

    }

    void Update/*Soaring*/ () {
        Vector3 bubbleRotation = bubbleAxis * ((Time.time * rotationFactor) /*% 360*/);
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
        //Debug.Log("OnMouseDown");
    }

    public void OnMouseDrag()
    {
        if (myState != LetterState.Soaring/*||myState!=LetterState.Frozen*/) return;
        //Debug.Log("OnMouseDrag");
        /*if (myState == LetterState.Frozen)
        {
            StopCoroutine("Freeze");
            myState = LetterState.Soaring;
        }*/
        myBubble.GetComponent<Rigidbody>().isKinematic = true;        
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mouse.x, mouse.y, transform.position.z);
    }

    public void OnMouseEnter()
    {
        //Debug.Log("OnMouseEnter");
    }

    public void OnMouseExit()
    {
        //Debug.Log("OnMouseExit");
    }

    public void OnMouseUp()
    {
        //Debug.Log("OnMouseUp");
    }

    public void OnMouseUpAsButton()
    {
        //Debug.Log("OnMouseUpAsButton");
        if (myWord)
            myWord.Assess();
        //myBubble.GetComponent<Rigidbody>().isKinematic = false;
        StartCoroutine("Freeze", freezeSeconds);
    }

    public void OnMouseOver()
    {
        //Debug.Log("OnMouseOver");
    }

    public void Return()
    {
        myBubble.GetComponent<Rigidbody>().isKinematic = true;
        myBubble.GetComponent<Collider>().enabled = false;
        myState = LetterState.Returning;
        StopCoroutine("UpdateSoaring");
        StartCoroutine("UpdateReturning");        
    }

    IEnumerator Freeze(float seconds)
    {
        myState = LetterState.Frozen;
        myBubble.GetComponent<Rigidbody>().isKinematic = true;
        yield return new WaitForSeconds(seconds);
        Unfreeze();
    }

    public void Unfreeze()
    {
        myBubble.GetComponent<Rigidbody>().isKinematic = false;
        myState = LetterState.Soaring;
    }
}
