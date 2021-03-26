using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAction : MonoBehaviour
{
    public enum Act
    {
        ATTACK, GUARD, ESCAPE, CAPTURE, 
        SKILL_0, SKILL_1, SKILL_2, SKILL_3, NULL
    }
    private Act act;
    private List<Unit> TargetList = new List<Unit>(); //todos os alvos de uma ação serão colocados dentro deste array

    public void SetAction(Act acao = Act.NULL, List<Unit> Targets = null){
        if (acao != Act.NULL)//a acao só vai ser nula quando o SetAction for chamado para atribuição apenas dos Targets
                             //ou seja, a função assume que a acao ja foi escolhida numa chamada anterior do SetAction
            act = acao;
        TargetList = Targets;
    }

    public void PerformAction(){
        switch (act)
        {
            case Act.ATTACK://deals normal fisical damage to the target
            {
                int damage = gameObject.GetComponent<Unit>().damage;
                //int damage = Damage_Calculation(blablabla);
                //play action animation
                //play soundfx
                //display mensage
                //wait for animations and mensage
                TargetList[0].TakeDamage(damage);
                break;
            }
            case Act.GUARD://Guard Action
            {
                TargetList[0].SetGuard(true);//aqui esta assumindo que o target está sendo corretamente associado ao usuário
                break;
            }
            case Act.ESCAPE://deals normal fisical damage to the target
            {
                TargetList[0].Escape();
                break;
            }

            default: break;
        }
    }
}
