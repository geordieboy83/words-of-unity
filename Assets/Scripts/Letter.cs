using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Letter : MonoBehaviour {
    //Letter class

    public enum LetterState { Appearing, Soaring, Frozen, Returning, TheEnd};
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
    public List<Vector2> returnTrail=new List<Vector2>();
    public AudioClip SFXAppear, SFXPop;
    protected AudioSource myAudio;

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
        myAudio = GetComponent<AudioSource>();
        if (SFXAppear && myAudio)
        {
            myAudio.clip = SFXAppear;
            myAudio.Play();
        }
        StartCoroutine("UpdateAppearing");
    }
	
    //Letter appearance animation
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

    //Letter final animation
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
        
        for(int i=0; i < returnTrail.Count; i++)
        {
            start = Time.time;
            Vector3 currentPosition = transform.position;
            while (Time.time < start + animationLength)
            {
                transform.position = Vector3.Lerp(currentPosition, returnTrail[i], (Time.time - start) / animationLength);
                yield return null;
            }
        }
        myState = LetterState.TheEnd;        
        yield break;
    }

    //Bubble physics!
    IEnumerator UpdateSoaring()
    {
        while (true)
        {
            if (myState != LetterState.Soaring) yield return null;
            myBubble.GetComponent<Rigidbody>().AddForce(Random.insideUnitCircle * 10.0f);
            yield return new WaitForSeconds(Random.Range(0.5f, 2.5f));
        }

    }

    // Rotate around the y axis
    void Update () {
        Vector3 bubbleRotation = bubbleAxis * ((Time.time * rotationFactor));
        myBubble.transform.rotation = Quaternion.Euler(bubbleRotation.x,bubbleRotation.y,bubbleRotation.z);        
	}

    // Make sure the letter stays the same no matter what the bubble rotation!
    void LateUpdate()
    {
        myText.transform.rotation = textRotation0;
    }

    // Freeze letter on touch
    public void OnMouseDown()
    {
        if (myState == LetterState.Frozen)
        {
            StopCoroutine("Freeze");
            Unfreeze();
        }
    }

    // Move letter
    public void OnMouseDrag()
    {
        if (!myWord.isReady||myState != LetterState.Soaring) return;
        myBubble.GetComponent<Rigidbody>().isKinematic = true;        
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mouse.x, mouse.y, transform.position.z);
    }

    // Check if the respective word has all its letters in the right order
    public void OnMouseUpAsButton()
    {
        if (myWord)
            myWord.Assess();
        StartCoroutine("Freeze", freezeSeconds);
    }

    // Trigger return animation
    public void Return()
    {
        myBubble.GetComponent<Rigidbody>().isKinematic = true;
        myBubble.GetComponent<Collider>().enabled = false;
        myState = LetterState.Returning;
        StopCoroutine("UpdateSoaring");
        StartCoroutine("UpdateReturning");        
    }

    // Freeze letter - pretend from moving for a bit
    IEnumerator Freeze(float seconds)
    {
        myState = LetterState.Frozen;
        myBubble.GetComponent<Rigidbody>().isKinematic = true;
        yield return new WaitForSeconds(seconds);
        Unfreeze();
    }

    // Unfreeze letter
    public void Unfreeze()
    {
        myBubble.GetComponent<Rigidbody>().isKinematic = false;
        myState = LetterState.Soaring;
    }

    // Pop the bubble!
    public void Pop()
    {
        if (myAudio && SFXPop)
        {
            myAudio.clip = SFXPop;
            myAudio.Play();
        }

        myBubble.enabled = false;
        myText.gameObject.SetActive(false);

    }

    // Is the letter ready to destroy?
    public bool IsEnded()
    {
        bool flag= myState == Letter.LetterState.TheEnd && !myBubble.enabled;
        if(flag&& myAudio && myAudio.isPlaying)
        {
            flag = false;
        }
        return flag;
    }
}
