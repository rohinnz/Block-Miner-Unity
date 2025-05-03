#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using TMPro;
using UnityEngine.Assertions;

/// <summary>
/// Editor only script for editing story puzzles
/// </summary>
public class StoryPuzzleEditor : MonoBehaviour
{
    [SerializeField] private TMP_InputField islandNumberInput;
    [SerializeField] private TMP_InputField puzzleNumberInput;
    [SerializeField] private PuzzleGrid puzzleGrid;

    public void OnClickSaveStory()
    {
        Puzzle puzzle = puzzleGrid.ToPuzzle();

        StoryPuzzle storyPuzzle = ScriptableObject.CreateInstance<StoryPuzzle>();
        storyPuzzle.Data = GM.LevelSerializer.SerializePuzzle(puzzle);
        storyPuzzle.IslandNumber = int.Parse(islandNumberInput.text);
        storyPuzzle.PuzzleNumber = int.Parse(puzzleNumberInput.text);

        string filepath = PuzzleFilepath();
        AssetDatabase.CreateAsset(storyPuzzle, filepath);
        AssetDatabase.SaveAssets();
    }

    public void OnClickLoadStory()
    {
        string filepath = PuzzleFilepath();
        StoryPuzzle storyPuzzle = AssetDatabase.LoadAssetAtPath<StoryPuzzle>(filepath);
        Puzzle puzzle = GM.LevelSerializer.LoadFromSerializedStrings(storyPuzzle.Data);
        puzzleGrid.LoadPuzzle(puzzle);
    }

    private string PuzzleFilepath()
    {
        int islandNumber = int.Parse(islandNumberInput.text);
        int puzzleNumber = int.Parse(puzzleNumberInput.text);
        Assert.IsTrue(islandNumber > 0);
        Assert.IsTrue(puzzleNumber > 0);

        return $"Assets/Data/StoryPuzzles/{islandNumber}-{puzzleNumber}.asset";
    }
}
#endif