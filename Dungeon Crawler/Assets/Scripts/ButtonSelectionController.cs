using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ButtonSelectionController : MonoBehaviour
{
    [SerializeField]
    private float               m_lerpTime;
    private ScrollRect          m_scrollRect;
    private Button[]            m_buttons;
    private int                 m_index;
    private float               m_verticalPosition;
    private bool                m_up;
    private bool                m_down;

    public void Start()
    {
        m_scrollRect        = GetComponent<ScrollRect>();
        m_buttons           = GetComponentsInChildren<Button>();
        m_buttons[m_index].Select();
        m_verticalPosition  = 1f - ((float)m_index / (m_buttons.Length - 1));
    }

    public void Update()
    {
        m_up    = Input.GetButtonDown("Cima");
        m_down  = Input.GetButtonDown("Baixo");

        if (m_up ^ m_down)
        {
            if (m_up){
                m_index = m_index - 1;
                if (m_index < 0){
                    m_index = m_buttons.Length-1;
                }
            }
            else{
                m_index = m_index + 1;
                if(m_index >= m_buttons.Length){
                    m_index = 0; 
                }
            }
            m_buttons[m_index].Select();
            m_verticalPosition = 1f - ((float)m_index / (m_buttons.Length - 1));
        }

        m_scrollRect.verticalNormalizedPosition = Mathf.Lerp(m_scrollRect.verticalNormalizedPosition, m_verticalPosition, Time.deltaTime / m_lerpTime);
    }
}
