using UnityEngine;
using System.Collections;

public class BotBehaviour : MonoBehaviour {

    public Renderer rend;

    [SerializeField]
    protected BotType botType = BotType.NORMAL;

    // the center of a bot
    public Transform centerPoint;

    public BotType getBotType() {
        return botType;
    }

    public virtual void setBotType(BotType botType) {
        // set the type
        this.botType = botType;

        // and create an effect based on the new type
        switch (botType) {
            case BotType.NORMAL: EffectControl.createEffect(Effect.EXPLOSION_GRAY, centerPoint);   rend.material.color = Color.white; break;
            case BotType.ROCKET: EffectControl.createEffect(Effect.EXPLOSION_ORANGE, centerPoint); rend.material.color = new Color(1,0.5F,0); break;
            case BotType.JETPACK: EffectControl.createEffect(Effect.EXPLOSION_BLUE, centerPoint);  rend.material.color = Color.blue; break;
            case BotType.SHOCK: EffectControl.createEffect(Effect.EXPLOSION_YELLOW, centerPoint); rend.material.color = Color.yellow; break;
            case BotType.MINI: EffectControl.createEffect(Effect.EXPLOSION_GREEN, centerPoint); rend.material.color = Color.green; StartCoroutine(miniRoutine()); break;
        }

        if (botType != BotType.MINI)
            stopMini();
    }

    protected void startMini() {
        StartCoroutine(miniRoutine());
    }

    protected void stopMini() {
        StartCoroutine(notMiniRoutineLolz());
    }

    protected IEnumerator miniRoutine() {
        // Shhrink!
        while (transform.localScale.x > 0.6F) {
            transform.localScale -= Vector3.one * 0.01F;
            yield return new WaitForEndOfFrame();
        }

    }

    protected IEnumerator notMiniRoutineLolz() {
        // Grrow!
        while (transform.localScale.x < 1F) {
            transform.localScale += Vector3.one * 0.01F;
            yield return new WaitForEndOfFrame();
        }

    }

}