using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpriteExtractor : MonoBehaviour
{
	public Texture2D texture; // ��������Ʈ ��Ʈ �ؽ�ó

	void Start()
	{
		// �ڵ� �����̵̽� ��������Ʈ ����
		ExtractSprites();
	}

	void ExtractSprites()
	{
		// �ؽ�ó���� ��������Ʈ �迭 ��������
		Sprite[] sprites = UnityEngine.Resources.LoadAll<Sprite>(texture.name);

		// ��������Ʈ�� �����ϴ��� Ȯ��
		if (sprites == null || sprites.Length == 0)
		{
			Debug.LogError("No sprites found in the texture.");
			return;
		}

		// ���� ���� (Assets/ExtractedSprites)
		string folderPath = Application.dataPath + "/ExtractedSprites";
		if (!Directory.Exists(folderPath))
		{
			Directory.CreateDirectory(folderPath);
		}

		// �� ��������Ʈ ���� �� ����
		foreach (Sprite sprite in sprites)
		{
			// ��������Ʈ�� Texture2D�� ��ȯ
			Texture2D spriteTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
			spriteTexture.SetPixels(texture.GetPixels((int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height));
			spriteTexture.Apply();

			// Texture2D�� PNG ���Ϸ� ����
			byte[] bytes = spriteTexture.EncodeToPNG();
			string spritePath = folderPath + "/" + sprite.name + ".png";
			File.WriteAllBytes(spritePath, bytes);

			Debug.Log("Sprite extracted and saved: " + spritePath);
		}

		Debug.Log("Extraction complete.");
	}
}
