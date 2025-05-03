using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_numMoves;
    [SerializeField] TextMeshProUGUI m_numCrystals;
    [SerializeField] TextMeshProUGUI m_numPicks;
    [SerializeField] TextMeshProUGUI m_numLadders;
    [SerializeField] TextMeshProUGUI m_numSoftBlocks;

    public void UpdateState(int numMoves, int numPicks, int numSoftBlocks, int numLadders)
    {
        m_numMoves.text = numMoves.ToString();
        m_numPicks.text = numPicks.ToString();
        m_numLadders.text = numLadders.ToString();
        m_numSoftBlocks.text = numSoftBlocks.ToString();
    }
}
