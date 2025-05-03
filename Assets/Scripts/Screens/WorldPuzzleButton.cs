using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor.Events;
#endif

public class WorldPuzzleButton : MonoBehaviour
{
    public enum State
    {
        Inaccessible,
        Available,
        Completed
    }

    private State state;


    [Serializable]
    public struct PuzzleNeighbor
    {
        public WorldPuzzleButton Puzzle;
        public GameObject PathObj;
    }

    [Serializable]
    public struct IslandCanUnlock
    {
        public WorldMapIsland Island;
        public WorldPuzzleButton Puzzle;
        public GameObject PathObj;
    }

    [SerializeField] int m_puzzleNumber;
    [SerializeField] PuzzleNeighbor[] neighbors;
    [SerializeField] IslandCanUnlock[] islandsCanUnlock;
    [SerializeField] WorldMapIsland m_island;
    [SerializeField] Image m_image;

    public PuzzleNeighbor[] Neighbors => neighbors;

    public void OnClickPuzzle()
    {
        GM.Screens.TransitionToPlayStory(m_island.IslandNumber, m_puzzleNumber);
    }

    public void SetState(State state)
    {
        if (state == State.Inaccessible)
        {

        }
    }

    public string GetPuzzleId()
    {
        return $"{m_island.IslandNumber}_{m_puzzleNumber}";
    }

#if UNITY_EDITOR
    private void Reset()
    {
        m_island = GetComponentInParent<WorldMapIsland>();
        Button button = GetComponent<Button>();

        // Add listener if none already set. This still allows for custom listener to be set
        if (button.onClick.GetPersistentEventCount() == 0)
        {
            UnityEventTools.AddPersistentListener(button.onClick, OnClickPuzzle);
        }
    }
#endif
}
