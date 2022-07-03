using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    //Abidik gubudik
    [SerializeField] private float speed, turnSpeed, lerpValue;
    [SerializeField] private LayerMask layer; 
    private Animator animator;
    private Camera camera;

    void Start()
    {
        camera = Camera.main;
        animator = GetComponent<Animator>();        
    }

    void FixedUpdate()
    {
        if(Input.GetMouseButton(0))
        {
            Movement();
        }
        else
        {
            if(animator.GetBool("running"))
                animator.SetBool("running", false);
        }
    }

    private void Movement()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = camera.transform.localPosition.z;
        Ray ray = camera.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, layer)) // Eğer bir yere çarpılmışsa
        {
            Vector3 hitVector = hit.point; // Mouse'nin çarptığı nokta
            hitVector.y = transform.position.y;  // Karakterin yukarıya kalkmasını engelleme ~ Freeze Position (Y) de yapılabilir.

            transform.position = Vector3.MoveTowards(transform.position,
                Vector3.Lerp(transform.position, hitVector, lerpValue), 
                speed * Time.deltaTime);
            
            Vector3 newMovePoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(newMovePoint - transform.position),
                turnSpeed * Time.deltaTime);

            if(!animator.GetBool("running"))
                animator.SetBool("running", true);
        }
    }
}
