/* Customer Manager Class
 * Emmanuel Vinas
 * 
 * Controls customer spawning
 * uses timer to spawn a new customer
 * Keeps track of the active customers using List
 * Keep track of score and how well the player does. 
 * Instantiate Customer
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerManager : MonoBehaviour
{
	public static CustomerManager Instance { get; private set; }

	#region Inspector Attributes

	[SerializeField] float maxTip = 1f;
	[SerializeField] float minTip = 0.5f;

	[Space(2)][Header("Spawns")]
	[SerializeField] int maxSpawns = 3;
	[SerializeField] float maxSpawnTime = 10;
	[SerializeField] float minSpawnTime = 3;
	[SerializeField] GameObject customerPrefab;
	[SerializeField] Transform[] spawnPoints;

	[Space(2)][Header("Positions")]
	[SerializeField] Transform cashierPos;
	[SerializeField] Transform waitForOrderPos;
	[SerializeField] Transform linePosition;
	#endregion

	#region Hidden Attributes

	Queue<Customer> _activeCustomers;
	Customer _selectedCustomer = null;
	float _endTimer;
	int _totalAmtCustomers = 0;
	float _totalPoints = 0;

	#endregion

	void Awake ()
	{
		Instance = this;

		_activeCustomers = new Queue<Customer> ();

		if (waitForOrderPos == null) 
			waitForOrderPos = GameObject.FindGameObjectWithTag ("OrderWaitPosition").transform;
		
		if(cashierPos == null)
			cashierPos = GameObject.FindGameObjectWithTag ("CustomerWaitPosition").transform;
	}

	// Use this for initialization
	void Start ()
	{
		SpawnTimerSetUp ();
	}

	/// <summary>
	/// Create a new customer.
	/// </summary>
	public void AddCustomer ()
	{
		if (CoffeeGameManager.Instance.isPaused)
			return;
		
		if (_activeCustomers.Count >= maxSpawns) {
			SpawnTimerSetUp ();
			return;
		}

		if (!CoffeeGameManager.Instance.GameOver) 
		{
			Transform t = spawnPoints[Random.Range(0,100) % spawnPoints.Length];
			Customer c = Instantiate (customerPrefab, t.position, customerPrefab.transform.rotation, t).GetComponent<Customer> ();
			_activeCustomers.Enqueue (c);
			c.isInFront = c == _activeCustomers.Peek ();
			Vector3 pos;

			if (c.isInFront) {
				pos = cashierPos.position;
			} 
			else {
				pos = new Vector3(linePosition.position.x, linePosition.position.y, linePosition.position.z + (10 * (_activeCustomers.Count-1)));
			}

			c.SetEndPos (pos);
			StartCoroutine ("UpdateCoffeeUI");
			_totalAmtCustomers++;
			c.name = "Customer " + _totalAmtCustomers;
			c.NewCoffeeForMe ();
			SpawnTimerSetUp ();
		}
	}

	/// <summary>
	/// Remove a customer when time is up
	/// </summary>
	/// <param name="r">The red component.</param>
	public void RemoveCustomer (Customer r)
	{
		if (CoffeeGameManager.Instance.isPaused)
			return;
		
		if (_activeCustomers.Contains (r)) {
			_activeCustomers.Dequeue ().SetEndPos (spawnPoints [Random.Range (0, spawnPoints.Length)].position);
			StartCoroutine ("UpdateCoffeeUI");
			if (_activeCustomers.Peek () != null) {
				_activeCustomers.Peek ().SetEndPos (cashierPos.position);
				_activeCustomers.Peek ().isInFront = true;
				UpdateActiveCustomerLinePositions ();
			}
		} else {
			OrderManager.Instance.CurrentCustomer.SetEndPos (spawnPoints [Random.Range (0, spawnPoints.Length)].position);
			CoffeeMakerUI.Instance.CustomerLeaves ();
			OrderManager.Instance.RemoveOrder (r);
		}
	}

	/// <summary>
	/// set up Spawn timer.
	/// </summary>
	public void SpawnTimerSetUp ()
	{
		_endTimer = Random.Range (minSpawnTime, maxSpawnTime) + (_activeCustomers.Count >= maxSpawns ? (10 * _activeCustomers.Count) : 0);
		Invoke ("AddCustomer", _endTimer);
	}

	/// <summary>
	/// Select a customer.
	/// </summary>
	public void SelectCustomer ()
	{
		if (CoffeeGameManager.Instance.isPaused)
			return;
		
		if (_activeCustomers.Peek() == null || CoffeeGameManager.Instance.GameOver) return; 

		if (!_activeCustomers.Peek ().isDone) 
		{
			_selectedCustomer = _activeCustomers.Dequeue ();
			_selectedCustomer.TendToCustomer ();
			Vector3 t =  new Vector3 (waitForOrderPos.position.x + Random.Range(-10,10), waitForOrderPos.position.y, waitForOrderPos.position.z + Random.Range(-10,10));
			_selectedCustomer.SetEndPos (t);
			CoffeeMakerUI.Instance.SelectCustomerUI ();
			OrderManager.Instance.AddOrder (_selectedCustomer);

			if (_activeCustomers.Count > 0) 
			{
				_activeCustomers.Peek ().SetEndPos (cashierPos.position);
				_activeCustomers.Peek ().isInFront = true;
                UpdateActiveCustomerLinePositions();
				StartCoroutine ("UpdateCoffeeUI");
			}
		}
	}

	/// <summary>
	/// Sends the finish coffee to be checked.
	/// </summary>
	public void SendFinishCoffee ()
	{
		if (OrderManager.Instance.CurrentCustomer == null)
			return;
		
		float b = OrderManager.Instance.CurrentCustomer.CheckCoffee ();
		if (b < 0)
			return;
		
		CoffeeMakerUI.Instance.FinishCustomerUI (b/10);
		_totalPoints += b;
		_selectedCustomer = null;

		//change tip calculations 5/22/18
		CoffeeGameManager.Instance.AddTip (Random.Range (minTip, maxTip) * (b/10));
	}

    /// <summary>
    /// Called when a customer in the front moves for any reason.
    /// Go through the list and give them their new positions
    /// *** DEBUG: Check what the order of the array  is ****
    /// </summary>
    public void UpdateActiveCustomerLinePositions()
    {
        if (_activeCustomers.Count <= 1 || _activeCustomers == null) return;

		Vector3 t; 

		Customer[] cust = new Customer[_activeCustomers.Count];

		for (int i = 0; i < cust.Length; i++) {
			cust [i] = _activeCustomers.Dequeue ();
		}

		_activeCustomers.Clear ();
        for (int i = 0; i < cust.Length; i++)
        {
			if (!cust [i].isInFront) {
				t = linePosition.position;
				t += new Vector3 (0, 0, (10 * (i - 1)));
				cust [i].SetEndPos (t);
			}

			_activeCustomers.Enqueue (cust [i]);
        }

    }

	/// <summary>
	/// Updates the coffee UI.
	/// </summary>
	public IEnumerator UpdateCoffeeUI ()
	{
		while (_activeCustomers.Peek ().DesiredCoffee == null)
			yield return null;
	}
		
	/// <summary>
	/// Gets the waiting customers count.
	/// </summary>
	/// <value>waiting customers count.</value>
	public int WaitingCustomersCount 
	{ 
		get { return _activeCustomers.Count; } 
	}

	/// <summary>
	/// Gets the total amt customers.
	/// </summary>
	/// <value>The total amt customers.</value>
	public int TotalAmtCustomers
	{
		get { return _totalAmtCustomers; }
	}

	/// <summary>
	/// Gets the total perfect customers.
	/// </summary>
	/// <value>total perfect customers.</value>
	public float TotalPerfectCustomers 
	{
		get { return _totalPoints; } 
	}

	/// <summary>
	/// Gets the max spawn time.
	/// </summary>
	/// <value>max spawn time.</value>
	public float MaxSpawnTime
	{
		get { return maxSpawnTime; }
	}

	/// <summary>
	/// Gets the selected customer.
	/// </summary>
	/// <value>The selected customer.</value>
	public Customer SelectedCustomer 
	{
		get { return _selectedCustomer; }
	}
}

/** Old Code
 * 
 * 	[Range (1, 10)][SerializeField] int maxSpawnAmt = 3;
 * 
 * 	int _currIndex = 0;
 * 	float _screenWidth;

void Start()
{
		_screenWidth = (Camera.main.orthographicSize * 2) * Camera.main.aspect;
}

   public void SelectCustomer(int index)
    {
        /** OLD CODE
        if (index < _activeCustomers.Count && _activeCustomers[index] != null && !_activeCustomers[index].isDone)
        {
            CoffeeMakerUI.Instance.SelectCustomerUI(index);
            _selectedCustomer = _activeCustomers[index];
            _selectedCustomer.TendToCustomer();
        }
    }

	AddCustomer:
	if (!CoffeeGameManager.Instance.GameOver) {
		/** OLD CODE
        *if (_activeCustomers[_currIndex] == null)
        *{
        *    Customer c = Instantiate(customerPrefab, transform.position, transform.rotation, transform).GetComponent<Customer>();
        *    _activeCustomers[_currIndex] = c;
        *    c.SetEndPos((1 + _currIndex) * (_screenWidth / maxSpawnAmt));
        *    c.ID = _currIndex;
        *    _currIndex = (_currIndex + 1) % maxSpawnAmt;
        *    StartCoroutine("UpdateCoffeeUI", c.ID);
        *    _totalAmtCustomers++;
        *}
        **/