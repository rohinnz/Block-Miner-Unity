using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapIsland : MonoBehaviour
{
    [SerializeField]
    private int m_islandNumber;

    public int IslandNumber => m_islandNumber;


    [SerializeField]
    private WorldPuzzleButton[] m_puzzles;

    public WorldPuzzleButton[] Puzzles => m_puzzles;


    //private bool m_isVisible;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateIslandButtons()
    {

    }
}
