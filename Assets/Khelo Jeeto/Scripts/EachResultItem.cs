using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace KheloJeeto
{
    public class EachResultItem : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI gameIdText, drawTimeText, XFText;
        [SerializeField] private Image betHistoryCardRanks;
        [SerializeField] private Image betHistoryCardSuits;
        [SerializeField] private Sprite jokerSprite, queenSprite, kingSprite;
        [SerializeField] private Sprite heartSprite, spadeSprite, diamondSprite, clubSprite;

        public void SetResultItems(string gameId, string drawTime, string result, string XF)
        {
            gameIdText.text = gameId;
            drawTimeText.text = drawTime;
            XFText.text = XF;
            string resultText = JeetoJokerManager.Instance.GetCardRankAndSuitAccordingToNumber(result);
            SetImageResult(resultText);
        }

        // private void SetImageResult(string cardRankAndSuit)
        // {
        //     print("Card Rank and Suit : " + cardRankAndSuit);
        //     switch (cardRankAndSuit[0])
        //     {
        //         case 'J':
        //             betHistoryCardRanks.sprite = jokerSprite;
        //             break;

        //         case 'Q':
        //             betHistoryCardRanks.sprite = queenSprite;
        //             break;

        //         case 'K':
        //             betHistoryCardRanks.sprite = kingSprite;
        //             break;

        //         default:
        //             break;
        //     }

        //     switch (cardRankAndSuit[1])
        //     {
        //         case 'H':
        //             betHistoryCardSuits.sprite = heartSprite;
        //             break;

        //         case 'S':
        //             betHistoryCardSuits.sprite = spadeSprite;
        //             break;

        //         case 'D':
        //             betHistoryCardSuits.sprite = diamondSprite;
        //             break;

        //         case 'C':
        //             betHistoryCardSuits.sprite = clubSprite;
        //             break;

        //         default:
        //             break;
        //     }
        // }
        private void SetImageResult(string cardRankAndSuit)
{
    if (string.IsNullOrEmpty(cardRankAndSuit) || cardRankAndSuit.Length < 2)
    {
        Debug.LogError("Invalid cardRankAndSuit value: " + (cardRankAndSuit ?? "NULL"));
        return;
    }

    print("Card Rank and Suit : " + cardRankAndSuit);

    // Assign rank image
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
            Debug.LogWarning("Unexpected rank: " + cardRankAndSuit[0]);
            break;
    }

    // Assign suit image
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
            Debug.LogWarning("Unexpected suit: " + cardRankAndSuit[1]);
            break;
    }
}

    }
}
