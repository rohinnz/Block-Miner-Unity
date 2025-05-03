using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

public partial class BlockchainManager
{
    [DllImport("__Internal")]
    private static extern void Web3Connect();

    [DllImport("__Internal")]
    private static extern string ConnectAccount();

    [DllImport("__Internal")]
    private static extern void SetConnectAccount(string value);


    public async Task<bool> Login(bool rememberMe)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return await WebLogin(rememberMe);
#else
        return await WalletLogin(rememberMe);
#endif
    }

    public void Logout()
    {
        // todo: Check if there is anything extra we need to do for WebGL Logout

        PlayerPrefs.DeleteKey("Account");
        PlayerPrefs.DeleteKey("RememberMe");
    }

    public (string, bool) GetAccount()
    {
        return (PlayerPrefs.GetString("Account", ""), PlayerPrefs.GetInt("RememberMe", 0) == 1);
    }

    private async Task<bool> WebLogin(bool rememberMe)
    {
        /*
        Web3Connect();
        string account = ConnectAccount();
        while (account == "")
        {
            await new WaitForSeconds(1f);
            account = ConnectAccount();
        };

        // save account for next scene
        PlayerPrefs.SetString("Account", account);
        // reset login message
        SetConnectAccount("");
        PlayerPrefs.SetInt("RememberMe", rememberMe ? 1 : 0);
        */

        return true;
    }

    // Non-WebGL. I.e Desktop, Mobile
    private async Task<bool> WalletLogin(bool rememberMe)
    {
        /*

        // get current timestamp
        int timestamp = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
        // set expiration time
        int expirationTime = timestamp + 60;
        // set message
        string message = expirationTime.ToString();
        // sign message
        string signature = await Web3Wallet.Sign(message);
        // verify account
        string account = await EVM.Verify(message, signature);
        int now = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
        // validate
        if (account.Length == 42 && expirationTime >= now)
        {
            // save account
            PlayerPrefs.SetString("Account", account);
            PlayerPrefs.SetInt("RememberMe", rememberMe ? 1 : 0);
            print("Account: " + account);

            return true;
        }
        else
        {
            return false;
        }

        */

        return false;
    }
}
