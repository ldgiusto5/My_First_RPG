using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Saving;

namespace RPG.Cinematics
{
	public class CinematicTrigger : MonoBehaviour, ISaveable
	{
		//SerializeField just to check in editing
		//This bool is to avoid repeating the cinamatics.
		[SerializeField] bool alreadyTriggered = false;

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.tag == "Player" && !alreadyTriggered)
			{
				alreadyTriggered = true;
				Debug.Log(alreadyTriggered);
				GetComponent<PlayableDirector>().Play();
			}
		}
		public object CaptureState()
		{
			return alreadyTriggered;
		}

		public void RestoreState(object state)
		{
			alreadyTriggered = (bool)state;
		}
	}
}
