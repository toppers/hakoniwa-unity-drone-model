using System.Collections;
using System.Collections.Generic;
using Hakoniwa.AR.Core;
using UnityEngine;

namespace Hakoniwa.AR.Assets
{
    public class VirtualSignalController : MonoBehaviour, IHakoPlayerState
    {
        public Renderer signal_red;
        public Renderer signal_yellow;
        public Renderer signal_blue;

        public Material[] reds;
        public Material[] yellows;
        public Material[] blues;

        public Light redLight; // 赤信号用ライト
        public Light yellowLight; // 黄色信号用ライト
        public Light blueLight; // 青信号用ライト

        private int signal_state = 0;
        private float timer = 0.0f;
        private bool isBlinking = false;

        // 信号の各状態の持続時間（秒）
        private float redDuration = 10.0f; // 赤信号の時間
        private float yellowDuration = 5.0f; // 黄色信号の時間
        private float blueDuration = 10.0f; // 青信号の時間
        private float blueBlinkDuration = 5.0f; // 青信号の点滅時間
        private float blinkInterval = 0.5f; // 点滅の間隔

        void Start()
        {
            SetSignalState(0);
        }

        void Update()
        {
            timer += Time.deltaTime;
            switch (signal_state)
            {
                case 0: // 赤信号
                    if (timer >= redDuration)
                    {
                        SetSignalState(1);
                    }
                    break;
                case 1: // 黄色信号
                    if (timer >= yellowDuration)
                    {
                        SetSignalState(2);
                    }
                    break;
                case 2: // 青信号
                    if (timer >= blueDuration)
                    {
                        SetSignalState(3);
                    }
                    break;
                case 3: // 青信号点滅
                    if (timer >= blueBlinkDuration)
                    {
                        SetSignalState(0);
                    }
                    else if (timer % blinkInterval < blinkInterval / 2)
                    {
                        SetBlinkingState(true);
                    }
                    else
                    {
                        SetBlinkingState(false);
                    }
                    break;
            }
        }

        private void SetSignalState(int state)
        {
            signal_state = state;
            timer = 0.0f;

            switch (signal_state)
            {
                case 0:
                    this.signal_red.material = reds[1];
                    this.signal_yellow.material = yellows[0];
                    this.signal_blue.material = blues[0];
                    redLight.enabled = true;
                    yellowLight.enabled = false;
                    blueLight.enabled = false;
                    isBlinking = false;
                    break;
                case 1:
                    this.signal_red.material = reds[0];
                    this.signal_yellow.material = yellows[1];
                    this.signal_blue.material = blues[0];
                    redLight.enabled = false;
                    yellowLight.enabled = true;
                    blueLight.enabled = false;
                    isBlinking = false;
                    break;
                case 2:
                    this.signal_red.material = reds[0];
                    this.signal_yellow.material = yellows[0];
                    this.signal_blue.material = blues[1];
                    redLight.enabled = false;
                    yellowLight.enabled = false;
                    blueLight.enabled = true;
                    isBlinking = false;
                    break;
                case 3: // 青信号点滅
                    redLight.enabled = false;
                    yellowLight.enabled = false;
                    isBlinking = true;
                    break;
            }
        }

        private void SetBlinkingState(bool isVisible)
        {
            if (isVisible)
            {
                this.signal_blue.material = blues[1];
                blueLight.enabled = true;
            }
            else
            {
                this.signal_blue.material = blues[0];
                blueLight.enabled = false;
            }
        }

        public int GetState()
        {
            return this.signal_state;
        }
    }
}
