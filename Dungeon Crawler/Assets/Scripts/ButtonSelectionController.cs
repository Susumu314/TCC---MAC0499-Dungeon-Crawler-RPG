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
    private List<Button>        m_buttons = new List<Button>();
    private int                 m_index;
    private float               m_verticalPosition;
    private bool                m_up;
    private bool                m_down;

    public void Start()
    {
        m_scrollRect        = GetComponent<ScrollRect>();
        foreach (Button button in GetComponentsInChildren<Button>()){
            m_buttons.Add(button);
        };
        m_buttons[m_index].Select();
        m_verticalPosition  = 1f - ((float)m_index / (m_buttons.Count - 1));
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
                    m_index = m_buttons.Count - 1;
                }
            }
            else{
                m_index = m_index + 1;
                if(m_index >= m_buttons.Count){
                    m_index = 0; 
                }
            }
            while(!m_buttons[m_index]){
                m_buttons.RemoveAt(m_index);
            }
            m_buttons[m_index].Select();
            m_verticalPosition = 1f - ((float)m_index / (m_buttons.Count - 1));
        }

        m_scrollRect.verticalNormalizedPosition = Mathf.Lerp(m_scrollRect.verticalNormalizedPosition, m_verticalPosition, Time.deltaTime / m_lerpTime);
    }
}
