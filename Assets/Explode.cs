using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour
{
    public int cubesPerAxis = 10;
    public Color cubeColor;
    public bool isExploding = false;

    List<GameObject> cubes;

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

    public void ExplodeCube()
    {
        isExploding = true;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        foreach(GameObject cube in cubes)
        {
            cube.SetActive(true);
            cube.GetComponent<Rigidbody>().AddExplosionForce(1000f, transform.position, 4f);
        }
        Invoke("DespawnCubes", 2f);
    }

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
