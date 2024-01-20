using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject Button;
    public void OnPointerEnter(PointerEventData eventData)
    {
        Button.GetComponent<Image>().color = Color.grey;
        Debug.Log("Above End Turn");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Button.GetComponent<Image>().color = Color.white;
        Debug.Log("Exited End Turn");
    }
}
