using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CoffeeCameraController : MonoBehaviour {

	public static CoffeeCameraController Instance{ get; private set;}

	[SerializeField]float speedX = 1;
	[SerializeField] float speedY = 1;
	[SerializeField] float maxX = 0;
	[SerializeField] float maxY = 0;
	[SerializeField] float minX = 0;
	[SerializeField] float minY = 0;
	float pitch = 0, yaw = 0;
	bool _prepPosition = false;

	void Awake()
	{
		Instance = this;
	}
		
	IEnumerator ToggleMouseCursor()
	{
		Cursor.lockState = (!Cursor.visible ? CursorLockMode.Confined : CursorLockMode.Locked);
		yield return new WaitForEndOfFrame ();
		Cursor.visible = !Cursor.visible;
	}

	public void TogglePrepPosition()
	{
		_prepPosition = !_prepPosition;
	}
}
