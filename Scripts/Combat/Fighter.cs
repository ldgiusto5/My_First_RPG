using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Attributes;
using RPG.Saving;
using RPG.Stats;
using RPG.Utils;
using System;
using System.Transactions;

namespace RPG.Combat
{
	public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
	{
		[SerializeField] float timeBetweenAttacks = 1f;
		[SerializeField] Transform rightHandTransform = null;
		[SerializeField] Transform leftHandTransform = null;
		[SerializeField] WeaponConfig defaultWeapon = null;
		[SerializeField] float moreRange = 0f;

		Health target;
		float timeSienceLastAttack = 0;
		bool hadSword = false;
		bool hadBow = false;
		bool hadFireball = false;
		public WeaponConfig currentWeaponConfig;
		LazyValue<Weapon> currentWeapon;

		private void Awake()
		{
			currentWeaponConfig = defaultWeapon;
			currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
		}

		private Weapon SetupDefaultWeapon()
		{
			return AttachWeapon(defaultWeapon);
		}

		private void Start()
		{
			currentWeapon.ForceInit();
		}

		private void Update()
		{
			timeSienceLastAttack += Time.deltaTime;
			//You can change your weapon if you have already obtained it
			if (Input.GetKeyDown(KeyCode.Q))
			{
				if (hadSword == true)
				{
					WeaponConfig weapon = Resources.Load<WeaponConfig>("Sword");
					EquipWeapon(weapon);
				}
			}
			else if (Input.GetKeyDown(KeyCode.W))
			{
				if (hadBow == true)
				{
					WeaponConfig weapon = Resources.Load<WeaponConfig>("Bow Blue Projectile");
					EquipWeapon(weapon);
				}
			}
			else if (Input.GetKeyDown(KeyCode.E))
			{
				if (hadFireball == true)
				{
					WeaponConfig weapon = Resources.Load<WeaponConfig>("Fireball");
					EquipWeapon(weapon);
				}
			}
			if (target == null) return;
			if (target.IsDead()) 
			{
				Cancel();
				return;
			}

			if (!GetIsInRange())
			{
				GetComponent<Mover>().MoveTo(target.transform.position, 1f);
			}
			else
			{
				GetComponent<Mover>().Cancel();
				AttackBehavior();
			}
		}

		public Health GetTarget()
		{
			return target;
		}


		private void AttackBehavior()
		{
			
			transform.LookAt(target.transform);
			if (timeSienceLastAttack > timeBetweenAttacks)
			{
				//triger the Hit() event in animation.
				TriggerAttack();
				timeSienceLastAttack = Mathf.Infinity;
			}
		}

		private void TriggerAttack()
		{
			GetComponent<Animator>().ResetTrigger("stopAttack");
			GetComponent<Animator>().SetTrigger("attack");
		}

		//Animation event
		void Hit()
		{
			if (target == null) return;
			float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
			if (currentWeapon.value != null)
			{
				currentWeapon.value.OnHit();
			}
			if (currentWeaponConfig.HasProjectile())
			{
				currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
			}
			else
			{
				if (currentWeaponConfig.HasEffect() != null)
				{
					Instantiate(currentWeaponConfig.HasEffect(), target.transform.position, target.transform.rotation);
				}
				target.TakeDamage(gameObject, damage);
			}
		}

		void Shoot()
		{
			Hit();
		}

		private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < TotalRange();
        }

		public float TotalRange()
		{
			return currentWeaponConfig.GetRange() + moreRange;
		}

		public bool CanAttack(GameObject combatTarget)
		{
			if (combatTarget == null) { return false; }
			Health targetToTest = combatTarget.GetComponent<Health>();
			return targetToTest != null && !targetToTest.IsDead();
		}

		public void Attack(GameObject combatTarget)
		{
			GetComponent<ActionScheduler>().StartAction(this);
			target = combatTarget.GetComponent<Health>();
		}

		public void Cancel()
		{
			TriggerStopAttack();
			target = null;
			GetComponent<Mover>().Cancel();
		}

		private void TriggerStopAttack()
		{
			GetComponent<Animator>().ResetTrigger("attack");
			GetComponent<Animator>().SetTrigger("stopAttack");
		}

		public IEnumerable<float> GetAdditiveModifiers(Stat stat)
		{
			if (stat == Stat.Damage)
			{
				yield return currentWeaponConfig.GetDamage();
			}
		}

		public IEnumerable<float> GetPercentageModifiers(Stat stat)
		{
			if (stat == Stat.Damage)
			{
				yield return currentWeaponConfig.GetPercentageBonus();
			}
		}

		public void EquipWeapon(WeaponConfig weapon)
		{
			currentWeaponConfig = weapon;
			if (currentWeaponConfig.name == "Sword")
			{
				hadSword = true;
			}
			if (currentWeaponConfig.name == "Bow Blue Projectile")
			{
				hadBow = true;
			}
			if (currentWeaponConfig.name == "Fireball")
			{
				hadFireball = true;
			}
			currentWeapon.value = AttachWeapon(weapon);
		}

		private Weapon AttachWeapon(WeaponConfig weapon)
		{
			Animator animator = GetComponent<Animator>();
			return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
		}

		[System.Serializable]
		struct FighterSaveData
		{
			public string name;
			public bool saveHadSword;
			public bool saveHadBow;
			public bool saveHadFireball;
		}

		public object CaptureState()
		{
			FighterSaveData data = new FighterSaveData();
			data.name = currentWeaponConfig.name;
			data.saveHadSword = hadSword;
			data.saveHadBow = hadBow;
			data.saveHadFireball = hadFireball;
			return data;
		}

		public void RestoreState(object state)
		{
			FighterSaveData data = (FighterSaveData)state;
			WeaponConfig weapon = Resources.Load<WeaponConfig>(data.name);
			EquipWeapon(weapon);
			hadSword = data.saveHadSword;
			hadBow = data.saveHadBow;
			hadFireball = data.saveHadFireball;
		}
	}
}
