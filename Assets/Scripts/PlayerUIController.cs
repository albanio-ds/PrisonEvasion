using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    public static PlayerUIController Instance = null;

    [SerializeField]
    private Button playButton;

    public Button PlayButton { get { return playButton; } }

    [SerializeField]
    private Text mainText;

    public Text MainText { get { return mainText; } }

    [SerializeField]
    private Text inventoryText;

    public Text InventoryText { get { return inventoryText; } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError($"Duplicated {this}");
        }
    }
}
