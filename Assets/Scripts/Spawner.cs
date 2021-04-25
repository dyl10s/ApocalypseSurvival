using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // How big of an area to spawn the enemies
    public float SpawnRadius = 10f;

    // Max items to spawn if they get removed it will add more in
    public int MaxSpawned = 3;

    // The minimum amount of time for an enemy to be spawned
    public float SpawnRate = 5f;

    // The variance in the spawn time
    public float SpawnRateVariance = 5f;

    // This is for verifying that there is enough space to spawn the item
    // this is the default for the current zombie
    public float EntityToSpawnHeight = 1.8f;
    public float EntityToSpawnRadius = .29f;

    public GameObject EntityToSpawn;

    List<GameObject> entitiesSpawned = new List<GameObject>();
    float timeSinceSpawn = 0f;
    float selectedSpawnTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        GetNewSpawnRate();
    }

    // Update is called once per frame
    void Update()
    {
        entitiesSpawned = entitiesSpawned.Where(x => x != null).ToList();

        if(entitiesSpawned.Count < MaxSpawned)
        {
            timeSinceSpawn += Time.deltaTime;

            if (timeSinceSpawn > selectedSpawnTime)
            {
                if (Spawn())
                {
                    GetNewSpawnRate();
                    timeSinceSpawn = 0f;
                }
            }
        }
    }

    void GetNewSpawnRate()
    {
        selectedSpawnTime = Random.Range(SpawnRate, SpawnRate + SpawnRateVariance);
    }

    // Returns a boolean stating if it spawned successfully
    bool Spawn()
    {
        // Select a random location
        var xLoc = Random.Range(transform.position.x - (SpawnRadius / 2), transform.position.x + (SpawnRadius / 2));
        var zLoc = Random.Range(transform.position.z - (SpawnRadius / 2), transform.position.z + (SpawnRadius / 2));
        var yLoc = transform.position.y;

        // Find the floor
        RaycastHit hit;
        if(Physics.Raycast(new Vector3(xLoc, yLoc, zLoc), Vector3.down, out hit, 100f))
        {
            yLoc = hit.transform.position.y + .2f;

            // No spawning on top of already spawned entities
            if(hit.transform.name == EntityToSpawn.transform.name)
            {
                return false;
            }
        }
        else if(Physics.Raycast(new Vector3(xLoc, yLoc, zLoc), Vector3.up, out hit, 100f))
        {
            yLoc = hit.transform.position.y + .2f;

            // No spawning on top of already spawned entities
            if (hit.transform.name == EntityToSpawn.transform.name)
            {
                return false;
            }
        }

        if (Physics.CheckCapsule(new Vector3(xLoc, yLoc, zLoc), new Vector3(xLoc, yLoc + EntityToSpawnHeight, zLoc), EntityToSpawnRadius) == true)
        {
            return false;
        }

        var spawnedItem = Instantiate(EntityToSpawn, new Vector3(xLoc, yLoc, zLoc), Quaternion.identity);
        entitiesSpawned.Add(spawnedItem);

        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, SpawnRadius);
    }
}
