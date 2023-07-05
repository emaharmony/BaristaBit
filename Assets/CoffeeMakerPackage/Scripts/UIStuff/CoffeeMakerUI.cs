/* CoffeeMakerUI
 * Emmanuel Vinas
 * 
 * Controls the UI functionality and changes for the entire coffee game.
 * using index from CustomerManger it will adjust the position panels and the text 
 * Shows the controls and the recipes 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoffeeMakerUI : MonoBehaviour
{

	public static CoffeeMakerUI Instance{ get; private set;}
	const int NUMBER_OF_INGREDIENTS = 7;

	#region Inspector
	[Header("General")]
	[SerializeField]TextMeshProUGUI tip;
	[SerializeField]TextMeshProUGUI tipAdded;
	[SerializeField]TextMeshProUGUI gameTime;

	[Space(2)][Header ("Prep Screen")]
	[SerializeField] CanvasGroup[] prepUI;
	[SerializeField] GameObject ingredientsPanel;   //ingredient panel
	[SerializeField] TextMeshProUGUI ingredientAdded;
	[SerializeField] TextMeshProUGUI currOrderName;
	[SerializeField] TextMeshProUGUI currCustomerName;
    [SerializeField] TextMeshProUGUI ingredientNameHover;
	[SerializeField] ParticleSystem particleSys;
	[SerializeField] Material[] particleMats;
	[SerializeField] CanvasGroup ingredentButtons;
	[SerializeField] CanvasGroup cupSizeButton;

	[Space (2)][Header ("Customer Screen")]
	[SerializeField] CanvasGroup[] customerUI;
	[SerializeField] Image orderPanels;
	[SerializeField] Image timerCircle;
	[SerializeField] Image moodFaceHolder; 
	[SerializeField] Sprite[] moodFaces;

	//Panels containing position
	[Space (2)][Header ("End Game Screen")]
	[SerializeField] Text score;
	[SerializeField] Text rank;
	[SerializeField] Text customerRatio;
	[SerializeField] Text money;

	[Space (2)][Header ("Coffee Visuals")]
	[SerializeField] Color[] ingredentColors = new Color[NUMBER_OF_INGREDIENTS];
	[SerializeField] CoffeeVisual visualCoffee;
	#endregion

	#region Hidden
	Animation tipAnimation;
	CanvasGroup endGamePanel;
	CanvasGroup _inText;
	bool _isPrepUI = false;
	GameObject es;

	#endregion

	void Awake ()
	{
		Instance = this;
		endGamePanel = score.transform.parent.GetComponent<CanvasGroup>();
		_inText = ingredientAdded.GetComponent<CanvasGroup> ();
		tipAnimation = tipAdded.GetComponent<Animation> ();
		es = GameObject.Find ("EventSystem");
	}

	// Use this for initialization
	void Start ()
	{
		endGamePanel.alpha = 0;
		endGamePanel.blocksRaycasts = endGamePanel.interactable = false;
        ingredientNameHover.text = "";

		foreach(CanvasGroup cg in prepUI){

			cg.alpha = 0;
			cg.blocksRaycasts = cg.interactable = false;
		}
			
		currCustomerName.text = "";
		currOrderName.text = "NO ORDERS";

		_isPrepUI = false;

		/** OLD CODE
         * for (int i = 0; i < positionText.Length; i++) {
                positionText[i] = orderPanels[i].GetComponentInChildren<Text>();
                positionText[i].text = "None";
                orderPanels[i].color = Color.red;
            }
                      */
	}

	void Update()
	{ 
		if (CoffeeMaker.Instance.CurrentCustomer == null) {
			cupSizeButton.interactable = ingredentButtons.interactable = false;
			return;
		}
		cupSizeButton.interactable = CoffeeMaker.Instance.CurrentCustomer.Input == null;
		ingredentButtons.interactable = !cupSizeButton.interactable;
	}

	//Called when a new customer enters the cafe
	public void NewCustomerUI (string custName, string drinkName)
	{
		if (CoffeeGameManager.Instance.isPaused)
			return;
		Dialogue d = new Dialogue (custName, drinkName);
		CoffeeDialogueManager.Instance.AddOrder (d);
	}

	//called when customer is selected
	public void SelectCustomerUI ()
	{
		if (CoffeeGameManager.Instance.isPaused)
			return;
		CoffeeDialogueManager.Instance.EndConversation ();
	}

	//Shows the reciepe page
	public void ReciepePage ()
	{
		if (CoffeeGameManager.Instance.isPaused)
			return;
		ingredientsPanel.SetActive (false);
	}

	//Shows the controls page
	public void IngredientPage ()
	{
		if (CoffeeGameManager.Instance.isPaused)
			return;
		ingredientsPanel.SetActive (true);
	}

	//Called when a customer is done and indicates if player finished with a customer.
	public void FinishCustomerUI (float r)
	{
		ParticleSystemRenderer rend = particleSys.GetComponent<ParticleSystemRenderer> ();
		if (r < 0.5f) {
			rend.material = particleMats [2];
		} else if (r < 0.75f) {
			rend.material = particleMats [1];
		} else {
			rend.material = particleMats [0];
		}

		particleSys.Play ();
		visualCoffee.EmptyCup ();
	}

	//called when a customer is done waiting.
	public void CustomerLeaves ()
	{
		if (CoffeeGameManager.Instance.isPaused)
			return;
		if	 (CustomerManager.Instance.WaitingCustomersCount <= 0) {
			orderPanels.gameObject.SetActive (false);
		} else {
			OrderManager.Instance.SetCurrentOrder (0);
		}
	}

	public void OrderUI (string cust)
	{
		if (CoffeeGameManager.Instance.isPaused)
			return;
		currOrderName.text = cust;
		currCustomerName.text = "";
	}

	public void OrderUI (Customer cust)
	{
		if (CoffeeGameManager.Instance.isPaused)
			return;
		currOrderName.text = cust.DesiredCoffee.Name;
		currCustomerName.text = cust.myName;

		if (cust.Mood > 66) {
			moodFaceHolder.sprite = moodFaces [0];
		} else if (cust.Mood > 33) {

			moodFaceHolder.sprite = moodFaces [1];
		} else {
			moodFaceHolder.sprite = moodFaces [2];
		}

	}

	public void EndGameScreen (float s, float m)
	{
		endGamePanel.alpha = 1;
		endGamePanel.blocksRaycasts = endGamePanel.interactable = true;
		score.text = (s > 0) ? s.ToString ("#.##") : "0.00";
		money.text = (m > 0) ? "$" + m.ToString ("#.##") : "$0.00";
		customerRatio.text = CustomerManager.Instance.TotalPerfectCustomers + "/" + CustomerManager.Instance.TotalAmtCustomers;
		rank.text = (s > 89 ? "A" : (s > 79 ? "B" : (s > 69 ? "C" : (s > 59 ? "D" : "F"))));
	}

	public void UpdateTip(float tipEarned, float tipAmt)
	{
		tipAdded.text = "$" + tipEarned.ToString ("F2");
		tip.text = "$" + tipAmt.ToString("F2");
		tipAnimation.Play ();
	}

	public void AddIngredient (string s)
	{
		ingredientAdded.text = s;
		StartCoroutine (TextMagic ());
	}

	public void CamTransition ()
	{

		if (_isPrepUI) {
			foreach (CanvasGroup cg in prepUI) {
				cg.alpha = 0;
				cg.blocksRaycasts = cg.interactable = false;
			}

			foreach (CanvasGroup cg in customerUI) {
				cg.alpha = 1;
				cg.blocksRaycasts = cg.interactable = true;
			}
		} else {
			foreach (CanvasGroup cg in prepUI) {
				cg.alpha = 1;
				cg.blocksRaycasts = cg.interactable = true;
			}

			foreach (CanvasGroup cg in customerUI) {
				cg.alpha = 0;
				cg.blocksRaycasts = cg.interactable = false;
			}
		}

		_isPrepUI = !_isPrepUI;
	}

    public void IngredientHoverName(string str)
    {
        ingredientNameHover.text = str;
    }

	public void UpdateGameTime(float time)
	{
		//00:00:00
		int hours, mins, seconds;
		hours = (int)(time / 3600);
		time -= hours * 3600;
		mins = (int)(time / 60);
		time -= 60 * mins;
		seconds = (int)time;
		gameTime.text = hours.ToString("D2") + ":" + mins.ToString("D2") + ":" + seconds.ToString("D2");
	}

	public void CoffeeThrownAway()
	{
		visualCoffee.EmptyCup();
	}

	public void CoffeeFillSection(int i)
	{
		visualCoffee.FillSection (ingredentColors [i]);

	}
		
	public void TimerChange(float ratio)
	{
		timerCircle.fillAmount = ratio;
	}

	public void UpdateCupForCurrentCustomer()
	{
		Customer c = OrderManager.Instance.CurrentCustomer;
		if (c == null ||  c.Input==null)
			return;
		
		visualCoffee.EmptyCup ();
		for (int i = 0; i < c.Input.IngredientInput.Count; i++) {
			CoffeeFillSection ((int)c.Input.IngredientInput [i].Type);
		}
	}
	IEnumerator TextMagic ()
	{
		_inText.alpha = 0;

		while (_inText.alpha < 1) {
			_inText.alpha += Time.deltaTime;
			yield return null;
		}

		yield return new WaitForSeconds (0.25f);
		_inText.alpha = 0;
	}

	public bool EventSystemEnabled
	{
		set{ es.SetActive (value);}
	}

}
	
/** OLD CODE
    //called when customer is selected
    public void SelectCustomerUI(int ind)
    {
        //orderPanels[ind].color = Color.green;
        ingredientsPanel.SetActive(true);
    }
    */