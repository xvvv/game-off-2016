using UnityEngine;

public class InputManager {

    public static bool Jump {
        get {
            return Input.GetKeyDown(KeyCode.Space);
        }
    }
    public static bool Forward {
        get {
            return Input.GetAxis("Vertical") > 0;
        }
    }
    public static bool Back {
        get {
            return Input.GetAxis("Vertical") < 0;
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

}