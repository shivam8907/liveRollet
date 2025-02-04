using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyLogoSwitch : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Sprite normalSprite;
    public Sprite hoverSprite;
    private Image image;

    void Start()
    {
        image = transform.GetComponent<Image>();
        if (image != null)
            normalSprite = transform.GetComponent<Image>().sprite;
    }

    void OnMouseEnter()
    {
        Debug.Log("Mouse Entered on Logo");
        if (image != null)
            image.sprite = hoverSprite;
    }

    private void OnMouseExit()
    {
        Debug.Log("Mouse Exit from Logo");
        if (image != null)
            image.sprite = normalSprite;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnMouseEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnMouseExit();
    }
}
