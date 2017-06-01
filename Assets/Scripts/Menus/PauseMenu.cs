using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : Menu
{
	public Window pauseWindow;
	public Menu mainSubmenu, quitSubmenu;

	private void Start()
	{
		pauseWindow.onOpenFinished += OnWindowOpenFinished;
		pauseWindow.onCloseFinished += OnWindowCloseFinished;
	}

	private void OnWindowOpenFinished()
	{
		SetActiveSubmenu(mainSubmenu, mainSubmenu.defaultSelect);
	}

	private void OnWindowCloseFinished()
	{
		gameObject.SetActive(GameState.isPaused = false);
	}

	public override void Open()
	{
		base.Open();

		GameState.isPaused = true;
		pauseWindow.HideAllChildren();
		pauseWindow.PlayOpenAnimation();
	}

	public override void Close()
	{
		pauseWindow.HideAllChildren();
		pauseWindow.PlayCloseAnimation();
	}

	public void SetActiveSubmenu(Menu submenu)
	{
		pauseWindow.SetChildrenOfParentActive(submenu.transform);
		submenu.Open();
	}

	public void SetActiveSubmenu(Menu submenu, Selectable select)
	{
		pauseWindow.SetChildrenOfParentActive(submenu.transform);
		submenu.Open(select);
	}

	public void Quit()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}
