using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using Unity.VisualScripting;
using UnityEngine.Playables;
using UnityEngine;

namespace RPG.Control
{
	public class AIController : MonoBehaviour
	{
		[SerializeField] float chaseDistance = 5f;
		[SerializeField] float suspicionTime = 3f;
		[SerializeField] PatrolPath patrolPath;
		[SerializeField] float waypointTolerance = 1f;
		[SerializeField] float waypointDwellTime = 1f;
		[Range(0, 1)]
		[SerializeField] float patrolSpeedFraction = 0.2f;
		[SerializeField] PlayableDirector pb = null;
		GameObject player;
		Fighter fighter;
		Health health;
		Mover mover;

		Vector3 guardPosition;
		float timeSinceLastSawPlayer = Mathf.Infinity;
		float timeSinceArrivedAtWaypoint = Mathf.Infinity;
		int currenWaypointIndex = 0;
		float saveChaseDistance = 0f;

		private void Start()
		{
			//Stop in cinematic
			if (pb != null)
			{
				pb.played += DisableControl;
				pb.stopped += EnableControl;
			}
			fighter = GetComponent<Fighter>();
			//Range by weapon
			if (chaseDistance < fighter.TotalRange())
			{
				chaseDistance = fighter.TotalRange();
			}
			health = GetComponent<Health>();
			mover = GetComponent<Mover>();
			player = GameObject.FindWithTag("Player");

			guardPosition = transform.position;
		}

		void DisableControl(PlayableDirector pd)
		{
			saveChaseDistance = chaseDistance;
			chaseDistance = 0f;
		}

		void EnableControl(PlayableDirector pd)
		{
			chaseDistance = saveChaseDistance;
		}

		private void Update()
		{
			if (health.IsDead()) return;
			if (InAttackRangeOfPlayer() && fighter.CanAttack(player))
			{
				AttackBehaviour();
			}
			else if (timeSinceLastSawPlayer < suspicionTime)
			{
				SuspicionBehaviour();
			}
			else
			{
				PatrolBehavior();
			}
			UpdateTimers();
		}

		private void UpdateTimers()
		{
			timeSinceLastSawPlayer += Time.deltaTime;
			timeSinceArrivedAtWaypoint += Time.deltaTime;
		}

		private void PatrolBehavior()
		{
			Vector3 nextPosition = guardPosition;
			if (patrolPath != null)
			{
				if (AtWaypoint())
				{
					timeSinceArrivedAtWaypoint = 0;
					CycleWayPoint();
				}
				nextPosition = GetCurrentWaypoint();
			}
			if (timeSinceArrivedAtWaypoint > waypointDwellTime)
			{
				mover.StartMoveAction(nextPosition, patrolSpeedFraction);
			}
		}

		private bool AtWaypoint()
		{
			float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
			return distanceToWaypoint < waypointTolerance;
		}

		private void CycleWayPoint()
		{
			currenWaypointIndex = patrolPath.GetNextIndex(currenWaypointIndex);
		}

		private Vector3 GetCurrentWaypoint()
		{
			return patrolPath.GetWaypoint(currenWaypointIndex);
		}

		private void SuspicionBehaviour()
		{
			GetComponent<ActionScheduler>().CancelCurrentAction();
		}

		private void AttackBehaviour()
		{
			timeSinceLastSawPlayer = 0;
			fighter.Attack(player);
		}

		private bool InAttackRangeOfPlayer()
		{
			float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
			return distanceToPlayer < chaseDistance;
		}
		//Acts when hit
		public void IncreaseRangeByDamage()
		{
			StartCoroutine(IncreaseRangeForSeconds(5f));
		}

		private IEnumerator IncreaseRangeForSeconds(float seconds)
		{
			chaseDistance += 5000f;
			yield return new WaitForSeconds(seconds);
			chaseDistance -= 5000f;
		}

		// Called by Unity
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, chaseDistance);
		}
	}
}