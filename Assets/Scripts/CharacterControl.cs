using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class CharacterControl : MonoBehaviour
{
    [SerializeField] private float speed, turnSpeed, lerpValue;
    [SerializeField] private LayerMask layer; 
    
    [SerializeField] private Transform collectPoint;
    [SerializeField] private GameObject collectedObject;  
    [SerializeField] private List<GameObject> cubes = new List<GameObject>();

    private Animator animator;
    private Camera camera;

    private void Start()
    {
        camera = Camera.main;
        animator = GetComponent<Animator>();        
    }

    void FixedUpdate()
    {
        if(Input.GetMouseButton(0))
            Movement();
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
            var position = transform.position;
            hitVector.y = position.y;  // Karakterin yukarıya kalkmasını engelleme ~ Freeze Position (Y) de yapılabilir.

            position = Vector3.MoveTowards(position,
                Vector3.Lerp(position, hitVector, lerpValue), 
                speed * Time.deltaTime);
            transform.position = position;

            Vector3 newMovePoint = new Vector3(hit.point.x, position.y, hit.point.z);
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(newMovePoint - position),
                turnSpeed * Time.deltaTime);

            if(!animator.GetBool("running"))
                animator.SetBool("running", true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.StartsWith(transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0,1)))
        {
            other.transform.SetParent(collectPoint);
            Vector3 position = collectedObject.transform.localPosition;
            position.x = 0;
            position.y += 0.22f;
            position.z = 0;

            other.transform.localRotation = new Quaternion(0, 0.7071068f, 0, 0.7071068f); // √2/2 = 0.7071068f;
            other.transform.DOLocalMove(position, 0.2f);
            collectedObject = other.gameObject;
            cubes.Add(other.gameObject);
            other.tag = "Untagged";
            
            GenerateCubes.instance.GenerateCube(1);

            if (cubes.Count > 1 && other.gameObject.tag == "AlignR" ||
                cubes.Count > 1 && other.gameObject.tag !=
                "Align" + transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1) &&
                other.gameObject.tag.StartsWith("Align"))
            {
                GameObject newObject = cubes[cubes.Count - 1];
                cubes.RemoveAt(cubes.Count - 1);
                Destroy(newObject);

                other.GetComponent<MeshRenderer>().material = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material;
                other.GetComponent<MeshRenderer>().enabled = true;
                other.tag = "Align" + transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>()
                    .material.name.Substring(0, 1);
                collectedObject = cubes[0].gameObject;
            }
        }
    }
}
