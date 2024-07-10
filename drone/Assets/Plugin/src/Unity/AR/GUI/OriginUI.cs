using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OriginUI : MonoBehaviour
{
    public const string session_savedPosKey = "session_savedPos";
    public const string session_savedRotKey = "session_savedRot";
    public Button saveButton;
    public Button backButton;
    public Text originView; // 位置情報を表示するText
    public Text rotationView; // 回転情報を表示するText（追加）
    public GameObject savePopupPanel;

    // Start is called before the first frame update
    void Start()
    {
        // ボタンのクリックイベントにメソッドを登録
        saveButton.onClick.AddListener(SaveOrigin);
        backButton.onClick.AddListener(ARSetting);
        savePopupPanel.SetActive(false);
    }

    void SaveOrigin()
    {
        // 位置情報のテキストを保存
        string session_savedPos = originView.text;
        PlayerPrefs.SetString(session_savedPosKey, session_savedPos);

        // 回転情報のテキストを保存（追加）
        if (rotationView != null)
        {
            string session_savedRot = rotationView.text;
            PlayerPrefs.SetString(session_savedRotKey, session_savedRot);
        }
        else
        {
            Debug.LogWarning("Rotation view is not assigned.");
        }

        PlayerPrefs.Save();
        Debug.Log("Position and rotation saved: " + session_savedPos + ", " + PlayerPrefs.GetString(session_savedRotKey, "0,0,0"));
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

    void ARSetting()
    {
        SceneManager.LoadScene("ARSetting");
    }
}
