using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;
using Unity.VisualScripting;

namespace RPG.Combat
{
	public class Projectile : MonoBehaviour
	{
		[SerializeField] float speed = 1;
		[SerializeField] float moreDamage = 0;
		[SerializeField] bool isHoming = true;
		[SerializeField] GameObject[] destroyOnHit = null;
		[SerializeField] float lifeAfterImpact = 2;
		[SerializeField] float maxDistanceMiss = 30f;
		[SerializeField] GameObject hitEffect = null;
		[SerializeField] UnityEvent onHit;	

		GameObject instigator = null;
		Health target = null;
		float damage = 0;

		private void Start()
		{
			transform.LookAt(GetAimLocation());
		}

		void Update()
		{
			if (target == null) return;
			//Remove projectiles
			if(Vector3.Distance(target.transform.position, transform.position) > maxDistanceMiss)
			{
				Destroy(gameObject);
			}
			//Follow enemies
			if (isHoming)
			{
				if (!target.IsDead())
				{
					transform.LookAt(GetAimLocation());
				}
			}
			transform.Translate(Vector3.forward * speed * Time.deltaTime);
		}
		public void SetRange(float rangeWeapon)
		{
			maxDistanceMiss += rangeWeapon;
		}
		
		public void SetTarget(Health target, GameObject instigator, float damage)
		{
			this.target = target;
			this.damage = damage + moreDamage;
			this.instigator = instigator;
		}

		private Vector3 GetAimLocation()
		{
			CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();
			if (targetCapsule == null)
			{
				return target.transform.position;
			}
			return target.transform.position + Vector3.up * targetCapsule.height / 2;
		}

		private void OnTriggerEnter(Collider other)
		{
			if(other.GetComponent<Health>() != target) return;
			if (target.IsDead()) return;
			target.TakeDamage(instigator, damage);

			speed = 0;

			onHit.Invoke();
			if (hitEffect != null)
			{
				Instantiate(hitEffect, GetAimLocation(), transform.rotation);
			}
			foreach (GameObject toDestroy in destroyOnHit)
			{
				Destroy(toDestroy);
			}
			Destroy(gameObject, lifeAfterImpact);
		}
	}
}


