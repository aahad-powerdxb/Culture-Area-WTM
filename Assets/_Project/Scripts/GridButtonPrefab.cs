// ButtonPrefab.cs

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ButtonPrefab : MonoBehaviour
{
    // Public "outlets" that we will link in the Inspector
    public TextMeshProUGUI buttonLabel;
    public Button buttonComponent;
    public Image mainImage; // You can use this later to set the image
    public Image photoContentImage;
}