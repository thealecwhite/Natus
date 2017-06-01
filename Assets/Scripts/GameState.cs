using UnityEngine;
using UnityEngine.SceneManagement;

using ActionInput;

public class GameState : Singleton<GameState>
{
	protected GameState() : base() {}

	private ItemDatabase _items;
	private PlayerMob _player;
	private InGameMenu _inGameMenu;
	private PauseMenu _pauseMenu;
	private CharacterMenu _characterMenu;
	private bool _isPaused;
	private bool _isMenuConsumingInput;
	private float consumeInputTime;
	private Resolution windowedResolution;

	// INPUT
	public static ActionMap pauseAction = new ActionMap(KeyCode.Escape, Button.RightSpecial);
	public static AxisMap moveAxis = new AxisMap(new AxisConfig(KeyCode.A, -1f), new AxisConfig(KeyCode.D, 1f), new AxisConfig(Axis.LeftStickX, 1f), new AxisConfig(Axis.DpadX, 1f));
	public static ActionMap interactAction = new ActionMap(KeyCode.L, Button.FaceBottom);
	public static ActionMap jumpAction = new ActionMap(KeyCode.W, Button.FaceRight);
	public static ActionMap mainUseAction = new ActionMap(KeyCode.J, Button.FaceLeft);
	public static ActionMap offUseAction = new ActionMap(KeyCode.K, Button.FaceTop);
	public static ActionMap mainAbilityModeAction = new ActionMap(KeyCode.LeftShift, Button.LeftShoulder);
	public static ActionMap offAbilityModeAction = new ActionMap(KeyCode.LeftAlt, Button.RightShoulder);
	public static ActionMap shortcutUseAction = new ActionMap(KeyCode.Alpha1, Button.LeftTrigger);

	public static ItemDatabase items
	{
		get
		{
			if (instance._items == null)
				instance._items = Resources.Load<ItemDatabase>("Item Database");

			return instance._items;
		}
	}

	public static PlayerMob player
	{
		get
		{
			if (instance._player == null)
				instance._player = FindAnyObjectOfType<PlayerMob>();

			return instance._player;
		}
	}

	public static InGameMenu inGameMenu
	{
		get
		{
			if (instance._inGameMenu == null)
				instance._inGameMenu = FindAnyObjectOfType<InGameMenu>();

			return instance._inGameMenu;
		}
	}

	public static PauseMenu pauseMenu
	{
		get
		{
			if (instance._pauseMenu == null)
				instance._pauseMenu = FindAnyObjectOfType<PauseMenu>();

			return instance._pauseMenu;
		}
	}

	public static CharacterMenu characterMenu
	{
		get
		{
			if (instance._characterMenu == null)
				instance._characterMenu = FindAnyObjectOfType<CharacterMenu>();

			return instance._characterMenu;
		}
	}

	public static bool isPaused
	{
		get { return instance._isPaused; }

		set
		{
			instance._isPaused = value;

			Time.timeScale = (instance._isPaused ? 0f : 1f);
			Cursor.visible = instance._isPaused;
		}
	}

	public static bool isMenuConsumingInput
	{
		get
		{
			return instance._isMenuConsumingInput && (Time.time - instance.consumeInputTime > Time.fixedDeltaTime);
		}

		set
		{
			instance._isMenuConsumingInput = value;
			instance.consumeInputTime = Time.time;
		}
	}

	private void Start()
	{
		isPaused = false;

		if (pauseMenu.isActiveAndEnabled)
			pauseMenu.Close();

		if (characterMenu.isActiveAndEnabled)
			characterMenu.Close();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F10))
		{
			if (QualitySettings.vSyncCount < 1) QualitySettings.vSyncCount = 1;
			else QualitySettings.vSyncCount = 0;
		}

		if (Input.GetKeyDown(KeyCode.F11))
		{
			if (!Screen.fullScreen)
			{
				windowedResolution = new Resolution() { width = Screen.width, height = Screen.height };
				Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
			}
			else
			{
				if (windowedResolution.width == 0 || windowedResolution.height == 0)
					windowedResolution = new Resolution() { width = Screen.currentResolution.width / 2, height = Screen.currentResolution.height / 2 };

				Screen.SetResolution(windowedResolution.width, windowedResolution.height, false);
			}
		}

		if (pauseAction.GetDown() && !isPaused)
			pauseMenu.Open();
	}

	private static T FindAnyObjectOfType<T>()
	{
		for (int i = 0; i < SceneManager.sceneCount; i++)
		{
			Scene scene = SceneManager.GetSceneAt(i);

			if (scene.isLoaded)
			{
				GameObject[] allGameObjects = scene.GetRootGameObjects();

				for (int j = 0; j < allGameObjects.Length; j++)
				{
					T result = allGameObjects[j].GetComponentInChildren<T>(true);

					if (result != null)
						return result;
				}
			}
		}

		return default(T);
	}
}
