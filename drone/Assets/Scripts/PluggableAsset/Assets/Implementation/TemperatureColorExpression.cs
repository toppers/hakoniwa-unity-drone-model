using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Environment
{
    public class TemperatureColorExpression : MonoBehaviour
    {
        public float minTemp = 0f; // 低温の基準値
        public float maxTemp = 100f; // 高温の基準値
        public float currentTemp = 20f; // 現在の温度

        public MeshRenderer[] renderer;

        public void SetTemperature(double temp)
        {
            this.currentTemp = (float)temp;
        }

        void Update()
        {
            UpdateColorBasedOnTemperature();
        }

        void UpdateColorBasedOnTemperature()
        {
            float t = (currentTemp - minTemp) / (maxTemp - minTemp);

            for (int i = 0; i < renderer.Length; i++)
            {

                if (t < 0.5f) // 低温から平温へ
                {
                    // 青から緑へ
                    renderer[i].material.color = Color.Lerp(Color.blue, Color.green, t * 2);
                }
                else // 平温から高温へ
                {
                    // 緑から赤へ
                    t = t - 0.5f; // スケールの調整
                    renderer[i].material.color = Color.Lerp(Color.green, Color.red, t * 2);
                }
            }

        }
    }
}
