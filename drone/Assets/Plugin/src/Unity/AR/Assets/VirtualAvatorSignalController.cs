using System.Collections;
using System.Collections.Generic;
using Hakoniwa.AR.Core;
using UnityEngine;

namespace Hakoniwa.AR.Assets
{
    public class VirtualAvatorSignalController : MonoBehaviour, IHakoAvatorState
    {
        public Renderer signal_red;
        public Renderer signal_yellow;
        public Renderer signal_blue;

        public Material[] reds;
        public Material[] yellow;
        public Material[] blue;

        public Light redLight; // 赤信号用ライト
        public Light yellowLight; // 黄色信号用ライト
        public Light blueLight; // 青信号用ライト

        void Start()
        {
            this.signal_red.material = reds[1];
            this.signal_yellow.material = yellow[0];
            this.signal_blue.material = blue[0];
        }
        int signal_state = 0;
        void FixedUpdate()
        {
            if ((signal_state & (0x1 << 0)) != 0)
            {
                this.signal_blue.material = blue[1];
                blueLight.enabled = true;
            }
            else
            {
                this.signal_blue.material = blue[0];
                blueLight.enabled = false;
            }
            if ((signal_state & (0x1 << 1)) != 0)
            {
                this.signal_yellow.material = yellow[1];
                yellowLight.enabled = true;
            }
            else
            {
                this.signal_yellow.material = yellow[0];
                yellowLight.enabled = false;
            }
            if ((signal_state & (0x1 << 2)) != 0)
            {
                this.signal_red.material = reds[1];
                redLight.enabled = true;
            }
            else
            {
                this.signal_red.material = reds[0];
                redLight.enabled = false;
            }
        }

        public void SetState(int state)
        {
            this.signal_state = state;
        }
    }

}
