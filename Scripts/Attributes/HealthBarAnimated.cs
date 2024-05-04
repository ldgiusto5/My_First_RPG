using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{

	public class HealthBarAnimated : MonoBehaviour
	{
		public void DestroyBar()
		{
			Destroy(gameObject);
		}
	}
}