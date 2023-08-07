using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class Billboard : MonoBehaviour
{
    private void Update()
    {
        if (!Application.isPlaying)
        {
            // Face the view in editor
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null)
            {
                Vector3 viewDirection = (sceneView.camera.transform.position - transform.position).normalized;
                transform.rotation = Quaternion.LookRotation(-viewDirection);
            }
        }
    }
    private Transform target;

    
}

