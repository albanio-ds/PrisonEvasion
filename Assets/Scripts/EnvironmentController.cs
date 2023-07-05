using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnvironmentController : MonoBehaviour
{
    public BoxController[] BoxControllers;
    public PrisonnerController PrisonnerController;
    public GuardController[] GuardControllers;
    public Transform[] PlayerSpawns;
    public SecurityCamera[] SecurityCameras;
    public SmartGuardAgent SmartGuardAgent;

    [SerializeField]
    private GameObject[] GuardCheckpointScriptParent;

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

        SecurityCameras = transform.GetComponentsInChildren<SecurityCamera>();
        SmartGuardAgent = transform.GetComponentInChildren<SmartGuardAgent>();

        foreach (var item in BoxControllers)
        {
            item.OnKeyFound += OnKeyFoundCallback;
        }
        foreach (var item in GuardControllers)
        {
            item.OnPlayerRepered += OnPlayerReperedCallback;
        }
        foreach (var item in SecurityCameras)
        {
            item.OnPlayerRepered += OnPlayerOnCameraCallback;
        }
        PrisonnerController.OnPlayerRunning += OnPlayerOnCameraCallback;

        transform.GetComponentInChildren<ExitScript>().OnPlayerLeaving += OnPlayerLeavingCallback;

        if (SmartGuardAgent != null)
            SmartGuardAgent.OnPlayerRepered += OnPlayerReperedCallback;

        PlayerUIController.Instance.PlayButton.onClick.AddListener(InitGame);
        PlayerUIController.Instance.MainText.text = "";
    }

    private void OnPlayerOnCameraCallback(object sender, Transform e)
    {
        Debug.Log("On camera !!");
        SmartGuardAgent?.PlayerOnCamera();
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
                SmartGuardAgent?.PlayerWin();
            }
            else
            {
                PlayerUIController.Instance.MainText.text = "You must find a key first.";
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
        foreach (var item in GuardControllers)
        {
            item.PlayerGameOver();
        }
        foreach (var item in SecurityCameras)
        {
            item.PlayerGameOver();
        }
        SmartGuardAgent?.PlayerGameOver();
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
        for (int i = 0; i < GuardControllers.Length; i++)
        {
            var checkpts = GuardCheckpointScriptParent[i].GetComponentsInChildren<Transform>(false).Where(curent => curent != GuardCheckpointScriptParent[i].transform) .Select(guard => guard.transform).ToArray();
            GuardControllers[i].Init(checkpts, (uint)Random.Range(0, checkpts.Length));
        }
        for (int i = 0; i < SecurityCameras.Length; i++)
        {
            SecurityCameras[i].Init();
        }
        PrisonnerController.Spawn = PlayerSpawns[Random.Range(0, PlayerSpawns.Length)];
        PrisonnerController.Init();
        CurrGameState = GameState.Playing;
        SmartGuardAgent?.CustomStart();
    }
}

public static class GameSettings
{
    public const float PlayerSpeed = 3.5f;
}