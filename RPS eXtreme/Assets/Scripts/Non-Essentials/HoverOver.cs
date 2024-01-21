using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject GOButton;
    public void OnPointerEnter(PointerEventData eventData)
    {
        GOButton.GetComponent<Image>().color = Color.grey;
        GOButton.GetComponent<AudioSource>().Play();
        GOButton.GetComponent<Button>().onClick.AddListener(delegate { OnClick(); });
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GOButton.GetComponent<Image>().color = Color.white;
    }

    public void OnClick(){
        GOButton.GetComponent<Image>().color = Color.white;
    }
}
