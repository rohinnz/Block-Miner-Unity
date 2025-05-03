using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "storyPuzzle.asset", menuName = "Block Miner/Story Puzzle")]
public class StoryPuzzle : ScriptableObject
{
    public int IslandNumber;
    public int PuzzleNumber;

    // Store main part of puzzle in compressed string because
    // we need to work with and test the serilization heaps.
    public string[] Data;

    // Add extra properties here that are specific to story puzzles
}
