using UnityEditor;
using UnityEngine;

public class AutoIncrementVersionCodeInCloudBuild : MonoBehaviour
{
#if UNITY_CLOUD_BUILD
    public static void PreExport(UnityEngine.CloudBuild.BuildManifestObject manifest)
    {
        string buildNumber = manifest.GetValue<string>("buildNumber");
        Debug.LogWarning("Setting build number to " + buildNumber);
        PlayerSettings.Android.bundleVersionCode = int.Parse(buildNumber);
        PlayerSettings.iOS.buildNumber = buildNumber;
    }
#endif
}
