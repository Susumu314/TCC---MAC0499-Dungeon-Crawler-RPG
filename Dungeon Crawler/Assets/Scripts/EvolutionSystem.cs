using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EvolutionSystem : MonoBehaviour
{
    public GameObject dialogBox;
    private List<Unit> evolutionQueue = new List<Unit>();
    private bool startTimer = false;
    private float timer = 0.0f;
    private bool skip = false;
    
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

    private void InitQueue(){
        GameObject party = GameManager.Instance.party;
        foreach (Unit u in party.GetComponentsInChildren<Unit>())
        {
            if(u.canEvolve){
                evolutionQueue.Add(u);
                u.canEvolve = false;
            }
        }
        StartCoroutine(ShowEvolutions());
    }

    private IEnumerator ShowEvolutions(){
        float skipTime = 2.0f;
        foreach (Unit u in evolutionQueue)
        {
            yield return ShowDialog("WHAT? " + u.unitName + " is evolving!", skipTime); 
            yield return EvolutionAnimation(u.species);
            yield return ShowDialog(u.unitName + " evolved into " + u.stats.evolution, skipTime); 
            u.Evolve();
        }
        yield return null;
        GameManager.Instance.state = GameManager.State.Overworld;
        SceneManager.LoadScene(GameManager.Instance.CurrentOverworldScene);
    }

    private IEnumerator ShowDialog(string text, float waitTime){
        dialogBox.GetComponentInChildren<Text>().text = text;
        startTimer = true;
        timer = 0f;
        yield return new WaitUntil(() => ((timer >= waitTime) || skip));//isso aqui nao esta funcionando por algum motivo
        startTimer = false;
        skip = false;
        yield return null;
    }

    private IEnumerator EvolutionAnimation(string demonSpecies){
        // GameObject Anim = Instantiate(Resources.Load("VFX/Skill_Animation"), target_transform.position, Quaternion.identity) as GameObject;
        // Animator animator = Anim.GetComponent<Animator>();
        // if(color != Color.white){
        //     Anim.GetComponent<SpriteRenderer>().color =  color;
        // }
        // animator.Play(animation);
        // //Fetch the current Animation clip information for the base layer
        // AnimatorClipInfo[] m_CurrentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        // return m_CurrentClipInfo[0].clip.length;
        yield return null;
    }
}
