using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float translateSpeed = 100f;
	public float rotateSpeed = 100f;

	void Update()
	{
		var x = Input.GetAxis("Horizontal") * Time.deltaTime * translateSpeed;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * rotateSpeed;

		transform.Rotate(0, x, 0);
		transform.Translate(0, 0, z);
	}
}