using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }
	List<Customer> _listOfOrders;
    int _currentOrder = 0;

    void Awake()
    {
        Instance = this;
		_listOfOrders = new List<Customer> ();
    }

	public void AddOrder(Customer order)
    {
        _listOfOrders.Add(order);
		_listOfOrders [_listOfOrders.Count - 1].NewCoffeeForMe ();

		if (_listOfOrders.Count < 2) 
		{
			ChangeCurrentOrder (0);
		}

    }

	public void RemoveOrder(Customer order)
    {
		if (_listOfOrders.Contains (order)) {
			_listOfOrders.Remove (order);
			ChangeCurrentOrder (1);
		}
    }

	public void SetCurrentOrder(int index)
	{
		_currentOrder = index;
	}

    public void ChangeCurrentOrder(int dir)
    {
        if (_listOfOrders.Count != 0)
        {
            _currentOrder += dir;
            _currentOrder = (_currentOrder < 0) ? _listOfOrders.Count - 1 : _currentOrder % _listOfOrders.Count;
			CoffeeMakerUI.Instance.OrderUI(_listOfOrders[_currentOrder]);
			CoffeeMaker.Instance.CurrentCustomer = _listOfOrders [_currentOrder];
			CoffeeMakerUI.Instance.UpdateCupForCurrentCustomer ();
        }
        else
        {
            CoffeeMakerUI.Instance.OrderUI("NO ORDERS");
        }
    }

	public Customer CurrentCustomer
	{
		get{ 
			if (_listOfOrders.Count <= 0)
				return null;
			return _listOfOrders [_currentOrder]; }
	}

	public int OrdersActive
	{
		get{ return _listOfOrders.Count; }
	}

}
