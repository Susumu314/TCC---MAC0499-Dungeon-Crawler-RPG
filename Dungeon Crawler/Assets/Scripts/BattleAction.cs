using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAction : MonoBehaviour
{
    public enum Act
    {
        ATTACK, GUARD, ESCAPE, CAPTURE, 
        SKILL_0, SKILL_1, SKILL_2, SKILL_3
    }

    private Unit User; //quem faz a ação
    private Act act;
    private List<Unit> TargetList = new List<Unit>(); //todos os alvos de uma ação serão colocados dentro deste array

    public void SetAction(Act acao, List<Unit> Targets){
        act = acao;
        TargetList = Targets;
    }

    public void PerformAction(){
        switch (act)
        {
            case Act.ATTACK://deals normal fisical damage to the target
            {
                int damage = User.damage;
                //int damage = Damage_Calculation(blablabla);
                TargetList[0].TakeDamage(damage);
                break;
            }

            default: break;
        }
    }
}
