using UnityEngine;
using System.Collections.Generic;

public class LocalizationManager : MonoBehaviour
{
	// �ٱ��� �ؽ�Ʈ �����͸� ������ ��ųʸ�
	private Dictionary<string, Dictionary<string, string>> localizedTexts = new Dictionary<string, Dictionary<string, string>>();

	// �ٱ��� �ؽ�Ʈ ������ �ε�
	public void LoadLocalizedTexts()
	{
		// �� �� �ؽ�Ʈ �����͸� �ε��Ͽ� ��ųʸ��� ����
		// ���� ���, �ѱ���� ���� �ؽ�Ʈ �����͸� �ε��ϰ� ��ųʸ��� �߰�
		Dictionary<string, string> koreanTexts = new Dictionary<string, string>();
		koreanTexts.Add("hello", "�ȳ��ϼ���!");
		koreanTexts.Add("goodbye", "�ȳ��� ������!");

		Dictionary<string, string> englishTexts = new Dictionary<string, string>();
		englishTexts.Add("hello", "Hello!");
		englishTexts.Add("goodbye", "Goodbye!");

		// �� �ؽ�Ʈ �����͸� ��ųʸ��� �߰�
		localizedTexts.Add("kor", koreanTexts);
		localizedTexts.Add("eng", englishTexts);
	}

	// �ش� ����� �ؽ�Ʈ ������ ��ȯ
	public string GetLocalizedText(string key, string language)
	{
		if (localizedTexts.ContainsKey(language))
		{
			Dictionary<string, string> languageTexts = localizedTexts[language];
			if (languageTexts.ContainsKey(key))
			{
				return languageTexts[key];
			}
		}
		return "Text not found";
	}
}