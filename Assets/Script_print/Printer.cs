using UnityEngine;

using System.IO;
using System;
using System.Text;

using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using iTextSharp.text;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using static BookTicketDetails;
using System.Xml.Linq;
namespace KheloJeeto
{
public class Printer : MonoBehaviour
{
    //  public Image Barcode;

    // public Button betButton;
    // public Text drawTimeText;
    //public CardSelect_TitliSorat cardselct;
    public MainData maindatacollect;
    public JeetoJokerManager jokerprint;

    public static Dictionary<int, string> gameplayDrawTimes = new Dictionary<int, string>(); // Stores draw times for each game ID

    private void Start()
    {
        Application.runInBackground = true;
        UnityEngine.Debug.Log("application running in background");
    }
    public async void PrintPdf()
    {
        // betButton.interactable = false;
        UnityEngine.Debug.Log("bet button deactivate print process running..");
        Stopwatch stopwatch = Stopwatch.StartNew();
        string printFolderPath = Path.Combine(Application.dataPath, "print");

        // Clean up old files
        if (Directory.Exists(printFolderPath))
        {
            foreach (string file in Directory.GetFiles(printFolderPath))
            {
                File.Delete(file);
            }
            foreach (string subfolder in Directory.GetDirectories(printFolderPath))
            {
                Directory.Delete(subfolder, true);
            }
        }
        else
        {
            Directory.CreateDirectory(printFolderPath);
        }

        // string pdfFilePath = Path.Combine(printFolderPath, BET.instance.ticketId + ".pdf");
        string pdfFilePath = Path.Combine(printFolderPath);
        // Create the PDF asynchronously
        PDFManager pdfMgr = PDFManager.CreatePDFBuilderWithPathAndTicketID(pdfFilePath);

string gameId = PlayerPrefs.GetString("Game_01", "Unknown");
        // Ensure PDF content is created before closing the document
        CreatePdfContent(pdfMgr, jokerprint.BetsDetailsList,gameId);

        pdfMgr.CloseDocument();
      // await Task.Run(() => pdfMgr.StartPrintTicketDirect());
       try
            {
                // Convert the PDF to an image before printing
                await Task.Run(() =>
                {
                    // This will convert the PDF to an image and then start printing
                    pdfMgr.PrintPdf();
                });
                UnityEngine.Debug.Log("Task executed successfully in new data.");
            }
              catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"Exception in Task.Runner data of new : {ex.Message}\n{ex.StackTrace}");
            }

        stopwatch.Stop();
        UnityEngine.Debug.Log("Preparing Data Elapsed time: " + stopwatch.ElapsedMilliseconds + "ms");
        UnityEngine.Debug.Log("bet button activate print process Complete.");
    }

   [System.Serializable]
public class BetsDetails
{
    public string Digit { get; set; }
    public string Qty { get; set; }
    public string SDigit { get; set; }

    public BetsDetails(string digit, string qty, string sDigit)
    {
        Digit = digit;
        Qty = qty;
        SDigit = sDigit;
    }
}

[System.Serializable]
public class BetsContainer
{
    public List<BetsDetails> Bets { get; set; }

    public BetsContainer()
    {
        Bets = new List<BetsDetails>();
    }
}

    
    


//new data with dynamic data
private void CreatePdfContent(PDFManager pdfManager, List<BookTicketDetails.BetsDetails> betsDetails,string gameid)
{
    UnityEngine.Debug.Log("enters in data pdf");
   // string gameId = ;
    string ticketId =UserTicketDetailsButton.ticketID;
    string totalPoint = KheloJeeto.JeetoJokerManager.Instance.playText.text.ToString();
    DateTime currentDateTime = DateTime.Now;
    string formattedDateTime = currentDateTime.ToString("hh:mm:ss tt");
    string ticketTime = formattedDateTime;
    string drawTime = KheloJeeto.JeetoJokerManager.Instance.drawTimeText.text.ToString();

    pdfManager.CreateParagraph("For Amusement only", Element.ALIGN_CENTER, 8f, iTextSharp.text.Font.NORMAL);
    pdfManager.CreateParagraph("JEETO GAMEZ", Element.ALIGN_CENTER, 17f, iTextSharp.text.Font.BOLD);
    pdfManager.CreateParagraph("GAME NAME: JEETO JOKER TIMER", Element.ALIGN_LEFT, 7f, iTextSharp.text.Font.BOLD);
    pdfManager.CreateParagraph($"Game ID: {gameid}, Ticket ID: {ticketId}", Element.ALIGN_LEFT, 7.5f, iTextSharp.text.Font.BOLD);
    pdfManager.CreateParagraph($"Ticket Time: {ticketTime}, Draw Time: {drawTime}", Element.ALIGN_LEFT, 7.5f, iTextSharp.text.Font.BOLD);
    pdfManager.CreateParagraph("Total Point " + totalPoint, Element.ALIGN_LEFT, 7.5f, iTextSharp.text.Font.BOLD);

    // Build cardData from betsDetails
    StringBuilder cardData = new StringBuilder();
cardData.AppendLine("Item   Point   Item   Point");

 if (betsDetails != null && betsDetails.Count > 0)
    {
        int cardsAdded = 0; // To track how many cards are added in each line
        for (int i = 0; i < betsDetails.Count; i++)
        {
            // Skip cards with Qty = 0
            if (int.TryParse(betsDetails[i].Qty, out int qty) && qty == 0)
            {
                continue;
            }

            // Map digits to card names (e.g., "11" -> "JH", "12" -> "QH", etc.)
            string cardName = MapToCardName(betsDetails[i].Digit);
            string cardValue = betsDetails[i].Qty;

            if (cardsAdded % 2 == 0)
            {
                cardData.Append($"  {cardName,-3}       {cardValue}");
            }
            else
            {
                cardData.Append($"      {cardName,-3}     {cardValue}");
                cardData.AppendLine(); // Add a new line after every two cards
            }

            cardsAdded++;
        }

        // If cardsAdded is odd, add a new line for formatting
        if (cardsAdded % 2 == 1)
        {
            cardData.AppendLine();
        }
    }
    else
    {
        cardData.AppendLine("No bets available.");
    }
pdfManager.CreateParagraph(cardData.ToString(), Element.ALIGN_LEFT, 12.5f, iTextSharp.text.Font.NORMAL);
StringBuilder bottomText = new StringBuilder();
bottomText.AppendLine("**Ticket not for sale**");
pdfManager.CreateParagraph(bottomText.ToString(), Element.ALIGN_CENTER, 12.5f, iTextSharp.text.Font.NORMAL);

}
private string MapToCardName(string digit)
{
    // Map digits to card notation
    return digit switch
    {
        "0" => "JH",
        "01" => "JS",
        "02" => "JD",
        "03" => "JC",
        "04" => "QH",
        "05" => "QS",
        "06" => "QD",
        "07" => "QC",
        "08" => "KH",
        "09" => "KS",
        "10" => "KD",
        "11" => "KC",
        _ => digit, // Default to the original digit if not mapped
    };
}






 //private void CreatePdfContent(PDFManager pdfManager)
    // {
    //     string gameId =KheloJeeto.JeetoJokerManager.Instance.GlobalGameID;
    //     string ticketId = "32815066";
    //     string TOtalpoint= KheloJeeto.JeetoJokerManager.Instance.playText.text.ToString();
    //     DateTime currentDateTime = DateTime.Now;
    //    string formattedDateTime = currentDateTime.ToString("hh:mm:ss tt");
    //     string Tickettime = formattedDateTime;
    //     string drawtime = KheloJeeto.JeetoJokerManager.Instance.drawTimeText.text.ToString();
    //     pdfManager.CreateParagraph("For Amusement only", Element.ALIGN_CENTER, 8f, iTextSharp.text.Font.NORMAL);
    //     pdfManager.CreateParagraph("JEETO GAMEZ", Element.ALIGN_CENTER, 17f, iTextSharp.text.Font.BOLD);
    //     pdfManager.CreateParagraph("GAME NAME: JEETO JOKER TIMER", Element.ALIGN_LEFT, 7f, iTextSharp.text.Font.BOLD);
    //     pdfManager.CreateParagraph($"Game ID: {gameId}, Ticket ID: {ticketId}", Element.ALIGN_LEFT, 7.5f, iTextSharp.text.Font.BOLD);
    //     pdfManager.CreateParagraph($"Ticket Time: {Tickettime}, Draw Time: {drawtime}", Element.ALIGN_LEFT, 7.5f, iTextSharp.text.Font.BOLD);
    //     pdfManager.CreateParagraph("Total Point " + TOtalpoint, Element.ALIGN_LEFT, 7.5f, iTextSharp.text.Font.BOLD);
    
    //     // Static data for cards and values
    //    List<string> cards = new List<string>
    // {
    //     "JH", "JS", "JD", "JC", "QH", "QS",
    //     "QD", "QC", "KH", "KS", "KD", "KC"
    // };

    //     List<int> values = new List<int>
    // {
    //     10, 20, 30, 4, 5, 60,
    //     70, 80, 20, 10, 11, 12
    // };

    //     // Validate the lists before proceeding
    //     if (cards == null || values == null || cards.Count != values.Count)
    //     {
    //         UnityEngine.Debug.Log("Invalid card or value data.");
    //         return;
    //     }

    //     StringBuilder cardData = new StringBuilder();
    //     cardData.AppendLine("Item   Point   Item   Point");

    //     int count = 0;
    //     for (int i = 0; i < cards.Count; i++)
    //     {
    //         string cardName = cards[i];
    //         int cardValue = values[i];

    //         if (count % 2 == 0)
    //         {
    //             cardData.Append($"  {cardName,-3}       {cardValue}");
    //         }
    //         else
    //         {
    //             cardData.Append($"      {cardName,-3}     {cardValue}");
    //         }

    //         count++;

    //         if (count % 2 == 0)
    //         {
    //             cardData.AppendLine();
    //         }
    //     }

    //     // Creating the PDF content
    //     // pdfManager.CreateParagraph("Jeeto Joker Print", Element.ALIGN_CENTER, 30f, iTextSharp.text.Font.BOLD);
    //     pdfManager.CreateParagraph(cardData.ToString(), Element.ALIGN_LEFT, 12.5f, iTextSharp.text.Font.NORMAL);
    //     StringBuilder bottomText = new StringBuilder();
    //     bottomText.AppendLine("**Ticket not for sale**");
    //     pdfManager.CreateParagraph(bottomText.ToString(), Element.ALIGN_CENTER, 12.5f, iTextSharp.text.Font.NORMAL);
    // }
}
}
