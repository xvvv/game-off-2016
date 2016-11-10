using UnityEngine;

public class PlayerBot : MonoBehaviour {

    private Vector3 moveDirection;

    //TODO change accessors
    [SerializeField]
    private BotType botType = BotType.NORMAL;

    public float moveSpeed = 5;
    public float rotationSpeed = 2.5F;
    public Transform modelTransform;

    private BoxCollider bc;

	void Start () {
        //Retrieve Components
        bc = GetComponent<BoxCollider>();

    }
	
	void Update () {
        handleInput();
    }

    private void handleInput() {/*
        if (Input.GetKey(KeyCode.LeftArrow))    //Roteer model naar links
            modelTransform.Rotate(-transform.up * rotationSpeed);

        if (Input.GetKey(KeyCode.RightArrow))   //Roteer model naar rechts
            modelTransform.Rotate(transform.up * rotationSpeed);*/

        modelTransform.Rotate(transform.up * rotationSpeed * Input.GetAxis("Horizontal"));

        if (Input.GetKey(KeyCode.UpArrow))      //Beweeg naar voren
            RPhysics.Move(transform, modelTransform.forward, moveSpeed);

        if (Input.GetKey(KeyCode.DownArrow))    //Beweeg naar achteren
            RPhysics.Move(transform, -modelTransform.forward, moveSpeed);

        if (InputManager.Jump)
            grab();
    }

    private void grab() {
        print("Attempting to grab...");

        RaycastHit hit = RPhysics.BoxCast(transform, modelTransform.forward);
        if (hit.transform.GetComponent<EnemyBot>()) {
            swapTypes(hit.collider.GetComponent<EnemyBot>());
            print("Grabbed an enemy! " + hit.transform.name);
        }

        print(hit.transform.name);

    }

    private void swapTypes(EnemyBot enemy) {
        BotType temp = botType;
        botType = enemy.getBotType();
        enemy.setBotType(temp);
    }

}
