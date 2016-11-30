using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TV : MonoBehaviour {

    public Text monitorText;

    public string txt;
    public float delay;
    public bool clearPreviousText;

	// Use this for initialization
	void Start () {
        type(txt, delay, clearPreviousText);
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    void type(string text, float letterDelay, bool clearText) {
        StartCoroutine(typingCoroutine(text, letterDelay, clearText));
    }

    IEnumerator typingCoroutine(string text, float letterDelay, bool clearText) {
        // clear previous text if needed
        if (clearText)
            monitorText.text = "";

        for(int i=0; i < text.Length; i++) {
            if (letterDelay > 0)
                yield return new WaitForSeconds(letterDelay);
            monitorText.text += text.Substring(i, 1);
        }
    }

}
