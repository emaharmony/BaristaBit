using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : DialogueManager 
{
	[Space(2)]
	[Header("Tutorial")]
	[SerializeField] GameObject[] stepsInTutorial;
	[TextArea()][SerializeField] string[] script;

	int index = -1;

	void Awake()
	{
		base.Awake ();
		foreach (GameObject go in stepsInTutorial)
			go.SetActive (false);
	}

	void Start()
	{
		if (script.Length < 0) {
			Debug.LogWarning ("Please Add Script");
			return;
		}

		CoffeeGameManager.Instance.isPaused = true;

		StartDialogue (new Dialogue ("Tutorial", script));

	}

	public override void ShowNextSentence ()
	{
		if (index == 4) {
			CoffeeMakerUI.Instance.CamTransition ();
		}
			

		if (stepsInTutorial.Length <= 0)
			return;

		if (index < 0)
			index++;
		else
			stepsInTutorial[index++].SetActive(false);
		
		if(index < stepsInTutorial.Length) stepsInTutorial[index].SetActive(true);

		if (sentences.Count <= 0) {
			EndConversation ();
			return;
		}

		dialogue.text = sentences.Dequeue ();

	}

	public override void EndConversation ()
	{
		CoffeeGameManager.Instance.isPaused = false;
		CoffeeMakerUI.Instance.CamTransition ();
		CustomerManager.Instance.AddCustomer ();
		base.EndConversation ();
	}

	public void SkipTutorial()
	{
		CoffeeGameManager.Instance.isPaused = false;
		CustomerManager.Instance.AddCustomer ();
		foreach (GameObject x in stepsInTutorial) {
			x.SetActive(false);
		}
		base.EndConversation ();
	}
}
