using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpriteExtractor : MonoBehaviour
{
	public Texture2D texture; // 스프라이트 시트 텍스처

	void Start()
	{
		// 자동 슬라이싱된 스프라이트 추출
		ExtractSprites();
	}

	void ExtractSprites()
	{
		// 텍스처에서 스프라이트 배열 가져오기
		Sprite[] sprites = UnityEngine.Resources.LoadAll<Sprite>(texture.name);

		// 스프라이트가 존재하는지 확인
		if (sprites == null || sprites.Length == 0)
		{
			Debug.LogError("No sprites found in the texture.");
			return;
		}

		// 폴더 생성 (Assets/ExtractedSprites)
		string folderPath = Application.dataPath + "/ExtractedSprites";
		if (!Directory.Exists(folderPath))
		{
			Directory.CreateDirectory(folderPath);
		}

		// 각 스프라이트 추출 및 저장
		foreach (Sprite sprite in sprites)
		{
			// 스프라이트를 Texture2D로 변환
			Texture2D spriteTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
			spriteTexture.SetPixels(texture.GetPixels((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height));
			spriteTexture.Apply();

			// Texture2D를 PNG 파일로 저장
			byte[] bytes = spriteTexture.EncodeToPNG();
			string spritePath = folderPath + "/" + sprite.name + ".png";
			File.WriteAllBytes(spritePath, bytes);

			Debug.Log("Sprite extracted and saved: " + spritePath);
		}

		Debug.Log("Extraction complete.");
	}
}
