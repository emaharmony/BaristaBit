using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Animator))]
public class DoorScript : MonoBehaviour
{
	const float WAIT_TIME = 0.66f;
	SphereCollider _coll; 
	Animator animate;


	int people = 0;
	bool _isOpen = true;
	float _timer;

	// Use this for initialization
	void Start () 
	{
		animate = GetComponent<Animator> ();
		_coll = GetComponent<SphereCollider> ();
		_coll.isTrigger = true;
		_coll.radius = 4;
	}
		
	void Update()
	{
		if (!_isOpen)
			return;

		if (people <= 0) {
			_timer += Time.deltaTime;
			if (_timer >= WAIT_TIME) {
				animate.SetBool ("isOpen", false);
				_isOpen = false;
			}
		}
	}
		

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Customer") 
		{
			animate.SetBool ("isOpen", true);
			people++;
			_isOpen = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Customer") {
			people--;
		}
	}
}
