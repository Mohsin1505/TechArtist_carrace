using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
	[SerializeField] private Vector2 minMaxXPos = new Vector2(5f, -5f);

	[SerializeField] private float horizontalStepCount = 5f;
	[SerializeField] private float swipeThreshold = 50f;
	[SerializeField] private Animator anim;
	[SerializeField] private ParticleSystem LeftParticle;
    [SerializeField] private ParticleSystem RightParticle;

    private bool isMoving = false;
	private int currentStep = 0;
	private Vector3 targetPosition;
	private Vector2 swipeStartPosMouse;
	private bool isSwipingMouse = false;
	private Vector2 swipeStartPosTouch;
	private bool isSwipingTouch = false;
    private void Update()
    {
        HandleMouseSwipe();
        HandleTouchSwipe();

        if (isMoving)
        {
            Vector3 smoothMovePos = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speed);
            transform.position = smoothMovePos;

            // Check if we've reached the target
            if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
            {
                isMoving = false;
                // Reset animations when stopped
                anim.SetBool("Right", false);
                anim.SetBool("Left", false);
            }
        }
        else
        {
            transform.position = new Vector3(targetPosition.x, transform.position.y, transform.position.z);
        }
    }

    private void HandleMouseSwipe()
	{
		if (Mouse.current == null)
		{
			return;
		}

		if (Mouse.current.leftButton.wasPressedThisFrame)
		{
			isSwipingMouse = true;
			swipeStartPosMouse = Mouse.current.position.ReadValue();
		}
		else if (Mouse.current.leftButton.isPressed && isSwipingMouse)
		{
		}
		else if (Mouse.current.leftButton.wasReleasedThisFrame && isSwipingMouse)
		{
			Vector2 endPos = Mouse.current.position.ReadValue();
			Vector2 delta = endPos - swipeStartPosMouse;
			isSwipingMouse = false;

			if (Mathf.Abs(delta.x) > swipeThreshold && Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
			{
				if (delta.x < 0f){
					MoveHorizontal(1);
               
                }
				else{
					MoveHorizontal(-1);
                   
                }
			}
		}
	}

	private void HandleTouchSwipe()
	{
		if (Touchscreen.current == null)
		{
			return;
		}

		var primary = Touchscreen.current.primaryTouch;
		if (!primary.press.isPressed && !isSwipingTouch)
		{
			return;
		}

		if (primary.press.wasPressedThisFrame)
		{
			isSwipingTouch = true;
			swipeStartPosTouch = primary.position.ReadValue();
		}
		else if (primary.press.isPressed && isSwipingTouch)
		{
		}
		else if (primary.press.wasReleasedThisFrame && isSwipingTouch)
		{
			Vector2 endPos = primary.position.ReadValue();
			Vector2 delta = endPos - swipeStartPosTouch;
			isSwipingTouch = false;

			if (Mathf.Abs(delta.x) > swipeThreshold && Mathf.Abs(delta.x) > Mathf.Abs(delta.y)) {
				if (delta.x < 0f) {

					MoveHorizontal(1);
					
				}else {
					MoveHorizontal(-1);
                   
                }
			}
		}
	}
	private void MoveHorizontal(int direction)
	{
		float stepSize = (minMaxXPos.y - minMaxXPos.x) / horizontalStepCount;
		currentStep += direction;
		currentStep = Mathf.Clamp(currentStep, 0, (int)horizontalStepCount);
		float newXPos = minMaxXPos.x + currentStep * stepSize;
		targetPosition = new Vector3(newXPos, transform.position.y, transform.position.z);
        // Handle animations based on direction
        if (direction < 0) // Moving Right
        {
            anim.SetBool("Right", true);
			RightParticle.Play();
            anim.SetBool("Left", false);
        }
        else if (direction > 0) // Moving Left
        {
            anim.SetBool("Right", false);
            LeftParticle.Play();
            anim.SetBool("Left", true);
        }
        if (Vector3.Distance(transform.position, targetPosition) > 0.1f)
		{
			isMoving = true;
		}
		else
		{
			isMoving = false;
		}
	}
}
