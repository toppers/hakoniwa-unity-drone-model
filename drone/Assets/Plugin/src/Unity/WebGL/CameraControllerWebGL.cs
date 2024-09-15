using System;
using Hakoniwa.PluggableAsset.Communication.Connector;
using Hakoniwa.PluggableAsset.Communication.Pdu;
using UnityEngine;
using UnityEngine.UI;

namespace Hakoniwa.Web.Core
{

    public class CameraControllerWebGL : MonoBehaviour
    {
        private RenderTexture RenderTextureRef;
        public int width = 640;
        public int height = 480;
        private Camera my_camera;
        public RawImage displayImage; // 映像を表示するUIのRawImage

        private void Start()
        {
            this.my_camera = this.GetComponentInChildren<Camera>();
            var texture = new Texture2D(this.width, this.height, TextureFormat.RGB24, false);
            this.RenderTextureRef = new RenderTexture(texture.width, texture.height, 32);
            this.my_camera.targetTexture = this.RenderTextureRef;
        }
        void Update()
        {
            displayImage.texture = this.RenderTextureRef;
        }
    }

}
