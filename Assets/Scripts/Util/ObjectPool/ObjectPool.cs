using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
	public List<T> Pool { get; private set; } = new List<T>();

	private T prefab;
	private Transform poolParent;
	private PoolData poolData; // 추가: PoolData를 저장하는 변수

	public ObjectPool(T prefab, PoolData poolData, Transform poolParent)
	{
		this.prefab = prefab;
		this.poolParent = poolParent;
		this.poolData = poolData; // 추가: PoolData 저장

		InitializePool(poolData.poolSize);
	}

	private void InitializePool(int initialSize)
	{
		for (int i = 0; i < initialSize; i++)
		{
			T obj = Object.Instantiate(prefab);
			obj.transform.SetParent(poolParent);
			obj.gameObject.SetActive(false);

			Pool.Add(obj);
		}
	}

	public T GetObject()
	{
		foreach (var obj in Pool)
		{
			if (!obj.gameObject.activeSelf)
			{
				obj.gameObject.SetActive(true);
				obj.transform.SetParent(poolParent);

				return obj;
			}
		}

		T newObj = Object.Instantiate(prefab);

		newObj.transform.SetParent(poolParent);

		Pool.Add(newObj);

		return newObj;
	}
}

[System.Serializable]
public class PoolData
{
	public string name;
	public int poolSize;
}