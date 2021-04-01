using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickableItem : MonoBehaviour
{
    public Material PickupMaterial;
    public GameObject Item;

    BoxCollider pickupCollider;
    MeshRenderer[] allRenderers;
    List<Material> oldMaterials = new List<Material>();

    // Start is called before the first frame update
    void Start()
    {
        var item = Instantiate(Item, transform);
        pickupCollider = this.GetComponent<BoxCollider>();
        FitColliderToChildren(item);
        allRenderers = GetComponentsInChildren<MeshRenderer>();
    }

    void FitColliderToChildren(GameObject parentObject)
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        bool hasBounds = false;
        Renderer[] renderers = parentObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer render in renderers)
        {
            if (hasBounds)
            {
                bounds.Encapsulate(render.bounds);
            }
            else
            {
                bounds = render.bounds;
                hasBounds = true;
            }
        }

        if (hasBounds)
        {
            pickupCollider.center = bounds.center - parentObject.transform.position;
            pickupCollider.size = bounds.size;
        }
        else
        {
            pickupCollider.size = pickupCollider.center = Vector3.zero;
            pickupCollider.size = Vector3.zero;
        }
    }

    public void Hover()
    {
        foreach(var mesh in allRenderers)
        {
            oldMaterials.Add(mesh.material);
            mesh.material = PickupMaterial;
        }
        
    }

    public void Unhover()
    {
        var curMesh = 0;

        foreach (var mesh in allRenderers)
        {
            mesh.material = oldMaterials[curMesh];
            curMesh++;
        }

        oldMaterials.Clear();
    }
}
