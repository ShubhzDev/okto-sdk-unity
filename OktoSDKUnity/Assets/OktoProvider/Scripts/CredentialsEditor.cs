using UnityEditor;
using UnityEngine;

public class CredentialsEditor : EditorWindow
{
    private Credentials credentials;

    [MenuItem("SDK/Credentials Manager")]
    public static void ShowWindow()
    {
        GetWindow<CredentialsEditor>("Credentials Manager");
    }

    private void OnGUI()
    {
        GUILayout.Label("SDK Credentials", EditorStyles.boldLabel);

        if (credentials == null)
        {
            credentials = Resources.Load<Credentials>("Credentials");
        }

        if (credentials == null)
        {
            if (GUILayout.Button("Create New Credentials"))
            {
                credentials = CreateCredentialsAsset();
            }
        }
        else
        {
            credentials.apiKey = EditorGUILayout.TextField("API Key", credentials.apiKey);
            credentials.clientId = EditorGUILayout.TextField("Client ID", credentials.clientId);
            credentials.clientSecret = EditorGUILayout.TextField("Client Secret", credentials.clientSecret);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(credentials);
            }
        }
    }

    private Credentials CreateCredentialsAsset()
    {
        var asset = ScriptableObject.CreateInstance<Credentials>();
        AssetDatabase.CreateAsset(asset, "Assets/Resources/Credentials.asset");
        AssetDatabase.SaveAssets();
        return asset;
    }
}
