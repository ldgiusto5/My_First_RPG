using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
namespace RPG.SceneManagement
{
	//For first portal
	public class EnableColliderWhen : MonoBehaviour
	{
		[SerializeField] Weapon weaponToEnable = null;
		GameObject player;
		void Start()
	    {
			player = GameObject.FindWithTag("Player");
			GetComponent<BoxCollider>().enabled = false;
	    }
	    void Update()
	    {
			if (GetComponent<BoxCollider>().enabled == true) return;
			else if(player.GetComponent<Fighter>().currentWeapon != weaponToEnable)
			{
				GetComponent<BoxCollider>().enabled = true;
			}
		}
	}
}
