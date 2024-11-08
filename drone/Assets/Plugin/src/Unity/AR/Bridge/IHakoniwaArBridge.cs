using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace hakoniwa.ar.bridge
{
    public struct HakoVector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public HakoVector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
    public interface IHakoniwaArBridgePlayer
    {
        Task<bool> StartService(string serverUri);
        bool StopService();
        void UpdatePosition(HakoVector3 position, HakoVector3 rotation);
        void ResetPostion();
        void UpdateAvatars();
    }
    public enum BridgeState
    {
        POSITIONING,
        PLAYING
    }
    public interface IHakoniwaArBridge
    {
        bool Register(IHakoniwaArBridgePlayer player);
        bool Start();
        void Run();
        BridgeState GetState();
        bool Stop();
    }
}
