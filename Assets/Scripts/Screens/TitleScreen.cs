using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public void OnClickStory()
    {
        GM.Screens.TransitionToScreen(ScreenId.WorldMap);
    }

    public void OnClickCommunityPuzzles()
    {
        GM.Screens.TransitionToScreen(ScreenId.CommunityPuzzles);
    }

    public void OnClickPuzzleEditor()
    {
        GM.Screens.TransitionToScreen(ScreenId.Editor);
    }

    public void OnClickSettings()
    {

    }

    public void OnClickConnectWallet()
    {

    }
}
