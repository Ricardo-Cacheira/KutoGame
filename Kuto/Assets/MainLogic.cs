using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainLogic : MonoBehaviour
{
    [SerializeField]
    private GameApi _api;

    [SerializeField]
    private string _dataForSave = string.Empty;

    [SerializeField]
    private InputField _usernameField, _passwordField;

    // Use this for initialization
    void Awake()
    {
        if (_api == null)
            _api = FindObjectOfType<GameApi>();

        if (_api == null)
            Debug.LogError("'Api' field must be set!");
    }

    public void OnRegisterButtonClick()
    {
        _api.Register(_usernameField.text, _passwordField.text, (bool error, string data) =>
        {
            if (error)
                Debug.LogError("Error:" + data);
            else
                Debug.Log("Response:" + data);
        });
    }

    public void OnGetDataButtonClick()
    {
        _api.GetData(_usernameField.text, _passwordField.text, (bool error, string data) =>
        {
            if (error)
                Debug.LogError("Error:" + data);
            else
            {
                Debug.Log("Response:" + data);

                //Example of using JSON parser 'SimpleJSON' http://wiki.unity3d.com/index.php/SimpleJSON
                //try parse json and get field 'account'->'data'

                var json = SimpleJSON.JSON.Parse(data);

                var account = json["account"];

                var accountData = account["data"];

                if (accountData["coins"] != null)
                {
                    var coins = accountData["coins"].AsInt;

                    Debug.Log("get coins:" + coins);
                }
                else
                {
                    Debug.Log("No field 'account.data.coins'!");
                }
            }
        });
    }

    public void OnSaveDataButtonClick()
    {
        _api.SaveData(_usernameField.text, _passwordField.text, _dataForSave, (bool error, string data) =>
        {
            if (error)
                Debug.LogError("Error:" + data);
            else
                Debug.Log("Response:" + data);
        });
    }
}
