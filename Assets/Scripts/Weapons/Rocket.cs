using UnityEngine;
using System.Collections;

public class Rocket : MonoBehaviour {

    public static readonly float MAX_SPEED = 20F;
    public static readonly float SECONDS_UNTIL_TIMEOUT = 5;
    public static readonly float SPEED_INCREMENT = 0.1F;
    public float speed = 1;
	private bool destroyed = false;

	void Start () {
        StartCoroutine(timeOutRoutine(SECONDS_UNTIL_TIMEOUT)); // time out after a few seconds
	}
	
	void Update () {
        // skip all logic if we've been disabled
		if (destroyed) return;

        if (speed < MAX_SPEED) {
            speed += SPEED_INCREMENT;
        }

        // move forward
        transform.position += transform.forward * speed * Time.deltaTime;

        // constantly rotate around the z-axis
        transform.Rotate(Vector3.forward, speed / 2);

        // check if we hit anything
        RaycastHit hit = RPhysics.BoxCast(transform, transform.forward);
        if (hit.transform.gameObject && hit.distance >0 && hit.distance < 0.8F) {

            if (!hit.transform) return; // failsave

            if (hit.transform.CompareTag("Fence")) {
                // We hit a fence, let it explode
                hit.transform.GetComponent<WoodenFence>().explode();
            }
            else if (hit.transform.GetComponent(typeof(BotBehaviour))) {
                PlayerBot.localPlayerBot.swapTypes((BotBehaviour)hit.collider.GetComponent(typeof(BotBehaviour)));
            }
            Explode();
        }
    }

    IEnumerator timeOutRoutine(float seconds) {
        yield return new WaitForSeconds(seconds);
        // If we get here, the rocket will time out and explode
        Explode();
    }

	void Explode() {
        EffectControl.createEffect(Effect.EXPLOSION_ORANGE, transform.position); // explode
        EffectControl.createEffect(Effect.FIRESPRAY_FINISH, transform.position); // finish up the spray

        PlayerBot.localPlayerBot.setRocketShot(false); // allow the player to shoot another rocket
        Destroy(gameObject); // and remove this object
	}

}
