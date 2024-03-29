using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Throwable : NetworkBehaviour
{
    // Start is called before the first frame update
    [Header("References")]
    public Transform cam;
    public Transform attackPoint;
    public GameObject objectToThrow;

    [Header("Settings")]
    public int totalThrows;
    public float throwCooldown;

    [Header("Throwing")]
    public KeyCode throwKey = KeyCode.Mouse0;
    public float throwForce;
    public float throwUpwardForce;

    bool readyToThrow;

    private void Start(){
        readyToThrow = true;
    }

    private void Update(){
        if(Input.GetKeyDown(throwKey))
        {
            Throw();
        }
    }

    private void Throw()
    {
        Quaternion quat = Quaternion.FromToRotation(Vector3.up, transform.forward);
        objectToThrow.transform.rotation = quat * attackPoint.transform.rotation;

        GameObject projectile = Instantiate(objectToThrow, attackPoint.position, quat);

        // Remove Rigidbody component
        Destroy(projectile.GetComponent<Rigidbody>());

        // Add Collider component
        Collider projectileCollider = projectile.AddComponent<BoxCollider>(); // Adjust the collider type as needed

        Vector3 forceDirection = cam.transform.forward;

        RaycastHit hit;

        if(Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackPoint.position).normalized;
        }

        Vector3 forceToAdd = forceDirection * throwForce; // + transform.up * throwUpwardForce;

        // Apply force directly without Rigidbody
        projectile.transform.Translate(forceToAdd * Time.deltaTime, Space.World);
    }
}