/* Customer Class
 * Emmanuel Vinas
 * 
 * Contains Info about an instated customer. 
 * Randomly selects a coffee and is assigned to _desire when instantiated. 
 * Timer that decides when customer leaves
 * Changes Sprite on start
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(SpriteRenderer))]
public class Customer : MonoBehaviour
{

	#region Inspector Attributes
	[SerializeField] float endTime = 10f;
	[SerializeField] GameObject[] spawnPrefabs = null;

	[Space (2)]
	[Header ("Animation(Old)")]
	[SerializeField] float animSpeed = 2f;
	[SerializeField] float amplitude = 0;
	[Range (1, 100)][SerializeField] float frequency = 1;

	[Space (2)]
	[Header ("Mood")]
	[SerializeField] float moodDecreaseRate = 5.0f;
	[SerializeField] float moodDecreaseTime = 3;

	Drink _input = null;
	#endregion

	#region HIdden Attributes
	CoffeeOrder _desire;	//Coffee Order that Customer wants
	Vector3 _startPos;
	Transform player;
	float _endPos;
	string _customerName, _drinkName;
	bool _done = false, _tending = false, _frontOfLine = false, _timerStart = false;
	float _mood = 100.0f, _moodTimer = 0, _waitTimerforUI = 0;
	string[] names = {"Rose", "Rebecca","Regla","Sarah","Susan", "Amy", "Nathalie", "WakandaFoeva", "Kiki"};
	NavMeshAgent agent;
	Animator animate;
	#endregion

	void Awake ()
	{
		_startPos = transform.position;
		_customerName = names [Random.Range (0, names.Length)];
		agent = GetComponent<NavMeshAgent> ();
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		GameObject x = Instantiate (spawnPrefabs [Random.Range (0, spawnPrefabs.Length)], transform);
		x.transform.localScale = new Vector3 (11, 11, 11);
		x.transform.localRotation = Quaternion.Euler (new Vector3 (0, 0, 0));
	}

	// Use this for initialization
	void Start ()
	{
		_desire = new CoffeeOrder ();
		_waitTimerforUI = endTime;
		CoffeeMakerUI.Instance.NewCustomerUI (_customerName ,_desire.Name);
		animate = GetComponentInChildren<Animator> ();
	}

	void FixedUpdate()
	{
		if (animate == null || CoffeeGameManager.Instance.isPaused)
			return;
		
		animate.SetFloat ("Forward", agent.velocity.magnitude);
	}

	void Update ()
	{
		agent.isStopped = CoffeeGameManager.Instance.isPaused;

		if (CoffeeGameManager.Instance.isPaused) 
			return;

		if (_timerStart) 
		{
			CoffeeMakerUI.Instance.TimerChange (_waitTimerforUI / endTime);
			_waitTimerforUI -= Time.deltaTime;
		}

		if (!_done && _tending) 
		{
			if (_moodTimer >= moodDecreaseTime) 
			{
				_mood -= moodDecreaseRate;
				_moodTimer = 0;
				if (this == OrderManager.Instance.CurrentCustomer) {
					CoffeeMakerUI.Instance.OrderUI (this);
				}
				if (_mood <= 0)
					TimesUp ();
			}
			_moodTimer += Time.deltaTime;
		}
			
		if (agent.velocity.sqrMagnitude < 1) {
			transform.LookAt (new Vector3(player.position.x, transform.position.y, player.position.z));
			return;
		}

	}

	/// <summary>
	/// Destroies the customer.
	/// </summary>
	void DestroyCustomer ()
	{
		CancelInvoke ();
		Destroy (gameObject);
	}

	/// <summary>
	/// Sets the end position of the customer.
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	public void SetEndPos (Vector3 x)
	{
		agent.SetDestination(x);
	}

	/// <summary>
	/// Tends to customer.
	/// </summary>
	public void TendToCustomer ()
	{
		if (CoffeeGameManager.Instance.isPaused)
			return;
		
		CancelInvoke ();
		_tending = true; 
		_mood *= (_waitTimerforUI / endTime);
	}

	/// <summary>
	/// Time's up, customer leaves
	/// </summary>
	void TimesUp ()
	{
		if (!_done) 
		{
			_done = true;
			_tending = false;	
			CustomerManager.Instance.RemoveCustomer (this);
			Invoke ("DestroyCustomer", 3.5f);
		}
	}

	/// <summary>
	/// Checks to see if inputed coffee matches the desired coffee
	/// </summary>
	/// <returns>-5 = if list is empty || # of mistakes</returns>
	public float CheckCoffee ()
	{
		if (_input.IngredientInput.Count <= 0)
			return -5 ;
		
		_done = true;
		CustomerManager.Instance.RemoveCustomer (this);
		Invoke ("DestroyCustomer", 3.5f);
		return _desire.IsCoffeeCorrect (_input);
	}

	/// <summary>
	/// New input Drink Object is created. Auto: Medium
	/// </summary>
	public void NewCoffeeForMe()
	{
		_input = null;
	}

	/// <summary>
	/// New input Drink Object is created
	/// </summary>
	/// <param name="s">Size of the drink</param>
	public void NewCoffeeForMe(CoffeeOrder.CoffeeSize s)
	{
		_input = new Drink (s);
	}

	/// <summary>
	/// Adds ingredients to input drink
	/// </summary>
	/// <param name="i">ingredient to add</param>
	public void AddToInput(Ingredient i)
	{
		_input.AddIngredient (i);
	}

	public void AddSleeve()
	{
		_input.AddSleeve ();
	}

	public void AddCap()
	{
		_input.AddCap ();
	}

	/// <summary>
	/// Gets the desired coffee.
	/// </summary>
	/// <value>The desired coffee.</value>
	public CoffeeOrder DesiredCoffee {
		get { return _desire; }
	}

	/// <summary>
	/// Gets a value indicating whether this <see cref="Customer"/> is done.
	/// </summary>
	/// <value><c>true</c> if is done; otherwise, <c>false</c>.</value>
	public bool isDone 
	{
		get { return _done; }
	}

	public float Mood
	{
		get{ return _mood; }
	}

	public string myName
	{
		get{ return _customerName;}
	}
	/// <summary>
	/// Gets the input drink.
	/// </summary>
	/// <value>The input.</value>
	public Drink Input
	{
		get{ return _input; }
	}

	public void StartTimer()
	{
		_timerStart = _frontOfLine && !_tending;
	}
	/// <summary>
	/// Sets a value indicating whether this <see cref="Customer"/> is in front.
	/// </summary>
	/// <value><c>true</c> if is in front; otherwise, <c>false</c>.</value>
	public bool isInFront
	{
		get{ return _frontOfLine; }
		set { _frontOfLine = value; if(value) Invoke ("TimesUp", endTime);}
	}

}

#region Old Code
/*Update
//if (transform.localPosition.x >= _endPos && !_done) return;
//else { 
//    _startPos += transform.right * Time.deltaTime * animSpeed;
//    transform.localPosition = _startPos + transform.up * Mathf.Sin(Time.time * frequency) * amplitude;
//}

public void SetEndPos (float x)
{
	_endPos = x;
}*/
#endregion