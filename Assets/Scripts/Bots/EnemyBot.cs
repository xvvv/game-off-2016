using UnityEngine;

public class EnemyBot : MonoBehaviour {

    [SerializeField]
    private BotType botType = BotType.NORMAL;

    protected virtual void Start() {
        /*Empty*/
    }

    protected virtual void Update() {
        /*Empty*/
    }

    public BotType getBotType() {
        return botType;
    }

    public void setBotType(BotType botType) {
        this.botType = botType;
    }

}