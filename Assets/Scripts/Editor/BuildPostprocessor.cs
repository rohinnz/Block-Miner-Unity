using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class BuildPostprocessor
{
    [PostProcessBuildAttribute(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target == BuildTarget.WebGL)
        {
            Debug.Log("Copying web.config to new WebGL Build at " + pathToBuiltProject);

            string ROOT_DIR = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length);
            string WebConfigDir = Path.Combine(ROOT_DIR, "WebConfig");
            string webConfigFilePath = Path.Combine(WebConfigDir, "web.config");
            
            string targetFileName = Path.Combine(pathToBuiltProject, "web.config");
            FileInfo fileInfo = new FileInfo(webConfigFilePath);
            fileInfo.CopyTo(targetFileName, true);
        }
    }
}
