﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static BookTicketDetails;

public class GameManager : MonoBehaviour
{

	public static GameManager Instance;

	[HideInInspector] public int selectedCoinAmt;

	[SerializeField] private TextMeshProUGUI balanceText;

	[SerializeField] private string drawDetailsLink;
	[SerializeField] private string bookTicketLink;
	[SerializeField] private TextMeshProUGUI userId, pointsText;
	[SerializeField] private TextMeshProUGUI gameIdText;
	[SerializeField] private NumberButton[] numberButtons;
	[SerializeField] private TextMeshProUGUI[] historyResultsText;
	[SerializeField] private TextMeshProUGUI playText;
	[SerializeField] private TextMeshProUGUI winPopupText;
	[SerializeField] private TextMeshProUGUI winText;

	public Transform BetGridT;
	private int balance;
	private int originalBalance;

	[SerializeField] private MainData mainData;
	[SerializeField] Timer timer;
	[SerializeField] private GameObject backPopUp;
	public UnityEvent OnWin;
	private int totalPointsSpent;

	public int TotalPointsSpent
	{
		get { return totalPointsSpent; }
		set 
		{ 
			totalPointsSpent = value;
			playText.SetText(totalPointsSpent.ToString());
		}
	}


	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(this);
		}

		Application.runInBackground = true;
	}

	public void DisableNumberButtons()
	{
		foreach (var item in numberButtons)
		{
			item.SwitchButtonInteraction(false);
		}
	}

	public void EnableNumberButtons()
	{
		foreach (var item in numberButtons)
		{
			item.SwitchButtonInteraction(true);
		}
	}

	public void RestartGame()
	{
		Invoke("SendPendingDrawDetails", 4f);
		Invoke("GetLastFewDrawDetails", 4f);
		Invoke("OnPressedClear", 4f);
	}

	// Start is called before the first frame update
	void Start()
	{
		userId.SetText(mainData.receivedLoginData.UserID);
		balance = int.Parse(mainData.receivedLoginData.Balance);
		selectedCoinAmt = 2;
		originalBalance = balance;
		ShowBalance();
		GetLastFewDrawDetails();
		SendPendingDrawDetails();
	}
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && !backPopUp.activeInHierarchy)
		{
			backPopUp.SetActive(true);
		}
	}
	private void SendPendingDrawDetails()
	{
		StartCoroutine(SendPendingDrawDetailsCoroutine());
	}

	IEnumerator SendPendingDrawDetailsCoroutine()
	{
		var drawDetails = new SentDrawDetails("01", "1", "P");

		var drawDetailsJson = JsonUtility.ToJson(drawDetails);
		print(drawDetailsJson);
		UnityWebRequest www = UnityWebRequest.Post(drawDetailsLink, drawDetailsJson);
		byte[] bodyRaw = Encoding.UTF8.GetBytes(drawDetailsJson);
		www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
		www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
		www.SetRequestHeader("Content-Type", "application/json");
		yield return www.SendWebRequest();
		www.uploadHandler.Dispose();

		if (www.isNetworkError || www.isHttpError)
		{
			Debug.Log(www.error);
		}
		else
		{
			Debug.Log(www.downloadHandler.text);
			mainData.pendingDrawDetails = JsonUtility.FromJson<PendingDrawDetails>(www.downloadHandler.text);
			www.downloadHandler.Dispose();
            //gameIdText.text = mainData.pendingDrawDetails.Draws[0].GID;
			

           ProcessTimer();
		}

		www.Dispose();
	}

	void ProcessTimer()
	{
        //var timeOfDay = Convert.ToDateTime(mainData.pendingDrawDetails.Now).TimeOfDay;
        //var drawTime = Convert.ToDateTime(mainData.pendingDrawDetails.Draws[0].DrawTime).TimeOfDay;
        var timeOfDay = Convert.ToDateTime(DateTime.Now).TimeOfDay;
        var drawTime = Convert.ToDateTime(DateTime.Now.AddSeconds(30)).TimeOfDay;
		
		timer.SetCurrentDrawTime(drawTime);		
		//var remainingTime = drawTime.Subtract(timeOfDay);
		//print(remainingTime);
		//timer.RunTimer((float)remainingTime.TotalSeconds);
		timer.RunTimer(30);
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus)
		{
			SendPendingDrawDetails();
		}
	}

	public void GetCurrentDrawDetailsResult()
	{
		StartCoroutine(GetCurrentDrawDetailsResultCoroutine());
	}
//mainData.receivedLoginData.UserID,,"Y");
	IEnumerator GetCurrentDrawDetailsResultCoroutine()
	{
		var drawDetails = new SendDrawDetailsCurrent(
			"04","D", "5");

		var drawDetailsJson = JsonUtility.ToJson(drawDetails);
		print(drawDetailsJson);
		UnityWebRequest www = UnityWebRequest.Post(drawDetailsLink, drawDetailsJson);
		byte[] bodyRaw = Encoding.UTF8.GetBytes(drawDetailsJson);
		www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
		www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
		www.SetRequestHeader("Content-Type", "application/json");
		yield return www.SendWebRequest();
		www.uploadHandler.Dispose();

		if (www.isNetworkError || www.isHttpError)
		{
			Debug.Log(www.error);
		}
		else
		{
			Debug.Log(www.downloadHandler.text);
			mainData.currentDrawDetails = JsonUtility.FromJson<CurrentDrawDetails>(www.downloadHandler.text);
			www.downloadHandler.Dispose();

			var result = int.Parse(mainData.currentDrawDetails.Draws[0].Result);
			var multiplier = int.Parse(mainData.currentDrawDetails.Draws[0].XF[0].ToString());

			bool win = int.Parse(mainData.currentDrawDetails.Draws[0].TotWin) > 0;

			SpinWheel.Instance.SetupResults(result, multiplier, win, () =>
			{
				OnWin?.Invoke();
				balance = originalBalance = int.Parse(mainData.currentDrawDetails.Balance);
				winText.text = winPopupText.text = int.Parse(mainData.currentDrawDetails.Draws[0].TotWin).ToString();
			});
		}
	}

	public void GetLastFewDrawDetails()
	{
		StartCoroutine(GetLastFewDrawDetailsCoroutine());
	}

	IEnumerator GetLastFewDrawDetailsCoroutine()
	{
		var drawDetails = new SentDrawDetails("01", "10", "D");

		var drawDetailsJson = JsonUtility.ToJson(drawDetails);
		print(drawDetailsJson);
		UnityWebRequest www = UnityWebRequest.Post(drawDetailsLink, drawDetailsJson);
		byte[] bodyRaw = Encoding.UTF8.GetBytes(drawDetailsJson);
		www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
		www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
		www.SetRequestHeader("Content-Type", "application/json");
		yield return www.SendWebRequest();
		www.uploadHandler.Dispose();

		if (www.isNetworkError || www.isHttpError)
		{
			Debug.Log(www.error);
		}
		else
		{
			Debug.Log(www.downloadHandler.text);
			mainData.lastFewDrawDetails = JsonUtility.FromJson<CurrentDrawDetails>(www.downloadHandler.text);
			www.downloadHandler.Dispose();

			for (int i = 0; i < mainData.lastFewDrawDetails.Draws.Length; i++)
			{
				if (i == 0)
				{
					timer.SetLastDrawTime(mainData.lastFewDrawDetails.Draws[i].DrawTime);
				}

				historyResultsText[i].text = mainData.lastFewDrawDetails.Draws[i].Result[1].ToString();
				string multiplier = mainData.lastFewDrawDetails.Draws[i].XF;

				if (multiplier.Equals("1X") == false)
				{
					historyResultsText[i].text += "\n" + multiplier;
				}
			}
		}
	}

	public void GoBackToLobby()
	{
		SceneManager.LoadScene("LobbyScene");
	}

	public bool DebitBalance(int amount)
	{
		if (balance - amount >= 0)
		{
			balance -= amount;
			ShowBalance();
			return true;
		}
		else
		{
			return false;
		}
	}

	public void ShowBalance()
	{
		balanceText.SetText(balance.ToString());
		pointsText.SetText(balance.ToString());
	}

	public void SelectCoinType(int coinAmt)
	{
		selectedCoinAmt = coinAmt;
	}

	public void OnPressedDouble()
	{
		for (int i = 0; i < BetGridT.childCount; i++)
		{
			BetGridT.GetChild(i).GetComponent<NumberButton>().DoubleAmount();
		}
	}

	public void OnPressedClear()
	{
		for (int i = 0; i < BetGridT.childCount; i++)
		{
			BetGridT.GetChild(i).GetComponent<NumberButton>().Clear();
		}

		balance = originalBalance;
		ShowBalance();
		TotalPointsSpent = 0;
	}

	public void OnPressedOdds()
	{
		for (int i = 0; i < BetGridT.childCount; i += 2)
		{
			BetGridT.GetChild(i).GetComponent<NumberButton>().OnPressedButton(selectedCoinAmt);
		}
	}

	public void OnPressedEven()
	{
		for (int i = 1; i < BetGridT.childCount; i += 2)
		{
			BetGridT.GetChild(i).GetComponent<NumberButton>().OnPressedButton(selectedCoinAmt);
		}

	}

	public void OnPressedRepeat()
	{
		for (int i = 0; i < BetGridT.childCount; i++)
		{
			var numberButton = BetGridT.GetChild(i).GetComponent<NumberButton>();
			numberButton.OnPressedButton(numberButton.LastStoredAmount);
		}
	}

	[ContextMenu("Book Ticket")]
	public void BookTicket()
	{
		StartCoroutine(BookTicketCoroutine());
	}

	IEnumerator BookTicketCoroutine()
{
    Dictionary<string, string> cardMapping = new Dictionary<string, string>
    {
        { "0", "JH" }, { "01", "JC" }, { "02", "JD" }, { "03", "JS" },
        { "04", "QH" }, { "05", "QC" }, { "06", "QD" }, { "07", "QS" },
        { "08", "KH" }, { "09", "KC" }, { "10", "KD" }, { "11", "KS" }
    };

    List<BookTicketDetails.BetsDetails> betsDetails = new List<BookTicketDetails.BetsDetails>();

    foreach (var item in numberButtons)
    {
        if (item.Amount != 0)
        {
            string cardValue = cardMapping.ContainsKey(item.Number) ? cardMapping[item.Number] : item.Number;
            betsDetails.Add(new BookTicketDetails.BetsDetails(cardValue, item.Amount.ToString(), "0"));
        }
    }

    string lastDrawGID = PlayerPrefs.GetString("LastDrawGID", "No Draw GID Found");
    var bookTicketDetails = new BookTicketDetails(
        mainData.pendingDrawDetails.GameID,
        lastDrawGID,
        betsDetails.ToArray()
    );

    var bookTicketDetailsJson = JsonUtility.ToJson(bookTicketDetails);
    Debug.Log("Modified JSON: " + bookTicketDetailsJson);

    UnityWebRequest www = UnityWebRequest.Post(bookTicketLink, bookTicketDetailsJson);
    byte[] bodyRaw = Encoding.UTF8.GetBytes(bookTicketDetailsJson);
    www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
    www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    www.SetRequestHeader("Content-Type", "application/json");

    yield return www.SendWebRequest();
    www.uploadHandler.Dispose();

    if (www.isNetworkError || www.isHttpError)
    {
        Debug.Log(www.error);
    }
    else
    {
        Debug.Log(www.downloadHandler.text);
	Logger.LogMessage(" book ticket  api log data: "+www.downloadHandler.text);
        JObject bookTicketJson = JObject.Parse(www.downloadHandler.text);
        int currentBalance = (int)bookTicketJson["Balance"];
        mainData.receivedLoginData.Balance = currentBalance.ToString();
        balance = originalBalance = currentBalance;
        www.downloadHandler.Dispose();
    }
}
}

public class SentDrawDetails
{
	public string GameID;
	public string Limit;
	public string Status;

	public SentDrawDetails(string gameID, string limit, string status)
	{
		GameID = gameID;
		Limit = limit;
		Status = status;
	}
}

public class SendDrawDetailsCurrent
{
	//public string UserID;
	public string GameID;
	public string Status;
	public string Limit;
	//public string AutoClaim;
//string userID,string autoClaim
	public SendDrawDetailsCurrent(string gameID,  string status,string limit )
	{
		//UserID = userID;
		GameID = gameID;
		Status = status;
		Limit = limit;
		//AutoClaim = autoClaim;
	}
}

[System.Serializable]
public class PendingDrawDetails
{
	public string retMsg;
	public string retStatus;
	public string Query;
	public string Now;
	public string GameID;
	public string Date;
	public Draw[] Draws;
}

[System.Serializable]
public class Draw
{
	public string GID;
	public string DrawDate;
	public string DrawTime;
	public string Status;
}

[System.Serializable]
public class CurrentDrawDetails
{
	public string retMsg;
	public string retStatus;
	public string Query;
	public string Now;
	public string GameID;
	public string Date;
	public CurrentDraws[] Draws;
	public WinDetails WinDtls;
	public string AutoClaimed;
	public string Balance;

	[System.Serializable]
	public class CurrentDraws
	{
		public string GID;
		public string DrawDate;
		public string DrawTime;
		public string Status;
		public string Result;
		public string XF;
		public string TotWin;
	}

	[System.Serializable]
	public class WinDetails
	{
		public string GIDGName;
		public string TicketID;
		public string Win;
		public string DrawDate;
		public string DrawTime;
	}


}

[System.Serializable]
public class BookTicketDetails
{
	//public string UserID;
	public string GameID;
	public string DrawId;
	public BetsDetails[] Bets;

	[System.Serializable]
	public class BetsDetails
	{
		public string Digit;
		public string Qty;
		public string SDigit;

		public BetsDetails(string digit, string qty, string sDigit)
		{
			Digit = digit;
			Qty = qty;
			SDigit = sDigit;
		}
	}

	public BookTicketDetails(string gameID, string draw, BetsDetails[] bets)
	{
		//UserID = userID;
		GameID = gameID;
		DrawId = draw;
		Bets = bets;
	}
}

