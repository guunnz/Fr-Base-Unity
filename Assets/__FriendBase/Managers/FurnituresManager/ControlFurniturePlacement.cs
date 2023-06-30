using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ControlFurniturePlacement : MonoBehaviour
{
    [SerializeField] private GameObject selectedAura;
    [SerializeField] private GameObject selectedAuraWrong;
    [SerializeField] private FurnitureRoomController furnitureRoomController;

    public delegate void StatusNotify(ControlFurniturePlacement controlFurniturePlacement, bool canPlace);
    public event StatusNotify OnStatusNotify;

    private int layerWall;
    private int layerFurniture;
    public bool CanPlaceFurniture { get; private set; }

    void Start()
    {
        layerWall = LayerMask.NameToLayer("Wall");
        layerFurniture = LayerMask.NameToLayer("Furniture");

        selectedAura.SetActive(false);
        selectedAuraWrong.SetActive(false);

        CanPlaceFurniture = false;
    }

    public void Select()
    {
        CanPlaceFurniture = false;
        StartCoroutine(SelectCoroutine());
    }

    IEnumerator SelectCoroutine()
    {
        //Force to wait 2 frames so that colliders can update well
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        while (furnitureRoomController.FurnitureGameObject == null)
        {
            yield return new WaitForEndOfFrame();
        }

        //Reset Aura Scale to 1
        selectedAura.transform.localScale = Vector2.one;

        SpriteRenderer spriteRendererFurniture = furnitureRoomController.FurnitureGameObject.GetComponentInChildren<SpriteRenderer>();

        //Get Bounds
        Bounds boundsSprite = spriteRendererFurniture.bounds;
        Bounds boundsAura = selectedAura.GetComponent<SpriteRenderer>().bounds;

        selectedAura.SetActive(true);
        selectedAuraWrong.SetActive(false);

        float percx = boundsSprite.size.x / boundsAura.size.x;
        float percy = boundsSprite.size.y / boundsAura.size.y;

        float maxPerc = Mathf.Max(percx, percy); //Do not use by now

        //Update Aura scale and Position
        selectedAura.transform.localScale = new Vector2(percx, percy) * 1.2f;
        selectedAura.transform.localPosition = spriteRendererFurniture.transform.localPosition;

        //Update WrongAura scale and Position
        selectedAuraWrong.transform.localScale = new Vector2(percx, percy) * 1.2f;
        selectedAuraWrong.transform.localPosition = spriteRendererFurniture.transform.localPosition;

        //Check if we can Place the Item
        CanPlaceFurniture = true;
        Collider2D collider = furnitureRoomController.GetPathfindingCollider();
        bool isFloorItem = furnitureRoomController.IsFloorItem();
        if (isFloorItem)
        {
            collider = furnitureRoomController.GetClickAreaCollider();
        }
        if (collider != null)
        {
            List<Collider2D> results = new List<Collider2D>();
            ContactFilter2D filter = new ContactFilter2D().NoFilter();
            Physics2D.OverlapCollider(collider, filter, results);
            List<Collider2D> obstaclesColliders;

            if (!isFloorItem)
            {
                obstaclesColliders = results.Where(x => (x.gameObject.layer == layerWall || x.gameObject.layer == layerFurniture)).ToList<Collider2D>();
            }
            else
            {
                //If it is a rug => We only check if it collides with walls but we accept collisions with other items
                obstaclesColliders = results.Where(x => (x.gameObject.layer == layerWall)).ToList<Collider2D>();
            }

            if (obstaclesColliders.Count > 0)
            {
                selectedAura.gameObject.SetActive(false);
                selectedAuraWrong.gameObject.SetActive(true);
                CanPlaceFurniture = false;
            }
        }

        if (OnStatusNotify!=null)
        {
            OnStatusNotify(this, CanPlaceFurniture);
        }
    }

    public void Unselect()
    {
        selectedAura.SetActive(false);
        selectedAuraWrong.SetActive(false);
    }
}
