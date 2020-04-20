using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// CameraManager controls the camera and various camera settings
public class CameraManager : MonoBehaviour
{
    public GameObject followObject; // GameObject the camera will follow
    private Transform tf; // Transform component for the camera
    public Transform fotf; // Follow object Transform component
    private float currentX = 0.0f; // Current X value of the mouse
    private float currentY = 0.0f; // Current Y value of the mouse
    private const float Y_ANGLE_MIN = 5.0f; // Constant lower boundary to clamp the camera angle (prevents camera from moving under the ground)
    private const float Y_ANGLE_MAX = 50.0f; // Constant upper boundary to clamp the camera angle (prevents camera from going too far over the player's head)

    // Start method
    private void Start()
    {
        tf = GetComponent<Transform>(); // Get the transform component for the camera
    }
    // Update is called once per frame
    void Update()
    {
        if (followObject == null) // Check if the follow object is null
        {
            return;
        }
        else // If the follow object is not null
        {
            if (GameManager.instance.isCameraMouseControlled) // If the camera is mouse controlled...
            {
                currentX += Input.GetAxis("Mouse X"); // Get the current x-axis position of the mouse
                currentY -= Input.GetAxis("Mouse Y"); // Get the current y-axis position of the mouse
                currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX); // Clamp the Y angle to prevent the camera from going beneath the ground or flipping the camera once too far overhead
                GameManager.instance.camZoomDistance -= Input.GetAxis("Mouse ScrollWheel") * GameManager.instance.camZoomSpeed; // Control the zoom distance with the mouse wheel
                // Clamp the zoom distance to prevent the camera from clipping inside the player, also has an upper boundary to limit how far the player can zoom out
                GameManager.instance.camZoomDistance = Mathf.Clamp(GameManager.instance.camZoomDistance, GameManager.instance.minZoomDistance, GameManager.instance.maxZoomDistance);
            }
            else
            {
                if (GameManager.instance.isMoveTowards) // If the camera uses the MoveTowards method...
                {
                    float step = GameManager.instance.camTrailSpeed * Time.deltaTime; // Calculate step based on speed multiplied by deltaTime
                    // Create a new Vector3 for the followObjectPosition. The y and z axis are adjusted to ensure the camera stays the "cameraTrailDistance" away from the followObject. This prevents the
                    // camera from moving directly into the player object
                    Vector3 followObjectPosition = new Vector3(fotf.position.x, (fotf.position.y + GameManager.instance.cameraTrailDistance), (fotf.position.z - GameManager.instance.cameraTrailDistance));
                    // Move the camera towards to the followObjectPosition with a maxDeltaDistance of step
                    tf.transform.position = Vector3.MoveTowards(tf.transform.position, followObjectPosition, step);
                }
                else // If the camera is not using MoveTowards...
                {
                    // This gives more control of the interpolation and allows for different smoothing and speed along different axes.
                    float interpolate = GameManager.instance.camTrailSpeed * Time.deltaTime; // Calculate interpolate time based on speed
                    Vector3 camera = tf.transform.position; // Get the camera position
                    camera.z = Mathf.Lerp(tf.transform.position.z, (followObject.transform.position.z - GameManager.instance.cameraTrailDistance), interpolate); // Lerp the camera's z position
                    camera.x = Mathf.Lerp(tf.transform.position.x, followObject.transform.position.x, interpolate); // Lerp the camera's x position
                    tf.transform.position = camera; // Set the new position of the camera
                }
            }
        }
    }
    // LateUpdate method (Called after Update method)
    private void LateUpdate()
    {
        if (!GameManager.instance.isPaused) // Freezes camera while game is paused
        {
            if (followObject != null) // Check if the follow object is not null
            {
                if (GameManager.instance.isCameraMouseControlled) // if the camera is mouse controlled...
                {
                    // Create a new Vector3, setting the z value to the inverse of the camZoomDistance
                    Vector3 dir = new Vector3(0, 0, -GameManager.instance.camZoomDistance);
                    Quaternion rotation = Quaternion.Euler(currentY, currentX, 0); // Calculate the rotation of the x and y axes based on the currentY and currentX mouse values

                    tf.position = fotf.position + rotation * dir; // Change the camera position based on the position of the followObject by adjusting the rotation and direction
                    tf.LookAt(fotf.position + transform.up); // LookAt method used to turn the camera in the direction of the followObject using world space
                }
            }
        }
    }
}
