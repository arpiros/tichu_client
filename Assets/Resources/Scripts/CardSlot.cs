using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSlot : MonoBehaviour, IDropHandler {

    public CardData m_card;

    public void OnDrop(PointerEventData eventData)
    {
        RectTransform rect = transform as RectTransform;

        if (!RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition))
        {
            Debug.Log("Drop");
        }
    }
}
