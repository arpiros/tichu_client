using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonTrigger : MonoBehaviour, IPointerUpHandler, IPointerDownHandler {

    public Image m_showdows;
    public float m_scaleValue;

    public void OnPointerDown(PointerEventData eventData)
    {
        transform.localScale -= new Vector3(m_scaleValue, m_scaleValue, 0.0f);
        m_showdows.transform.localScale -= new Vector3(m_scaleValue, m_scaleValue, 0.0f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.localScale += new Vector3(m_scaleValue, m_scaleValue, 0.1f);
        m_showdows.transform.localScale += new Vector3(m_scaleValue, m_scaleValue, 0.0f);
    }
}
