using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextGradation : MonoBehaviour {

    public Gradient m_graident;

    [Range(0, 1)]
    public float t;

    public Image m_image;

	// Use this for initialization
	void Start () {

        m_image.color = m_graident.Evaluate(t);
    }
}
