using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;

namespace GAOS.EventSystem.Editor
{
    /// <summary>
    /// Editor utility for opening the Event System documentation
    /// </summary>
    public static class EventSystemDocumentation
    {
        private const string MenuPath = "GAOS/Event System/Documentation";
        private const string PackageName = "com.gaos.eventsystem";

        [MenuItem(MenuPath)]
        public static void OpenDocumentation()
        {
            string docPath = GetDocumentationPath();
            if (string.IsNullOrEmpty(docPath))
            {
                UnityEngine.Debug.LogError("Could not find Event System documentation. Please ensure the package is properly installed.");
                return;
            }

            string indexPath = Path.Combine(docPath, "index.html");
            if (!File.Exists(indexPath))
            {
                UnityEngine.Debug.LogError($"Documentation index not found at: {indexPath}");
                return;
            }

            // Open in default browser
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = indexPath,
                    UseShellExecute = true
                });
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"Failed to open documentation: {ex.Message}");
                
                // Fallback: Show the path to the user
                UnityEngine.Debug.Log($"Please open the documentation manually at: {indexPath}");
                
                // Select the file in the Project window
                var relativePath = $"Packages/{PackageName}/Documentation~/index.html";
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(relativePath);
                if (asset != null)
                {
                    Selection.activeObject = asset;
                    EditorGUIUtility.PingObject(asset);
                }
            }
        }

        private static string GetDocumentationPath()
        {
            // Try to find in Packages folder first
            string packagePath = Path.GetFullPath("Packages/" + PackageName);
            if (Directory.Exists(packagePath))
            {
                string docPath = Path.Combine(packagePath, "Documentation~");
                if (Directory.Exists(docPath))
                {
                    return docPath;
                }
            }

            // Try to find in PackageCache
            string userPath = Path.Combine(Application.dataPath, "..", "Library", "PackageCache");
            if (Directory.Exists(userPath))
            {
                var packageDirs = Directory.GetDirectories(userPath, PackageName + "@*");
                if (packageDirs.Length > 0)
                {
                    string docPath = Path.Combine(packageDirs[0], "Documentation~");
                    if (Directory.Exists(docPath))
                    {
                        return docPath;
                    }
                }
            }

            return null;
        }

        // Validate the menu item
        [MenuItem(MenuPath, true)]
        private static bool ValidateOpenDocumentation()
        {
            return !string.IsNullOrEmpty(GetDocumentationPath());
        }
    }
} 