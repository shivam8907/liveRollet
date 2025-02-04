using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace KheloJeeto
{
    public class UserTicketDetailsButton : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI ticketIdText, drawTimeText, playText, winText, multiplierTxt;

        [SerializeField] private Image betHistoryCardRanks;
        [SerializeField] private Image betHistoryCardSuits;
        [SerializeField] private Sprite jokerSprite, queenSprite, kingSprite;
        [SerializeField] private Sprite heartSprite, spadeSprite, diamondSprite, clubSprite;
        public static string ticketID;

       public void SetTicketDetails(string ticketId, string drawTime, string play, string win, string result)
{
    ticketID = ticketId;
    ticketIdText.text = ticketId;
    drawTimeText.text = drawTime;
    playText.text = play;
    winText.text = win;

    // Handle empty or invalid result data
    if (!string.IsNullOrEmpty(result) && result.Contains("-"))
    {
        var resultArray = result.Split('-');

        if (resultArray.Length > 1)
        {
            multiplierTxt.text = resultArray[1];
        }
        else
        {
            multiplierTxt.text = "N/A";  // Default value if missing
        }

        string resultText = JeetoJokerManager.Instance.GetCardRankAndSuitAccordingToNumber(resultArray[0]);
        SetImageResult(resultText);
    }
    else
    {
        multiplierTxt.text = "N/A"; // Default value if result is empty or invalid
        SetImageResult(""); // Avoids errors in SetImageResult
    }
}

private void SetImageResult(string cardRankAndSuit)
{
    if (string.IsNullOrEmpty(cardRankAndSuit) || cardRankAndSuit.Length < 2)
    {
        betHistoryCardRanks.sprite = null;
        betHistoryCardSuits.sprite = null;
        return;
    }

    print("Card Rank and Suit : " + cardRankAndSuit);

    switch (cardRankAndSuit[0])
    {
        case 'J':
            betHistoryCardRanks.sprite = jokerSprite;
            break;
        case 'Q':
            betHistoryCardRanks.sprite = queenSprite;
            break;
        case 'K':
            betHistoryCardRanks.sprite = kingSprite;
            break;
        default:
            betHistoryCardRanks.sprite = null;
            break;
    }

    switch (cardRankAndSuit[1])
    {
        case 'H':
            betHistoryCardSuits.sprite = heartSprite;
            break;
        case 'S':
            betHistoryCardSuits.sprite = spadeSprite;
            break;
        case 'D':
            betHistoryCardSuits.sprite = diamondSprite;
            break;
        case 'C':
            betHistoryCardSuits.sprite = clubSprite;
            break;
        default:
            betHistoryCardSuits.sprite = null;
            break;
    }
}

    }
}
