/* Coffee Order Class
 * Emmanuel Vinas
 * 
 * Holds the information of the coffee.
 * The needed inputs 
 * holds enum of all coffees. 
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeOrder
{

	#region keys
	Ingredient[] _americano_Key = { new Ingredient(CoffeeMaker.Ingredients.Espresso), new Ingredient(CoffeeMaker.Ingredients.Water), new Ingredient(CoffeeMaker.Ingredients.Water) };
	Ingredient[] _cappuccino_Key = { new Ingredient(CoffeeMaker.Ingredients.Espresso), new Ingredient(CoffeeMaker.Ingredients.Steamed_Milk), new Ingredient(CoffeeMaker.Ingredients.Foam_Milk), new Ingredient(CoffeeMaker.Ingredients.Foam_Milk) };
	Ingredient[] _Espresso_Key = { new Ingredient(CoffeeMaker.Ingredients.Espresso) };
	Ingredient[] _espressoMacchiato_Key = { new Ingredient(CoffeeMaker.Ingredients.Espresso), new Ingredient(CoffeeMaker.Ingredients.Foam_Milk) };
	Ingredient[] _espressoConPanna_Key = { new Ingredient(CoffeeMaker.Ingredients.Espresso), new Ingredient(CoffeeMaker.Ingredients.WhipCream) };
	Ingredient[] _caffeLatte_Key = { new Ingredient(CoffeeMaker.Ingredients.Espresso), new Ingredient(CoffeeMaker.Ingredients.Steamed_Milk), new Ingredient(CoffeeMaker.Ingredients.Steamed_Milk), new Ingredient(CoffeeMaker.Ingredients.Foam_Milk) };
	Ingredient[] _flatWhite_Key = { new Ingredient(CoffeeMaker.Ingredients.Espresso), new Ingredient(CoffeeMaker.Ingredients.Steamed_Milk), new Ingredient(CoffeeMaker.Ingredients.Steamed_Milk) };
	Ingredient[] _cafeBreve_Key = { new Ingredient(CoffeeMaker.Ingredients.Espresso), new Ingredient(CoffeeMaker.Ingredients.Steamed_HH), new Ingredient(CoffeeMaker.Ingredients.Steamed_HH), new Ingredient(CoffeeMaker.Ingredients.Foam_Milk) };
	Ingredient[] _caffeMocha_Key = { new Ingredient(CoffeeMaker.Ingredients.Espresso), new Ingredient(CoffeeMaker.Ingredients.Chocolate_Syrup), new Ingredient(CoffeeMaker.Ingredients.Steamed_Milk), new Ingredient(CoffeeMaker.Ingredients.WhipCream) };
	#endregion

	[System.Serializable]
    public enum CoffeeSize
    {
        small,
        medium,
        large
    }

    //coffee type enum
    public enum CoffeeType
    {
        Espresso,
        E_Macchiato,
        E_conPanna,
        C_Latte,
        FlatWhite,
        C_Breve,
        Cappuccino,
        C_Mocha,
        Americano
    }

    CoffeeSize _size;
    CoffeeType _type;

    public string Name;

	bool _iced = false;

    public CoffeeOrder()
    {
		_type = (CoffeeType)Random.Range(0,Mathf.Clamp(CustomerManager.Instance.TotalAmtCustomers,1,9));
		_size = (CoffeeSize)Random.Range(0, 3);

		Name = _size == CoffeeSize.small ? "Small " : _size == CoffeeSize.medium ? "Medium " : "Large ";
        switch (_type)
        {
            case CoffeeOrder.CoffeeType.Americano:
                Name += "Americano";
                break;
            case CoffeeOrder.CoffeeType.Cappuccino:
                Name += "Cappuccino";
                break;
            case CoffeeOrder.CoffeeType.C_Breve:
                Name += "Cafe Breve";
                break;
            case CoffeeOrder.CoffeeType.C_Latte:
                Name += "Caffe Latte";
                break;
            case CoffeeOrder.CoffeeType.C_Mocha:
                Name += "Caffe Mocha";
                break;
            case CoffeeOrder.CoffeeType.Espresso:
                Name += "Espresso";
                break;
            case CoffeeOrder.CoffeeType.E_conPanna:
                Name += "Espresso con Panna";
                break;
            case CoffeeOrder.CoffeeType.E_Macchiato:
                Name += "Espresso Macchiato";
                break;
            case CoffeeOrder.CoffeeType.FlatWhite:
                Name += "Flat White";
                break;
        }
    }

    public CoffeeType Coffee
    {
        get { return _type; }
    }
		
    //answer key for all coffees
	public float IsCoffeeCorrect(Drink io)
    {
        Ingredient[] keys = new Ingredient[1];
		float totalScore = 10;
		float mistakes = (_size != io.SizeInput) ?  Mathf.Abs(io.SizeInput - _size) : 0;

        switch (_type)
        {
			case CoffeeType.Americano:
				keys = _americano_Key;
                break;
            case CoffeeType.Cappuccino:
                keys = _cappuccino_Key;
                break;
            case CoffeeType.Espresso:
                keys = _Espresso_Key;
                break;
            case CoffeeType.E_Macchiato:
                keys = _espressoMacchiato_Key;
                break;
            case CoffeeType.E_conPanna:
                keys = _espressoConPanna_Key;
                break;
            case CoffeeType.C_Latte:
                keys = _caffeLatte_Key;
                break;
            case CoffeeType.FlatWhite:
                keys = _flatWhite_Key;
                break;
            case CoffeeType.C_Breve:
                keys = _cafeBreve_Key;
                break;
            case CoffeeType.C_Mocha:
                keys = _caffeMocha_Key;
                break;
        }

		//is the item in the array?
		for (int i = 0; i < keys.Length; i++)
        {
			if (keys [i].Type == io.IngredientInput [i].Type) {
				continue;
			} else if (io.IngredientInput.Contains (keys [i])) {
				mistakes += 0.5f;
			} else {
				mistakes += 1;
			}
        }

		mistakes += io.SizeInput == _size ? 0 : 1;
		totalScore -= (mistakes * (totalScore / keys.Length));
		Debug.Log (totalScore);
		return totalScore < 0 ? 0 : totalScore;
    }


}
