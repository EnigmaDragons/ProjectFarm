using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Petting_Emoji : MonoBehaviour
{

	// object references
	public SpriteRenderer sprite;

	// properties
	public Vector3 moveDirection = new Vector3(0, 1, 0);
	public float moveSpeed = 0.1f;
	public float fadeStartDelay = 1.0f;
	public float fadeDuration = 1.0f;

	// values
	Color startColor;
	Color endColor;


	private void OnEnable()
	{
		// set color values
		startColor = sprite.color;

		// end color is start color but with an alpha value of 0
		endColor = sprite.color;
		endColor.a = 0;

		// start the fade coroutine
		StartCoroutine(Activate());
	}


	// This makes the emoji go up (or whichever direction specified by [moveDirection]) at the speed specified by [moveSpeed].
	private void Update()
	{
		// this controls moving up
		transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
	}

	
	// It will wait for [fadeStartDelay] seconds, then it will fade out for [fadeDuration] seconds.
	IEnumerator Activate ()
	{
		// wait before starting the fade
		yield return new WaitForSeconds(fadeStartDelay);

		// fade out
		float t = 0;
		while (t < 1.0f)
		{
			// this makes it so it takes [fadeDuration] seconds for t to get from 0 to 1
			t += Time.deltaTime / fadeDuration;

			sprite.color = Color.Lerp(startColor, endColor, t);

			yield return null;
		}

		// destroy the emoji
		Destroy(gameObject);

	}



}
