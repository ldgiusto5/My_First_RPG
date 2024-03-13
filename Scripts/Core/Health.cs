using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;
using RPG.Saving;
using RPG.Control;

namespace RPG.Core
{
	public class Health : MonoBehaviour, ISaveable//, IJsonSaveable
	{

		[SerializeField] float healthPoints = 100f;

		bool isDead = false;

		public bool IsDead()
		{
			return isDead;
		}

		public void TakeDamage(float damage)
		{
			healthPoints = Mathf.Max(healthPoints - damage, 0);
			if (gameObject.tag != "Player")
			{
				GetComponent<AIController>().IncreaseRangeByDamage();
			}
			print(healthPoints);
			if (healthPoints == 0)
			{
				Die();
			}
		}

		public object CaptureState()
		{
			return healthPoints;
		}

		public void RestoreState(object state)
		{
			healthPoints = (float)state;

			if (healthPoints == 0)
			{
				Die();
			}
		}

		// public JToken CaptureAsJToken()
		// {
		// 	return JToken.FromObject(health);
		// }

		// public void RestoreFromJToken(JToken state)
		// {
		// 	health = state.ToObject<float>();
		// 	UpdateState();
		// }

		private void Die()
		{
			if (isDead == true) return;
			isDead = true;
			GetComponent<Animator>().SetTrigger("die");
			GetComponent<ActionScheduler>().CancelCurrentAction();
			//mio
			//GetComponent<CapsuleCollider>().enabled = false;
		}
	}
}

