using System.Collections.Generic;
using Hakoniwa.PluggableAsset.Assets.Robot.Parts;
using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    public List<Image> batteryBars; // BatteryBar1〜10のImageコンポーネントをリストで設定

    private float fullVoltage = 14.8f;
    private float currentVoltage = 9.0f;
    public Text fullVoltageText;
    public Text currVoltageText;
    public Text percentageText;
    public GameObject battery;
    private IDroneBatteryStatus drone_battery_status;

    private void Start()
    {
        drone_battery_status = battery.GetComponentInChildren<IDroneBatteryStatus>();
    }


    void Update()
    {
        fullVoltage = (float)drone_battery_status.get_full_voltage();
        currentVoltage = (float)drone_battery_status.get_curr_voltage();
        float batteryPercentage = currentVoltage / fullVoltage;
        float percentValue = batteryPercentage * 100.0f;
        uint battery_status_level = drone_battery_status.get_status();
        fullVoltageText.text = fullVoltage.ToString("F1");
        currVoltageText.text = currentVoltage.ToString("F1");
        percentageText.text = percentValue.ToString("F1");
        int active_index = Mathf.CeilToInt(batteryPercentage * batteryBars.Count);
        Color color = Color.white;

        // 残量に応じた色を設定
        if (battery_status_level == 0)
            color = Color.green;
        else if (battery_status_level == 1)
            color = Color.yellow;
        else
            color = Color.red;

        int percent_index = 0;
        // 各バーの色を設定
        for (int i = batteryBars.Count - 1; i >= 0 ; i--)
        {
            if (percent_index <= active_index)
            {
                batteryBars[i].color = color;
            }
            else
            {
                // 使用されていないバーはグレーにする
                batteryBars[i].color = Color.white;
            }
            percent_index++;
        }
    }
}
