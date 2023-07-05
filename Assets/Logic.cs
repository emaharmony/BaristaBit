using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Logic : MonoBehaviour 
{
    public static Logic Instance { get; private set; }

	//private WWW www;
	string wwwText = "";
	string wwwError = "";
	private string userAccount;
	bool isServerRunning;

	[SerializeField]
	string pid;
	[SerializeField]
	int level_id;
	[SerializeField]
	Time currentTime;
	[SerializeField]
	private int score = 0;
	[SerializeField]
	private string notes = "";

	string URL = "localhost";

     void Awake()
    {
        Instance = this;   
    }
    void Start () 
	{
		constructor();
	}

	void constructor()
	{
		pid = "";
	/*	for(int i = 0; i<7;i++)
			pid+=Random.Range(1,9).ToString();*/
		level_id = 1;
		score = 0;
		notes = "";
		StartCoroutine(WaitForAPI(URL));
	}

	public void login(string username,string password)
	{


	}


	public void SaveScore() 
	{
		Dictionary<string, string> postData =  new Dictionary<string, string>();

		postData.Add("PID",pid);
		postData.Add("Level_id",level_id.ToString());
		postData.Add("Actions",notes);

		//API ("https://radiusgame.fiu.edu/api/scores/", false, postData);
		WWW www =null ;
		API (ref www, URL, false, postData);
		StartCoroutine(WaitForAPI(www));


	}

	public void API(ref WWW www,string request, bool method, Dictionary<string, string> postData) 
	{
		// Reset any previous responses
		wwwText = "";
		wwwError = "";
		// GET
		if (method) 
		{
			www = new WWW(request);
		}
		// POST
		else 
		{
			WWWForm wwwForm = new WWWForm();
			foreach(string key in postData.Keys) 
			{
				wwwForm.AddField(key, postData[key]);
			}
			www = new WWW(request, wwwForm);
		}
	}

	public IEnumerator WaitForAPI(WWW www) 
	{
		yield return www;
		if (!string.IsNullOrEmpty(www.error)) 
		{
			wwwError = www.error.Trim();
			Debug.LogError("failed to connect to server: "+wwwError);
			//print("error: "+www.error);
		}
		else {
			wwwText = www.text.Trim();
			print("inside: "+wwwText);
		}
	}

	public IEnumerator WaitForAPI(string request) 
	{
		WWW www2 = new WWW(request);

		yield return www2;
		if (!string.IsNullOrEmpty(www2.error)) 
		{
			wwwError = www2.error.Trim();
			Debug.LogError("failed to connect to server:1 "+wwwError);
			isServerRunning = false;
		}
		else 
		{
			wwwText = www2.text.Trim();
			isServerRunning = true;
		}

		if(string.Compare( wwwText,"Failed")==0 )
			print(":Connection Established:");
		else
			Debug.LogError("failed to connect to server:2 "+wwwText);
	}

	public string account
	{
		get{return userAccount;}
		set{userAccount = value.Trim();}
	}

	public int IncScore (int s) 
	{
		return score += s;
	}

	public int setScore 
	{
		get{return score;}
		set{score += value;}
	}
	int i = 0;
	public string setNotes 
	{
		get{return notes;}
		set{
				System.DateTime a = System.DateTime.Now;
				i++;
				notes = notes+"\n"+i+". "+value.Trim()+" : \t"+a.Date.Month+"/"+a.Date.Day+"/"+a.Date.Year+", "+a.Hour+":"+a.Minute;
			}
	}


	void OnDisable()
	{

		if(!string.IsNullOrEmpty(notes) && isServerRunning)
		{
			print("saving");
			SaveScore();
		}
	}
	/*public float setElapseTime
	{
		get{return userTime;}
		set{userTime = value;}
	}

	public int setGameID
	{
		get{return gameID;}
		set{gameID = value;}
	}*/




}
