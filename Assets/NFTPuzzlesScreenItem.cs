using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class NFTPuzzlesScreenItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;

    private int nftId;
    private string[] data;

    public void SetData(int nftId, string[] data)
    {
        this.nftId = nftId;
        this.data = data;

        buttonText.text = $"NFT {nftId}";
    }

    public void OnClickButton()
    {


        GM.Screens.TransitionToScreen(ScreenId.Play);
    }
}
