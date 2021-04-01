using UnityEngine;

public class PickupItem : MonoBehaviour
{
    Camera mainCamera;

    PickableItem lastItem;
    PlayerController playerController;

    void Start()
    {
        mainCamera = Camera.main;
        playerController = this.GetComponent<PlayerController>();
    }

    void Update()
    {
        GetHoveredItem();

        if (Input.GetKeyDown(KeyCode.F))
        {
            var weapon = lastItem.gameObject.GetComponentInChildren<BaseWeapon>();
            playerController.Equip(weapon);
            Destroy(lastItem.gameObject);
        }
    }

    void GetHoveredItem()
    {
        RaycastHit centerOfScreen;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out centerOfScreen, 10f))
        {
            var newItem = centerOfScreen.collider.gameObject.GetComponent<PickableItem>();

            // Dont do anything if we are looking at the same item
            if (lastItem == newItem)
            {
                return;
            }

            if (newItem)
            {
                // Dont do anything if the player is to far away
                if (Vector3.Distance(this.transform.position, newItem.transform.position) > 3)
                {
                    return;
                }

                newItem.Hover();
            }

            if (lastItem)
            {
                lastItem.Unhover();
            }

            lastItem = newItem;
        }
    }
}