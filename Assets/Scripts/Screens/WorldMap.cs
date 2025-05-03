using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap : MonoBehaviour
{
    [SerializeField] WorldMapIsland m_worldMapIsland1;

    //[SerializeField] WorldMapPuzzle[] m_worldMapIslands;

    // Start is called before the first frame update
    void Start()
    {
        //GM.State.TempState.PuzzleJustCompleted

        // todo: First read value in order to animate new puzzles unlocked
        GM.State.TempState.PuzzleJustCompleted = null;


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RefreshPuzzleStates()
    {
        //m_worldMapIslands

        var otherVisibleIslands = new List<WorldPuzzleButton>();

        // todo: start with all buttons set to be inactive

        /*
        foreach (var puzzle in m_worldMapIsland1.Puzzles)
        {
            if (GM.State.IsPuzzleUnlocked(puzzle.PuzzleId))
            {
                if (puzzle.PuzzleId == GM.State.TempState.PuzzleJustCompleted)
                {
                    // todo: Play completion animation
                }

                foreach (var neighbor in puzzle.Neighbors)
                {
                    if (!GM.State.IsPuzzleUnlocked(neighbor.Puzzle.PuzzleId))
                    {
                        // todo: Change color and animate slightly
                    }
                }
            }
        }*/

        // Algorithm:
        // Island 1 is visible by default

        // Iterate through 
    }

    private void UpdateIslandStates()
    {
        
    }

    public void OnClickMainMenu()
    {
        GM.Screens.TransitionToScreen(ScreenId.Title);
    }
}
