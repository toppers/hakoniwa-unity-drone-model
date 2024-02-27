using UnityEngine;
using System.Collections;
using Hakoniwa.PluggableAsset.Assets.Robot.Parts;

namespace Hakoniwa.PluggableAsset.Assets.Environment
{
    public class EnvironmentEffector : MonoBehaviour, IRobotPartsController, IRobotPartsConfig
    {
        private IEnvironmentObject[] environments;
        object my_root = null;

        public void Initialize(object root)
        {
            Debug.Log("Effector enter");
            if (my_root == null)
            {
                this.environments = this.GetComponentsInChildren<IEnvironmentObject>();
            }
            else
            {
                my_root = root;
                this.ClearAll();
            }
            foreach (var env in this.environments) {
                env.Initialize();
            }
        }
        private void ClearAll()
        {
            foreach (var env in this.environments)
            {
                env.ClearAll();
            }
        }
        public void DoControl()
        {
            foreach (var env in this.environments)
            {
                env.UpdateRobotProperty();
            }
        }

        public RoboPartsConfigData[] GetRoboPartsConfig()
        {
            return new RoboPartsConfigData[0];
        }

        public RosTopicMessageConfig[] getRosConfig()
        {
            return new RosTopicMessageConfig[0];
        }

    }
}
