using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ScreenId
{
    Title,
    WorldMap,
    Play,
    Editor,
    CommunityPuzzles,
    Settings
}

public class PlayScreenArgs
{
    public bool IsStory;
    public int IslandNumber;
    public int PuzzleNumber;
    public string Filename;
}

public class ScreenManager : MonoBehaviour
{
    private PlayScreenArgs playScreenArgs;
    public PlayScreenArgs GetPlayScreenArgs() { return playScreenArgs; }

    public Object ScreenArgs { get; private set; }

    [SerializeField] private List<StoryPuzzle> storyPuzzles;
    private Dictionary<string, StoryPuzzle> storyPuzzlesDict = new Dictionary<string, StoryPuzzle>();

    private ScreenId currentScreenId;
    private ScreenId previousScreenId;

    public StoryPuzzle GetStoryPuzzle(int islandNumber, int puzzleNumber)
    {
        return storyPuzzlesDict[$"{islandNumber}-{puzzleNumber}"];
    }

    Dictionary<ScreenId, string> screenIdToScene = new Dictionary<ScreenId, string>()
    {
        { ScreenId.Title, "TitleScreen" },
        { ScreenId.WorldMap, "WorldMapScreen" },
        { ScreenId.Play, "PlayScreen" },
        { ScreenId.Editor, "EditorScreen" },
    };

    private void Awake()
    {
        foreach (var puzzle in storyPuzzles)
        {
            storyPuzzlesDict.Add($"{puzzle.IslandNumber}-{puzzle.PuzzleNumber}", puzzle);
        }
    }

    public void TransitionToScreen(ScreenId screenId, Object screenArgs = null)
    {
        ScreenArgs = screenArgs;
        string sceneName = screenIdToScene[screenId];
        previousScreenId = currentScreenId;
        currentScreenId = screenId;

        // todo: Later load async with transition animation
        SceneManager.LoadScene(sceneName);
    }

    public void TransitionToPreviousScreen()
    {
        TransitionToScreen(previousScreenId);
    }

    public void TransitionToPlayStory(int islandNumber, int puzzleNumber)
    {
        playScreenArgs = new PlayScreenArgs()
        {
            Filename = "story",
            IsStory = true,
            IslandNumber = islandNumber,
            PuzzleNumber = puzzleNumber
        };

        TransitionToScreen(ScreenId.Play);
    }
}
