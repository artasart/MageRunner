using TMPro;

public class Panel_Logo : Panel_Base
{
	TMP_Text txtmp_Download;
	TMP_Text txtmp_LoginMessage;
	
	protected override void Awake()
	{
		base.Awake();

		txtmp_Download = GetUI_TMPText(nameof(txtmp_Download), "loading...");
		txtmp_LoginMessage = GetUI_TMPText(nameof(txtmp_LoginMessage), "username");
		txtmp_Download.UsePingPong();
		txtmp_Download.StartPingPong(.25f);
	}

	public void SetDownload(string message)
	{
		txtmp_Download.text = message;
	}

	public void SetUserLogin(string message)
	{
		txtmp_LoginMessage.text = message;
	}
}
