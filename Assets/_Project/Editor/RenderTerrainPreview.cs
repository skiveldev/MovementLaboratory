using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class RenderTerrainPreview
{
    public static void Render()
    {
        EditorSceneManager.OpenScene("Assets/_Project/Scenes/MovementLab.unity", OpenSceneMode.Single);
        var camera = Object.FindFirstObjectByType<Camera>();
        if (camera == null) throw new System.InvalidOperationException("No camera found.");

        var texture = new RenderTexture(1440, 540, 24);
        var output = new Texture2D(1440, 540, TextureFormat.RGBA32, false);
        var previous = camera.targetTexture;
        camera.targetTexture = texture;
        camera.Render();
        RenderTexture.active = texture;
        output.ReadPixels(new Rect(0, 0, 1440, 540), 0, 0);
        output.Apply();
        File.WriteAllBytes("C:/Users/Skivel/AppData/Local/Temp/opencode/movementlab-terrain-preview.png", output.EncodeToPNG());
        camera.targetTexture = previous;
        RenderTexture.active = null;
        Object.DestroyImmediate(output);
        Object.DestroyImmediate(texture);
    }
}
