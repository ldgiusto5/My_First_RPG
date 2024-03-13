using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using System.Transactions;
using RPG.Saving;
using System;

namespace RPG.Combat
{
	public class Fighter : MonoBehaviour , IAction, ISaveable
	{
		[SerializeField] float timeBetweenAttacks = 1f;
		[SerializeField] Transform rightHandTransform = null;
		[SerializeField] Transform leftHandTransform = null;
		[SerializeField] Weapon defaultWeapon = null;
		[SerializeField] float moreRange = 0f;

		Health target;
		float timeSienceLastAttack = 0;
		public Weapon currentWeapon = null;
		bool hadSword = false;
		bool hadBow = false;
		bool hadFireball = false;

		private void Awake()
		{
			if (currentWeapon == null)
			{
				EquipWeapon(defaultWeapon);
			}
		}

		private void Update()
		{
			timeSienceLastAttack += Time.deltaTime;
			//You can change your weapon if you have already obtained it
			if (Input.GetKeyDown(KeyCode.Q))
			{
				if (hadSword == true)
				{
					Weapon weapon = Resources.Load<Weapon>("Sword");
					EquipWeapon(weapon);
				}
			}
			else if (Input.GetKeyDown(KeyCode.W))
			{
				if (hadBow == true)
				{
					Weapon weapon = Resources.Load<Weapon>("Bow Blue Projectile");
					EquipWeapon(weapon);
				}
			}
			else if (Input.GetKeyDown(KeyCode.E))
			{
				if (hadFireball == true)
				{
					Weapon weapon = Resources.Load<Weapon>("Fireball");
					EquipWeapon(weapon);
				}
			}
			if (target == null) return;
			if (target.IsDead()) return;

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
			if (currentWeapon.HasProjectile())
			{
				currentWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, target);
			}
			else
			{
				if (currentWeapon.HasEffect() != null)
				{
					Instantiate(currentWeapon.HasEffect(), target.transform.position, target.transform.rotation);
				}
				target.TakeDamage(currentWeapon.GetDamage());
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
			return currentWeapon.GetRange() + moreRange;
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

		public void EquipWeapon(Weapon weapon)
		{
			currentWeapon = weapon;
			if (currentWeapon.name == "Sword")
			{
				hadSword = true;
			}
			if (currentWeapon.name == "Bow Blue Projectile")
			{
				hadBow = true;
			}
			if (currentWeapon.name == "Fireball")
			{
				hadFireball = true;
			}
			Animator animator = GetComponent<Animator>();
			weapon.Spawn(rightHandTransform, leftHandTransform, animator);
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
			data.name = currentWeapon.name;
			data.saveHadSword = hadSword;
			data.saveHadBow = hadBow;
			data.saveHadFireball = hadFireball;
			return data;
		}

		public void RestoreState(object state)
		{
			FighterSaveData data = (FighterSaveData)state;
			Weapon weapon = Resources.Load<Weapon>(data.name);
			EquipWeapon(weapon);
			hadSword = data.saveHadSword;
			hadBow = data.saveHadBow;
			hadFireball = data.saveHadFireball;
		}
	}
}
