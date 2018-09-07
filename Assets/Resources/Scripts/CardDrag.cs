using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    Vector3 startPosition;
    Transform startParent;
    Vector3 originPosition;
    Quaternion originRotation;
    int originSiblingIdx = 0;

    bool isfirstPosition = false;

    public CardData m_card;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.rutine != RutineState.ExchangeState)
        {
            return;
        }

        startPosition = transform.position;

        if (!isfirstPosition)
        {
            originPosition = transform.position;
            originRotation = transform.rotation;

            isfirstPosition = true;
        }
        originSiblingIdx = transform.GetSiblingIndex();

        transform.SetSiblingIndex(originSiblingIdx + 60);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.rutine != RutineState.ExchangeState)
        {
            return;
        }

        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.rutine != RutineState.ExchangeState)
        {
            return;
        }

        //드래그 끝 부분이 어느슬롯과 충돌한건지 확인해보자.
        RectTransform leftform = GameManager.Instance.m_ExchangeUi.m_leftCard.transform as RectTransform;
        RectTransform partnerform = GameManager.Instance.m_ExchangeUi.m_partnerCard.transform as RectTransform;
        RectTransform rightform = GameManager.Instance.m_ExchangeUi.m_rightCard.transform as RectTransform;

        if (RectTransformUtility.RectangleContainsScreenPoint(leftform, Input.mousePosition, Camera.main))
        {
            transform.position = GameManager.Instance.m_ExchangeUi.m_leftCard.transform.position;
            transform.rotation = GameManager.Instance.m_ExchangeUi.m_leftCard.transform.rotation;

            m_card.m_ownerPlayer.AddExchangeCard(m_card, 0);
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(partnerform, Input.mousePosition, Camera.main))
        {
            transform.position = GameManager.Instance.m_ExchangeUi.m_partnerCard.transform.position;
            transform.rotation = GameManager.Instance.m_ExchangeUi.m_partnerCard.transform.rotation;

            m_card.m_ownerPlayer.AddExchangeCard(m_card, 1);
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(rightform, Input.mousePosition, Camera.main))
        {
            transform.position = GameManager.Instance.m_ExchangeUi.m_rightCard.transform.position;
            transform.rotation = GameManager.Instance.m_ExchangeUi.m_rightCard.transform.rotation;

            m_card.m_ownerPlayer.AddExchangeCard(m_card, 2);
        }
        else
        {
            transform.position = originPosition;
            transform.rotation = originRotation;
        }

        transform.SetSiblingIndex(originSiblingIdx);
    }
}
