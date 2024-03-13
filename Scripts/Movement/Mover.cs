using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
	public class Mover : MonoBehaviour , IAction, ISaveable//, IJsonSaveable
	{
		[SerializeField] Transform target;
		[SerializeField] float maxSpeed = 6f;
		NavMeshAgent navMeshAgent;
		Health health;
		private void Start ()
		{
			navMeshAgent = GetComponent<NavMeshAgent>();
			health = GetComponent<Health>();
		}

		void Update()
		{
			navMeshAgent.enabled = !health.IsDead();
			UpdateAnimator();
		}

		public void StartMoveAction(Vector3 destination, float speedFraction)
		{
			GetComponent<ActionScheduler>().StartAction(this);
			MoveTo(destination, speedFraction);
		}

		public void MoveTo(Vector3 destination, float speedFraction)
		{
			navMeshAgent.destination = destination;
			navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
			navMeshAgent.isStopped = false;
		}

		public void Cancel()
		{
			navMeshAgent.isStopped = true;
		}

		private void UpdateAnimator()
		{
			Vector3 velocity = navMeshAgent.velocity;
			Vector3 localVelocity = transform.InverseTransformDirection(velocity);
			float speed = localVelocity.z;
			GetComponent<Animator>().SetFloat("forwardSpeed", speed);
		}
		[System.Serializable]
		struct MoverSaveData
		{
			public SerializableVector3 position;
			public SerializableVector3 rotation;
		}
		public object CaptureState()
		{
			MoverSaveData data = new MoverSaveData();
			data.position = new SerializableVector3(transform.position);
			data.rotation = new SerializableVector3(transform.eulerAngles);
			return data;
		}

		public void RestoreState(object state)
		{
			MoverSaveData data = (MoverSaveData)state;
			GetComponent<NavMeshAgent>().enabled = false;
			transform.position = data.position.ToVector();
			transform.eulerAngles = data.rotation.ToVector();
			GetComponent<NavMeshAgent>().enabled = true;
		}

	}

}
