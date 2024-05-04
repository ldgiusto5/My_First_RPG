using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

namespace RPG.SceneManagement
{
	public class BlackBars : MonoBehaviour
	{
		private bool fadeIn = false;
		[SerializeField] double size = 3.50;
		public void TriggerBarsIn()
		{
			if (GetComponent<RectTransform>().transform.localScale.y == 1)
			{
				fadeIn = true;
				GetComponent<Image>().transform.SetAsLastSibling();
			}
			else
			{
				fadeIn = false;
			}
		}
		private void Update()
		{
			if (fadeIn == true)
			{
				if (GetComponent<RectTransform>().transform.localScale.y < size)
				{
					float next = GetComponent<RectTransform>().transform.localScale.y + 0.03f;
					GetComponent<RectTransform>().localScale = new Vector2(1, next);
				}
			}
			else
			{
				if (GetComponent<RectTransform>().transform.localScale.y > 1)
				{
					float next = GetComponent<RectTransform>().transform.localScale.y - 0.03f;
					GetComponent<RectTransform>().localScale = new Vector2(1, next);
				}
			}
		}
	}

}
