using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient
{

    CoffeeMaker.Ingredients _type;
    int _quantity;

    public Ingredient(CoffeeMaker.Ingredients t)
    {
        _type = t;
        _quantity = 1;
    }

    public Ingredient(CoffeeMaker.Ingredients t, int q)
    {
        _type = t;
        _quantity = q;
    }

    public void AddIngredient()
    {
        _quantity++;
    }

    public CoffeeMaker.Ingredients Type { get { return _type; } }
    public int Amt { get { return _quantity; } }

	public override bool Equals(System.Object obj)
	{
		if (obj == null)
			return false;
		
		Ingredient i = obj as Ingredient;
		if ((System.Object)i == null)
			return false;

		return _type == i.Type;
	}

	public String Name
	{
		get
		{
			switch (_type) 
			{	
			case CoffeeMaker.Ingredients.Chocolate_Syrup:
				return "Chocolate Syrup";// x" + _quantity;
			case CoffeeMaker.Ingredients.Espresso:
				return "Espresso";// x" + _quantity;
			case CoffeeMaker.Ingredients.Foam_Milk:
				return "Foamed Milk";// x" + _quantity;
			case CoffeeMaker.Ingredients.Steamed_HH:
				return "Steamed Half & Half";// x" + _quantity;
			case CoffeeMaker.Ingredients.Steamed_Milk:
				return "Steamed Milk"; //x" + _quantity;
			case CoffeeMaker.Ingredients.Water:
				return "Water";// x" + _quantity;
			case CoffeeMaker.Ingredients.WhipCream:
				return "Whipped Cream";// x" + _quantity;
			default:
				return "";
			}
		}
	}
}
