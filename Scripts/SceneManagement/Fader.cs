using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.SceneManagement
{
	public class Fader : MonoBehaviour
	{
		CanvasGroup _canvasGroup;
		Coroutine currentActiveFade = null;
		CanvasGroup canvasGroup
		{
			get
			{
				if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
				return _canvasGroup;
			}
		}

		public void FadeOutImmediate()
		{
			canvasGroup.alpha = 1;
		}

		public Coroutine FadeOut(float time)
		{
			return Fade(1, time);
		}

		public Coroutine FadeIn(float time)
		{
			return Fade(0, time);
		}

		public Coroutine Fade(float target, float time)
		{
			if (currentActiveFade != null)
			{
				StopCoroutine(currentActiveFade);
			}
			currentActiveFade = StartCoroutine(FadeRoutine(target, time));
			return currentActiveFade;
		}

		private IEnumerator FadeRoutine(float target, float time)
		{
			while (!Mathf.Approximately(canvasGroup.alpha, target))
			{
				canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime / time);
				yield return null;
			}
		}
	}
}
