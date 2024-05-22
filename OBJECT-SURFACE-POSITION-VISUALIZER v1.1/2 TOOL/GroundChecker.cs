using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class GroundCheckerAutoAssign
{
    static GroundCheckerAutoAssign()
    {
        Selection.selectionChanged += OnSelectionChanged;
    }

    private static void OnSelectionChanged()
    {
        GameObject selectedObject = Selection.activeGameObject;

        if (selectedObject != null)
        {
            GroundChecker groundChecker = selectedObject.GetComponent<GroundChecker>();

            if (groundChecker == null)
            {
                groundChecker = selectedObject.AddComponent<GroundChecker>();
            }

            groundChecker.groundCheckPosition = selectedObject.transform;
            groundChecker.groundCheckRadius = 0.1f;
            groundChecker.groundLayer = -1; // Everything layer
            groundChecker.indicatorColor = Color.green;

            SceneView.RepaintAll();
        }
    }
}

public class GroundChecker : MonoBehaviour
{
    public Transform groundCheckPosition; // The position from which to check for ground
    public float groundCheckRadius = 0.2f; // The radius of the sphere used for ground checking
    public LayerMask groundLayer; // The layer(s) considered as ground
    public Color indicatorColor = Color.green; // Color of the ground indicator

    private void OnDrawGizmosSelected()
    {
        // Perform ground check
        RaycastHit hit;
        if (Physics.Raycast(groundCheckPosition.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            // Draw ground snapping spot
            Handles.color = indicatorColor;
            Handles.DrawSolidDisc(hit.point, hit.normal, groundCheckRadius);
        }
    }
}

public class GroundCheckerAssetProcessor : UnityEditor.AssetModificationProcessor
{
    // This method is called whenever assets are deleted
    public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
    {
        // Check if the deleted asset is the GroundCheckerAutoAssign script
        if (assetPath.EndsWith("GroundCheckerAutoAssign.cs"))
        {
            // Find all GroundChecker components in the scene and delete them
            GroundChecker[] allCheckers = Object.FindObjectsOfType<GroundChecker>();
            foreach (var checker in allCheckers)
            {
                UnityEngine.Object.DestroyImmediate(checker);
            }
        }

        return AssetDeleteResult.DidNotDelete;
    }
}