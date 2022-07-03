using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class CharacterAI : MonoBehaviour
{
    [SerializeField] private GameObject targetsParent;
    [SerializeField] List<GameObject> targets = new List<GameObject>();
    [SerializeField] private float sphereRadius = 2;
    [SerializeField] private Transform collectPoint;
    [SerializeField] private GameObject collectedObject;  
    [SerializeField] private List<GameObject> cubes = new List<GameObject>();
    [SerializeField] private Transform[] ropes;
    
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private bool haveTarget = false;
    private Vector3 targetTransform;

    void Start()
    {
        for(int i=0; i<targetsParent.transform.childCount; i++)
        {
            targets.Add(targetsParent.transform.GetChild(i).gameObject);
        }
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if(!haveTarget && targets.Count > 0)
            ChooseTarget();
    }

    void ChooseTarget()
    {
        int randomNumber = Random.Range(0, 3);
        if(randomNumber == 0 && cubes.Count >= 5)
        {
            int randomRope = Random.Range(0, ropes.Length);
            List<Transform> ropesNonActiveChild = new List<Transform>();
            foreach(Transform item in ropes[randomRope])
            {
                if(!item.GetComponent<MeshRenderer>().enabled || item.GetComponent<MeshRenderer>().enabled && item.gameObject.tag != "Align" + 
                    transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1))
                {
                    ropesNonActiveChild.Add(item);
                }
            }
            targetTransform = cubes.Count > ropesNonActiveChild.Count ? ropesNonActiveChild[ropesNonActiveChild.Count - 1].position : 
                              ropesNonActiveChild[cubes.Count].position; // if-else
        }

        else
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, sphereRadius);
            List<Vector3> ourColors = new List<Vector3>();
            for(int i=0; i<hitColliders.Length; i++)
            {
                // GreenCharacter'in tag'ı transform'un çayldının çayldının 1.çayldındaki SkinnedMeshRenderer'in materialinin 0. indexinden başlayarak 1 harf al demek
                // greenMat = g alacak. | degiştir 
                if(hitColliders[i].tag.StartsWith(transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0,1)))
                { 
                    ourColors.Add(hitColliders[i].transform.position);
                }
            }
            if(ourColors.Count > 0)
                targetTransform = ourColors[0];
            else
            {
                int random = Random.Range(0, targets.Count);
                targetTransform = targets[random].transform.position;
            }
        }
            navMeshAgent.SetDestination(targetTransform);
            if(!animator.GetBool("running"))
                animator.SetBool("running", true);
            haveTarget = true;
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
        
            targets.Remove(other.gameObject);
            other.tag = "Untagged";
            haveTarget = false;
        }    
    }
}
