using UnityEngine;
using System.Collections;

public class MenuTitle : MonoBehaviour {

    private Vector3 startPos;
    private float distance = 16F, ease = 3.25F;
    private RectTransform rt;

	void Start () {
        rt = GetComponent<RectTransform>();
        startPos = rt.anchoredPosition;
	}
	
	void Update () {
        rt.anchoredPosition = startPos + Vector3.up * distance * Mathf.Cos(Time.time * ease);
	}
}
