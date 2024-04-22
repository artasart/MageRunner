using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class AttackRange : MonoBehaviour
{
	CircleCollider2D circleCollider2D;

	private void Awake()
	{
		circleCollider2D = GetComponent<CircleCollider2D>();
		circleCollider2D.isTrigger = true;
		circleCollider2D.radius = this.GetComponentInParent<MonsterActor>().noticeRange;
	}

	//private void OnTriggerEnter2D(Collider2D other)
	//{
	//	if (other.gameObject.CompareTag(Define.PLAYER))
	//	{
	//		this.GetComponentInParent<MonsterActor>()?.WatchTarget(other.transform);
	//	}
	//}
	//private void OnTriggerExit2D(Collider2D other)
	//{
	//	if (other.gameObject.CompareTag(Define.PLAYER))
	//	{
	//		this.GetComponentInParent<MonsterActor>()?.StopTarget();
	//	}
	// }
}