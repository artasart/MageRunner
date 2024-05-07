using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MEC;
using System.Collections.Generic;

public class Splash_Gold : Splash_Base
{
	[Header("References")]
	Image img_Thumbnail;
	Image img_Golds;
	Transform group_Golds;
	TMP_Text txtmp_Message;
	ParticleSystem particle_Fireworks;

	Material material;

	int amount;

	protected override void OnEnable()
	{
		base.OnEnable();
		ResetMaterial();
	}

	private void OnDisable()
	{
		ResetThumbnail();
		CancelInvoke(nameof(PopSplash));
	}

	protected override void Awake()
	{
		base.Awake();
		useAutoTimeOut = false;

		img_Thumbnail = GetUI_Image(nameof(img_Thumbnail));
		img_Golds = GetUI_Image(nameof(img_Golds));
		txtmp_Message = GetUI_TMPText(nameof(txtmp_Message), string.Empty);
		img_Thumbnail = GetUI_Image(nameof(img_Thumbnail));
		group_Golds = this.transform.Search(nameof(group_Golds));
		particle_Fireworks = Util.GetParticle(this.gameObject, nameof(particle_Fireworks));

		material = img_Thumbnail.material;
		group_Golds.gameObject.SetActive(false);
	}

	public void OpenBox()
	{
		GameManager.Sound.PlaySound("BoxOpen");

		GameManager.Scene.Dim(false);
		float duration = 1.5f;

		Util.RunCoroutine(Co_Emission(duration), nameof(Co_Emission));

		img_Thumbnail.GetComponent<RectTransform>().DOShakePosition(duration, new Vector3(10, 10, 0), 40, 90, false).OnComplete(() =>
		{
			GameManager.Sound.PlaySound("Gold");

			PlayFireworks();

			int gold = CalculateGold();
			ShowGold(gold);

			Invoke(nameof(PopSplash), 2f);
		});
	}

	private void PlayFireworks()
	{
		particle_Fireworks.Play();
	}

	private int CalculateGold()
	{
		int gold = Random.Range(1, 21);
		return gold;
	}

	private void ShowGold(int gold)
	{
		int value = Mathf.Clamp((gold - 1) / 5 + 1, 1, 4);
		img_Golds.sprite = Resources.Load<Sprite>(Define.PATH_SPRITE + $"HandDrawn/Icon_ShopIcon_Coin_0{value}");
		group_Golds.gameObject.SetActive(true);
		ResizeImage(img_Golds, img_Golds.sprite);
		txtmp_Message.text = $"You received <color=#FFC700>{gold * 5000}</color> gold..!";
		img_Thumbnail.gameObject.SetActive(false);
		UpdateGold(gold);
	}

	private void UpdateGold(int gold)
	{
		int goldAmount = gold * 5000;
		LocalData.gameData.gold += goldAmount;
		amount = goldAmount;

		UpdateGoldUI();
		SaveGameData();
	}

	private void ShowToast(string message)
	{
		GameManager.Scene.callback_ShowToast = () => GameManager.UI.FetchPanel<Panel_Main>()?.ShowTopMenu(false);
		GameManager.Scene.callback_CloseToast = () => GameManager.UI.FetchPanel<Panel_Main>()?.ShowTopMenu(true);
		GameManager.Scene.callback_ClickToast = () => GameManager.UI.FetchPanel<Panel_Main>()?.ShowTopMenu(true);
		GameManager.Scene.ShowToastAndDisappear(message);
	}

	private void UpdateGoldUI()
	{
		GameManager.UI.FetchPanel<Panel_Main>().SetGoldUI(LocalData.gameData.gold);
		GameManager.UI.PopPopup();
		GameManager.Scene.Dim(false);
		GameManager.UI.FetchPanel<Panel_Main>().BlockUI();
	}

	private void SaveGameData()
	{
		JsonManager<GameData>.SaveData(LocalData.gameData, Define.JSON_GAMEDATA);
	}

	private void ResetMaterial()
	{
		material.SetFloat("_Value", 0f);
		material.SetFloat("_EmissionStrength", 0f);
	}

	private void ResetThumbnail()
	{
		img_Thumbnail.GetComponent<RectTransform>().localPosition = Vector3.up * 20f;
		img_Thumbnail.gameObject.SetActive(true);
		ResetMaterial();
		group_Golds.gameObject.SetActive(false);
	}

	public static void ResizeImage(Image image, Sprite sprite)
	{
		float spriteWidth = sprite.bounds.size.x * sprite.pixelsPerUnit;
		float spriteHeight = sprite.bounds.size.y * sprite.pixelsPerUnit;
		RectTransform rectTransform = image.rectTransform;
		rectTransform.sizeDelta = new Vector2(spriteWidth, spriteHeight);
	}

	private void PopSplash()
	{
		GameManager.UI.PopSplash();

		ShowToast($"You gained {amount} gold..!");
	}


	private IEnumerator<float> Co_Emission(float duration)
	{
		float elapsedTime = 0.0f;
		float startValue = material.GetFloat("_Value");
		float targetValue = 1.0f;

		while (elapsedTime < duration)
		{
			float newValue = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
			material.SetFloat("_Value", newValue);
			material.SetFloat("_EmissionStrength", newValue);

			yield return Timing.WaitForOneFrame;

			elapsedTime += Time.deltaTime;
		}

		material.SetFloat("_Value", targetValue);
		material.SetFloat("_EmissionStrength", targetValue);
	}
}
