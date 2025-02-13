using OktoProvider;
using UnityEngine;
using UnityEngine.UI;

public class OktoWebViewWidget : MonoBehaviour
{
    private WebViewObject webView;
    public Button showModalButton;
    public Button closeButton;
    private string widgetUrl = "https://3p-sandbox.okto.tech/#/home";

    private Vector2 touchStartPosition;
    private bool isSwipingDown = false;

    public int topMargin = 300;

    [System.Serializable]
    public class Theme
    {
        public string textPrimaryColor = "0xFFFFFFFF";
        public string textSecondaryColor = "0xFFFFFFFF";
        public string textTertiaryColor = "0xFFFFFFFF";
        public string accent1Color = "0x80433454";
        public string accent2Color = "0x80905BF5";
        public string strokeBorderColor = "0xFFACACAB";
        public string strokeDividerColor = "0x4DA8A8A8";
        public string surfaceColor = "0xFF1F0A2F";
        public string backgroundColor = "0xFF000000";
    }

    private Theme defaultTheme = new Theme();

    public void loggedIn()
    {
        webView = gameObject.AddComponent<WebViewObject>();
        webView.Init(
            cb: (msg) => Debug.Log($"Message from WebView: {msg}"),
            ld: (msg) =>
            {
                Debug.Log($"Page Loaded: {msg}");
                InjectJavaScript();
            }
        );
        webView.LoadURL(widgetUrl);
        webView.SetMargins(0, topMargin, 0, 0);
        webView.SetVisibility(false);

        if (showModalButton != null)
        {
            showModalButton.onClick.AddListener(OpenWebView);
        }
    }

    public void OpenWebView()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        webView.SetVisibility(true);
        closeButton.gameObject.SetActive(true);
        closeButton.onClick.AddListener(closeWebView);
    }

    public void closeWebView()
    {
        if (webView != null)
        {
            webView.SetVisibility(false);
        }
        closeButton.onClick.RemoveListener(closeWebView);
        closeButton.gameObject.SetActive(false);
        Screen.orientation = ScreenOrientation.AutoRotation;
    }

    void InjectJavaScript()
    {
        string authToken = OktoProviderSDK.Instance.AuthToken;
        string injectJs = $@"
            window.localStorage.setItem('ENVIRONMENT', '{OktoProviderSDK.Instance.buildStage}');
            window.localStorage.setItem('textPrimaryColor', '{defaultTheme.textPrimaryColor}');
            window.localStorage.setItem('textSecondaryColor', '{defaultTheme.textSecondaryColor}');
            window.localStorage.setItem('textTertiaryColor', '{defaultTheme.textTertiaryColor}');
            window.localStorage.setItem('accent1Color', '{defaultTheme.accent1Color}');
            window.localStorage.setItem('accent2Color', '{defaultTheme.accent2Color}');
            window.localStorage.setItem('strokeBorderColor', '{defaultTheme.strokeBorderColor}');
            window.localStorage.setItem('strokeDividerColor', '{defaultTheme.strokeDividerColor}');
            window.localStorage.setItem('surfaceColor', '{defaultTheme.surfaceColor}');
            window.localStorage.setItem('backgroundColor', '{defaultTheme.backgroundColor}');
        ";

        if (!string.IsNullOrEmpty(authToken))
        {
            injectJs += $"window.localStorage.setItem('authToken', '{authToken}');";
        }

        webView.EvaluateJS(injectJs);
    }

    void Update()
    {

        // Swipe detection logic
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPosition = touch.position;
                    isSwipingDown = false;
                    break;
                case TouchPhase.Moved:
                    // Detect swipe down gesture
                    if (touch.position.y < touchStartPosition.y - 50) // swipe down threshold (e.g., 50 units)
                    {
                        isSwipingDown = true;
                    }
                    break;
                case TouchPhase.Ended:
                    if (isSwipingDown)
                    {
                        closeWebView();
                    }
                    break;
            }
        }
    }
}
