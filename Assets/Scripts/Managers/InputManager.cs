using UnityEngine;

public class InputManager {

    public static bool Action {
        get {
            return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0);
        }
    }
    public static bool ActionHold {
        get {
            return Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse0);
        }
    }
    public static bool Forward {
        get {
            return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        }
    }
    public static bool Back {
        get {
            return Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        }
    }
    public static bool Left {
        get {
            return Input.GetAxis("Horizontal") < 0;
        }
    }
    public static bool Right {
        get {
            return Input.GetAxis("Horizontal") > 0;
        }
    }
    public static bool ChangeView {
        get {
            return Input.GetKeyDown(KeyCode.Tab);
        }
    }

}