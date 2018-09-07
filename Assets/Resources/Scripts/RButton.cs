using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RButton : MonoBehaviour {

    public GameObject buttonObject;
    public Image m_grayImage;

    public void Inactive()
    {
        m_grayImage.gameObject.SetActive(true);
    }

    public void Active()
    {
        m_grayImage.gameObject.SetActive(false);
    }

    public void Hide()
    {
        buttonObject.SetActive(false);
    }

    public void View()
    {
        buttonObject.SetActive(true);
    }

    public bool IsActive()
    {
        return m_grayImage.gameObject.activeSelf;
    }

    public bool IsVisible()
    {
        return buttonObject.activeSelf;
    }
}
