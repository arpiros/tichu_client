using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WishCardBtn : MonoBehaviour {

    //Button Image의 sprite를 변경해서 선택된 버튼의 색깔이 바뀌도록 한다.
    public Image m_image;
    public Sprite m_normalImg;
    public Sprite m_selectImg;

    //text값을 읽어와서 소원이 값인지 확인한다.
    public Text m_text;

    //선택됬는지 여부를 결정한다.
    public bool m_bisSelect = false;

    public void SelectBtn()
    {
        m_bisSelect = !m_bisSelect;

        if (m_bisSelect)
        {
            m_image.sprite = m_selectImg;
            GameManager.Instance.m_wishCardPopup.SetSelectedBtn(this);
        }
        else
        {
            m_image.sprite = m_normalImg;
            GameManager.Instance.m_wishCardPopup.UnselectBtn();
        }
    }

    public void UnselectBtn()
    {
        m_bisSelect = false;
        m_image.sprite = m_normalImg;
    }

}
