using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SmartContractTests : MonoBehaviour
{
    [Header("Get/Set Mint Fee")]
    [SerializeField] TMP_InputField m_mintFee;

    [Header("Get Puzzles")]
    [SerializeField] TMP_InputField m_puzzleToId;
    [SerializeField] TMP_InputField m_puzzleFromId;

    [Header("Get Puzzle")]
    [SerializeField] TMP_InputField m_puzzleId;

    [Header("Mint Puzzle")]
    [SerializeField] TMP_InputField m_data1;
    [SerializeField] TMP_InputField m_data2;
    [SerializeField] TMP_InputField m_data3;
    [SerializeField] TMP_InputField m_data4;
    [SerializeField] TMP_InputField m_tokenURI;

    [Header("Wallet Connect")]
    [SerializeField] TextMeshProUGUI m_walletButtonText;
    [SerializeField] TextMeshProUGUI m_walletAddress;
    [SerializeField] Toggle m_rememberMe;
    
    void Start()
    {
        // Populate example level data
        m_data1.text = "11111111111111111111111111111111111111111111111111111110011111111111111111153";
        m_data2.text = "11111111111111111111111111111111111111111111111111111111111111111111111111111";
        m_data3.text = "11111111111111111111111111111111111111111111111111111111111111111111111111111";
        m_data4.text = "99999999999999991110110010011111111111111111111111111111111111111111111111111";

        RefreshWalletUI();
    }

    public async void OnClickConnectWallet()
    {
        (string account, _) = GM.Blockchain.GetAccount();
        if (string.IsNullOrEmpty(account))
        {
            bool success = await GM.Blockchain.Login(m_rememberMe.isOn);
            print("Login success: " + success);
        }
        else
        {
            GM.Blockchain.Logout();
        }

        RefreshWalletUI();
    }

    public async void OnClickSetMintFee()
    {
        string response = await GM.Blockchain.SetMintFee(m_mintFee.text);
        Debug.Log("OnClickSetMintFee() response: " + response);
    }

    public async void OnClickGetMintFee()
    {
        string mintFee = await GM.Blockchain.GetMintFee();
        Debug.Log("OnClickGetMintFee() response: " + mintFee);
    }

    public async void OnClickGetPuzzles()
    {
        string puzzles = await GM.Blockchain.GetPuzzles(int.Parse(m_puzzleFromId.text), int.Parse(m_puzzleToId.text));
        Debug.Log("OnClickGetPuzzles() response: " + puzzles);
    }

    public async void OnClickGetPuzzle()
    {
        string puzzle = await GM.Blockchain.GetPuzzle(int.Parse(m_puzzleId.text));
        Debug.Log("OnClickGetPuzzle() response: " + puzzle);
    }

    public async void OnClickGetNumPuzzles()
    {
        string numPuzzles = await GM.Blockchain.GetNumPuzzles();
        Debug.Log("OnClickGetNumPuzzles() response: " + numPuzzles);
    }

    public async void OnClickMintPuzzle()
    {
        string puzzleExistsResponse = await GM.Blockchain.PuzzleExists(new string[] {m_data1.text, m_data2.text, m_data3.text, m_data4.text});
        print("OnClickMintPuzzle() response: " + puzzleExistsResponse);

        if (bool.Parse(puzzleExistsResponse))
        {
            Debug.LogError("Puzzle Already Exists!");
            return;
        }

        string mintFee = await GM.Blockchain.GetMintFee();
        string response = await GM.Blockchain.MintPuzzle(new string[]{m_data1.text, m_data2.text, m_data3.text, m_data4.text}, m_tokenURI.text, mintFee);
        print("OnClickMintPuzzle() response: " + response);
    }

    public async void OnClickAllAccountNFTs()
    {
        await GM.Blockchain.AllPuzzlesForAccount();
    }

    private void RefreshWalletUI()
    {
        // todo: Need to handle WebGL too

        (string account, bool rememberMe) = GM.Blockchain.GetAccount();

        if (string.IsNullOrEmpty(account))
        {
            m_walletButtonText.text = "Login";
            m_rememberMe.gameObject.SetActive(true);
            m_rememberMe.isOn = rememberMe;
        }
        else
        {
            m_walletButtonText.text = "Logout";
            m_rememberMe.gameObject.SetActive(false);
        }

        m_walletAddress.text = account;
    }
}
