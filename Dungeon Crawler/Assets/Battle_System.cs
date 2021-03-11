using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class Battle_System : MonoBehaviour
{
    public BattleState state;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    Unit playerUnit;
    Unit enemyUnit;
    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        SetupBattle();
    }

    void SetupBattle(){
        playerUnit = Instantiate(playerPrefab, playerBattleStation).GetComponent<Unit>();
        enemyUnit = Instantiate(enemyPrefab, enemyBattleStation).GetComponent<Unit>();

        enemyUnit.unitName;
    }
}
