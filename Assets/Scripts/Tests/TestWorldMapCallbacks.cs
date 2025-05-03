using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestWorldMapCallbacks : MonoBehaviour
{
    [SerializeField] TMP_InputField m_numMoves;
    [SerializeField] Toggle m_gotDiamond;

    private string m_puzzleId;


    public void OnClickGiveUp()
    {
        SceneManager.LoadScene("WorldMap");
    }

    public void OnClickCompletePuzzle()
    {
        int numMoves = int.Parse(m_numMoves.text);
        if (numMoves <= 0)
        {
            Debug.LogError("Invalid num moves");
            return;
        }

        GM.State.SetPuzzleCompleted(m_puzzleId, numMoves, m_gotDiamond.isOn);

        SceneManager.LoadScene("WorldMap");
    }

    void Start()
    {
        m_puzzleId = GM.State.TempState.LoadPuzzleId;

        // Set to 20 moves default
        m_numMoves.text = "20";
    }
}
