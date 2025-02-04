using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CoinChipController : MonoBehaviour
{
    
    public static int SelectedCoinAmt; // Static variable to store the selected coin amount
    public GameObject selectedChipBackground; // Reference to the currently selected chip background
    public float rotationSpeed = 100f; // Speed of rotation in degrees per second
public GameObject clickedChip;
    private bool isRotating = false;
    
 public Toggle firstDefaultChip;
  private void Start()
    {
        // Ensure the default chips are selected at the start
        if (firstDefaultChip != null)
        {
            firstDefaultChip.isOn = true;
            OnToggleValueChanged(true);
        }
    }
    public void OnToggleValueChanged(bool isOn)
    {
        // Ensure this is the currently toggled "on" chip
        if (!isOn)
            return;

        // Get the clicked chip (current toggle)
     clickedChip = EventSystem.current.currentSelectedGameObject;

        if (clickedChip == null)
        {
            Debug.LogWarning("No chip selected.");
            return;
        }

        // Get the CoinChip component to access the coin value
        CoinChip coinChip = clickedChip.GetComponent<CoinChip>();
        if (coinChip == null)
        {
            Debug.LogError("The clicked chip does not have a CoinChip component.");
            return;
        }

        int coinAmount = coinChip.coinValue;

        // Find the Background GameObject (child of the toggle)
        Transform chipBackgroundTransform = clickedChip.transform.Find("Background");
        if (chipBackgroundTransform != null)
        {
            GameObject chipBackground = chipBackgroundTransform.gameObject;

            // Rotate the selected chip
            ChipRotate(coinAmount, chipBackground);
        }
    }

    public void ChipRotate(int coinAmount, GameObject chipBackground)
    {
        SelectedCoinAmt = coinAmount;

        // Stop rotating the previously selected chip background, if any
        if (selectedChipBackground != null && selectedChipBackground != chipBackground)
        {
            StopRotating(selectedChipBackground);
        }

        // Assign the new chip background and start rotating it
        selectedChipBackground = chipBackground;

        if (selectedChipBackground != null)
        {
            StartRotating(selectedChipBackground);
        }
    }

    private void StartRotating(GameObject chipBackground)
    {
        isRotating = true;
    }

    private void StopRotating(GameObject chipBackground)
    {
        isRotating = false;
        chipBackground.transform.rotation = Quaternion.identity; // Reset rotation
    }

    private void Update()
    {
        if (isRotating && selectedChipBackground != null)
        {
            // Rotate the selected chip background clockwise
            selectedChipBackground.transform.Rotate(Vector3.forward * -rotationSpeed * Time.deltaTime);
        }
    }
}
