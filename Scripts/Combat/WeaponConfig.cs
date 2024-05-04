using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
	//Asset Menu class
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] Weapon equippedPrefab = null;
		[SerializeField] float weaponDamage = 5f;
		[SerializeField] float weaponRange = 2f;
		[SerializeField] float percentageBonus = 0;
		[SerializeField] bool isRightHanded = true;
		[SerializeField] GameObject meleeEffect = null;
		[SerializeField] Projectile projectile = null;

		const string weaponName = "Weapon";

		/* Instantiate + animation */
		public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
		{
			DestroyOldWeapon(rightHand,leftHand);

			Weapon weapon = null;


			if (equippedPrefab != null)
			{
				Transform handTransform = GetTransform(rightHand, leftHand);
				weapon = Instantiate(equippedPrefab, handTransform);
				weapon.gameObject.name = weaponName;
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
			return weapon;
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

		public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
		{
			Projectile projectileInstance = Instantiate(projectile, GetTransform(rightHand,leftHand).position, Quaternion.identity);
			projectileInstance.SetRange(weaponRange);
			projectileInstance.SetTarget(target, instigator, calculatedDamage);
		}

		public float GetDamage()
		{
			return weaponDamage;
		}

		public float GetPercentageBonus()
		{
			return percentageBonus;
		}
		
		public float GetRange()
		{
			return weaponRange;
		}
	}
}