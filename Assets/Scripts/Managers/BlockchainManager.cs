
using System;
//using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using System.Net;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;


// todo: Need to handle errors (I.e. "Returned error: daily request count exceeded, request rate limited")
// todo: Add events for when transactions are completed
// todo: Listen to new NFT event


public partial class BlockchainManager : MonoBehaviour
{ 
    [SerializeField] private List<SmartContract> goerliContracts;
    [SerializeField] private List<SmartContract> mainnetContracts;

    SmartContract nftSmartContract;
    SmartContract nftMinterSmartContract;

    // todo: Load these from scriptable object:
    private string chain = "ethereum";
    private string network = "goerli";
    private string chainId = "5";

    private void Awake()
    {
        foreach (var contract in goerliContracts)
        {
            if (contract.name == "NFT")
            {
                nftSmartContract = contract;
            }
            else if (contract.name == "NFTMinter")
            {
                nftMinterSmartContract = contract;
            }
        }

        Assert.IsNotNull<SmartContract>(nftSmartContract);
        Assert.IsNotNull<SmartContract>(nftMinterSmartContract);
    }

    // =============================== Contract Functions ===============================

    public async UniTask<string> MintPuzzle(string[] data, string tokenURI, string mintFee)
    {
        Assert.AreEqual<int>(data[0].Length, 77);
        Assert.AreEqual<int>(data[1].Length, 77);
        Assert.AreEqual<int>(data[2].Length, 77);
        Assert.AreEqual<int>(data[3].Length, 77);

        // todo: generate image on IPFS and attach token URI
        string puzzleData = $"[\"{data[0]}\",\"{data[1]}\",\"{data[2]}\",\"{data[3]}\"]";
        string args = $"[{puzzleData},\"{tokenURI}\"]";

        // Uncomment for test data
        //string puzzleData = "[\"11111111111111111111111111111111111111111111111111111110011111111111111111153\"," +
        //    "\"11111111111111111111111111111111111111111111111111111111111111111111111111111\"," +
        //    "\"11111111111111111111111111111111111111111111111111111111111111111111111111111\"," +
        //    "\"99999999999999991110110010011111111111111111111111111111111111111111111111111\"]";
        //string args = $"[{puzzleData},\"test\"]";

        print("args: " + args);
        string response = "";
        try
        {
            // todo: Try catch Exception
            response = await CreateTransaction(nftMinterSmartContract.Address, nftMinterSmartContract.ABI, "mint", args, mintFee);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        return response;
    }

    public async UniTask<string> PuzzleExists(string[] data)
    {
        string puzzleData = $"[\"{data[0]}\",\"{data[1]}\",\"{data[2]}\",\"{data[3]}\"]";
        string args = $"[{puzzleData}]";

        return await FetchData(nftSmartContract.Address, nftSmartContract.ABI, "puzzleExists", args);
    }

    public async UniTask<string> SetMintFee(string fee)
    {
        string args = $"[\"{fee}\"]";
        // todo: Try catch Exception
        return await CreateTransaction(nftMinterSmartContract.Address, nftMinterSmartContract.ABI, "setMintFee", args);
    }

    public async UniTask<string> GetMintFee()
    {
        return await FetchData(nftMinterSmartContract.Address, nftMinterSmartContract.ABI, "mintFee");
    }

    public async UniTask<string> GetNumPuzzles()
    {
        // todo: Try catch Exception
        return await FetchData(nftSmartContract.Address, nftSmartContract.ABI, "getNumPuzzles");
    }

    public async UniTask<string> GetPuzzle(int puzzleId)
    {
        // todo: Try catch Exception
        string puzzleData = await FetchData(nftSmartContract.Address, nftSmartContract.ABI, "getPuzzle", $"[{puzzleId}]");
        Debug.Log("puzzleData: " + puzzleData);
        return puzzleData;
    }

    public async UniTask<string> GetPuzzles(int fromId, int toId)
    {
        // todo: Try catch Exception
        string puzzlesData = await FetchData(nftSmartContract.Address, nftSmartContract.ABI, "getPuzzles", $"[{fromId},{toId}]");
        Debug.Log("puzzlesData: " + puzzlesData);
        return puzzlesData;
    }

    // =============================== Experimental Functions ===============================

    private class NFTs
    {
        public string contract { get; set; }
        public string tokenId { get; set; }
        public string uri { get; set; }
        public string balance { get; set; }
    }

    public async Task AllPuzzlesForAccount()
    {
        /*
        string account = "0x09D2cBA3056024c7312DFBcc25EfCa828358285A";

        int first = 500;
        int skip = 0;
        string response = await EVM.AllErc721(chain, network, account, nftSmartContract.Address, first, skip);
        try
        {
            NFTs[] erc721s = JsonConvert.DeserializeObject<NFTs[]>(response);
            print(erc721s[0].contract);
            print(erc721s[0].tokenId);
            print(erc721s[0].uri);
            print(erc721s[0].balance);
        }
        catch
        {
            print("Error: " + response);
        }
        */
    }

    // =============================== Generic Functions ===============================

    private async Task<string> FetchData(string contract, string abi, string method, string args = "[]")
    {
        /*
        string response = await EVM.Call(chain, network, contract, abi, method, args); ;
        Debug.Log($"{method} response: {response}");
        return response;
        */
        return null;
    }

    private async Task<string> CreateTransaction(string contract, string abi, string method, string args, string value = "0")
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return await CreateTransactionForWebGL(contract, abi, method, args, value);
#else
        return await CreateTransactionForWalletConnect(contract, abi, method, args, value);
#endif
    }

    
    private async Task<string> CreateTransactionForWebGL(string contract, string abi, string method, string args, string value)
    {
        /*
        string gasLimit = "";
        string gasPrice = "";
        string response = response = await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, gasPrice);
        Debug.Log($"{method} response: {response}");
        return response;
        */
        return "";
    }

    private async Task<string> CreateTransactionForWalletConnect(string contract, string abi, string method, string args, string value)
    {
        /*
        string gasLimit = "";
        string gasPrice = "";

        Debug.Log($"{method} contract args: {args}");
        string data = await EVM.CreateContractData(abi, method, args);
        Debug.Log($"{method} contract data: {data}");
        string response = await Web3Wallet.SendTransaction(chainId, contract, value, data, gasLimit, gasPrice);
        Debug.Log($"{method} response: {response}");
        return response;
        */
        return null;
    }
}
