using System.Collections.Generic;
using UnityEngine;
using MEC;

public static class PoolManager
{
	private static Dictionary<string, ObjectPool<RePoolObject>> objectPools = new Dictionary<string, ObjectPool<RePoolObject>>();
	private static List<PoolData> poolDatas = new List<PoolData>();

	private static Transform spawnParent;
	private static Transform poolParent;

	public static void InitPool()
	{
		objectPools.Clear();

		spawnParent = GameObject.Find(Define.SPAWN).transform;
		poolParent = GameObject.Find(Define.POOL).transform;
	}

	public static void SetPoolData(string name, int size, string path)
	{
		var poolData = new PoolData();
		poolData.name = name;
		poolData.poolSize = size;

		poolDatas.Add(poolData);
		
		var prefab = UnityEngine.Resources.Load<RePoolObject>(path+ name);
		var objectPool = new ObjectPool<RePoolObject>(prefab, poolData, poolParent);

		objectPools.Add(poolData.name, objectPool);
	}

	public static RePoolObject GetPoolObject(string poolName)
	{
		if (objectPools.TryGetValue(poolName, out var pool))
		{
			return pool.GetObject();
		}

		return null;
	}

	public static Dictionary<string, ObjectPool<RePoolObject>> GetObjectPool() => objectPools;



	public static void RePool(GameObject obj, float delay = 0f) => Timing.RunCoroutine(Co_RePool(obj, delay));

	private static IEnumerator<float> Co_RePool(GameObject obj, float delay = 0f)
	{
		yield return Timing.WaitForSeconds(delay);

		obj.SetActive(false);

		obj.transform.SetParent(poolParent);
		obj.transform.localPosition = Vector3.zero;
		obj.transform.localRotation = Quaternion.identity;
		obj.transform.localScale = Vector3.one;
	}


	public static GameObject Spawn(string poolName, Vector3 position, Quaternion rotation, Transform parent = null)
	{
		var obj = GetPoolObject(poolName);

		if (obj != null)
		{
			if (parent == null) obj.transform.SetParent(spawnParent);
			else obj.transform.SetParent(parent);

			obj.transform.localPosition = position;
			obj.transform.localRotation = rotation;

			obj.gameObject.SetActive(true);
		}

		return obj.gameObject;
	}
}