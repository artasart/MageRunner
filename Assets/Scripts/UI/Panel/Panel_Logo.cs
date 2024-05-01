using TMPro;

public class Panel_Logo : Panel_Base
{
	TMP_Text txtmp_Download;

	protected override void Awake()
	{
		base.Awake();

		txtmp_Download = GetUI_TMPText(nameof(txtmp_Download), "loading...");
		txtmp_Download.UsePingPong();
		txtmp_Download.StartPingPong(.25f);
	}

	public void SetMessage(string message)
	{
		txtmp_Download.text = message;
	}
}
