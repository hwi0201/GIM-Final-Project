using UnityEngine;

[ExecuteInEditMode]
public class MirrorCamera : MonoBehaviour
{
    private Camera mirCam;

    void OnPreCull()
    {
        if (mirCam == null) mirCam = GetComponent<Camera>();

        mirCam.ResetWorldToCameraMatrix();
        mirCam.ResetProjectionMatrix();
        mirCam.projectionMatrix = mirCam.projectionMatrix * Matrix4x4.Scale(new Vector3(-1, 1, 1));
    }

    void OnPreRender()
    {
        GL.invertCulling = true;
    }

    void OnPostRender()
    {
        GL.invertCulling = false;
    }
}
