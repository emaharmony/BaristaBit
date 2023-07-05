using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CoffeeVisual : MonoBehaviour 
{
	[SerializeField] Image[] sections;
	int _currSection = 0;

	void Start()
	{
		EmptyCup ();
	}

	public void EmptyCup()
	{
		for (int i = 0; i < sections.Length; i++) 
		{
			sections [i].gameObject.SetActive (false);
		}

		_currSection = 0;
	}

	public void FillSection(Color color)
	{
		if (_currSection >= sections.Length)
			return;
		
		sections [_currSection].gameObject.SetActive (true);
		sections [_currSection++].color = color;

	}

	public void HoverOverSection(int i)
	{
		Debug.Log ("Hover Over");
		if (CoffeeMaker.Instance.CurrentCustomer == null 
			|| CoffeeMaker.Instance.CurrentCustomer.Input == null
			|| CoffeeMaker.Instance.CurrentCustomer.Input.IngredientInput.Count <= 0)
			return;
		
		TextMeshProUGUI temp = sections [i].transform.parent.GetComponentInChildren<TextMeshProUGUI> ();
		temp.GetComponent<CanvasGroup> ().alpha = 1;
		temp.text = (OrderManager.Instance.CurrentCustomer.Input.IngredientInput [i] == null ?  "" : OrderManager.Instance.CurrentCustomer.Input.IngredientInput [i].Name);
	}

	public void PointerExitiSection(int i)
	{

		TextMeshProUGUI temp = sections [i].transform.parent.GetComponentInChildren<TextMeshProUGUI> ();
		temp.GetComponent<CanvasGroup> ().alpha = 0;
	}
		
}
