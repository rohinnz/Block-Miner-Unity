using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Temp data that doesn't get saved
public class TempState
{
    public string LoadPuzzleId;
    public string PuzzleJustCompleted;
}

[Serializable]
public class PuzzleAchievement
{
    public bool Diamond;
    public int MinMoves;
}

// Data that gets saved
[Serializable]
public class SaveState
{
    public Dictionary<string, PuzzleAchievement> PuzzleAchievements = new();
}

// todo: Rename to StateManager
public class SaveStateManager : MonoBehaviour
{
    public TempState TempState { get; private set; } = new TempState();
    public SaveState SaveState { get; private set; } = new SaveState();

    public void SetPuzzleCompleted(string puzzleId, int minMoves, bool gotDiamond)
    {
        if (SaveState.PuzzleAchievements.TryGetValue(puzzleId, out PuzzleAchievement achievement))
        {
            if (gotDiamond)
                achievement.Diamond = true;

            if (minMoves < achievement.MinMoves)
                achievement.MinMoves = minMoves;
        }
        else
        {
            SaveState.PuzzleAchievements.Add(puzzleId, new PuzzleAchievement()
            {
                Diamond = gotDiamond,
                MinMoves = minMoves
            });

            TempState.PuzzleJustCompleted = puzzleId;
        }
    }

    public bool IsPuzzleUnlocked(string puzzleId)
    {
        return SaveState.PuzzleAchievements.ContainsKey(puzzleId);
    }

    private void Awake()
    {
 
    }
}
