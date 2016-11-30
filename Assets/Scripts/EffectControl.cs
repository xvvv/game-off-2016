using UnityEngine;

public class EffectControl : MonoBehaviour {

    public GameObject explosion_blue, explosion_gray, explosion_orange, explosion_yellow, explosion_green;  // Explosions
    public GameObject firespray_finish; // Sprays
    public GameObject lightning_Shock;

    public static EffectControl instance;

    public void Awake() {
        GameObject instanceGO = GameObject.Find("EffectControl");

        if (!instanceGO) {
            Debug.LogWarning("WARNING! Make sure you have a gameobject named 'EffectControl' with the needed script in your scene!");
            return;
        }

        instance = instanceGO.GetComponent<EffectControl>();
    }

    private static void createEffect(Effect effect, Vector3 position, Transform parent) {
        GameObject g;

        switch(effect) {
        case Effect.EXPLOSION_BLUE:
            g = spawnEffectGO(instance.explosion_blue);
        break;
        case Effect.EXPLOSION_GRAY:
            g = spawnEffectGO(instance.explosion_gray);
            break;
        case Effect.EXPLOSION_ORANGE:
            g = spawnEffectGO(instance.explosion_orange);
            break;
            case Effect.EXPLOSION_YELLOW:
                g = spawnEffectGO(instance.explosion_yellow);
                break;
            case Effect.EXPLOSION_GREEN:
                g = spawnEffectGO(instance.explosion_green);
                break;
            case Effect.FIRESPRAY_FINISH:
            g = spawnEffectGO(instance.firespray_finish);
            break;
        case Effect.LIGHTNING_SHOCK:
            g = spawnEffectGO(instance.lightning_Shock);
                g.transform.eulerAngles = PlayerBot.localPlayerBot.transform.eulerAngles - Vector3.up * 90 - Vector3.left * 90;
            break;
            default:
            g = spawnEffectGO(instance.explosion_blue);
            break;
        }

        g.transform.position = position;
        g.transform.parent = parent;

        Destroy(g, g.GetComponent<ParticleSystem>().startLifetime);
    }

    private static GameObject spawnEffectGO(GameObject gO) {
        return Instantiate(gO, gO.transform.position, gO.transform.rotation) as GameObject;
    }

    // public wrapper methods for createEffect
    public static void createEffect(Effect effect, Vector3 position) {
        createEffect(effect, position, null);
    }

    public static void createEffect(Effect effect, Transform centerPoint) {
        createEffect(effect, centerPoint.position, centerPoint);
    }

}

public enum Effect {
    EXPLOSION_BLUE = 0,
    EXPLOSION_GRAY,
    EXPLOSION_ORANGE,
    EXPLOSION_YELLOW,
    EXPLOSION_GREEN,
    FIRESPRAY_FINISH,
    LIGHTNING_SHOCK
}