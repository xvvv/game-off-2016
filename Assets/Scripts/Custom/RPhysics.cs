using UnityEngine;

public class RPhysics : MonoBehaviour {

    public static void Move(Transform t, Vector3 direction, float speed) {
        RaycastHit hit;
        bool b = Physics.BoxCast(t.position, t.GetComponent<BoxCollider>().size / 2, direction, out hit);

        if (!b || hit.distance >= speed * Time.deltaTime) {
            //Move the given transform to its desired location
            t.position += direction * speed * Time.deltaTime;
        }
        else if (hit.distance > 0) {
            //Clip the given transform to the wall
            t.position += direction * (hit.distance - 0.01F);
        }
    }

    public static RaycastHit BoxCast(Transform t, Vector3 direction) {
        RaycastHit hit;
        bool b = Physics.BoxCast(t.position, t.GetComponent<BoxCollider>().size / 2, direction, out hit);

        return hit;
    }

}
