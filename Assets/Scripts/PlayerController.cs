using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    float speed = 0f;
    float rotateSpeed = 75f;

    void Update()
    {
        if (Keyboard.current.wKey.isPressed)
            speed += 5f * Time.deltaTime;

        if (Keyboard.current.sKey.isPressed)
            speed -= 10f * Time.deltaTime;

        speed = Mathf.Max(speed, 0f);

        float h = 0f;
        if (Keyboard.current.aKey.isPressed) h = -1f;
        if (Keyboard.current.dKey.isPressed) h = 1f;

        transform.Rotate(0, h * rotateSpeed * Time.deltaTime, 0);
        transform.Translate(0, 0, speed * Time.deltaTime);
    }
}