using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hakoniwa.ar.bridge
{
    public class HakoniwaArBridgeStateManager
    {
        private BridgeState state;
        public HakoniwaArBridgeStateManager()
        {
            this.state = BridgeState.POSITIONING;
        }
        public BridgeState GetState()
        {
            return state;
        }
        public void EventReset()
        {
            if (this.state == BridgeState.PLAYING) {
                this.state = BridgeState.POSITIONING;
            }
        }
        public void EventPlayStart()
        {
            if (this.state == BridgeState.POSITIONING) {
                this.state = BridgeState.PLAYING;
            }
        }
    }
}