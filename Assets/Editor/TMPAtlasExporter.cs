using UnityEngine;
using UnityEditor;
using System.IO;

public class TMPAtlasExporter
{
    [MenuItem("Tools/Export Selected Atlas")]
    static void ExportAtlas()
    {
        Texture2D atlas = Selection.activeObject as Texture2D;

        if (atlas == null)
        {
            Debug.LogError("Select the atlas texture.");
            return;
        }

        RenderTexture rt = RenderTexture.GetTemporary(
            atlas.width,
            atlas.height,
            0,
            RenderTextureFormat.ARGB32);

        Graphics.Blit(atlas, rt);

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D output = new Texture2D(
            atlas.width,
            atlas.height,
            TextureFormat.RGBA32,
            false);

        output.ReadPixels(
            new Rect(0, 0, atlas.width, atlas.height),
            0, 0);

        output.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        string path = AssetDatabase.GenerateUniqueAssetPath(
            "Assets/ExportedAtlas.png");

        File.WriteAllBytes(path, output.EncodeToPNG());

        AssetDatabase.Refresh();

        Debug.Log("Saved atlas to: " + path);
    }
}