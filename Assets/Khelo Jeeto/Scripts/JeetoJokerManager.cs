using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static BookTicketDetails;
using UnityEngine.Events;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace KheloJeeto
{
	public class JeetoJokerManager : BaseGameManager
	{
		public Final_Data_Manager Final_Data_Manager;
        public static JeetoJokerManager Instance { get; private set; }
		public int SelectedCoinAmt { get => selectedCoinAmt; private set => selectedCoinAmt = value; }
		public int TotalPointsSpent
		{
			get => totalPointsSpent;
			set
			{
				totalPointsSpent = value;
				playText.text = totalPointsSpent.ToString();
				spinPanelPlayText.text = totalPointsSpent.ToString();
			}
		}

		[SerializeField] private Text balanceText;
		[SerializeField] private Text balancetopText;
		[SerializeField] private Text spinPanelBalanceText;
		[SerializeField] private string bookTicketUrl;
		[SerializeField] private Sprite jokerSprite, queenSprite, kingSprite;
		[SerializeField] private Sprite heartSprite, spadeSprite, diamondSprite, clubSprite;
		[SerializeField] private Image[] betHistoryCardRanks;
		[SerializeField] private Image[] betHistoryCardSuits;
		[SerializeField] private TextMeshProUGUI[] betHistoryMultiplier;
		[SerializeField] private TextMeshProUGUI[] betHistoryTime;
		[SerializeField] private EachCard[] eachCards;
		[SerializeField] private Button clearButton, doubleButton, repeatButton;
		[SerializeField] private Button allHeartsButton, allSpadesButton, allDiamondsButton, allClubsButton;
		[SerializeField] private Button allJokersButton, allQueensButton, allKingsButton;
		[SerializeField] private Timer timer;
		[SerializeField] private Text gameIdText;
		//[SerializeField]
		 public  Text drawTimeText;
		[SerializeField] private Text spinPanelGameIdText;
		//[SerializeField]
		 public Text playText;
		[SerializeField] private Text spinPanelPlayText;
		public Text winText;
		public Text spinPanelWinText;
		public Text spinPanelPopupWinText;
		[SerializeField] private GameObject boardWheelPanel;
		[SerializeField] private GameObject boardObject;
		[SerializeField] private Transform boardHolder;

		private bool isAllButtonDiasabled;

		[SerializeField] private EachCard[] allJokerCards;
		[SerializeField] private EachCard[] allQueenCards;
		[SerializeField] private EachCard[] allKingCards;

		[SerializeField] private EachCard[] allHeartCards;
		[SerializeField] private EachCard[] allSpadeCards;
		[SerializeField] private EachCard[] allDiamondCards;
		[SerializeField] private EachCard[] allClubCards;

		[SerializeField] private EachCard[] allCards;
		[SerializeField] private int[] randomCardCount;
		[SerializeField] private MessageView messageView;
		[Header("Sound Handler")]
		[SerializeField] private Image soundImg;
		[SerializeField] private Sprite[] soundSprite;
		public bool isMusicSoundEnabled;
		public readonly string AudioPlayerPrefs = "AudioEnabled";//0= off 1= on

		private int selectedCoinAmt = 2;
		private string gameId = "01";
		private int balance;
		private string userid;
		private int originalBalance;
		private int totalPointsSpent;
		public UnityEvent OnWin;
		[SerializeField] private GameObject backPopUp;

        [SerializeField] private WheelController wheelController;
		  public Text dateTime;
		  public Text userNameTxt;
		  public Button BetButton;
		  public Printer print12;
		  public  Toggle Printtoggle;
		  public Toggle Autoclaimtoggle;
		// public static int SelectedCoinAmtnew;
   // [SerializeField] private  GameObject selectedChip; // Reference to the currently selected chip
    //public float rotationSpeed = 100f; // Speed of rotation in degrees per second
	// private bool isRotating = false;
        public 
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
			if (!PlayerPrefs.HasKey(AudioPlayerPrefs))
			{
				PlayerPrefs.SetInt(AudioPlayerPrefs, 1);
			}
			isMusicSoundEnabled = PlayerPrefs.GetInt(AudioPlayerPrefs) == 1;
			/*if (isMusicSoundEnabled)
				soundImg.sprite = soundSprite[1];
			else soundImg.sprite = soundSprite[0];*/
			if (isMusicSoundEnabled)
				soundImg.color = Color.white;
			else soundImg.color = Color.gray;
			Application.runInBackground = true;
		}

		// Start is called before the first frame update
		void Start()
		{

			  // Add listeners to toggles to dynamically enable/disable the Bet button
    Printtoggle.onValueChanged.AddListener(OnToggleChanged);
    Autoclaimtoggle.onValueChanged.AddListener(OnToggleChanged);
	  UpdateBetButtonState();

			//ShowResults();
               userid= mainData.receivedLoginData.UserID;
			   userNameTxt.text=userid.ToString();
Debug.Log("user id from login data is:"+userNameTxt.text);
			balance = int.Parse(mainData.receivedLoginData.Balance);// need to open later
            originalBalance = balance;
			ShowBalance();
			GetLastFewDrawDetailsJeetoJoker();
			
			SendPendingDrawDetailsJeetoJoker();

        }
		void OnToggleChanged(bool isOn)
{
    UpdateBetButtonState();
}

void UpdateBetButtonState()
{
    // Enable BetButton only if both toggles are on
    BetButton.interactable = Printtoggle.isOn && Autoclaimtoggle.isOn;
}


/*private void StartRotating(GameObject chip)
    {
        isRotating = true;
    }

    private void StopRotating(GameObject chip)
    {
        isRotating = false;
    }
	public void SelectCoindata(int amt,GameObject chip)
		{
			SoundManager.instance.PlayButtonSound();
			SelectedCoinAmtnew = amt;
			  // Stop rotating the previously selected chip, if any
		Debug.Log("selected chip:"+chip);
        if (selectedChip != null)
        {
            StopRotating(selectedChip);
        }

        // Assign the new chip and start rotating it
        selectedChip =chip;
        StartRotating(selectedChip);   
		}*/
		private void Update()
		{
			 //if (isRotating && selectedChip != null)
       // {
            // Rotate the selected chip clockwise
          //  selectedChip.transform.Rotate(Vector3.forward * -rotationSpeed * Time.deltaTime);
       // }
		DateTime currentDateTime = DateTime.Now;
        string formattedDateTime = currentDateTime.ToString("dd-MM-yy h:mm:ss tt");
        dateTime.text = formattedDateTime;
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				if (!backPopUp.activeInHierarchy)
				{
					backPopUp.SetActive(true); 
				}
			}
		}

		public void ChangeAudioEnableDisable()
		{
			isMusicSoundEnabled = !isMusicSoundEnabled;
			if (isMusicSoundEnabled)
			{
				PlayerPrefs.SetInt(AudioPlayerPrefs, 1);
				soundImg.color = Color.white;
			}
			else
			{
				PlayerPrefs.SetInt(AudioPlayerPrefs, 0);
				soundImg.color = Color.grey;

			}

		}

// 		private void SendPendingDrawDetailsJeetoJoker()
// 		{
// 			base.SendPendingDrawDetails(gameId, (result) =>
// 			{
// 				mainData.pendingDrawDetails = JsonUtility.FromJson<PendingDrawDetails>(result);
// 				gameIdText.text = mainData.pendingDrawDetails.Draws[0].GID;
// 				DateTime drawDateTime = DateTime.Parse(mainData.pendingDrawDetails.Draws[0].DrawTime);
// string formattedTime = drawDateTime.ToString("hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);
// drawTimeText.text = mainData.pendingDrawDetails.Draws[0].DrawDate + "  " + formattedTime;
// Debug.Log("drawtime in game is:"+drawTimeText.text);
// 				drawTimeText.text = mainData.pendingDrawDetails.Draws[0].DrawDate + "  " + mainData.pendingDrawDetails.Draws[0].DrawTime;
// 				spinPanelGameIdText.text = mainData.pendingDrawDetails.Draws[0].GID;
// 				ProcessTimer();
// 			});
// 		}
		private void SendPendingDrawDetailsJeetoJoker()
{
    base.SendPendingDrawDetails(gameId, (result) =>
    {
        mainData.pendingDrawDetails = JsonUtility.FromJson<PendingDrawDetails>(result);
        gameIdText.text = mainData.pendingDrawDetails.Draws[0].GID;

        // Parse DrawTime and format it to show only the time in hh:mm tt format
        DateTime drawDateTime = DateTime.Parse(mainData.pendingDrawDetails.Draws[0].DrawTime);
        string formattedTime = drawDateTime.ToString("hh:mm tt", System.Globalization.CultureInfo.InvariantCulture);

        // Update drawTimeText to display only the formatted time
        drawTimeText.text = formattedTime;
        Debug.Log("drawtime in game is: " + drawTimeText.text);

        spinPanelGameIdText.text = mainData.pendingDrawDetails.Draws[0].GID;
        ProcessTimer();
    });
}



		void ProcessTimer()
		{
            var timeOfDay = Convert.ToDateTime(mainData.pendingDrawDetails.Now).TimeOfDay;
			var drawTime = Convert.ToDateTime(mainData.pendingDrawDetails.Draws[0].DrawTime).TimeOfDay;
			Debug.Log(drawTime);
			Debug.Log(timeOfDay);
			timer.SetCurrentDrawTime(drawTime);
			var remainingTime = drawTime.Subtract(timeOfDay);
			print("timer data"+remainingTime);
			if(remainingTime.TotalSeconds>0)
			{
			timer.RunTimer((float)remainingTime.TotalSeconds);
			}
			else{
				Debug.Log("timer daata.."+remainingTime.TotalSeconds);
			}
		}

		private void GetLastFewDrawDetailsJeetoJoker()
{
    base.GetLastFewDrawDetails(gameId, 10, (result) =>
    {
        Debug.Log("API Response: " + result);

        if (string.IsNullOrEmpty(result))
        {
            Debug.LogError("Empty API result.");
            return;
        }

        mainData.lastFewDrawDetails = JsonUtility.FromJson<CurrentDrawDetails>(result);

        if (mainData.lastFewDrawDetails?.Draws == null || mainData.lastFewDrawDetails.Draws.Length == 0)
        {
            Debug.LogWarning("No draws available in the response.");
            return;
        }

        int maxDraws = Mathf.Min(
            mainData.lastFewDrawDetails.Draws.Length,
            betHistoryMultiplier.Length,
            betHistoryCardRanks.Length,
            betHistoryCardSuits.Length,
            betHistoryTime.Length
        );

        for (int i = 0; i < maxDraws; i++)
        {
            var drawDetail = mainData.lastFewDrawDetails.Draws[i];

            if (i == 0)
            {
                timer.SetLastDrawTime(drawDetail.DrawTime);
            }

            string xFVal = drawDetail.XF;
            bool isSpecialXF = !(xFVal == "N" || xFVal.ToLower() == "1x");
            betHistoryMultiplier[i].gameObject.SetActive(isSpecialXF);

            if (isSpecialXF)
            {
                betHistoryMultiplier[i].text = xFVal.ToLower();
            }

            // Use the GetCardRankAndSuitAccordingToNumber function to handle the Result
            string cardRankAndSuit = GetCardRankAndSuitAccordingToNumber(drawDetail.Result);

            // Ensure the card data is valid
            if (string.IsNullOrEmpty(cardRankAndSuit) || cardRankAndSuit.Length < 2) continue;

            // Process the rank and suit from the result
            switch (cardRankAndSuit[0])
            {
                case 'J': betHistoryCardRanks[i].sprite = jokerSprite; break;
                case 'Q': betHistoryCardRanks[i].sprite = queenSprite; break;
                case 'K': betHistoryCardRanks[i].sprite = kingSprite; break;
                default: continue;
            }

            switch (cardRankAndSuit[1])
            {
                case 'H': betHistoryCardSuits[i].sprite = heartSprite; break;
                case 'S': betHistoryCardSuits[i].sprite = spadeSprite; break;
                case 'D': betHistoryCardSuits[i].sprite = diamondSprite; break;
                case 'C': betHistoryCardSuits[i].sprite = clubSprite; break;
                default: continue;
            }

            betHistoryTime[i].text = drawDetail.DrawTime;
        }
    });
}

public string GetCardRankAndSuitAccordingToNumber(string result)
{
    string cardRankAndSuit;

    switch (result)
    {
        case "00":
            cardRankAndSuit = "JH";
            break;

        case "01":
            cardRankAndSuit = "JS";
            break;

        case "02":
            cardRankAndSuit = "JD";
            break;

        case "03":
            cardRankAndSuit = "JC";
            break;

        case "04":
            cardRankAndSuit = "QH";
            break;

        case "05":
            cardRankAndSuit = "QS";
            break;

        case "06":
            cardRankAndSuit = "QD";
            break;

        case "07":
            cardRankAndSuit = "QC";
            break;

        case "08":
            cardRankAndSuit = "KH";
            break;

        case "09":
            cardRankAndSuit = "KS";
            break;

        case "10":
            cardRankAndSuit = "KD";
            break;

        case "11":
            cardRankAndSuit = "KC";
            break;

        default:
            cardRankAndSuit = "";
            break;
    }

    return cardRankAndSuit;
}

		public void EnableCardButtons()
		{
			foreach (var eachCard in eachCards)
			{
				eachCard.SwitchButtonInteraction(true);
			}

			clearButton.interactable = true;
			doubleButton.interactable = true;
			repeatButton.interactable = true;

			allClubsButton.interactable = true;
			allDiamondsButton.interactable = true;
			allHeartsButton.interactable = true;
			allSpadesButton.interactable = true;

			allJokersButton.interactable = true;
			allKingsButton.interactable = true;
			allQueensButton.interactable = true;
			isAllButtonDiasabled = false;
		}

		public void DisableCardButtons()
		{
			foreach (var eachCard in eachCards)
			{
				eachCard.SwitchButtonInteraction(false);
			}
			isAllButtonDiasabled = true;

			clearButton.interactable = false;
			doubleButton.interactable = false;
			repeatButton.interactable = false;

			allClubsButton.interactable = false;
			allDiamondsButton.interactable = false;
			allHeartsButton.interactable = false;
			allSpadesButton.interactable = false;

			allJokersButton.interactable = false;
			allKingsButton.interactable = false;
			allQueensButton.interactable = false;
		}

		public void BetOnAllJokerCards()
		{
			foreach (var eachCard in allJokerCards)
			{
				eachCard.PlaceBetOnCard(CoinChipController.SelectedCoinAmt);
			}
		}



		public void BetOnAllQueenCards()
		{
			foreach (var eachCard in allQueenCards)
			{
				eachCard.PlaceBetOnCard(CoinChipController.SelectedCoinAmt);
			}
		}

		public void BetOnAllKingCards()
		{
			foreach (var eachCard in allKingCards)
			{
				eachCard.PlaceBetOnCard(CoinChipController.SelectedCoinAmt);
			}
		}

		public void BetOnAllHearts()
		{
			foreach (var eachCard in allHeartCards)
			{
				eachCard.PlaceBetOnCard(CoinChipController.SelectedCoinAmt);
			}
		}

		public void BetOnAllSpades()
		{
			foreach (var eachCard in allSpadeCards)
			{
				eachCard.PlaceBetOnCard(CoinChipController.SelectedCoinAmt);
			}
		}

		public void BetOnAllDiamonds()
		{
			foreach (var eachCard in allDiamondCards)
			{
				eachCard.PlaceBetOnCard(CoinChipController.SelectedCoinAmt);
			}
		}

		public void BetOnAllClubs()
		{
			foreach (var eachCard in allClubCards)
			{
				eachCard.PlaceBetOnCard(CoinChipController.SelectedCoinAmt);
			}
		}
		#region Pick and Place Random Card Bet
		public void OnRandomButtonClick(int id)
		{
			if (isAllButtonDiasabled) return;

			int count = randomCardCount[id];
			if (GetTotalAmountToClearOnRandomButtonClick(count))
			{
				Debug.Log("<color =red>Not enough points</color>");
				messageView.ShowMsg("Not enough balance");
				return;
			}
			OnPressedClear();
			PickRandomCards(count);


		}
		private bool GetTotalAmountToClearOnRandomButtonClick(int count)
		{
			int totalBet = selectedCoinAmt * count;
			Debug.Log(totalBet);
			if (balance - totalBet > 0)
			{
				return false;
			}
			else return true;

		}
		private void PickRandomCards(int count)
		{
			List<EachCard> eachCards = new List<EachCard>(allCards);
			List<EachCard> tempcard = new List<EachCard>();
			for (int i = 0; i < count; i++)
			{
				int randIndex = UnityEngine.Random.Range(0, eachCards.Count);
				tempcard.Add(eachCards[randIndex]);
				eachCards.RemoveAt(randIndex);
			}
			foreach (var eachCard in tempcard)
			{
				eachCard.PlaceBetOnCard(SelectedCoinAmt);
			}
		}

		#endregion Pick and Place Random Card Bet
		public void GetCurrentDrawDetailsResultJeetoJoker()
{
    base.GetCurrentDrawDetailsResult(gameId, (result) =>
    {
        mainData.currentDrawDetails = JsonUtility.FromJson<CurrentDrawDetails>(result);

        if (mainData.currentDrawDetails?.Draws == null || mainData.currentDrawDetails.Draws.Length == 0)
        {
            Debug.LogError("Invalid or empty draw result.");
            return;
        }

        int gameResult = 0;
        int multiplier = 1;

        if (int.TryParse(mainData.currentDrawDetails.Draws[0].Result, out int parsedGameResult))
        {
            gameResult = parsedGameResult;
        }
        else
        {
            Debug.LogWarning("Invalid game result format.");
        }

        if (mainData.currentDrawDetails.Draws[0].XF.Length > 0 && 
            int.TryParse(mainData.currentDrawDetails.Draws[0].XF[0].ToString(), out int parsedMultiplier))
        {
            multiplier = parsedMultiplier;
        }
        else
        {
            Debug.LogWarning("Invalid multiplier format. Defaulting to 1.");
        }

        AppearWheelBoard();

        if (string.IsNullOrEmpty(mainData.currentDrawDetails.Draws[0].TotWin))
        {
            mainData.currentDrawDetails.Draws[0].TotWin = "0";
        }

        bool win = int.TryParse(mainData.currentDrawDetails.Draws[0].TotWin, out int totalWin) && totalWin > 0;

        wheelController.PlaceBet(gameResult, multiplier, win, () =>
        {
            OnWin?.Invoke();
            balance = originalBalance = int.Parse(mainData.currentDrawDetails.Balance);
            winText.text = spinPanelWinText.text = spinPanelPopupWinText.text = totalWin.ToString();
			ShowResults();
			Debug.Log("total win:"+totalWin.ToString());
        },
        () =>
        {
            winText.text = spinPanelWinText.text = spinPanelPopupWinText.text = totalWin.ToString();
        });
    });
	//   SpinWheel.Instance.SetupResults(gameResult, multiplier, win, () =>
	// 			{
	// 				OnWin?.Invoke();
	// 				balance = originalBalance = int.Parse(mainData.currentDrawDetails.Balance);
	// 				winText.text = spinPanelWinText.text = spinPanelPopupWinText.text = int.Parse(mainData.currentDrawDetails.Draws[0].TotWin).ToString();
	// 				//	ShowResults();
	// 				//OnWin?.Invoke();
	// 				//balance = originalBalance = int.Parse(mainData.currentDrawDetails.Balance);
	// 				//winText.text = winPopupText.text = int.Parse(mainData.currentDrawDetails.Draws[0].TotWin).ToString();
	// 			},
	// 			() =>
	// 			{
	// 				winText.text = spinPanelWinText.text = spinPanelPopupWinText.text = int.Parse(mainData.currentDrawDetails.Draws[0].TotWin).ToString();
	// 			}
	// 			);
		}


//new function for get current draw details old code...
// public void GetCurrentDrawDetailsResultJeetoJoker()
// 		{
// 			base.GetCurrentDrawDetailsResult(gameId, (result) =>
// 			{
// 				mainData.currentDrawDetails = JsonUtility.FromJson<CurrentDrawDetails>(result);

// 				var gameResult = int.Parse(mainData.currentDrawDetails.Draws[0].Result);
// 				var multiplier = int.Parse(mainData.currentDrawDetails.Draws[0].XF[0].ToString());

// 				AppearWheelBoard();
// 				if (string.IsNullOrEmpty((mainData.currentDrawDetails.Draws[0].TotWin)))
// 					mainData.currentDrawDetails.Draws[0].TotWin = "0";

//                 bool win = int.Parse(mainData.currentDrawDetails.Draws[0].TotWin) > 0;
// 				wheelController.PlaceBet(gameResult, multiplier, win, () =>
//                 {
//                     OnWin?.Invoke();
//                     balance = originalBalance = int.Parse(mainData.currentDrawDetails.Balance);
//                     winText.text = spinPanelWinText.text = spinPanelPopupWinText.text = int.Parse(mainData.currentDrawDetails.Draws[0].TotWin).ToString();
//                     	ShowResults();
//                     // OnWin?.Invoke();
//                     // balance = originalBalance = int.Parse(mainData.currentDrawDetails.Balance);
//                     // winText.text = winPopupText.text = int.Parse(mainData.currentDrawDetails.Draws[0].TotWin).ToString();
//                 },
//                 () =>
//                 {
//                     winText.text = spinPanelWinText.text = spinPanelPopupWinText.text = int.Parse(mainData.currentDrawDetails.Draws[0].TotWin).ToString();
//                 }
//                 );

//                SpinWheel.Instance.SetupResults(gameResult, multiplier, win, () =>
// 				{
// 					OnWin?.Invoke();
// 					balance = originalBalance = int.Parse(mainData.currentDrawDetails.Balance);
// 					winText.text = spinPanelWinText.text = spinPanelPopupWinText.text = int.Parse(mainData.currentDrawDetails.Draws[0].TotWin).ToString();
// 					//	ShowResults();
// 					//OnWin?.Invoke();
// 					//balance = originalBalance = int.Parse(mainData.currentDrawDetails.Balance);
// 					//winText.text = winPopupText.text = int.Parse(mainData.currentDrawDetails.Draws[0].TotWin).ToString();
// 				},
// 				() =>
// 				{
// 					winText.text = spinPanelWinText.text = spinPanelPopupWinText.text = int.Parse(mainData.currentDrawDetails.Draws[0].TotWin).ToString();
// 				}
// 				);
// 			});
// 		}


		public void OnPressedClear()
		{
			foreach (EachCard eachCard in eachCards)
			{
				eachCard.ClearCard();
			}
            Final_Data_Manager.ResetData();
			balance = originalBalance;
			ShowBalance();
			TotalPointsSpent = 0;
		}


//for clear bet data with bet inseriton logic only bet reset value not added in balnce
	public void OnPressedClearbet()
		{
			foreach (EachCard eachCard in eachCards)
			{
				eachCard.ClearCard();
			}
            Final_Data_Manager.ResetData();
			//balance = originalBalance;
			//ShowBalance();
			//TotalPointsSpent = 0;
		}
		public void RestartGame()
		{
			Invoke(nameof(SendPendingDrawDetailsJeetoJoker), 3f);
			Invoke(nameof(GetLastFewDrawDetailsJeetoJoker), 3f);
			Invoke(nameof(OnPressedClear), 3f);
		}

		public void OnPressedDouble()
		{
			foreach (EachCard eachCard in eachCards)
			{
				eachCard.DoubleBet();
			}
		}

		public void OnPressedRepeat()
		{
			foreach (EachCard eachCard in eachCards)
			{
				eachCard.PlaceBetOnCard(eachCard.LastStoredAmount);
			}
		}

		public void ShowBalance()
		{
			balanceText.text = balance.ToString();
			balancetopText.text=balance.ToString();
			spinPanelBalanceText.text = balance.ToString();
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

		public void ShowResults()
		{
			GameObject duplicatedBoard = Instantiate(boardObject, boardHolder);
			duplicatedBoard.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
			duplicatedBoard.transform.localScale = Vector3.one * 1.8f;
			AppearWheelBoard();
		}
	

		[ContextMenu("please show")]
		private void AppearWheelBoard()
{
    Debug.LogError("Calling for show");

    // Activate panel without moving it
    boardWheelPanel.SetActive(true);

    // Start rotation after delay
    DOVirtual.DelayedCall(2, () => wheelController.StartRotation());
}


		public void DisappearWheelBoard()
		{
			boardWheelPanel.transform.DOLocalMoveX(0f, 0f);
            Final_Data_Manager.ResetData();
		}


		/*public void BookTicket()
		{
			StartCoroutine(BookTicketCoroutine());
		}*/


	//public Toggle printToggle;       // Reference to the Print Toggle
//public Toggle autoClaimToggle;   // Reference to the AutoClaim Toggle
//public Button BetButton;         // Reference to the Bet Button
public string GlobalGameID;
public List<BetsDetails> BetsDetailsList = new List<BetsDetails>();


public void bookticketforbet()
{
	StartCoroutine(BookTicketCoroutine());
}

// IEnumerator BookTicketCoroutine()
// {
//     if (!Printtoggle.isOn || !Autoclaimtoggle.isOn)
//     {
//         Debug.Log("Both toggles must be enabled to place a bet.");
//         yield break;
//     }

//     BetButton.interactable = false;
//     BetsDetailsList.Clear();

//     foreach (var item in allCards)
//     {
//         BetsDetailsList.Add(new BetsDetails(item.Number, item.CardAmt.ToString(), ""));
//     }

//     string drawTime = mainData.pendingDrawDetails.Draws[0].GID;//PlayerPrefs.GetString("LastDrawGID", "No Draw GID Found");
//     string drawCode = drawTime; 

//     var bookTicketDetails = new BookTicketDetails(
//         "04",
//         drawCode,
//         BetsDetailsList.ToArray()
//     );

//     var bookTicketDetailsJson = JsonUtility.ToJson(bookTicketDetails);
//     bookTicketDetailsJson = bookTicketDetailsJson.Replace("\"Draw\":", "\"draw_code\":");
    
//     Debug.Log("Request JSON: " + bookTicketDetailsJson);

//     UnityWebRequest www = UnityWebRequest.Post(bookTicketUrl, bookTicketDetailsJson);
//     byte[] bodyRaw = Encoding.UTF8.GetBytes(bookTicketDetailsJson);
//     www.uploadHandler = new UploadHandlerRaw(bodyRaw);
//     www.downloadHandler = new DownloadHandlerBuffer();
//     www.SetRequestHeader("Content-Type", "application/json");

//     string token = PlayerPrefs.GetString("AuthToken", "");
//     if (!string.IsNullOrEmpty(token))
//     {
//         www.SetRequestHeader("Authorization", "Bearer " + token);
//     }
//     else
//     {
//         Debug.LogError("Token not found. Authorization header will not be set.");
//     }

//     yield return www.SendWebRequest();
//     www.uploadHandler.Dispose();

//     Debug.Log($"Raw Response: {www.downloadHandler.text}"); // Log full response before parsing

//     if (www.isNetworkError || www.isHttpError)
//     {
//         Debug.LogError($"Error: {www.error}");
//         Debug.LogError($"Response: {www.downloadHandler.text}");
//     }
//     else
//     {
//         try
//         {
//             JObject bookTicketJson = JObject.Parse(www.downloadHandler.text);

//             // Check if "game_id" is available before parsing
//             if (bookTicketJson.ContainsKey("game_id"))
//             {
//                 GlobalGameID = (string)bookTicketJson["game_id"];
//                 Debug.Log($"Parsed GameID: {GlobalGameID}");
//             }
//             else
//             {
//                 Debug.LogError("game_id not found in response.");
//             }

//             // Check if "Balance" is available before parsing
//             if (bookTicketJson.ContainsKey("Balance"))
//             {
//                 int currentBalance = (int)bookTicketJson["Balance"];
//                 mainData.receivedLoginData.Balance = currentBalance.ToString();
//                 balance = originalBalance = currentBalance;
//                 Debug.Log($"Parsed Balance: {currentBalance}");
//             }
//             else
//             {
//                 Debug.LogError("Balance not found in response.");
//             }
//         }
//         catch (Exception ex)
//         {
//             Debug.LogError($"JSON Parsing Error: {ex.Message}");
//         }
//     }

//     www.downloadHandler.Dispose();
//     BetButton.interactable = true;
// }

//new function for bookticket----shivam8907----
IEnumerator BookTicketCoroutine()
{
    if (!Printtoggle.isOn || !Autoclaimtoggle.isOn)
    {
        Debug.Log("Both toggles must be enabled to place a bet.");
        yield break;
    }

    BetButton.interactable = false;
    BetsDetailsList.Clear();

 Dictionary<string, string> cardMapping = new Dictionary<string, string>
    {
        { "0", "JH" }, { "01", "JC" }, { "02", "JD" }, { "03", "JS" },
        { "04", "QH" }, { "05", "QC" }, { "06", "QD" }, { "07", "QS" },
        { "08", "KH" }, { "09", "KC" }, { "10", "KD" }, { "11", "KS" }
    };

    List<BookTicketDetails.BetsDetails> betsDetails = new List<BookTicketDetails.BetsDetails>();

    foreach (var item in allCards)
    {
        if (item.CardAmt != 0)
        {
            string cardValue = cardMapping.ContainsKey(item.Number) ? cardMapping[item.Number] : item.Number;
            betsDetails.Add(new BookTicketDetails.BetsDetails(cardValue, item.CardAmt.ToString(), "0"));
        }
    }
    // foreach (var item in allCards)
    // {
    //     BetsDetailsList.Add(new BetsDetails(item.Number, item.CardAmt.ToString(), "0")); // Ensure SDigit is "0"
    // }

    string drawTime = mainData.pendingDrawDetails.Draws[0].GID; 
    string drawCode = drawTime; 

    var bookTicketDetails = new BookTicketDetails(
        "04", // Ensure this matches the expected "GameID"
        drawCode, // "DrawId"
       // BetsDetailsList.ToArray()
	   betsDetails.ToArray()
    );

    string bookTicketDetailsJson = JsonUtility.ToJson(bookTicketDetails);
    
    // Ensure correct field names
    bookTicketDetailsJson = bookTicketDetailsJson.Replace("\"Draw\":", "\"DrawId\":");

    Debug.Log("Request JSON: " + bookTicketDetailsJson);

    UnityWebRequest www = UnityWebRequest.Post(bookTicketUrl, bookTicketDetailsJson);
    byte[] bodyRaw = Encoding.UTF8.GetBytes(bookTicketDetailsJson);
    www.uploadHandler = new UploadHandlerRaw(bodyRaw);
    www.downloadHandler = new DownloadHandlerBuffer();
    www.SetRequestHeader("Content-Type", "application/json");

    string token = PlayerPrefs.GetString("AuthToken", "");
    if (!string.IsNullOrEmpty(token))
    {
        www.SetRequestHeader("Authorization", "Bearer " + token);
    }
    else
    {
        Debug.LogError("Token not found. Authorization header will not be set.");
    }

    yield return www.SendWebRequest();
    www.uploadHandler.Dispose();

    Debug.Log($"Raw Response: {www.downloadHandler.text}");

    if (www.isNetworkError || www.isHttpError)
    {
        Debug.LogError($"Error: {www.error}");
        Debug.LogError($"Response: {www.downloadHandler.text}");
    }
    else
    {
        try
        {
            JObject bookTicketJson = JObject.Parse(www.downloadHandler.text);

            // Parse GameID
            // if (bookTicketJson.ContainsKey("GameID"))
            // {
            //     GlobalGameID = (string)bookTicketJson["GameID"];
            //     Debug.Log($"Parsed GameID: {GlobalGameID}");
            // }
            // else
            // {
            //     Debug.LogError("GameID not found in response.");
            // }

            // Parse Balance
            if (bookTicketJson.ContainsKey("Balance"))
            {
                int currentBalance = (int)bookTicketJson["Balance"];
                mainData.receivedLoginData.Balance = currentBalance.ToString();
                balance = originalBalance = currentBalance;
                Debug.Log($"Parsed Balance: {currentBalance}");
            }
            else
            {
                Debug.LogError("Balance not found in response.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"JSON Parsing Error: {ex.Message}");
        }
    }

    www.downloadHandler.Dispose();
    BetButton.interactable = true;
	OnPressedClearbet();
}


// IEnumerator BookTicketCoroutine()
// 		{
// 			BetButton.interactable=false;
// 			List<BetsDetails> betsDetails = new List<BetsDetails>();

// 			foreach (var item in allCards)
// 			{
// 				betsDetails.Add(new BetsDetails(item.Number, item.CardAmt.ToString(), ""));
// 			}


// 			var bookTicketDetails = new BookTicketDetails(
// 				mainData.receivedLoginData.UserID,
// 				mainData.pendingDrawDetails.GameID,
// 				mainData.pendingDrawDetails.Draws[0].DrawTime,
// 				betsDetails.ToArray()
// 				);

// 			var bookTicketDetailsJson = JsonUtility.ToJson(bookTicketDetails);
// 			print(bookTicketDetailsJson);
// 			UnityWebRequest www = UnityWebRequest.Post(bookTicketUrl, bookTicketDetailsJson);
// 			byte[] bodyRaw = Encoding.UTF8.GetBytes(bookTicketDetailsJson);
// 			www.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
// 			www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
// 			www.SetRequestHeader("Content-Type", "application/json");
// 			yield return www.SendWebRequest();
// 			www.uploadHandler.Dispose();

// 			if (www.isNetworkError || www.isHttpError)
// {
//     Debug.Log(www.error);
// }
// else
// {
//     Debug.Log(www.downloadHandler.text);
//     JObject bookTicketJson = JObject.Parse(www.downloadHandler.text);
    
//     // Extract GameID and store it globally
//     GlobalGameID = (string)bookTicketJson["GameID"];
    
//     int currentBalance = (int)bookTicketJson["Balance"];
//     mainData.receivedLoginData.Balance = currentBalance.ToString();
//     balance = originalBalance = currentBalance;
//     www.downloadHandler.Dispose();
// }
// 			BetButton.interactable=true;
// 		}
		public void ShowWinAmount(string winAmount)
		{
			winText.text = winAmount;
			spinPanelWinText.text = winAmount;
		}
		public void OnyesBtnClick()
		{
			SceneManager.LoadScene("LobbyScene");
		}

		private void OnApplicationFocus(bool focus)
		{
			if (focus)
			{
				SendPendingDrawDetailsJeetoJoker();
			}
		}
		
	}
}


