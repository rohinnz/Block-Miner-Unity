using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
#endif

public class ToolbarButton : MonoBehaviour
{
    public enum BrushType
    {
        Eraser,
        Tile,
        Object
    }

    [SerializeField]
    private BrushType m_type;

    [SerializeField]
    private TileBase m_tile;

    [SerializeField]
    private Sprite m_sprite;

    [SerializeField]
    private Toolbar m_toolbar;

    [SerializeField]
    private GameObject[] m_spriteObjects;

    public GameObject[] SpriteObjects => m_spriteObjects;

    private int m_spriteObjectIndex = 0;


    public BrushType GetBrushType() { return m_type; }

    public TileBase Tile => m_tile;

    public Sprite Sprite => m_sprite;

    private void Awake()
    {
        
    }

    private void OnClick()
    {
        m_toolbar.SelectedButton = this;
    }

    public GameObject NextSpriteObject()
    {
        if (m_spriteObjectIndex >= m_spriteObjects.Length - 1)
        {
            m_spriteObjectIndex = 0;
        }
        else
        {
            ++m_spriteObjectIndex;
        }
        return m_spriteObjects[m_spriteObjectIndex];
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        m_toolbar = GetComponentInParent<Toolbar>();
        m_sprite = GetComponent<Image>().sprite;

        // Add listener if none already set. This still allows for custom listener to be set
        var button = GetComponent<Button>();
        if (button.onClick.GetPersistentEventCount() == 0)
        {
            UnityEventTools.AddPersistentListener(button.onClick, OnClick);
        }
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(ToolbarButton))]
public class ToolbarButtonEditor : Editor
{
    SerializedProperty m_type;
    SerializedProperty m_tile;
    SerializedProperty m_spriteObjects;

    private void OnEnable()
    {
        m_type = serializedObject.FindProperty("m_type");
        m_tile = serializedObject.FindProperty("m_tile");
        m_spriteObjects = serializedObject.FindProperty("m_spriteObjects");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_type);

        ToolbarButton.BrushType t = (ToolbarButton.BrushType)m_type.enumValueIndex;
        if (t == ToolbarButton.BrushType.Eraser)
        {
        }
        else if (t == ToolbarButton.BrushType.Tile)
        {
            EditorGUILayout.PropertyField(m_tile);
        }
        else
        {
            EditorGUILayout.PropertyField(m_spriteObjects);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
