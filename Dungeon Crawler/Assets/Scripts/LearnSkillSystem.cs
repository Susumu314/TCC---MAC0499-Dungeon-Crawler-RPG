using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LearnSkillSystem : MonoBehaviour
{
    public GameObject skillText;
    public GameObject dialogBoxText;
    private List<Unit> learnQueue = new List<Unit>();
    private bool startTimer = false;
    private float timer = 0.0f;
    private bool skip = false;
    private bool learnSkill = false;
    public Stat_Screen stat_Screen;
    private enum State{SETUP, CHOOSING, CONFIRM, SHOWDIALOG}
    private State state = State.SETUP;
    private int selectedSkill;
    private int selectedUnit;
    public GameObject skillMenu;
    
    void Start(){
        InitQueue();
    }

    void Update()
    {
        if(startTimer){
            timer += Time.deltaTime;
            if(Input.anyKeyDown){
                skip = true;
            }
        }
    }

    private void SetupMenu(){
        for (int i = 0; i < 5; i++)
        {   
            int skillID;
            if(i < 4){
                skillID = learnQueue[selectedUnit].skillList[i];
                if(skillID == -1){
                    skillMenu.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = "Empty";
                    continue;
                }
            }else{
                skillID = learnQueue[selectedUnit].newSkillID;
            }
            Skill.SkillData s = Skill.SkillList[skillID];
            skillMenu.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = s.Name;
            string description =s.Name + "\nPower:" + Mathf.Abs(s.Power) + "  Accuracy:" + s.Accuracy;
            if(s.IsSpecial){
                description += "\nCost:" + s.Cost + " MP Type:" + s.Type;
            }      
            else{
                description += "\nCost:" + s.Cost*learnQueue[selectedUnit].maxHP/100 + " HP Type:" + s.Type;
            }   
            if(s.IsRanged){
                description += " Long Range";
            }      
            else{
                description += " Close Range";
            }   
            description += "\n" + s.DESC;           
            SelectableElement selectable = skillMenu.transform.GetChild(i).GetComponent<SelectableElement>();
            selectable.text = description;
            if(!selectable.dialogText){
                selectable.dialogText= this.skillText.GetComponent<Text>();
            }
        }
    }

    private void InitQueue(){
        GameObject party = GameManager.Instance.party;
        foreach (Unit u in party.GetComponentsInChildren<Unit>())
        {
            if(u.canLearnSkill){
                learnQueue.Add(u);
                u.canLearnSkill = false;
            }
        }
        selectedUnit = 0;
        stat_Screen.UpdateStatScreen(learnQueue[selectedUnit]);
        SetupMenu();
        StartCoroutine(ChooseSkills());
    }

    private IEnumerator ChooseSkills(){
        state = State.CHOOSING;
        yield return ShowDialog("This unit has learned more than 4 SKILLS.\nSelect a Skill to forget.", 0.5f);
    }

    private IEnumerator ShowDialog(string text, float waitTime){
        //dialogBox.GetComponentInChildren<Text>().text = text;
        startTimer = true;
        timer = 0f;
        dialogBoxText.GetComponent<Text>().text = text;
        yield return new WaitUntil(() => ((timer >= waitTime) || skip));//isso aqui nao esta funcionando por algum motivo
        startTimer = false;
        skip = false;
        yield return null;
    }

    /**
    * Função chamada quando um dos botões do menu de seleção de ação é pressionado.
    * Registra a ação e inicia a etapa de seleção de alvo.
    * 
    * @param index Index que identifica o botão que fez a chamada da função, determina qual ação está sendo selecionada.
    */
    public void OnChoiceButton(int index)
    {
        if (state != State.CHOOSING){
            return;
        }
        selectedSkill = index;
        StartCoroutine(ConfirmChoice());//inicia a ação de escolher um alvo
        
    }

    private IEnumerator ConfirmChoice(){
        state = State.CONFIRM;
        if(selectedSkill == 4){
            yield return (ShowDialog("Are you sure you want to forget " + Skill.SkillList[learnQueue[selectedUnit].newSkillID].Name + " ?", 0.5f));
        }
        yield return (ShowDialog("Are you sure you want to forget " + Skill.SkillList[learnQueue[selectedUnit].skillList[selectedSkill]].Name + " ?", 0.5f));
        yield return null;
    }

    public void OnConfirmButton(int index)
    {
        if (state != State.CONFIRM){
            return;
        }
        if(index == 1){
            learnQueue[selectedUnit].LearnSkill(selectedSkill);
            selectedUnit++;
            if(selectedUnit >= learnQueue.Count){
                GameManager.Instance.state = GameManager.State.Overworld;
                SceneManager.LoadScene(GameManager.Instance.CurrentOverworldScene);
            }
            stat_Screen.UpdateStatScreen(learnQueue[selectedUnit]);
            SetupMenu();
        }
        StartCoroutine(ChooseSkills());
    }

}
