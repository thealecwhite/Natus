public interface IInteractable
{
	string interactInfo { get; }

	void OnInteract(PlayerMob mob);

	bool CanInteract(PlayerMob mob);

	void OnInteractEnter();

	void OnInteractExit();
}
