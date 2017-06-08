public interface IInteractable
{
	string interactInfo { get; }

	bool CanInteract(PlayerMob mob);

	void OnInteract(PlayerMob mob);

	void OnInteractEnter();

	void OnInteractExit();
}
