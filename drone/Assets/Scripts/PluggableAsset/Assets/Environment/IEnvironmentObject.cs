using System;

namespace Hakoniwa.PluggableAsset.Assets.Environment
{
    public interface IEnvironmentObject
    {
        public void Initialize();
        public void UpdateRobotProperty();
        public void ClearAll();
    }
}
