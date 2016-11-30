using UnityEngine;
using System.Collections;

public class WoodenFence : MonoBehaviour {

    private bool exploded = false;

    public void explode() {
        if (exploded) return; // can't explode twice...

        foreach (Explode e in GetComponentsInChildren<Explode>())
            e.Play();

        exploded = true;

        GetComponent<Collider>().enabled = false;

        StartCoroutine(cleanUpRoutine());
    }

    private IEnumerator cleanUpRoutine() {
        yield return new WaitForSeconds(3);
        Destroy(transform.gameObject);
    }
}
