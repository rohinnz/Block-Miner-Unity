using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Cysharp.Threading.Tasks;


public class NFTPuzzlesScreen : MonoBehaviour
{
    [SerializeField] private NFTPuzzlesScreenItem itemPrefab;
    [SerializeField] private GameObject content;

    void Start()
    {
        Refresh().Forget();
    }

    private void OnDestroy()
    {
        // todo: Cancel any async task
    }

    public void OnClickRefresh()
    {
        Refresh().Forget();
    }

    public void OnClickBack()
    {
        // todo: Return to title screen
        GM.Screens.TransitionToScreen(ScreenId.Title);
    }

    private async UniTask Refresh()
    {
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }

        // todo: Show loading spinner


        string numPuzzlesResponse = await GM.Blockchain.GetNumPuzzles();
        int fromId = 1;
        int toId = int.Parse(numPuzzlesResponse);
        string puzzlesResponse = await GM.Blockchain.GetPuzzles(fromId, toId);


        // Unity's JsonUtility does not parse arrays, so just split it
        string[] puzzleDataStrs = puzzlesResponse.Replace(" ", "").Replace("\"", "").Replace("\t", "").Replace("\n", "").Split("],[");

        int nftId = fromId;
        foreach (var s in puzzleDataStrs)
        {
            string[] puzzleData = s.Replace("[", "").Replace("]", "").Split(',');
            NFTPuzzlesScreenItem item = Instantiate<NFTPuzzlesScreenItem>(itemPrefab, content.transform);
            item.SetData(nftId, puzzleData);

            ++nftId;
        }
    }
}
