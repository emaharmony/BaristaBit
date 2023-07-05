/* CoffeeGameManager
 * Emmanuel Vinas
 * 
 * Controls when game ends, the score, and the recording of players progress. 
 * Will send recorded information to Database.
 * Rate the player at the end. 
 * If there is a selected Customer is not done, then wait till its done to end the game
 * At end of game show player how they did
 * Base score on how many can be done within Time limit(totalTime/(customer limit + Human error)) 
 * Game cannot customize order so just pass or fail, so score will be based on if you passed a threshold + correct/totalCustomers
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoffeeGameManager : MonoBehaviour
{
	public static CoffeeGameManager Instance { get; private set; }

	#region visible
	[Range (60, 3600)]
	[SerializeField] float totalGameTime = 600f;
	//Total Time of game in seconds
	[Tooltip ("The amount of time it takes a human player to solve a coffee order")]
	[SerializeField] float humanError = 30f;
	//Human Error in seconds
	[SerializeField] Logic l = null;

	[Space (2)]
	[Header ("Camera Stuff")]
	[SerializeField]
	GameObject mainCam = null;
	[SerializeField] Transform[] camPos = null;
	[SerializeField] float camMoveSpeed = 3;

	#endregion

	#region hidden
	float _score;
	//Overall SCore
	CustomerManager _cm;
	//Customer Manager Ref.
	CoffeeMakerUI _cUI;
	//CoffeeMakerUI ref.
	float _gameTime;
	//Gametime
	int _threshHold;
	//threshold check
	bool _gameover = false, _paused = false;
	float _moneyEarned;
	int _camIndex = 0;
	const float CUSTOMER_TIMER = 15f;
	#endregion

	void Awake ()
	{
		Instance = this;
	}

	void Start ()
	{
		_cm = CustomerManager.Instance;
		_cUI = CoffeeMakerUI.Instance;
		_score = 0;
		_gameTime = 0;
		_threshHold = Mathf.FloorToInt ((totalGameTime / (CUSTOMER_TIMER + humanError + _cm.MaxSpawnTime)));
		l.setNotes = "Start Job";
		_moneyEarned = 0;

	}

	void Update ()
	{
		if (_gameover || _paused)
			return;
		
		_gameTime += Time.deltaTime;
		CoffeeMakerUI.Instance.UpdateGameTime (totalGameTime - _gameTime);

		if (_gameTime >= totalGameTime) {
			FinishGame ();
		}

		if (Input.GetKeyDown (KeyCode.C)) {
			CameraTransition ();
		}
	}

	public void FinishGame ()
	{
		_gameover = true;

		if (_cm.SelectedCustomer != null) {
			StartCoroutine (WaitForCustomer ());
		} else {
			ShowEndScreen ();
		}
	}

	void ShowEndScreen ()
	{
		_score = CalculateScore ();
		_cUI.EndGameScreen (_score, _moneyEarned);
		l.setNotes = "Money: " + _moneyEarned + "\nScore: " + _score + "\nPoints Ratio: " + _cm.TotalPerfectCustomers + "/" + (_cm.TotalAmtCustomers * 10);
	}

	IEnumerator WaitForCustomer ()
	{
		while (OrderManager.Instance.OrdersActive > 0) {
			yield return null;
		}

		ShowEndScreen ();
	}

	float CalculateScore ()
	{
		float p = ((float)_cm.TotalPerfectCustomers / (float)_cm.TotalAmtCustomers);
		//_moneyEarned += dayOfPay * p; 
		return (_cm.TotalPerfectCustomers < _threshHold) ? 0.00f : (p * 100.0f);
	}

	public bool GameOver {
		get { return _gameover; }
	}

	public void AddTip (float tip)
	{
		_moneyEarned += tip;
		CoffeeMakerUI.Instance.UpdateTip (tip, _moneyEarned);
	}

	public void ResetGame ()
	{
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}

	public void QuitGame(){
	
		Application.Quit ();
	}
	public void CameraTransition ()
	{
		_camIndex = (_camIndex + 1) % camPos.Length;
		CoffeeCameraController.Instance.TogglePrepPosition ();
		StartCoroutine (CamTransitionAux (camPos [_camIndex]));
		_cUI.CamTransition ();
	}

	IEnumerator CamTransitionAux (Transform newPos)
	{
		Vector3 originalPos = mainCam.transform.position;
		float value = 0;
		while (transform.position != newPos.position) {
			value += Time.deltaTime * camMoveSpeed;
			mainCam.transform.position = Vector3.Lerp (originalPos, newPos.position, value);
			if (value >= 1) {
				mainCam.transform.position = newPos.position;
				mainCam.transform.rotation = newPos.rotation;
				break;
			}

			yield return null;
		}

		yield return null;
	}

	public bool isPaused
	{
		get{ return _paused;}
		set{_paused = value;} 
	}
}

#region Old Code
/*
[SerializeField] float dayOfPay = 50f;                            //a day of pay for the work
*/
#endregion
