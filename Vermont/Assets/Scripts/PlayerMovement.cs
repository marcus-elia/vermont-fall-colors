// Some of this code is from a tutorial by Brackeys
// https://www.youtube.com/watch?v=_QajrabyTJc

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float speed = 4f;
    public float gravity = -9.81f;
    public float jumpBoostFactor = 1000f;

    private int jumpHeldTime = 0;

    Vector3 velocity;

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetKey(KeyCode.E))
        {
            controller.Move(move * 2*speed * Time.deltaTime);
        }

        if(Input.GetKey(KeyCode.Space))
        {
            if(velocity.y < 0)
            {
                velocity.y = 0;
            }
            jumpHeldTime++;
            velocity.y += GetJumpAmount() * Time.deltaTime;
        }
        else
        {
            jumpHeldTime = 0;

            if (velocity.y > gravity)
            {
                velocity.y += gravity * Time.deltaTime;
            }
        }

        if(transform.position.y > 200)
        {
            velocity.y = gravity;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    private float GetJumpAmount()
    {
        return jumpBoostFactor / (1 + Mathf.Exp(0.01f*(-jumpHeldTime + 5)* (-jumpHeldTime + 5)));
    }
}

