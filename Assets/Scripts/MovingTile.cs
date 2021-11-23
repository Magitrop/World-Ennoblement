using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    None,
    Field,
    Farmland,
    Forest,
    Mountains,
    Water,
    Town
}

public class MovingTile : MonoBehaviour
{
    public int x, z;
    public bool isEmpty;
    public int tileID;

    public Outline outline;

    public TileType type;
    public List<StrictTileCondition> strictConditions;
    public List<TileCondition> conditions;

    public TilesController controller;
    public GameObject tileVisual;

    /// <summary>
    /// For level generation only.
    /// </summary>
    public int movementsCount;

    public void Start()
    {
        if (controller == null)
            controller = FindObjectOfType<TilesController>();

        /*
        GameObject visual = Instantiate(tileVisual, transform.position, Quaternion.identity);
        visual.transform.SetParent(transform);
        if (type != TileType.Field && type != TileType.None)
            visual.transform.localRotation = Quaternion.Euler(-90f, 0, 0);
        visual.transform.localScale = (type == TileType.Field || type == TileType.None) ? new Vector3(1f, 0.25f, 1f) : new Vector3(50f, 50f, 4f);
        outline = visual.GetComponent<Outline>();
        */
    }
}