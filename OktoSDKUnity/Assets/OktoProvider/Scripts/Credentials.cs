using UnityEngine;

[CreateAssetMenu(fileName = "Credentials", menuName = "SDK/Credentials")]
public class Credentials : ScriptableObject
{
    [Header("API Credentials")]
    public string apiKey;
    public string clientId;
    public string clientSecret;
}
