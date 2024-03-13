using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Movement;
using RPG.Combat;

namespace RPG.Control
{
	public class PlayerController : MonoBehaviour {
		Health health;

		private void Start()
		{
			health = GetComponent<Health>();
		}
		void Update()
		{
			if (health.IsDead()) return;
			if (InteractWithCombat()) return;
			if (InteractWithMovement()) return;
		}

		private bool InteractWithCombat()
		{
			RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
			//Search for enemies by raycast
			foreach (RaycastHit hit in hits)
			{
				CombatTarget target = hit.transform.GetComponent<CombatTarget>();
				if (target == null) continue;

				if (!GetComponent<Fighter>().CanAttack(target.gameObject))
				{
					continue;
				}

				if (Input.GetMouseButtonDown(1))
				{
					GetComponent<Fighter>().Attack(target.gameObject);
				}
				return true;
			}
			return false;
		}

		public bool InteractWithMovement()
		{
			RaycastHit hit;
			bool hashit = Physics.Raycast(GetMouseRay(), out hit);
			if (hashit)
			{
				if (Input.GetMouseButton(1))
				{
					GetComponent<Mover>().StartMoveAction(hit.point, 1f);
				}
				return true;
			}
			return false;
		}

		private static Ray GetMouseRay()
		{
			return Camera.main.ScreenPointToRay(Input.mousePosition);
		}
	}
}
