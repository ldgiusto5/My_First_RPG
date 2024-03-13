using System;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
	//Asset Menu class
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] GameObject equippedPrefab = null;
		[SerializeField] float weaponDamage = 5f;
		[SerializeField] float weaponRange = 2f;
		[SerializeField] bool isRightHanded = true;
		[SerializeField] GameObject meleeEffect = null;
		[SerializeField] Projectile projectile = null;

		const string weaponName = "Weapon";

		/* Instantiate + animation */
		public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
			DestroyOldWeapon(rightHand,leftHand);
			if (equippedPrefab != null)
			{
				Transform handTransform = GetTransform(rightHand, leftHand);
				GameObject weapon = Instantiate(equippedPrefab, handTransform);
				weapon.name = weaponName;
			}

			var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

			if (animatorOverride != null)
			{
				animator.runtimeAnimatorController = animatorOverride; 
			}
			else if (overrideController != null)
			{
				animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
			}
		}

		private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
		{
			Transform oldweapon = rightHand.Find(weaponName);
			if (oldweapon != null)
			{
				oldweapon.name = "DETROYING";
				Destroy(oldweapon.gameObject);
			}
			oldweapon = leftHand.Find(weaponName);
			if (oldweapon != null) 
			{
				oldweapon.name = "DETROYING";
				Destroy(oldweapon.gameObject);
			}
		}

		private Transform GetTransform(Transform rightHand, Transform leftHand)
		{
			Transform handTransform;
			if (isRightHanded == true) handTransform = rightHand;
			else handTransform = leftHand;
			return handTransform;
		}

		public GameObject HasEffect()
		{
			return meleeEffect;
		}

		public bool HasProjectile()
		{
			return projectile != null;
		}

		public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target)
		{
			Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand,leftHand).position, Quaternion.identity);
			projectileInstance.SetRange(weaponRange);
			projectileInstance.SetTarget(target, weaponDamage);
		}

		public float GetDamage()
		{
			return weaponDamage;
		}
		public float GetRange()
		{
			return weaponRange;
		}
	}
}