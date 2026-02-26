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
    public GameObject centeredLastPanel; // NEW: Assign your horizontal layout panel here
    public GameObject mainButtonPrefab;

    // --- UI References for Showcase Panel ---
    public TextMeshProUGUI showcaseTitleText;
    public TextMeshProUGUI showcaseContentText;
    public Image showcaseImage;

    // --- Private Variables ---
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
        if (_cultureAreaData == null || _cultureAreaData.buttons == null) return;

        // Clear existing children from BOTH containers
        foreach (Transform child in mainButtonContainer.transform) Destroy(child.gameObject);
        if (centeredLastPanel != null)
        {
            foreach (Transform child in centeredLastPanel.transform) Destroy(child.gameObject);
        }

        int totalButtons = _cultureAreaData.buttons.Count;
        bool isOdd = totalButtons % 2 != 0;

        for (int i = 0; i < totalButtons; i++)
        {
            ButtonData buttonData = _cultureAreaData.buttons[i];
            
            // Default target is the main grid container
            Transform targetContainer = mainButtonContainer.transform;

            // If the total count is odd AND this is the very last button, change the target
            if (isOdd && i == totalButtons - 1 && centeredLastPanel != null)
            {
                targetContainer = centeredLastPanel.transform;
            }

            // Instantiate the button inside the chosen container
            GameObject buttonInstance = Instantiate(mainButtonPrefab, targetContainer);
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

    public void GoHome()
    {
        showcasePanel.SetActive(false);
        alAinPanel.SetActive(true);
    }

    // --- Helper Function ---

    private void ShowPanel(GameObject panelToShow)
    {
        alAinPanel.SetActive(false);
        panelToShow.SetActive(true);

        var scrollRect = panelToShow.GetComponentInChildren<ScrollRect>();
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }
    }
}