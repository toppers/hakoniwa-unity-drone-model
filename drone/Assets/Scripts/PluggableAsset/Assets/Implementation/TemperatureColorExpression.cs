using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hakoniwa.PluggableAsset.Assets.Environment
{
    public class TemperatureColorExpression : MonoBehaviour
    {
        private float minTemp = -40f; // 低温の基準値
        private float maxTemp = 60f; // 高温の基準値
        private float currentTemp = 20f; // 現在の温度
        public float alpha = 1.0f; // 透明度の設定（0.0から1.0の範囲）

        public MeshRenderer[] renderer;

        private void Start()
        {
            for (int i = 0; i < renderer.Length; i++)
            {
                if (alpha != 1.0)
                {
                    // マテリアルが透明度をサポートするように設定
                    renderer[i].material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    renderer[i].material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    renderer[i].material.SetInt("_ZWrite", 0);
                    renderer[i].material.DisableKeyword("_ALPHATEST_ON");
                    renderer[i].material.EnableKeyword("_ALPHABLEND_ON");
                    renderer[i].material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    renderer[i].material.renderQueue = 3000;
                }
            }

        }

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
                Color color;
                if (t < 0.5f) // 低温から平温へ
                {
                    // 青から緑へ
                    color = Color.Lerp(Color.blue, Color.green, t * 2);
                }
                else // 平温から高温へ
                {
                    // 緑から赤へ
                    t = t - 0.5f; // スケールの調整
                    color = Color.Lerp(Color.green, Color.red, t * 2);
                }
                color.a = alpha;
                renderer[i].material.color = color;
            }

        }
    }
}
