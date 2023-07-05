/* CoffeeMaker
 * Emmanuel Vinas
 * 
 * User Input controller for the coffee game.
 * Records input and sends it to Cutomer Manager to check if correct. 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeMaker : MonoBehaviour
{

    public static CoffeeMaker Instance { get; private set; }

    public enum Ingredients
    {
        Espresso,
        Foam_Milk,
        WhipCream,
        Steamed_Milk,
        Steamed_HH,
        Chocolate_Syrup,
        Water
    }

//    ..CustomerManager _cm;
	Customer _currCustomer = null;
	const int INPUT_CAP = 6;
    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
		if (CoffeeGameManager.Instance.isPaused)
			return;
		
		if(_currCustomer != null && Input.GetButtonDown("Serve"))
		{
			CustomerManager.Instance.SendFinishCoffee ();
		}

    }

    public void AddIngredient(Ingredients t, string s)
    {
		if (CoffeeGameManager.Instance.isPaused || _currCustomer.Input.IngredientInput.Count >= INPUT_CAP)
			return;
		
		if (_currCustomer != null) {
			CoffeeMakerUI.Instance.AddIngredient (s);

			_currCustomer.AddToInput (new Ingredient (t));
		}
    }

    public void AddIngredient(int t)
    {
		if (CoffeeGameManager.Instance.isPaused || _currCustomer.Input.IngredientInput.Count >= INPUT_CAP)
			return;
		
		if (_currCustomer != null) {
			switch (t) {
			case 0:
				CoffeeMakerUI.Instance.AddIngredient ("Espresso Added");
				break;
			case 1:
				CoffeeMakerUI.Instance.AddIngredient ("Foamed Milk Added");
				break;
			case 2:
				CoffeeMakerUI.Instance.AddIngredient ("Whipped Cream Added");
				break;
			case 3:
				CoffeeMakerUI.Instance.AddIngredient ("Steamed Milk Added");
				break;
			case 4:
				CoffeeMakerUI.Instance.AddIngredient ("Steamed Half & Half Added");
				break;
			case 5:
				CoffeeMakerUI.Instance.AddIngredient ("Chocolate Syrup Added");
				break;
			case 6:
				CoffeeMakerUI.Instance.AddIngredient ("Water Added");
				break;
			}

			_currCustomer.AddToInput (new Ingredient ((Ingredients)t));
			CoffeeMakerUI.Instance.CoffeeFillSection (t);
		}
    }

	public void AddSleeve()
	{
		if (_currCustomer == null)
			return;
		
		_currCustomer.AddSleeve ();
	}

	public void AddCap()
	{
		if (_currCustomer == null)
			return;

		_currCustomer.AddCap ();
	}

    public void StartNewDrink(int s)
    {
		if(_currCustomer != null)
			_currCustomer.NewCoffeeForMe((CoffeeOrder.CoffeeSize)s);
    }

	public void StartNewDrink()
	{
		if (_currCustomer == null)
			return;

		_currCustomer.NewCoffeeForMe();
		CoffeeMakerUI.Instance.CoffeeThrownAway ();
	}
		
	public Customer CurrentCustomer
	{
		get{ return _currCustomer; }
		set{ _currCustomer = value; }
	}

}

/** OLD CODE
        if (_cm.SelectedCustomer != null)
        {
            if (Input.GetButtonDown("Espresso")) AddIngredient(Ingredients.Espresso, "Espresso");
            if (Input.GetButtonDown("Foam_Milk")) AddIngredient(Ingredients.Foam_Milk, "Foamed Milk");
            if (Input.GetButtonDown("WhipCream")) AddIngredient(Ingredients.WhipCream, "WhipCream");
            if (Input.GetButtonDown("Steamed_Milk")) AddIngredient(Ingredients.Steamed_Milk, "Steamed Milk");
            if (Input.GetButtonDown("Steamed_HH")) AddIngredient(Ingredients.Steamed_HH, "Steamed Half & Half");
            if (Input.GetButtonDown("Chocolate_Syrup")) AddIngredient(Ingredients.Chocolate_Syrup, "Chocolate Syrup");
            if (Input.GetButtonDown("Water")) AddIngredient(Ingredients.Water, "Water");
            if (Input.GetButtonDown("Submit") && _input.IngredientInput.Count > 0) SendInputToActiveCustomer();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Keypad1)) _cm.SelectCustomer(0);
            if (Input.GetKeyDown(KeyCode.Keypad2)) _cm.SelectCustomer(1);
            if (Input.GetKeyDown(KeyCode.Keypad3)) _cm.SelectCustomer(2);
            if (Input.GetKeyDown(KeyCode.Keypad4)) _cm.SelectCustomer(3);
            if (Input.GetKeyDown(KeyCode.Keypad5)) _cm.SelectCustomer(4);
            if (Input.GetKeyDown(KeyCode.Keypad6)) _cm.SelectCustomer(5);
            if (Input.GetKeyDown(KeyCode.Keypad7)) _cm.SelectCustomer(6);
            if (Input.GetKeyDown(KeyCode.Keypad8)) _cm.SelectCustomer(7);
            if (Input.GetKeyDown(KeyCode.Keypad9)) _cm.SelectCustomer(8);
        }
    */