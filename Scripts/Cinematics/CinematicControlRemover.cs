using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Control;
using System.Runtime.CompilerServices;
using System.Collections;
using RPG.SceneManagement;
using UnityEngine.Events;

namespace RPG.Cinematics
{
	public class CinematicControlRemover : MonoBehaviour
	{
		GameObject player;
		GameObject hud;
		[SerializeField] GameObject imageDown;
		[SerializeField] GameObject imageUp;
		[SerializeField] UnityEvent CinematicIn;
		[SerializeField] UnityEvent CinematicOut;

		private void Awake()
		{
			player = GameObject.FindWithTag("Player");
		}

		private void OnEnable()
		{
			GetComponent<PlayableDirector>().played += DisableControl;
			GetComponent<PlayableDirector>().stopped += EnableControl;
		}

		private void OnDisable()
		{
			GetComponent<PlayableDirector>().played -= DisableControl;
			GetComponent<PlayableDirector>().stopped -= EnableControl;
		}
		
		void DisableControl(PlayableDirector pd)
		{
			CinematicIn.Invoke();
			hud = GameObject.Find("HUD");
			print("DisableControl");
			hud.GetComponent<Canvas>().enabled = false;
			BlackBarsFadeIn();
			player.GetComponent<ActionScheduler>().CancelCurrentAction();
			player.GetComponent<PlayerController>().enabled = false;
		}

		private void BlackBarsFadeIn()
		{
			imageDown.GetComponent<BlackBars>().TriggerBarsIn();
			imageUp.GetComponent<BlackBars>().TriggerBarsIn();
		}

		void EnableControl(PlayableDirector pd)
		{
			CinematicOut.Invoke();
			print("EnableControl");
			hud.GetComponent<Canvas>().enabled = true;
			BlackBarsFadeIn();
			player.GetComponent<PlayerController>().enabled = true;
		}
	}
}