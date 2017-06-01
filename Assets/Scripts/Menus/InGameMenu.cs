using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class InGameMenu : Menu
{
	public Text interactText;
	[Header("Dialogue")]
	public Text dialogueText;
	public Text dialogueNameText;
	public GameObject dialogueArrow;
	[Header("HP")]
	public Text HPValueText;
	public Slider HPSlider;
	public Slider HPDamageSlider;
	public GameObject dangerOutline;
	[Header("AP")]
	public Text APValueText;
	public Slider APSlider;
	[Header("Item Get")]
	public RectTransform itemGetWindow;
	public GameObject itemGetPrefab;
	public delegate bool OnDialogueContinue();
	public event OnDialogueContinue onDialogueContinue;

	private float damageTime;
	private Coroutine dialogueCoroutine;
	private Image APSliderFillImage;
	private Color32 APDefaultColor;
	private Color32 canInteractColor = new Color32(0xFC, 0xFC, 0xFC, 0xFF), cantInteractColor = new Color32(0x12, 0x12, 0x12, 0xFF);

	private void Start()
	{
		GameState.player.onDamaged += OnPlayerDamaged;
		APSliderFillImage = APSlider.fillRect.GetComponent<Image>();
		APDefaultColor = APSliderFillImage.color;
		ClearDialogue();
	}

	private void Update()
	{
		if (!GameState.player)
			return;

		// HP

		HPValueText.text = string.Format("{0}/{1}", Mathf.Round(GameState.player.currentHP), GameState.player.maxHP);
		HPSlider.value = Mathf.Round(GameState.player.currentHP) / GameState.player.maxHP;

		if ((Time.time - damageTime) > 1f)
			HPDamageSlider.value = Mathf.Clamp(Mathf.Round(HPDamageSlider.value * 100f - (50f * Time.deltaTime)) / GameState.player.maxHP, HPSlider.value, HPSlider.maxValue);

		dangerOutline.SetActive(Mathf.Round(GameState.player.currentHP) / GameState.player.maxHP <= 0.3f);

		// AP

		APValueText.text = string.Format("{0}/{1}", Mathf.Round(GameState.player.currentAP), GameState.player.maxAP);
		APSlider.value = Mathf.Round(GameState.player.currentAP) / GameState.player.maxAP;
		APSliderFillImage.color = (GameState.player.isAPRecharging ? new Color32(0x94, 0x00, 0x84, 0xFF) : APDefaultColor);

		// INTERACTION

		if (GameState.player.interactor.interactee != null)
		{
			interactText.gameObject.SetActive(true);
			interactText.color = (GameState.player.interactor.interactee.CanInteract(GameState.player) ? canInteractColor : cantInteractColor);
			interactText.text = GameState.player.interactor.interactee.interactInfo;
		}
		else interactText.gameObject.SetActive(false);

		if (GameState.isMenuConsumingInput)
		{
			if (GameState.interactAction.GetDown() || GameState.pauseAction.GetDown())
			{
				if (itemGetWindow.gameObject.activeSelf)
				{
					GameState.isMenuConsumingInput = false;
					itemGetWindow.gameObject.SetActive(false);
				}
				else if (dialogueText.transform.parent.gameObject.activeSelf && dialogueArrow.activeSelf)
				{
					onDialogueContinue();
				}
			}
		}
	}

	public void SetItemGet(params Item[] items)
	{
		// Destroy the previous item get prefabs.
		{
			LayoutElement[] layoutElements = itemGetWindow.GetComponentsInChildren<LayoutElement>(true);

			for (int i = 0; i < layoutElements.Length; i++)
				if (!layoutElements[i].ignoreLayout)
					Destroy(layoutElements[i]);
		}

		// Used to ensure any duplicate items in the array get combined into one.
		List<Item> previousItems = new List<Item>();

		for (int i = 0; i < items.Length; i++)
		{
			if (previousItems.Contains(items[i]))
				continue;

			int amount = items.Count(x => x == items[i]);

			GameObject temp = Instantiate(itemGetPrefab, itemGetWindow, false);
			temp.GetComponent<Text>().text = items[i].name + (amount > 1 ? " x" + amount.ToString() : string.Empty);
			
			previousItems.Add(items[i]);
		}

		GameState.isMenuConsumingInput = true;
		itemGetWindow.gameObject.SetActive(true);
	}

	private IEnumerator DoDialogue(string dialogue, string name)
	{
		int i = -1;

		GameState.isMenuConsumingInput = true;
		dialogueText.text = string.Empty;
		dialogueNameText.text = name;
		dialogueText.transform.parent.gameObject.SetActive(true);
		dialogueArrow.SetActive(false);

		while (++i < dialogue.Length)
		{
			float time = 0.04f;

			if (dialogue.IndexOf("\t", i) == i)
			{
				int startIndex = dialogue.IndexOf("=", i);
				int endIndex = dialogue.IndexOf("f", i);

				time = float.Parse(dialogue.Substring(startIndex + 1, endIndex - startIndex - 1));

				i = endIndex + 1;
			}

			yield return new WaitForSeconds(time);

			dialogueText.text += dialogue[i].ToString();
		}

		dialogueArrow.SetActive(true);

		yield return null;
	}

	public void SetDialogue(string dialogue, string name = "???")
	{
		if (dialogueCoroutine != null)
			StopCoroutine(dialogueCoroutine);

		dialogueCoroutine = StartCoroutine(DoDialogue(dialogue, name));
	}

	public void ClearDialogue()
	{
		if (dialogueCoroutine != null)
			StopCoroutine(dialogueCoroutine);

		GameState.isMenuConsumingInput = false;
		dialogueText.transform.parent.gameObject.SetActive(false);
		dialogueArrow.SetActive(false);
		onDialogueContinue = null;
	}

	private void OnPlayerDamaged(float damage, GameObject instigator, GameObject causer)
	{
		damageTime = Time.time;
		HPDamageSlider.value = HPSlider.value;
	}
}
