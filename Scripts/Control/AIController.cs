using System;
using System.Collections;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using UnityEngine.Playables;
using UnityEngine;
using Unity.VisualScripting;
using RPG.Utils;

namespace RPG.Control
{
	public class AIController : MonoBehaviour
	{
		[SerializeField] PlayableDirector pb = null;
		[SerializeField] PatrolPath patrolPath;
		[SerializeField] float agroCooldownTime = 5f;
		[SerializeField] float chaseDistance = 5f;
		[SerializeField] float suspicionTime = 3f;
		[SerializeField] float waypointTolerance = 1f;
		[SerializeField] float waypointDwellTime = 1f;
		[Range(0, 1)]
		[SerializeField] float patrolSpeedFraction = 0.2f;
		[SerializeField] float shoutDistance = 5f;
		GameObject player;
		Fighter fighter;
		Health health;
		Mover mover;
		LazyValue<Vector3> guardPosition;
		float timeSinceAggrevated = Mathf.Infinity;
		float timeSinceLastSawPlayer = Mathf.Infinity;
		float timeSinceArrivedAtWaypoint = Mathf.Infinity;
		int currenWaypointIndex = 0;

		private void Awake()
		{
			fighter = GetComponent<Fighter>();
			health = GetComponent<Health>();
			mover = GetComponent<Mover>();
			player = GameObject.FindWithTag("Player");
			guardPosition = new LazyValue<Vector3>(GetGuardPosition);
		}

		private Vector3 GetGuardPosition()
		{
			return transform.position;
		}

		private void Start()
		{
			guardPosition.ForceInit();
			if (chaseDistance < fighter.TotalRange())
			{
				chaseDistance = fighter.TotalRange();
			}
			if (pb != null)
			{
				pb.played += DisableControl;
				pb.stopped += EnableControl;
			}
		}

		void DisableControl(PlayableDirector pd)
		{
			gameObject.GetComponent<ActionScheduler>().CancelCurrentAction();
			gameObject.GetComponent<Fighter>().enabled = false;

		}

		void EnableControl(PlayableDirector pd)
		{
			gameObject.GetComponent<Fighter>().enabled = true;
		}

		private void Update()
		{
			if (health.IsDead()) return;
			if (IsAggrevated() && fighter.CanAttack(player))
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

		public void Aggrevate()
		{
			timeSinceAggrevated = 0;
		}

		private void UpdateTimers()
		{
			timeSinceLastSawPlayer += Time.deltaTime;
			timeSinceArrivedAtWaypoint += Time.deltaTime;
			timeSinceAggrevated += Time.deltaTime;

		}

		private void PatrolBehavior()
		{
			Vector3 nextPosition = guardPosition.value;
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

			AggrevateNearbyEnemies();
		}

		private void AggrevateNearbyEnemies()
		{
			RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
			foreach (RaycastHit hit in hits)
			{
				AIController ai = hit.collider.GetComponent<AIController>();
				if (ai == null) continue;

				ai.Aggrevate();
			}
		}

		private bool IsAggrevated()
		{
			float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
			return distanceToPlayer < chaseDistance || timeSinceAggrevated < agroCooldownTime;
		}

		// Called by Unity
		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, chaseDistance);
		}

	}
}