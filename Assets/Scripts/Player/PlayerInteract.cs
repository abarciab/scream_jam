using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public Camera playerCamera;
    [SerializeField] LayerMask interactableRaycastLayerMask;

    void Update()
    {
        if (PlayerManager.i.playerCameraController.locked || Time.timeScale == 0) return;
        if (Input.GetMouseButtonDown(0)) CastRay();
    }

    void CastRay()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        bool hit = (Physics.Raycast(ray.origin, ray.direction, out var hitData, 100, interactableRaycastLayerMask));
        if (!hit) return;

        Interactable interactable = hitData.collider.GetComponent<Interactable>();
        if (interactable != null) {
            interactable.Interact();
        }
    }


}
