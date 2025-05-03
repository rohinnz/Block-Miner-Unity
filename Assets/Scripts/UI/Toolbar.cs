using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

/// <summary>
/// Keeps track of which toolbar button is selected
/// </summary>
public class Toolbar : MonoBehaviour
{
    [SerializeField]
    private ToolbarButton[] m_toolbarButtons;

    [SerializeField]
    private Transform m_selectionBox;

    public delegate void SelectionChanged(ToolbarButton selectedButton);
    public event SelectionChanged SelectionChangedEvent;


    private ToolbarButton m_selectedButton = null;
    
    public ToolbarButton SelectedButton
    {
        get {
            return m_selectedButton;
        }
        set {
            UpdateSelection(value);
        }
    }

    private void Start()
    {
        UpdateSelection(m_toolbarButtons[0]);
    }

    private void UpdateSelection(ToolbarButton selectedButton)
    {
        m_selectedButton = selectedButton;
        m_selectionBox.transform.SetParent(m_selectedButton.transform, false);
        SelectionChangedEvent?.Invoke(selectedButton);
    }

    private void OnValidate()
    {
        m_toolbarButtons = GetComponentsInChildren<ToolbarButton>();
    }
}
