using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using RPG.Combat;

namespace RPG.Objects
{
	public class WeaponPickup : MonoBehaviour
	{
		[SerializeField] Weapon weapon = null;
		[SerializeField] float respawnTime = 5;

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.tag == "Player")
			{
				other.GetComponent<Fighter>().EquipWeapon(weapon);
				StartCoroutine(HideForSeconds(respawnTime));
			}
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
	}
}