using UnityEngine;
using System.Collections.Generic;

public class LocalizationManager : MonoBehaviour
{
	// 다국어 텍스트 데이터를 저장할 딕셔너리
	private Dictionary<string, Dictionary<string, string>> localizedTexts = new Dictionary<string, Dictionary<string, string>>();

	// 다국어 텍스트 데이터 로드
	public void LoadLocalizedTexts()
	{
		// 각 언어별 텍스트 데이터를 로드하여 딕셔너리에 저장
		// 예를 들어, 한국어와 영어 텍스트 데이터를 로드하고 딕셔너리에 추가
		Dictionary<string, string> koreanTexts = new Dictionary<string, string>();
		koreanTexts.Add("hello", "안녕하세요!");
		koreanTexts.Add("goodbye", "안녕히 가세요!");

		Dictionary<string, string> englishTexts = new Dictionary<string, string>();
		englishTexts.Add("hello", "Hello!");
		englishTexts.Add("goodbye", "Goodbye!");

		// 언어별 텍스트 데이터를 딕셔너리에 추가
		localizedTexts.Add("kor", koreanTexts);
		localizedTexts.Add("eng", englishTexts);
	}

	// 해당 언어의 텍스트 데이터 반환
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