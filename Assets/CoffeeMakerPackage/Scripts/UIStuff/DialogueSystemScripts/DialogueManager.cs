using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : Singleton<DialogueManager> {

	protected DialogueManager(){}
	[SerializeField]Animator animate;
	[SerializeField]protected TextMeshProUGUI dialogueName;
	[SerializeField]protected TextMeshProUGUI dialogue;
	protected Queue<string> sentences;

	bool _typing = false;

	protected virtual void Awake()
	{
		sentences = new Queue<string>();
	}

	public void StartDialogue(Dialogue d)
	{
		if (animate != null) {
			animate.SetBool ("IsOpen", true);

			dialogueName.text = d.name;

			sentences.Clear ();

			foreach (string s in d.sentences) {
				sentences.Enqueue (s);
			}

			ShowNextSentence ();
		}
	}

	public virtual void ShowNextSentence()
	{
		if (sentences.Count <= 0) {
			EndConversation ();
			return;
		}
			
		StartCoroutine (TypeWriter (sentences.Dequeue()));
	}

	public virtual void EndConversation ()
	{
		animate.SetBool ("IsOpen", false);
	}

	IEnumerator TypeWriter(string str)
	{
		if (_typing)
			yield break;
		
		_typing = true;
		dialogue.text = "";

		foreach (char c  in str.ToCharArray()) 
		{
			dialogue.text += c;
			yield return null;
		}

		_typing = false;
	}

	public GameObject DialogueBox
	{
		get{ return animate.gameObject;}
	}

}
