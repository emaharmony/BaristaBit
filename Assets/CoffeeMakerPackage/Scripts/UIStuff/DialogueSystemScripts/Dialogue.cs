using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue 
{
	[SerializeField]public string name;
	[TextArea(3,10)]
	[SerializeField]public string[] sentences;

	public Dialogue(string name, string[] entry){
		sentences = entry;
		this.name = name;
	}


	public Dialogue(string name, string entry)
	{
		sentences = new string[1];
		sentences [0] = entry;
		this.name = name;
	}
}
