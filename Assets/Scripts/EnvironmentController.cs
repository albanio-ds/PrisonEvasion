using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnvironmentController : MonoBehaviour
{
    public BoxController[] BoxControllers;
    public PrisonnerController PrisonnerController;
    public GuardController[] GuardControllers;
    private enum GameState { None, Playing, Lose, Win }

    GameState CurrGameState = GameState.None;

    private void Start()
    {
        BoxControllers = transform.GetComponentsInChildren<BoxController>();
        PrisonnerController = transform.GetComponentInChildren<PrisonnerController>();
        GuardControllers = transform.GetComponentsInChildren<GuardController>();
        UnityEngine.Assertions.Assert.IsNotNull(BoxControllers);
        UnityEngine.Assertions.Assert.IsNotNull(PrisonnerController);
        UnityEngine.Assertions.Assert.IsNotNull(GuardControllers);

        foreach (var item in BoxControllers)
        {
            item.OnKeyFound += OnKeyFoundCallback;
        }
        foreach (var item in GuardControllers)
        {
            item.OnPlayerRepered += OnPlayerReperedCallback;
        }
        transform.GetComponentInChildren<ExitScript>().OnPlayerLeaving += OnPlayerLeavingCallback;

        PlayerUIController.Instance.PlayButton.onClick.AddListener(InitGame);
        PlayerUIController.Instance.MainText.text = "";
    }

    private void OnPlayerLeavingCallback(object sender, Transform player)
    {
        if (CurrGameState != GameState.Playing)
        {
            return;
        }
        player.TryGetComponent(out PrisonnerController controller);
        if (controller != null)
        {
            if (controller.Inventory.Contains(typeof(ExitKey)))
            {
                PlayerUIController.Instance.MainText.text = "Win !";
                PlayerUIController.Instance.PlayButton.gameObject.SetActive(true);
                PlayerUIController.Instance.MainText.color = Color.green;
                Debug.Log("player win !");
                CurrGameState = GameState.Win;
            }
        }
    }

    private void OnPlayerReperedCallback(object sender, Transform player)
    {
        if (CurrGameState != GameState.Playing)
        {
            return;
        }
        PrisonnerController.CanMove = false;
        CurrGameState = GameState.Lose;
        Debug.Log("player found !");
        PlayerUIController.Instance.MainText.text = "Lose !";
        PlayerUIController.Instance.PlayButton.gameObject.SetActive(true);
        PlayerUIController.Instance.MainText.color = Color.red;
    }

    private void OnKeyFoundCallback(object sender, Transform player)
    {
        if (CurrGameState != GameState.Playing)
        {
            return;
        }
        player.TryGetComponent(out PrisonnerController controller);
        if (controller != null)
        {
            controller.Inventory.EquipItem(new ExitKey());
            Debug.Log("key found !");
            PlayerUIController.Instance.InventoryText.text += "Key\n";
        }
    }

    public void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            InitGame();
        }
    }

    public void InitGame()
    {
        PlayerUIController.Instance.InventoryText.text = "";
        PlayerUIController.Instance.MainText.text = "";
        PlayerUIController.Instance.PlayButton.gameObject.SetActive(false);
        int selected = Random.Range(0, BoxControllers.Length);
        for (int i = 0; i < BoxControllers.Length; i++)
        {
            BoxControllers[i].HaveKey = i == selected;
        }
        Transform[] checkpts = transform.GetComponentsInChildren<GuardCheckpointScript>().Select(guard => guard.transform).ToArray();
        uint index = 0;
        foreach (var guard in GuardControllers)
        {
            guard.Init(checkpts, index);
            index++;
        }
        PrisonnerController.Init();
        CurrGameState = GameState.Playing;
    }
}
