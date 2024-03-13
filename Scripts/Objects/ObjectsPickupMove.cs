using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Objects
{
	public class ObjectsPickupMove : MonoBehaviour
	{
		[SerializeField] float velocity = 0.035f;
		[SerializeField] float rotation = 130f;
		private float speedY = 0f;
		private int flag = 0;

		public Rigidbody rb;
		private void Start()
		{
			rb = GetComponent<Rigidbody>();
		}

		private void Update()
		{
			if (flag == 0)
			{
				speedY = speedY + velocity;
				if (speedY > 1f)
					flag = 1;
			}
			else if (flag == 1)
			{
				speedY = speedY - velocity;
				if (speedY < -1f)
					flag = 0;
			}
			Vector3 movement = new Vector3(rb.velocity.x, speedY, rb.velocity.z);
			rb.velocity = movement;
			transform.Rotate(new Vector3(0f, rotation, 0f) * Time.deltaTime);
		}
	}
}
