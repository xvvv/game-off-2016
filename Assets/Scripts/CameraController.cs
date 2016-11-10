using UnityEngine;

public class CameraController : MonoBehaviour {

    //TODO zorg ervoor dat niet alles public is.. heh
    public Camera myCamera;
    public Transform camTransform;
    public Transform pivot;
    public Transform character;

    public float offset = -6.5F;
    public float camFollow = 8;

    public LayerMask mask;

    void Start() {
        pivot = transform;
        myCamera = Camera.main;
        camTransform = myCamera.transform;
        character = transform.parent.transform;
        camTransform.position = pivot.TransformPoint(Vector3.forward * offset);
        mask = 1 << LayerMask.NameToLayer("Clip") | 0 << LayerMask.NameToLayer("NonClip");
    }

    void LateUpdate() {

        //Central Ray
        float unobstructed = offset;
        Vector3 idealPosition = pivot.TransformPoint(Vector3.forward * offset);

        RaycastHit hit;
        if (Physics.Linecast(pivot.position,idealPosition,out hit,mask.value)) {
            unobstructed = -hit.distance + .01f;
        }

        //Smoothing
        Vector3 desiredPos = pivot.TransformPoint(Vector3.forward * unobstructed);
        Vector3 currentPos = camTransform.position;

        Vector3 goToPos = new Vector3(Mathf.Lerp(currentPos.x,desiredPos.x,camFollow), Mathf.Lerp(currentPos.y,desiredPos.y,camFollow), Mathf.Lerp(currentPos.z,desiredPos.z,camFollow));

        camTransform.localPosition = goToPos;
        camTransform.LookAt(pivot.position);

        //Prevent Viewport Bleeding
        float c = myCamera.nearClipPlane;
        bool clip = true;
        while (clip) {
            Vector3 pos1 = myCamera.ViewportToWorldPoint(new Vector3(0,0,c));
            Vector3 pos2 = myCamera.ViewportToWorldPoint(new Vector3(.5f,0,c));
            Vector3 pos3 = myCamera.ViewportToWorldPoint(new Vector3(1,0,c));
            Vector3 pos4 = myCamera.ViewportToWorldPoint(new Vector3(0,.5f,c));
            Vector3 pos5 = myCamera.ViewportToWorldPoint(new Vector3(1,.5f,c));
            Vector3 pos6 = myCamera.ViewportToWorldPoint(new Vector3(0,1,c));
            Vector3 pos7 = myCamera.ViewportToWorldPoint(new Vector3(.5f,1,c));
            Vector3 pos8 = myCamera.ViewportToWorldPoint(new Vector3(1,1,c));

            //Debug
            Debug.DrawLine(camTransform.position,pos1,Color.yellow); Debug.DrawLine(camTransform.position,pos2,Color.yellow);
            Debug.DrawLine(camTransform.position,pos3,Color.yellow); Debug.DrawLine(camTransform.position,pos4,Color.yellow);
            Debug.DrawLine(camTransform.position,pos5,Color.yellow); Debug.DrawLine(camTransform.position,pos6,Color.yellow);
            Debug.DrawLine(camTransform.position,pos7,Color.yellow); Debug.DrawLine(camTransform.position,pos8,Color.yellow);

            //The clip check
            clip = (Physics.Linecast(camTransform.position, pos1, out hit, mask.value) ||
                    Physics.Linecast(camTransform.position, pos2, out hit, mask.value) ||
                    Physics.Linecast(camTransform.position, pos3, out hit, mask.value) ||
                    Physics.Linecast(camTransform.position, pos4, out hit, mask.value) ||
                    Physics.Linecast(camTransform.position, pos5, out hit, mask.value) ||
                    Physics.Linecast(camTransform.position, pos6, out hit, mask.value) ||
                    Physics.Linecast(camTransform.position, pos7, out hit, mask.value) ||
                    Physics.Linecast(camTransform.position, pos8, out hit, mask.value));

            if (clip) camTransform.localPosition += camTransform.forward * c;
        }
    }

}