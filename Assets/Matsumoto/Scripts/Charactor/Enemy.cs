using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public Vector2 MovePosition;
	public float Speed;

	private Vector2 _startPosition;

	// Use this for initialization
	void Start () {
		_startPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		StartCoroutine(Moving());
	}

	public void Attack() {
		Destroy(gameObject);
	}

	IEnumerator Moving() {

		var t = 0.0f;
		while((t += Time.deltaTime * Speed) <= 1.0f) {
			transform.position = Vector2.Lerp(_startPosition, _startPosition + MovePosition, t);
			yield return null;
		}

		while((t -= Time.deltaTime * Speed) >= 0.0f) {
			transform.position = Vector2.Lerp(_startPosition, _startPosition + MovePosition, t);
			yield return null;
		}

		StartCoroutine(Moving());
	}
}
