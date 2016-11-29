using UnityEngine;

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
        }
    }

}