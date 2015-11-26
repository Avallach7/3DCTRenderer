using UnityEngine;

namespace Ct3dRenderer.Gui
{
	public class FreeCamera : MonoBehaviour
	{
		private float normalSpeed = 2.0f;
		private float fasterSpeed = 12.0f;
		private float rotationSpeed = 2.0f;
		private float xAngleLimit = 10f;

		public void Start()
		{
			var camera = GetComponent<Camera>();
			camera.nearClipPlane = 0.01f; //50;
			camera.farClipPlane = 10000;
			camera.fieldOfView = 90;
		}

		public void Update()
		{
			if (Input.GetMouseButton(2) || Input.GetMouseButton(1))
			{
				transform.Rotate(-Input.GetAxis("Mouse Y") * rotationSpeed, Input.GetAxis("Mouse X") * rotationSpeed, 0f);
				float xAngle = transform.eulerAngles.x;
				if ((90 - xAngleLimit) < xAngle && xAngle < 180)
					xAngle = (90 - xAngleLimit);
				else if (180 < xAngle && xAngle < (270 + xAngleLimit))
					xAngle = (270 + xAngleLimit);
				transform.eulerAngles = new Vector3(xAngle, transform.eulerAngles.y, 0);
			}

			float verticalMovement = Input.GetKey(KeyCode.Space) ? 1 : 0;
			float moveMultiplier = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? fasterSpeed : normalSpeed;
			bool useWorldSpace = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) ||
								 Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
			transform.Translate(moveMultiplier * (Vector3.forward * Input.GetAxis("Vertical") +
													Vector3.right * Input.GetAxis("Horizontal") +
													Vector3.up * verticalMovement),
								(useWorldSpace ? Space.World : Space.Self));
		}
	}
}
