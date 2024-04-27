using TMPro;

public class Panel_Logo : Panel_Base
{
	TMP_Text txtmp_Download;

	protected override void Awake()
	{
		base.Awake();

		txtmp_Download = GetUI_TMPText(nameof(txtmp_Download), string.Empty);
	}

	public void SetMessage(string message)
	{
		txtmp_Download.text = message;
	}
}
