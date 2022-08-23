using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateCubes : MonoBehaviour
{
    public static GenerateCubes instance;
    [SerializeField] private GameObject redCube, greenCube, blueCube;
    [SerializeField] private Transform redCubeParent, greenCubeParent, blueCubeParent;
    [SerializeField] private int minimumX, maximumX, minimumZ, maximumZ;
    [SerializeField] private LayerMask layerMask;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    
    public void GenerateCube(int number, CharacterAI characterAI = null) // 0, 1, 2 - R, B, G
    {
        if (number == 0)
            Generate(redCube, redCubeParent, characterAI); 
        if (number == 1)
            Generate(blueCube, blueCubeParent);  
        if (number == 2)
            Generate(greenCube, greenCubeParent, characterAI);  
    }

    private void Generate(GameObject gameObject, Transform parent, CharacterAI characterAI = null)
    {
            GameObject gameObjectI = Instantiate(gameObject);
            Vector3 destinationPosition = GiveRandomPosition();
            gameObjectI.SetActive(false);
            Collider[] colliders = Physics.OverlapSphere(destinationPosition, 1, layerMask);
            // Sadece bu layerMask'te olanlarin colliderlarini al.
            while (colliders.Length != 0)
            {
                Debug.Log("Explode" + colliders[0].gameObject + " " + destinationPosition);
                destinationPosition = GiveRandomPosition();
                colliders = Physics.OverlapSphere(destinationPosition, 1, layerMask);
            }
            gameObjectI.SetActive(true);
            gameObjectI.transform.position = destinationPosition;
            
    }

    private Vector3 GiveRandomPosition()
    { 
        return new Vector3(Random.Range(minimumX, maximumX), redCube.transform.position.y, Random.Range(minimumZ, maximumZ));
    }
}
