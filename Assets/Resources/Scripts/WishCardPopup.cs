using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WishCardPopup : MonoBehaviour {

    public WishCardBtn m_selectedBtn;
    private string m_selectValue = "";
    private bool m_bIsSelected = false;

    public void Init()
    {
        m_selectValue = "";
        m_bIsSelected = false;

        if (m_selectedBtn != null)
        {
            m_selectedBtn.UnselectBtn();
        }
    }

    public void SetSelectedBtn(WishCardBtn selectBtn)
    {
        //기존의 선택된 버튼의 셀렉트를 해제한다.
        if (m_selectedBtn != null)
        {
            m_selectedBtn.UnselectBtn();
        }

        m_selectedBtn = selectBtn;
        m_selectValue = selectBtn.m_text.text;
    }

    public void UnselectBtn()
    {
        m_selectedBtn = null;
        m_selectValue = "";
    }

    public void PushSelectBtn()
    {
        if (m_selectedBtn == null)
        {
            return;
        }

        //소원이 없으면 없는것처럼 하자
        if (m_selectValue != "X")
        {
            m_bIsSelected = true;
        }
        
        //비활성화 해서 창을 닫는다.
        this.gameObject.SetActive(false);
    }

    public float GetWishCardValue()
    {
        switch (m_selectValue)
        {
            case "2":
            case "3":
            case "4":
            case "5":
            case "6":
            case "7":
            case "8":
            case "9":
            case "10":
                {
                    return float.Parse(m_selectValue);
                }
            case "J": { return 11.0f; }
            case "Q": { return 12.0f; }
            case "K": { return 13.0f; }
            case "A": { return 14.0f; }
        }

        return 0.0f;
    }

    public string GetWishCard()
    {
        return m_selectValue;
    }

    public bool Selected()
    {
        return m_bIsSelected;
    }

    public void NPCPlayerCallWish(string wish)
    {
        if (wish != "X")
        {
            m_selectValue = wish;
            m_bIsSelected = true;
        }
    }

    public void AchieveWish()
    {
        m_bIsSelected = false;
    }

}
