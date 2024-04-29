using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    public int cubesPerAxis = 10; // How many cubes are spawnded cubed. Very expensive so keep number low enough
    public Color cubeColor; // Red for attacker blue for defender
    public bool isExploding = false;

    List<GameObject> cubes = new List<GameObject>();

    // Instanstiates all the dummy cubes to explode. Is very expensive to typically run a little before physics is added
    public void SpawnCubes()
    {
        cubes = new List<GameObject>();
        for (int x = 0; x < cubesPerAxis; x++)
        {
            for (int y = 0; y < cubesPerAxis; y++)
            {
                for (int z = 0; z < cubesPerAxis; z++)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.GetComponent<MeshRenderer>().material.color = cubeColor;
                    cube.transform.localScale = transform.localScale / cubesPerAxis / 20;

                    Vector3 bottomLeftCube = transform.position - (transform.localScale / 20) / 2 + cube.transform.localScale / 2;
                    cube.transform.position = bottomLeftCube + Vector3.Scale(new Vector3(x, y, z), cube.transform.localScale);

                    cube.AddComponent<Rigidbody>();
                    cube.transform.parent = transform;
                    cube.SetActive(false);
                    cubes.Add(cube);
                }
            }
        }
    }

    // Enables the cubes to recieve physics as well as giving them an explosion force so they shoot out
    public void ExplodeCube()
    {
        isExploding = true;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        foreach(GameObject cube in cubes)
        {
            cube.SetActive(true);
            cube.GetComponent<Rigidbody>().AddExplosionForce(1000f, transform.position, 4f);
        }
    }

    // Despawns all the cubes otherwise framerate takes a big hit
    public void DespawnCubes()
    {
        foreach(GameObject c in cubes)
        {
            Destroy(c);
        }
        cubes = new List<GameObject>();
        isExploding = false;
    }
}
