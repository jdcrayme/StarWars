using UnityEngine;

namespace Assets
{
    public class Blackout : MonoBehaviour {
    
        public Material Background;

        public void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            // Copy the source Render Texture to the destination,
            // applying the material along the way.
            Graphics.Blit(src, dest, Background);
        }
    }
}
