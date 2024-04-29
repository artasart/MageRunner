using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Splash_Reward : Splash_Base
{
	TMP_Text txtmp_Name;
	TMP_Text txtmp_Title;
	TMP_Text txtmp_Description;

	Image img_Thumbnail;


	protected override void Awake()
	{
		base.Awake();

		txtmp_Name = GetUI_TMPText(nameof(txtmp_Name), "You have reward!");
		txtmp_Title = GetUI_TMPText(nameof(txtmp_Title), "Chicken Fly");
		txtmp_Description = GetUI_TMPText(nameof(txtmp_Description), "Description must be written here.");
		img_Thumbnail = GetUI_Image(nameof(img_Thumbnail));
		timeout = 2f;
	}

	public void SetSplashInfo(string name, string title, string description, string thumbnailPath)
	{
		txtmp_Name.text = name;
		txtmp_Title.text = title;
		txtmp_Description.text = description;
	}
}
