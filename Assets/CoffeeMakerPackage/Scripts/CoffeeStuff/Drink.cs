using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drink
{
	List<Ingredient> _ingredients;
    CoffeeOrder.CoffeeSize _size;

	bool _sleeve, _cap; 

    public Drink(CoffeeOrder.CoffeeSize size)
    {
		_sleeve = false;
		_cap = false;
        _size = size;
        _ingredients = new List<Ingredient>();
    }

    public void AddIngredient(Ingredient x)
    {
        _ingredients.Add(x);
    }

    public void ChooseSize(CoffeeOrder.CoffeeSize s)
    {
        _size = s;
    }

	public void AddSleeve()
	{
		_sleeve = true;
	}

	public void AddCap()
	{
		_cap = true;
	}

    public List<Ingredient> IngredientInput { get { return _ingredients; } }

    public CoffeeOrder.CoffeeSize SizeInput { get { return _size; } }

}
