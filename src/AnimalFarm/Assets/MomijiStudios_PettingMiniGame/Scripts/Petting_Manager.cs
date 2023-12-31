using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class Petting_Manager : MonoBehaviour
{
	// properties to set in inspector
	[Header("Timer Settings")]
	public float timerStartSeconds = 60;
	public bool showTimerWithMinutes = false;

	[Header("Pet Settings")]
	public float petTooRoughStateDuration = 2.0f;
	public float petScoreIncreaseModifier = 1.0f;
	public float sweetSpotPetScoreMultiplier = 1.25f;
	public int petStateAnimationIndex_normal = 0;
	public int petStateAnimationIndex_rough = 6;

	[Header("Misc. Settings")]
	public float emojiIntervalTime = 0.5f;
	public Vector3 emojiSpawnPositionOffset = new Vector3(0, 0, -.1f);
	public float currentPet_refreshRate = 0.5f;
	public float smileEmojiChance = 0.1f;
	public float animalSoundIntervalTime = 0.5f;
	public float animalSoundChance = 0.01f;

	// object references
	[Header("Object / Component References")]
	public Camera cam;
	public DynamicPettingAnimal animal;
	public Transform touchTargetMarker;
	public AudioSource pettingSound;

	// prefabs
	[Header("Prefabs")]
	public GameObject heartEmojiPrefab;
	public GameObject smileEmojiPrefab;

	// ui references
	[Header("UI References")]
	public TMP_Text timerText;
	public TMP_Text petScoreText;
	public TMP_Text countdownText;
	public TMP_Text petSpeedPerSecondText;
	public TMP_Text finalScoreText;
	public GameObject mainUIParent;
	public GameObject finalScoreParent;

	[Header("Sounds")] 
	public UiSfxPlayer sfx;
	public AudioClipWithVolume countdownBeep;
	public AudioClipWithVolume countdownGo;

	// state control
	bool isPettingEnabled;
	bool isTimerActive;

	// touch control info
	Vector3 previousTouchPosition;

	// session values
	float currentTimerValue;
	float petTooRoughStateEndTime;
	float petScore;
	Collider sweetSpot;
	float nextPossibleEmojiTime;
	private float nextPossibleAnimalSoundTime;

	float currentPet_startTime;
	float currentPet_amount;
	float previousPetSpeed;


	private void Start()
	{
		Setup();

		StartCoroutine(StartPettingMinigame() );
	}


	void Setup ()
	{
		touchTargetMarker.gameObject.SetActive(false);
		currentTimerValue = timerStartSeconds;
		RefreshTimerUI();
		Message.Publish(new ReadyForPettingInit());
		
		if (animal.sweetSpots.Length > 0)
		{
			sweetSpot = animal.sweetSpots[UnityEngine.Random.Range(0, animal.sweetSpots.Length)];
		}
		else
		{
			print("No sweet spots set in Animal script.");
		}
		
		animal.animator.SetInteger("animation", petStateAnimationIndex_normal);
	}



	IEnumerator StartPettingMinigame ()
	{
		// do 3, 2, 1 countdown
		yield return StartCoroutine(Countdown() );
		//yield return null;

		// start timer
		isTimerActive = true;

		// enable petting
		isPettingEnabled = true;
	}


	IEnumerator Countdown ()
	{
		countdownText.gameObject.SetActive(true);

		countdownText.text = "3";
		sfx.Play(countdownBeep);
		yield return new WaitForSeconds(1.0f);

		countdownText.text = "2";
		sfx.Play(countdownBeep);
		yield return new WaitForSeconds(1.0f);

		countdownText.text = "1";
		sfx.Play(countdownBeep);
		yield return new WaitForSeconds(1.0f);

		countdownText.text = "GO!";
		sfx.Play(countdownGo);

		// start fading out countdown text, but go ahead and start minigame now
		StartCoroutine(FadeOutCountdown());
	}
	
	IEnumerator FadeOutCountdown ()
	{
		Color _startColor = countdownText.color;

		Color _endColor = _startColor;
		_endColor.a = 0.0f;


		// fade out
		float t = 0;
		while (t < 1.0f)
		{
			// this makes it so it 1 second total to fade out
			t += Time.deltaTime / 1.0f;

			countdownText.color = Color.Lerp(_startColor, _endColor, t);

			yield return null;
		}


		// disable countdown text
		countdownText.gameObject.SetActive(false);

		// set color to start color in case this minigame restarts without resetting scene
		countdownText.color = _startColor;

	}




	private void Update()
	{
		if (isTimerActive)
		{
			TimerUpdate();
		}

		// if petting is enabled handle petting controls
		if (isPettingEnabled) {
			RefreshCurrentPet();

			PettingUpdate();
		}


		// check if rough pet state needs to end
		if (animal.petState == PetState.pettedTooRough)
		{
			RefreshRoughPetState();
		}
	}


	// This handles decreasing timer value and checking if it hit 0
	void TimerUpdate ()
	{
		// decrease timer value
		currentTimerValue -= Time.deltaTime;

		// if timer value < 0, set it to 0
		if (currentTimerValue < 0)
		{
			currentTimerValue = 0;
		}


		// refresh timer UI
		RefreshTimerUI();


		// check if timer hit 0
		if (currentTimerValue <= 0.0f)
		{
			// timer DID hit 0, so stop minigame
			EndMinigame();
		}
		else
		{
			// timer did NOT hit 0 yet, so keep going

		}

	}


	// Refresh the timer value
	void RefreshTimerUI ()
	{
		if (showTimerWithMinutes)
		{
			// create a string that formats the seconds into MM:SS format
			TimeSpan _timeSpan = TimeSpan.FromSeconds(currentTimerValue);

			string _millisecondsTwoDigits = _timeSpan.Milliseconds.ToString();
			if (_millisecondsTwoDigits.Length > 2)
			{
				_millisecondsTwoDigits = _millisecondsTwoDigits.Substring(0, 2);
			}
			_millisecondsTwoDigits = string.Format(_millisecondsTwoDigits, "00");

			string _timeString = string.Format(_timeSpan.Minutes + ":" + _timeSpan.Seconds.ToString("00") + "." + _millisecondsTwoDigits);

			timerText.text = _timeString;
		}
		else
		{
			timerText.text = currentTimerValue.ToString("0.00");
		}
		
	}


	// Refresh pet score UI
	void RefreshPetScoreUI ()
	{
		petScoreText.text = Mathf.RoundToInt(petScore).ToString();
	}



	// This is where the petting controls happen.
	void PettingUpdate ()
	{
		// if animal is in the petted too rough state don't process petting
		if (animal.petState == PetState.pettedTooRough)
		{
			return;
		}

		// just started touch. still set position of target marker and set previousTouchPosition. don't do anything
		// else that we do in the drag input
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 touchPosition = TouchPositionPercentage;

			// check if player is touching the animal's mesh
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hits = Physics.RaycastAll(ray, 100f);


			// check if there were hits
			if (hits.Length > 0)
			{

				// check if any of the colliders hit are the animals collider
				bool hitAnimal = false;
				RaycastHit animalHit = new RaycastHit();

				foreach (var hit in hits)
				{
					if (hit.collider == animal.collider)
					{
						hitAnimal = true;
						animalHit = hit;

						continue;
					}
				}


				// if the animal was hit proceed with petting
				if (hitAnimal)
				{
					// successfully touched animal collider

					// move touch indicator to hit point
					touchTargetMarker.gameObject.SetActive(true);
					touchTargetMarker.position = animalHit.point;

					// set previous touch position to current one
					previousTouchPosition = touchPosition;

				}

			}

		}

		// get left click mouse drag / touch input
		else if (Input.GetMouseButton(0))
		{
			Vector3 touchPosition = TouchPositionPercentage;

			// check if player is touching the animal's mesh
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hits = Physics.RaycastAll(ray, 100f);


			// check if there were hits
			if (hits.Length > 0)
			{

				// check if any of the colliders hit are the animals collider
				bool hitAnimal = false;
				bool hitSweetSpot = false;
				RaycastHit animalHit = new RaycastHit();
				RaycastHit sweetSpotHit = new RaycastHit();

				foreach (var hit in hits)
				{
					if (hit.collider == animal.collider)
					{
						hitAnimal = true;
						animalHit = hit;

						continue;
					}

					if (sweetSpot != null)
					{
						if (hit.collider == sweetSpot)
						{
							hitSweetSpot = true;
							sweetSpotHit = hit;
						}
					}
					
				}


				// if the animal was hit proceed with petting
				if (hitAnimal)
				{
					// check if petting too fast
					if (previousPetSpeed > animal.petSpeedThreshold)
					{
						SetToPetTooRoughState();
						return;
					}
					else
					{
						SetToBeingPettedState();
					}


					// move touch indicator to hit point
					touchTargetMarker.gameObject.SetActive(true);
					touchTargetMarker.position = animalHit.point;

					// check how far touch dragged since last touch (measured in percentage of screen space
					float dragDistanceFrame = Vector3.Distance(previousTouchPosition, touchPosition);


					// reset currentPet if player wasn't petting last frame
					if (dragDistanceFrame > 0)
					{
						// calculate current pet speed per second
						currentPet_amount += dragDistanceFrame;

						//float _currentPet_totalSeconds = Time.time - currentPet_startTime;
						//float _petsPerSecond = currentPet_amount / _currentPet_totalSeconds;

						//// set speed ui for debug
						//petSpeedPerSecondText.text = _petsPerSecond.ToString("F2");

					}


					// set score value to add to petScore
					float _petScoreToAdd = dragDistanceFrame * petScoreIncreaseModifier;


					// check if this is the sweet spot
					if (hitSweetSpot)
					{
						// if this is the sweet spot increase pet score more
						_petScoreToAdd *= sweetSpotPetScoreMultiplier;

						// show heart emoji
						ShowEmoji(heartEmojiPrefab, animalHit.point);
					}
					else if (Rng.Dbl() < smileEmojiChance)
					{
						ShowEmoji(smileEmojiPrefab, animalHit.point);
					}

					if (Rng.Dbl() < animalSoundChance)
						PlayAnimalSound();
					
					// increase score
					petScore += _petScoreToAdd;


					// refresh pet score text
					RefreshPetScoreUI();


					// set previous touch position to current one for the next frame's check
					previousTouchPosition = touchPosition;

				}

			}
		}

		// when releasing touch hide the target marker
		else if (Input.GetMouseButtonUp(0))
		{
			touchTargetMarker.gameObject.SetActive(false);
			SetToNotBeingPettedState();
		}
	}

	void RefreshCurrentPet ()
	{
		float _timeSinceLastRefresh = Time.realtimeSinceStartup - currentPet_startTime;

		if (_timeSinceLastRefresh >= currentPet_refreshRate)
		{
			// calculate pet speed for tick that's now ending. this will be the "current" pet speed
			previousPetSpeed = currentPet_amount / _timeSinceLastRefresh;
			petSpeedPerSecondText.text = previousPetSpeed.ToString("F2");

			// reset values
			currentPet_startTime = Time.realtimeSinceStartup;
			currentPet_amount = 0;
		}
	}

	// This makes an emoji pop up. This will ONLY work if Time.time is past the [nextPossibleEmojiTime]. This lets
		// there be a delay between emojis.
	// Calling this function with [force] = true will make the emoji spawn regardless of if it is past time or not.
	// By default [useOffset] is true. This makes it so the emoji will spawn at [originPosition] + [emojiSpawnPositionOffset
		// This is useful with a small -Z value so that the emoji won't spawn inside the animal's mesh since the touch point
		// is right against the mesh collider. You can call this function with that false if you want.
	void ShowEmoji (GameObject emojiPrefab, Vector3 originPosition, bool force = false, bool useOffset = true)
	{
		if (!force)
		{
			// if it's not time for the next emoji, stop right here
			if (Time.realtimeSinceStartup < nextPossibleEmojiTime)
			{
				return;
			}
		}

		// set next possible emoji time
		nextPossibleEmojiTime = Time.realtimeSinceStartup + emojiIntervalTime;

		// spawn the emoji
		if (useOffset)
		{
			Instantiate(emojiPrefab, originPosition + emojiSpawnPositionOffset, Quaternion.identity);
		}
		else
		{
			Instantiate(emojiPrefab, originPosition, Quaternion.identity);
		}

	}

	void PlayAnimalSound(bool force = false)
	{
		if (!force && Time.realtimeSinceStartup < nextPossibleAnimalSoundTime)
			return;

		nextPossibleAnimalSoundTime = Time.realtimeSinceStartup + animalSoundIntervalTime;
		Message.Publish(new PlayCurrentAnimalSound());
	}
	
	void EndMinigame ()
	{
		// disable petting, timer, etc.
		isPettingEnabled = false;
		isTimerActive = false;
		pettingSound.volume = 0f;

		// disable touch target marker
		touchTargetMarker.gameObject.SetActive(false);

		// disable ui
		mainUIParent.SetActive(false);

		// set final score value
		finalScoreText.text = Mathf.RoundToInt(petScore).ToString();

		// show final score. the animation will automatically play
		finalScoreParent.SetActive(true);

		// start coroutine to do anything else that needs to happen at the end
		StartCoroutine(EndMinigame_Coroutine());
	}


	IEnumerator EndMinigame_Coroutine ()
	{
		Message.Publish(new ShowFinalSmiles(Mathf.RoundToInt(Mathf.Clamp(Mathf.FloorToInt(petScore / 50f), 1f, 10f))));
		// anything that needs to happen at the end can go here
		yield return null;
	}

	void SetToPetTooRoughState ()
	{
		// hide target marker
		touchTargetMarker.gameObject.SetActive(false);

		// set animal's state
		animal.petState = PetState.pettedTooRough;

		// set state end time
		petTooRoughStateEndTime = Time.realtimeSinceStartup + petTooRoughStateDuration;

		// set animation
		animal.animator.SetInteger("animation", petStateAnimationIndex_rough);
		
		pettingSound.volume = 0f;
	}

	void RefreshRoughPetState ()
	{
		if (Time.realtimeSinceStartup >= petTooRoughStateEndTime)
		{
			// set pet state back to normal
			animal.petState = PetState.notBeingPetted;

			// set animation back to normal
			animal.animator.SetInteger("animation", petStateAnimationIndex_normal);
		}
	}


	void SetToBeingPettedState ()
	{
		// set animal's state
		animal.petState = PetState.beingPetted;

		// set animation
		//animal.animator.SetInteger("animation", 6);
		
		pettingSound.volume = 0.6f;
	}


	void SetToNotBeingPettedState ()
	{
		// set animal's state
		animal.petState = PetState.notBeingPetted;

		// set animation
		//animal.animator.SetInteger("animation", 6);
		
		pettingSound.volume = 0f;
	}
	
	// This is used to get touch screen input as a percentage of screen width / height instead of raw pixels. This is necessary
	// for calculations because this would mean higher resolution device drags would be interpreted as faster drag movement.
	// Returns 0.0 - 1.0 decimal representation of percentage, NOT 0 - 100 percentage.
	Vector3 TouchPositionPercentage
	{
		get
		{
			return new Vector3(Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.height);
		}
	}
}
