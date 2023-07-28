using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Petting_Animal : MonoBehaviour
{

	// properties to set in inspector
	[Header("Settings")]
	public float petSpeedThreshold = 2.0f;


	// references
	[Header("Component References")]
	public Collider collider;
	public Animator animator;
	public Collider[] sweetSpots;

	// pet state and enum
	[HideInInspector]
	public PetState petState;
	
	public enum PetState
	{
		notBeingPetted,
		beingPetted,
		pettedTooRough,
	}


}
