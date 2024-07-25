using UnityEngine;

public class SetDepthTextureMode : MonoBehaviour
{
    private Camera gameCamera;

    void Awake()
    {
        // Find the camera component on this GameObject
        gameCamera = GetComponent<Camera>();
        gameCamera.depthTextureMode = DepthTextureMode.Depth;
    }

}
