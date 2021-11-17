using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
	public TextMeshProUGUI dialogueText;
	private IEnumerator typeSentenceCo= null;  // Im sure theres a better name for this field

	//public Animator animator;

	private Queue<string> sentences;
	private bool canSkip = false;
	private float pitchModifier = 1f;
	//public GameObject Menu;
	public bool dialogueFinished = true;


	// Use this for initialization
	void Awake(){
		sentences = new Queue<string>();
	}
	void Update(){
		if(canSkip){
			if(Input.GetButtonDown("Submit")){
				DisplayNextSentence();
			}
		}
	}

	public void StartDialogue (Dialogue dialogue)
	{
		//animator.SetBool("IsOpen", true);
		dialogueFinished = false;
		pitchModifier = dialogue.voicePitch;

		nameText.text = dialogue.name;

		sentences.Clear();

		foreach (string sentence in dialogue.sentences)
		{
			sentences.Enqueue(sentence);
		}

		DisplayNextSentence();
	}

	public void DisplayNextSentence ()
	{
		if (sentences.Count == 0)
		{
			EndDialogue();
			return;
		}

		string sentence = sentences.Dequeue();
		if (typeSentenceCo!= null)
            StopCoroutine(typeSentenceCo);

        typeSentenceCo = TypeSentenceCoroutine(sentence);
		StartCoroutine(typeSentenceCo);
	}

	public void EndDialogue()
	{
		dialogueFinished = true;
		// if(Menu){
		// 	if(!Menu.gameObject.activeSelf){
		// 		Menu.gameObject.SetActive(true);
		// 	}
		// }
	}
	private IEnumerator TypeSentenceCoroutine(string sentence)
	{
		AudioManager.instance.Play("Text", pitchModifier);
		canSkip = false;
		dialogueText.text = "";
		foreach (char letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
			yield return null;
		}
		AudioManager.instance.Stop("Text");
		yield return new WaitForSeconds(0.2f);
		canSkip = true;
		typeSentenceCo= null;
	}
}
