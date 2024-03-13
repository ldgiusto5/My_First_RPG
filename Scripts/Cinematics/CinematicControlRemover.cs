using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Control;
using System.Runtime.CompilerServices;
using System.Collections;

namespace RPG.Cinematics
{
	public class CinematicControlRemover : MonoBehaviour
	{
		GameObject player;

		private void Start()
		{
			player = GameObject.FindWithTag("Player");
			//Includes the PlayableDirector in the funcion
			GetComponent<PlayableDirector>().played += DisableControl;
			GetComponent<PlayableDirector>().stopped += EnableControl;
		}

		void DisableControl(PlayableDirector pd)
		{
			print("DisableControl");
			player.GetComponent<ActionScheduler>().CancelCurrentAction();
			player.GetComponent<PlayerController>().enabled = false;
		}

		void EnableControl(PlayableDirector pd)
		{
			print("EnableControl");
			player.GetComponent<PlayerController>().enabled = true;
		}
	}
}