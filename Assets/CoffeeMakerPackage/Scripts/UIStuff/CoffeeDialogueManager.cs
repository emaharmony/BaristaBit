using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoffeeDialogueManager : MonoBehaviour
{	

    public static CoffeeDialogueManager Instance { get; private set; }
	[SerializeField]Animator animate = null;
	[SerializeField]TextMeshProUGUI dialogueName = null;
	[SerializeField]TextMeshProUGUI dialogue = null;
	protected Queue<Dialogue> orders;

	bool _ordersActive = false;

    void Awake()
    {
        Instance = this;
    }

	void Start()
	{
		orders = new Queue<Dialogue>();
	}

	public void StartDialogue()
	{
		if (animate != null) {
			if (!animate.gameObject.activeSelf) {
				animate.gameObject.SetActive (true);
			}
			_ordersActive = true;
			animate.SetBool ("IsOpen", true);
			ShowNextSentence ();
		}
	}

	public void AddOrder(Dialogue d)
	{
		orders.Enqueue (d);
	}

	public void ShowNextSentence()
	{
		if (orders.Count <= 0) {
			EndConversation ();
			return;
		}

		StartCoroutine (TypeWriter (orders.Dequeue()));
	}

	public void EndConversation ()
	{
		animate.SetBool ("IsOpen", false);
		_ordersActive = false;
	}

	IEnumerator TypeWriter(Dialogue str)
	{
		dialogue.text = "";
		dialogueName.text = str.name;

		foreach (char c  in str.sentences[0].ToCharArray()) 
		{
			dialogue.text += c;
			yield return null;
		}

	}

	void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag ("Customer")) 
		{
			other.GetComponent<Customer> ().StartTimer ();
			if ( !_ordersActive) 
			{
				StartDialogue();
				return;
			}
			ShowNextSentence ();
		} 

	}
		
}
