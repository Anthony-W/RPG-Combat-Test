using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System;
public class CameraRaycaster : MonoBehaviour
{
    float maxRaycastDepth = 100f;

    Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);

    public delegate void OnMouseOverEnemy(Enemy enemy);
    public event OnMouseOverEnemy onMouseOverEnemy;

    void Update()
    {
        // Check if pointer is over an interactable UI element
        //if (EventSystem.current.IsPointerOverGameObject())
        //{
            //implement UI interaction
        //}
        //else
        //{
            performRaycasts();
        //}
    }

    void performRaycasts()
    {
        if (screenRect.Contains(Input.mousePosition))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (RaycastForEnemy(ray)) return;
        }
    }

    bool RaycastForEnemy(Ray ray)
    {
        RaycastHit hitInfo;
        Physics.Raycast(ray, out hitInfo, maxRaycastDepth);
        if (!hitInfo.collider) return false;
        var gameObjectHit = hitInfo.collider.gameObject;
        var enemyHit = gameObjectHit.GetComponent<Enemy>();
        if (enemyHit)
        {
            onMouseOverEnemy(enemyHit);
            return true;
        }
        return false;
    }
}