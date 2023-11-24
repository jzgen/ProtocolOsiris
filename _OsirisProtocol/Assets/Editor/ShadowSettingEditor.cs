using UnityEngine;
using UnityEditor;

public class ShadowSettingsEditor : EditorWindow
{
    [MenuItem("Tools/Cambiar Ajustes de Sombra")]
    public static void ShowWindow()
    {
        GetWindow(typeof(ShadowSettingsEditor));
    }

    void OnGUI()
    {
        if (GUILayout.Button("Cambiar a TwoSided"))
        {
            CambiarAjustesDeSombra();
        }
    }

    void CambiarAjustesDeSombra()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            var renderers = obj.GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in renderers)
            {
                // Comprobación de seguridad
                if (renderer != null)
                {
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
                }
            }
        }
    }
}