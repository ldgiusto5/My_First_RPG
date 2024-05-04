using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using RPG.Combat;
using RPG.Control;
using RPG.Attributes;

using RPG.Movement;


namespace RPG.Objects
{
	public class WeaponPickup : MonoBehaviour, IRaycastable
	{
		[SerializeField] WeaponConfig weapon = null;
		[SerializeField] float healthToRestore = 0;
		[SerializeField] float respawnTime = 5;

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.tag == "Player")
			{
				Pickup(other.gameObject);
			}
		}

		private void Pickup(GameObject subject)
		{
			if (weapon != null)
			{
				subject.GetComponent<Fighter>().EquipWeapon(weapon);
			}
			if (healthToRestore > 0)
			{
				subject.GetComponent<Health>().Heal(healthToRestore);
			}
			StartCoroutine(HideForSeconds(respawnTime));
		}

		private IEnumerator HideForSeconds(float seconds)
		{
			ShowPickup(false);
			yield return new WaitForSeconds(seconds);
			ShowPickup(true);
		}

		private void ShowPickup(bool shouldShow)
		{
			GetComponent<Collider>().enabled = shouldShow;
			foreach (Transform child in transform)
			{
				child.gameObject.SetActive(shouldShow);
			}
		}

		public bool HandleRaycast(PlayerController callingController)
		{
			Vector3 target;
			bool hasHit = callingController.RaycastNavMesh(out target);
			if (hasHit)
			{
				if (Input.GetMouseButton(1))
				{
					callingController.gameObject.GetComponent<Mover>().StartMoveAction(target, 1f);
				}
			}
			return true;
		}

		public CursorType GetCursorType()
		{
			return CursorType.Pickup;
		}
	}
}