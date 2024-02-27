using System;

namespace Hakoniwa.PluggableAsset.Assets.Environment
{
    public interface IRobotProperty
    {
        public double GetTemperature();
        public void SetTemperature(double temperature);
        public void IncrementTemperatureRegion();
        public void DecrementTemperatureRegion();
        public bool IsInTemeratureRegion();
    }
}
