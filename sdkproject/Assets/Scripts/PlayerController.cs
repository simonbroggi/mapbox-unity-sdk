using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float translateSpeed = 100f;
	public float rotateSpeed = 100f;

	private Animator animator;
	private void Start()
	{
		animator = GetComponent<Animator>();
	}
	void Update()
	{
		var x = Input.GetAxis("Horizontal") * Time.deltaTime * rotateSpeed;
		var z = Input.GetAxis("Vertical") * Time.deltaTime * translateSpeed;

		transform.Rotate(0, x, 0);
		if (z > 0)
		{
			transform.Translate(0, 0, z);
			animator.SetBool("IsRunning", true);
		}
		else
		{
			animator.SetBool("IsRunning", false);
		}
	}
}