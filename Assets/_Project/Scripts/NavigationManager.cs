// In NavigationManager.cs

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class NavigationManager : MonoBehaviour
{
    public static NavigationManager Instance;

    // --- Panel References ---
    public GameObject alAinPanel; // Your main home panel
    public GameObject showcasePanel;

    // --- Data References ---
    public ExploreData cultureAreaDataFile;

    // --- UI References for Main Panel ---
    public GameObject mainButtonContainer;
    public GameObject mainButtonPrefab;

    // --- UI References for Showcase Panel ---
    public TextMeshProUGUI showcaseTitleText;
    public TextMeshProUGUI showcaseContentText;
    public Image showcaseImage;

    // --- Private Variables ---
    // REMOVED: The navigation history list is no longer needed
    private ExploreDataContent _cultureAreaData; // Holds the parsed data

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (cultureAreaDataFile != null && cultureAreaDataFile.jsonFile != null)
        {
            _cultureAreaData = JsonUtility.FromJson<ExploreDataContent>(cultureAreaDataFile.jsonFile.text);
            PopulateMainPanel();
        }
        else
        {
            Debug.LogError("Culture Area JSON data file is not assigned in the Inspector!");
        }

        // Start the app on the home screen
        GoHome();
    }

    // --- Populates the MAIN screen buttons ---
    void PopulateMainPanel()
    {
        if (_cultureAreaData == null) return;

        foreach (Transform child in mainButtonContainer.transform) Destroy(child.gameObject);

        foreach (ButtonData buttonData in _cultureAreaData.buttons)
        {
            GameObject buttonInstance = Instantiate(mainButtonPrefab, mainButtonContainer.transform);
            SetupMainButton(buttonInstance, buttonData);
        }
    }

    // --- Helper to set up a button on the main panel ---
    void SetupMainButton(GameObject buttonInstance, ButtonData buttonData)
    {
        ButtonPrefab prefabScript = buttonInstance.GetComponent<ButtonPrefab>();
        if (prefabScript != null)
        {
            prefabScript.buttonLabel.text = buttonData.name;

            string iconPath = $"{_cultureAreaData.category}/icons/{buttonData.iconStr}";
            Sprite iconSprite = Resources.Load<Sprite>(iconPath);

            if (iconSprite != null && prefabScript.photoContentImage != null)
            {
                prefabScript.photoContentImage.sprite = iconSprite;

                AspectRatioFitter fitter = prefabScript.photoContentImage.GetComponent<AspectRatioFitter>();
                if (fitter != null)
                {
                    fitter.aspectRatio = iconSprite.bounds.size.x / iconSprite.bounds.size.y;
                }
            }
            else
            {
                Debug.LogWarning($"Button icon not found at path: {iconPath}");
            }

            prefabScript.buttonComponent.onClick.AddListener(() => LoadShowcaseScreen(buttonData));
        }
    }

    // --- Navigation Functions ---

    public void LoadShowcaseScreen(ButtonData buttonData)
    {
        PopulateShowcasePanel(buttonData);
        ShowPanel(showcasePanel);
    }

    void PopulateShowcasePanel(ButtonData data)
    {
        showcaseTitleText.text = data.name;
        showcaseContentText.text = data.content;

        string imagePath = $"{_cultureAreaData.category}/images/{data.imageStr}";
        Sprite showcaseSprite = Resources.Load<Sprite>(imagePath);

        if (showcaseSprite != null)
        {
            showcaseImage.sprite = showcaseSprite;
            showcaseImage.enabled = true;

            AspectRatioFitter fitter = showcaseImage.GetComponent<AspectRatioFitter>();
            if (fitter != null)
            {
                fitter.aspectRatio = showcaseSprite.bounds.size.x / showcaseSprite.bounds.size.y;
            }
        }
        else
        {
            showcaseImage.enabled = false;
            Debug.LogWarning($"Showcase image not found at path: {imagePath}");
        }
    }

    // --- Back and Home Button Logic ---

    // REMOVED: The GoBack() function is no longer needed.

    public void GoHome()
    {
        // Hide the showcase panel
        showcasePanel.SetActive(false);
        // Show the home panel
        alAinPanel.SetActive(true);
    }

    // --- Helper Function ---

    // SIMPLIFIED: ShowPanel no longer manages history
    private void ShowPanel(GameObject panelToShow)
    {
        // Hide the home panel
        alAinPanel.SetActive(false);

        // Show the new panel
        panelToShow.SetActive(true);

        // Reset scroll position
        var scrollRect = panelToShow.GetComponentInChildren<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }
}