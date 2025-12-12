using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		[Header("Look Sensitivity")]
		[Range(0.05f, 3f)] public float mouseXSensitivity = 0.6f;
		[Range(0.05f, 3f)] public float mouseYSensitivity = 0.6f;
		[Range(10f, 540f)] public float stickXSensitivity = 120f;
		[Range(10f, 540f)] public float stickYSensitivity = 120f;
		public bool invertY = false;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

		public void LookInput(Vector2 newLookDirection)
		{
			//look = newLookDirection;
#if ENABLE_INPUT_SYSTEM
			bool isMouse = false;
			var playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
			if (playerInput != null) isMouse = playerInput.currentControlScheme == "KeyboardMouse";
#else
			bool isMouse = true;
#endif
			if (isMouse)
			{
				float y = invertY ? -newLookDirection.y : newLookDirection.y;
				look = new Vector2(newLookDirection.x * mouseXSensitivity, y * mouseYSensitivity);
			}
            else
            {
				float y = invertY ? -newLookDirection.y : newLookDirection.y;
				look = new Vector2(newLookDirection.x * stickXSensitivity, y * stickYSensitivity);
            }

		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}
		
		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}