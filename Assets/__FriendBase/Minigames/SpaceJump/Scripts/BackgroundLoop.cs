using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundLoop : MonoBehaviour
{
    [SerializeField] GameObject[] levels;
    private Camera mainCamera;
    private Vector2 screenBounds;
    [SerializeField] float choke;


    private void Awake()
    {
        mainCamera = Camera.main;
        mainCamera.transform.position = this.transform.position;
    }

    private void Start()
    {
       
        screenBounds = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.transform.position.z));
        foreach (GameObject obj in levels)
        {
            LoadChildObject(obj);
        }
    }

    void LoadChildObject(GameObject obj)
    {
        float objectHeight = obj.GetComponent<SpriteRenderer>().bounds.size.y - choke;
        int childsNeeded = 3;

        GameObject clone = Instantiate(obj) as GameObject;
        for (int i = 0; i <= childsNeeded; i++)
        {
            GameObject c = Instantiate(clone) as GameObject;

            c.transform.SetParent(obj.transform);
            c.transform.position = new Vector3(obj.transform.position.x, objectHeight * i, obj.transform.position.z);
            c.name = obj.name + i;
        }
        Destroy(clone);
        Destroy(obj.GetComponent<SpriteRenderer>());
    }

    private void LateUpdate()
    {
        foreach (GameObject obj in levels)
        {
            RepositionChildObjects(obj);
        }
    }

    void RepositionChildObjects(GameObject obj)
    {
        Transform[] children = obj.GetComponentsInChildren<Transform>();

        if (children.Length > 1)
        {
            GameObject firstChild = children[1].gameObject;
            GameObject lastChild = children[children.Length - 1].gameObject;
            float halfObjectHeight = lastChild.GetComponent<SpriteRenderer>().bounds.extents.y - choke;

            if (transform.position.y + 20 + screenBounds.y > lastChild.transform.position.y + halfObjectHeight)
            {
                firstChild.transform.SetAsLastSibling();
                firstChild.transform.position = new Vector3(lastChild.transform.position.x, lastChild.transform.position.y + halfObjectHeight * 2, lastChild.transform.position.z);
            }
            else if (transform.position.y +20 - screenBounds.y < lastChild.transform.position.y - halfObjectHeight)
            {
                lastChild.transform.SetAsFirstSibling();
                lastChild.transform.position = new Vector3(firstChild.transform.position.x, firstChild.transform.position.y - halfObjectHeight * 2, firstChild.transform.position.z);
            }
        }
    }
}