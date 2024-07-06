using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class PlayerNetwork : NetworkBehaviour
{
    public float moveSpeed = 5f; // Speed of movement

    void Update()
    {
        if (!IsOwner) return;

        // Read input from the legacy input system
        float moveHorizontal = Input.GetAxis("Horizontal"); // X axis movement
        float moveVertical = Input.GetAxis("Vertical");     // Z axis movement

        // Calculate movement direction based on input
        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);

        // Normalize the movement vector if you want constant speed in all directions
        movement = movement.normalized * moveSpeed * Time.deltaTime;

        // Apply movement to the object
        transform.Translate(movement);
    }
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }
}
