using UnityEngine;
using System;
using RPG.Saving;
using RPG.Control;
using RPG.Stats;
using RPG.Core;
using RPG.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
	public class Health : MonoBehaviour, ISaveable
	{
		[SerializeField] float regenerationPercentage = 70;
		LazyValue<float> healthPoints;
		[SerializeField] TakeDamageEvent takeDamage;
		float maxHealth;
		[SerializeField] UnityEvent levelUp;
		[SerializeField] UnityEvent onDie;

		[System.Serializable]
		public class TakeDamageEvent : UnityEvent<float>
		{
		}

		bool isDead = false;

		private void Awake()
		{
			healthPoints = new LazyValue<float>(GetInitialHealth);
		}

		private float GetInitialHealth()
		{
			return GetComponent<BaseStats>().GetStat(Stat.Health);
		}

		private void Start()
		{
			healthPoints.ForceInit();
			maxHealth = healthPoints.value;
		}

		private void OnEnable()
		{
			GetComponent<BaseStats>().onLevelUp += LevelUp;
		}

		private void OnDisable()
		{
			GetComponent<BaseStats>().onLevelUp -= LevelUp;
		}

		public bool IsDead()
		{
			return isDead;
		}

		public void TakeDamage(GameObject instigator,float damage)
		{
			healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
			print(healthPoints.value);
			takeDamage.Invoke(damage);
			if (healthPoints.value == 0)
			{
				onDie.Invoke();
				Die();
				AwardExperience(instigator);
			}
		}

		public void Heal(float healthToRestore)
		{
			healthPoints.value = Mathf.Min(healthPoints.value + healthToRestore, GetMaxHealthPoints());
		}
		
		public float GetHealthPoints()
		{
			return healthPoints.value;
		}

		public float GetMaxHealthPoints()
		{
			return GetComponent<BaseStats>().GetStat(Stat.Health);
		}

		private void AwardExperience(GameObject instigator)
		{
			Experience experience = instigator.GetComponent<Experience>();
			if (experience == null) return;

			experience.GainExperience(GetComponent<BaseStats>().GetExperienceReward());
		}

		private void LevelUp()
		{
			levelUp.Invoke();
			RegenerationHealth();
		}

		private void RegenerationHealth()
		{
			float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenerationPercentage / 100);
			if (GetComponent<BaseStats>().GetStat(Stat.Health) < healthPoints.value + regenHealthPoints)
			{
				healthPoints.value = GetComponent<BaseStats>().GetStat(Stat.Health);
			}
			else
			{
				healthPoints.value += regenHealthPoints;
			}
			maxHealth = GetComponent<BaseStats>().GetStat(Stat.Health);
		}

		public object CaptureState()
		{
			return healthPoints.value;
		}

		public void RestoreState(object state)
		{
			healthPoints.value = (float)state;

			if (healthPoints.value == 0)
			{
				Die();
			}
		}

		public float GetPercentage()
		{
			return 100 * GetFraction();
		}

		public float GetFraction()
		{
			return healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health);
		}

		private void Die()
		{
			if (isDead == true) return;
			isDead = true;
			GetComponent<Animator>().SetTrigger("die");
			GetComponent<ActionScheduler>().CancelCurrentAction();
		}
	}
}

