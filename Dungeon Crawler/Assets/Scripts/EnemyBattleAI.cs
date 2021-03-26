using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBattleAI : MonoBehaviour
{
    enum AI_type{ NULL, DUMB, SMART, AGRESSIVE, DEFENSIVE, ESPECIFIC, STATUS_CONDITION_USER, BUFF_USER, DEBUF_USER}

    private AI_type AI = AI_type.DUMB;
    /*AI for Enemy Movement durring a battle
    Is only used for EnemyUnits and returns the priority of the move
    This Function must be dependent of what type of enemy it is
    Precisa receber as informação de todas unidades em batalha para decidir uma ação
    Valores de retorno:
    LowPriority: 0
    NormalPriority: 1
    HighPriority: 2
    MaxPriority: 3
    */
    public int EnemyMove(Unit[] PlayerUnits, Unit[] EnemyUnits){

        int priority = 1;
        BattleAction.Act act = BattleAction.Act.ATTACK;
        List<Unit> PUnits = new List<Unit>();
        List<Unit> EUnits = new List<Unit>(); 
        List<Unit> TargetList = new List<Unit>(); 
        for (int i = 0; i < 6; i++)
        {
            if (PlayerUnits[i])
            {
                PUnits.Add(PlayerUnits[i]);
            }
            if (EnemyUnits[i])
            {
                EUnits.Add(EnemyUnits[i]);
            }
        }
        float rng = Random.Range(0f, 100.0f);
        print(rng + "\n");

        switch (AI)
        {
            case AI_type.DUMB:{
                if (rng >= 50.0f){//ATTACK A RANDOM UNIT
                    act = BattleAction.Act.ATTACK;
                    TargetList.Add(PUnits[Random.Range(0, PUnits.Count - 1)]);// out of range?
                    priority = 1;
                }
                else{//GUARD
                    act = BattleAction.Act.GUARD;
                    TargetList.Add(gameObject.GetComponent<Unit>());
                    priority = 3;
                }
                gameObject.GetComponent<BattleAction>().SetAction(act, TargetList);
            }break;

            default:
            break;
        }
        return priority;
    }
}
