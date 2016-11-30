using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerBot : BotBehaviour {

    public static PlayerBot localPlayerBot;

    public Image sprType;
    public Sprite[] typeSprites;
    public Slider powerBar;
    public Image powerBarFill;
    public Transform shootHandPoint;
    private bool grabbing;
    private bool shocking;
    private bool flying;
    private int jetpackFuel = FRAMES_JETPACK_FLY;

    private int viewIndex = 0;
    public Vector3[] localViewPosition;
    public Vector3[] localViewRotation;
    public Transform pivotPoint;

    public static readonly int FRAMES_UNTIL_GRAB = 30,
                               FRAMES_UNTIL_SHOT = 30,
                               FRAMES_JETPACK_FLY = 120,
                               FRAMES_UNTIL_SHOCK = 30;

    private Vector3 moveDirection;

    public float moveSpeed = 5;
    public float rotationSpeed = 2.5F;
    public Transform modelTransform;

    public GameObject rocketPrefab;
    private bool rocketShot = false;

    private Rigidbody rb;
    private BoxCollider bc;
    private Animator animator;

    void Start() {
        // Retrieve Components
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
        animator = GetComponent<Animator>();

        // Make us accessible to other scripts
        localPlayerBot = this;
    }

    void Update() {
        animator.SetBool("walking", false);

        if (grabbing || shocking) return;

        // Jetpack stuff
        if (getBotType() == BotType.JETPACK) {
            handleJetpack();
        }

        handleInput();
    }

    private void handleInput() {
        // roteer model horizontaal
        modelTransform.Rotate(transform.up * rotationSpeed * Input.GetAxis("Horizontal"));

        if (InputManager.Forward) {      // beweeg naar voren
            RPhysics.Move(transform, modelTransform.forward, moveSpeed);
            animator.SetBool("walking", true);
        }
        else if (InputManager.Back) {    // beweeg naar achteren
            RPhysics.Move(transform, -modelTransform.forward, moveSpeed);
            animator.SetBool("walking", true);
        }

        // Action
        if (InputManager.Action)
            performAction();

        // Changing views
        if (InputManager.ChangeView)
            changeView();
    }

    private void performAction() {
        // action is based on bot type

        switch (botType) {
            case BotType.NORMAL: startGrab(); break;
            case BotType.ROCKET: startRocket(); break;
            case BotType.JETPACK: startJetpack(); break;
            case BotType.SHOCK: startShock(); break;
            case BotType.MINI: startGrab(); break;
        }
    }

    private void changeView() {
        viewIndex++;
        if (viewIndex >= localViewPosition.Length) viewIndex = 0; // < Stay within the boundaries
        
        // and set the new local pos and rotation
        pivotPoint.localPosition    = localViewPosition[viewIndex];
        pivotPoint.localEulerAngles = localViewRotation[viewIndex];
    }

    #region grabbing
    private void startGrab() {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Grab")) return;

        animator.Play("Grab");
        StartCoroutine(grabRoutine());
        grabbing = true;
    }

    private void tryGrab() {
        // try to grab something
        RaycastHit hit = RPhysics.BoxCast(transform, modelTransform.forward);

        if (hit.distance < 2) {
            if (hit.transform.GetComponent(typeof(BotBehaviour))) {
                swapTypes( (BotBehaviour) hit.collider.GetComponent(typeof(BotBehaviour)) );
                print("Grabbed another bot! (" + hit.transform.name + ")");
            }
        }
        else {
            print("Did not grab anything");
        }
        grabbing = false;
        powerBar.value = 1;
    }

    private IEnumerator grabRoutine() {
        for (int i = 0; i < FRAMES_UNTIL_GRAB; i++) {
            powerBar.value = i * 1F / FRAMES_UNTIL_GRAB;
            yield return new WaitForEndOfFrame();
        }

        tryGrab();
    }
    #endregion

    #region rockets
    private void startRocket() {
        if (rocketShot) return; // limit rockets to one at a time

        modelTransform.GetComponent<Animator>().Play("Shoot Rocket");
        StartCoroutine(rocketCoroutine());
        setRocketShot(true);
    }

    public void setRocketShot(bool b) {
        this.rocketShot = b;

        if (!b) powerBar.value = 1;
    }

    private void shootRocket() {
        GameObject rocket = Instantiate(rocketPrefab, shootHandPoint.position, shootHandPoint.rotation) as GameObject;
    }

    private IEnumerator rocketCoroutine() {
        for (int i = 0; i < FRAMES_UNTIL_SHOT; i++) {
            powerBar.value = 1 - i * 1F / FRAMES_UNTIL_GRAB;
            yield return new WaitForEndOfFrame();
        }

        powerBar.value = 0;

        shootRocket();
    }
    #endregion

    #region jetpacks
    void startJetpack() {
        if (rb.velocity.y != 0) return; // not on the ground, can't start jetpack
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        flying = true;
        animator.Play("Flying");
    }

    void stopJetpack(){
        rb.useGravity = true;
        flying = false;
        animator.SetTrigger("stopflying");
    }

    void handleJetpack() {
        if (flying) {
                jetpackFly();
            }
        else {
            if (jetpackFuel < FRAMES_JETPACK_FLY)
                jetpackFuel++;
            powerBar.value = jetpackFuel * 1F / FRAMES_JETPACK_FLY;
        }
    }

    void jetpackFly() {
        EffectControl.createEffect(Effect.FIRESPRAY_FINISH, centerPoint.position + Vector3.down * 2);
        RPhysics.Move(transform, modelTransform.up, moveSpeed);

        if (jetpackFuel > 0) {
            jetpackFuel--;
            powerBar.value = jetpackFuel * 1F / FRAMES_JETPACK_FLY;
        }else {
            // no more fuel... lets stop the jetpack.
            stopJetpack();
        }
    }
    #endregion

    #region shock
    private void startShock() {
        animator.Play("Zapping");
        StartCoroutine(shockCoroutine());
        shocking = true;
    }

    private void shock() {
        EffectControl.createEffect(Effect.LIGHTNING_SHOCK, shootHandPoint.position + transform.right * -1F + transform.forward * -2F);
        shocking = false;
    }

    private IEnumerator shockCoroutine() {
        for (int i = 0; i < FRAMES_UNTIL_SHOCK; i++) {
            powerBar.value = 1 - i * 1F / FRAMES_UNTIL_SHOCK;
            yield return new WaitForEndOfFrame();
        }

        powerBar.value = 1;

        shock();
    }
    #endregion

    #region mini

    //bla

    #endregion

    public void swapTypes(BotBehaviour otherBot) {
        // swap bot-types with another bot
        BotType temp = botType;
        setBotType(otherBot.getBotType());
        otherBot.setBotType(temp);
    }

    public override void setBotType(BotType botType) {
        base.setBotType(botType);

        //extra stuff
        switch (botType) {
            case BotType.NORMAL: powerBarFill.color = Color.white; sprType.sprite = typeSprites[0]; break;
            case BotType.ROCKET: powerBarFill.color = new Color(1, 0.5F, 0); sprType.sprite = typeSprites[1]; break;
            case BotType.JETPACK: powerBarFill.color = Color.blue; sprType.sprite = typeSprites[2]; break;
            case BotType.SHOCK: powerBarFill.color = Color.yellow; sprType.sprite = typeSprites[3]; break;
            case BotType.MINI: powerBarFill.color = Color.green; sprType.sprite = typeSprites[4]; break;
        }
    }

}
