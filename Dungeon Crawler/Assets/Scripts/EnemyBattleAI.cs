using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBattleAI : MonoBehaviour
{
    enum AI_type{ NULL, DUMB, SMART, AGRESSIVE, DEFENSIVE, ESPECIFIC, STATUS_CONDITION_USER, BUFF_USER, DEBUF_USER}

    private AI_type AI = AI_type.DUMB;
    /**
    * IA usada para decidir a ação de um inimigo durante a batalha, 
    * usa as informações recebidas como parâmetro para decidir a ação de uma unidade controlada pelo PC.
    *
    * @param PlayerUnits Array com as informações da equipe do jogador.
    * @param EnemyUnits Array com as informações dos demonios inimigos.
    *
    * @retval 0 Ação decidida é de baixa prioridade
    * @retval 1 Ação decidida é de prioridade normal
    * @retval 2 Ação decidida é de alta prioridade
    * @retval 3 Ação decidida é de máxima prioridade
    */
    public int EnemyMove(Unit[] PlayerUnits, Unit[] EnemyUnits){

        int priority = 1;
        BattleAction.Act act = BattleAction.Act.ATTACK;
        List<Unit> PUnits = new List<Unit>();
        List<Unit> EUnits = new List<Unit>(); 
        List<Unit> TargetList = new List<Unit>(); 
        for (int i = 0; i < 6; i++)
        {
            if (PlayerUnits[i] && !PlayerUnits[i].isDead)
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
                    TargetList.Add(PUnits[Random.Range(0, PUnits.Count)]);
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
