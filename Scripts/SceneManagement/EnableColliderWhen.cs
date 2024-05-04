using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
namespace RPG.SceneManagement
{
	//For first portal
	public class EnableColliderWhen : MonoBehaviour
	{
		[SerializeField] WeaponConfig weaponToEnable = null;
		GameObject player;
		void Start()
	    {
			player = GameObject.FindWithTag("Player");
			GetComponent<BoxCollider>().enabled = false;
	    }
	    void Update()
	    {
			if (GetComponent<BoxCollider>().enabled == true) return;
			else if(player.GetComponent<Fighter>().currentWeaponConfig != weaponToEnable)
			{
				GetComponent<BoxCollider>().enabled = true;
			}
		}
	}
}
