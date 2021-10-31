using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue {

	public string name;
	public float voicePitch = 1f;

	[TextArea(3, 10)]
	public string[] sentences;

	public Dialogue(string name,string[] sentences,  float pitch = 1f){
		this.name = name;
		this.voicePitch = pitch;
		this.sentences = sentences;
	}

}
