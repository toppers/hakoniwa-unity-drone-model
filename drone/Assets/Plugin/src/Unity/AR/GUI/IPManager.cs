using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IPManager : MonoBehaviour
{
    public InputField server_ipInputField;
    public InputField client_ipInputField;
    public InputField session_positionField;
    public InputField session_rotationField;

    public Button saveButton;
    public Button adjustButton;
    public Button startButton;
    public GameObject savePopupPanel;
    public string main_scene = "ARDevice";
    public string adjust_scene = "ARAdjust";
    private string server_savedIPKey = "server_savedIP";
    private string client_savedIPKey = "client_savedIP";

    void Start()
    {
        // 保存されたIPアドレスを読み込み、InputFieldに設定
        string server_savedIP = PlayerPrefs.GetString(server_savedIPKey, "127.0.0.1:54002");
        server_ipInputField.text = server_savedIP;
        string client_savedIP = PlayerPrefs.GetString(client_savedIPKey, "127.0.0.1:54001");
        client_ipInputField.text = client_savedIP;

        LoadData();

        // ボタンのクリックイベントにメソッドを登録
        saveButton.onClick.AddListener(SaveIP);
        startButton.onClick.AddListener(StartSim);
        if (adjustButton)
        {
            adjustButton.onClick.AddListener(Adjust);
        }

        // ポップアップパネルを非表示に設定
        savePopupPanel.SetActive(false);
    }
    private void OnEnable()
    {
        LoadData();
    }
    private void LoadData()
    {
        string session_savedPos = PlayerPrefs.GetString(OriginUI.session_savedPosKey, "0,0,0");
        session_positionField.text = session_savedPos;
        string session_savedRot = PlayerPrefs.GetString(OriginUI.session_savedRotKey, "0,0,0");
        session_rotationField.text = session_savedRot;

    }

    void SaveIP()
    {
        // InputFieldのテキストを保存
        string server_ipToSave = server_ipInputField.text;
        PlayerPrefs.SetString(server_savedIPKey, server_ipToSave);
        string client_ipToSave = client_ipInputField.text;
        PlayerPrefs.SetString(client_savedIPKey, client_ipToSave);

        string session_savedPos = session_positionField.text;
        PlayerPrefs.SetString(OriginUI.session_savedPosKey, session_savedPos);
        string session_savedRot = session_rotationField.text;
        PlayerPrefs.SetString(OriginUI.session_savedRotKey, session_savedRot);


        PlayerPrefs.Save();

        // ポップアップパネルを表示
        StartCoroutine(ShowPopup());
    }

    System.Collections.IEnumerator ShowPopup()
    {
        // ポップアップパネルを表示
        savePopupPanel.SetActive(true);

        // 2秒待機
        yield return new WaitForSeconds(2);

        savePopupPanel.SetActive(false);
    }
    void StartSim()
    {
        SceneManager.LoadScene(main_scene);
    }
    void Adjust()
    {
        SceneManager.LoadScene(adjust_scene);
    }
}
