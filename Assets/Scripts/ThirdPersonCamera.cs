using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {

    private Transform player;

    private Color drawColor = Color.green;
    public Vector3 camOffset;

    //Afstanden
    public static readonly float MIN_DISTANCE = 2, MAX_DISTANCE = 10;
    public float currentDistance = MAX_DISTANCE;

    public float distanceToCheck = 1;


    void Start () {
        player = transform.parent.transform;
	}

	
	void Update () {

        transform.LookAt(player);

        if (ViewIsObstructed(transform.position)) {
            while(ViewIsObstructed(transform.position) && currentDistance > MIN_DISTANCE) {
                currentDistance -= Time.deltaTime;
                print("ZOOM IN!");
            }
        }
        else{
            while (!ViewIsObstructed(transform.position) && currentDistance < MAX_DISTANCE) {
                currentDistance += Time.deltaTime;
                print("ZOOM OUT");
            }
        }

        transform.position = player.position + camOffset * currentDistance;

        Debug.DrawLine(transform.position,player.position, drawColor);
	}

    private bool ViewIsObstructed(Vector3 origin) {
        //Controleer of we de speler vanaf de opgegeven positie kunnen zien
        RaycastHit hit;

        if (Physics.Linecast(origin,player.position,out hit)) {
            if (hit.collider.CompareTag("Obstruction")) { drawColor = Color.red;  return true; }
            if (hit.collider.CompareTag("Player")) { drawColor = Color.green;  return false; }
        }

        drawColor = Color.yellow;
        return false;
    }

}
