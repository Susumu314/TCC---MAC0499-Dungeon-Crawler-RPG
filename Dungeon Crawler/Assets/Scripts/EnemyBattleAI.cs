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
                if (rng < 20.0f){// 20% de chance de dar um ataque normal em uma unidade aleatoria
                    act = BattleAction.Act.ATTACK;
                    TargetList.Add(PUnits[Random.Range(0, PUnits.Count)]);
                    priority = 1;
                    gameObject.GetComponent<BattleAction>().SetAction(act, TargetList);
                }
                else if (rng < 30.0f){// 10% de chance de defender no turno
                    act = BattleAction.Act.GUARD;
                    TargetList.Add(gameObject.GetComponent<Unit>());
                    priority = 3;
                    gameObject.GetComponent<BattleAction>().SetAction(act, TargetList);
                }
                else{//70% de chance de usar uma habilidade aleatoria
                    priority = SetSkill(PUnits, EUnits);
                }
            }break;

            default:
            break;
        }
        return priority;
    }

    private int SetSkill(List<Unit> PUnits, List<Unit> EUnits){
        //Escolhe uma skill aleatoria
        List<Unit> TargetList = new List<Unit>(); 
        int i = Random.Range(0,4);
        int skillID = gameObject.GetComponent<Unit>().skillList[i];
        while(skillID == -1){
            i = (i+1)%4;
            skillID = gameObject.GetComponent<Unit>().skillList[i];
        }
        int[] r = gameObject.GetComponent<BattleAction>().SetSkill(i);
        switch ((Skill.TARGET_TYPE)r[1])
        {
            case Skill.TARGET_TYPE.SINGLE:
                TargetList.Add(PUnits[Random.Range(0, PUnits.Count)]);
            break;
            case Skill.TARGET_TYPE.SINGLE_ALLY:
                TargetList.Add(EUnits[Random.Range(0, EUnits.Count)]);
            break;
            case Skill.TARGET_TYPE.SELF:
                TargetList.Add(gameObject.GetComponent<Unit>());
            break;
            case Skill.TARGET_TYPE.ALLY_PARTY:
                TargetList = EUnits;
            break;
            case Skill.TARGET_TYPE.PARTY:
                TargetList = PUnits;
            break;
            case Skill.TARGET_TYPE.ROW:
                //escolhe uma linha aleatoria do player para atacar
                i = Random.Range(0,2);
                foreach (Unit unit in PUnits)
                {
                    if(unit.isBackLine == i){
                        TargetList.Add(unit);
                    }
                }
                if(TargetList.Count == 0){
                    foreach (Unit unit in PUnits)
                    {
                        if(unit.isBackLine != i){
                            TargetList.Add(unit);
                        }
                    }
                }
            break;
            case Skill.TARGET_TYPE.ALLY_ROW:
                //escolhe uma linha aleatoria dos aliados para ajudar
                i = Random.Range(0,2);
                foreach (Unit unit in EUnits)
                {
                    if(unit.isBackLine == i){
                        TargetList.Add(unit);
                    }
                }
                if(TargetList.Count == 0){
                    foreach (Unit unit in EUnits)
                    {
                        if(unit.isBackLine != i){
                            TargetList.Add(unit);
                        }
                    }
                }
            break;
            default:
            break;
        }
        gameObject.GetComponent<BattleAction>().SetAction(BattleAction.Act.SKILL, TargetList);
        return r[0];
    }
}
