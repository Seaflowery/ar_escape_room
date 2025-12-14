using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject hand;
    public float height;
    public float length;
    public bool triggerDown;

    private Tile[] tiles;   // cache Tile components (faster + safer)

    void Start()
    {
        // collect all tiles in scene (tag-based like your original)
        List<Tile> list = new List<Tile>();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("interactable"))
        {
            if (obj.name.StartsWith("tile"))
            {
                Tile t = obj.GetComponent<Tile>();
                if (t != null) list.Add(t);
            }
        }
        tiles = list.ToArray();
    }

    void Update()
    {
        if (!triggerDown || hand == null) return;

        Vector3 newPos = transform.position;

        if (name.StartsWith("tile_h"))
            newPos.z = hand.transform.position.z;
        else if (name.StartsWith("tile_v"))
            newPos.y = hand.transform.position.y;

        bool withinBounds;
        if (name != "tile_h_2")
        {
            withinBounds =
                newPos.y + height / 2 <= 3.7746f &&
                newPos.y - height / 2 >= 1.6012f &&
                newPos.z + length / 2 <= 1.4613f &&
                newPos.z - length / 2 >= -0.71216f;
        }
        else
        {
            withinBounds =
                newPos.y + height / 2 <= 3.7746f &&
                newPos.y - height / 2 >= 1.6012f &&
                newPos.z + length / 2 <= 1.4613f &&
                newPos.z - length / 2 >= -1.0743f;
        }

        if (!CheckCollisions(newPos) && withinBounds)
            transform.position = newPos;
    }

    private bool CheckCollisions(Vector3 newPos)
    {
        if (tiles == null) return false;

        foreach (Tile other in tiles)
        {
            if (other == null || other == this) continue;

            bool overlapY =
                (newPos.y - height / 2 <= other.transform.position.y + other.height / 2) &&
                (newPos.y + height / 2 >= other.transform.position.y - other.height / 2);

            bool overlapZ =
                (newPos.z - length / 2 <= other.transform.position.z + other.length / 2) &&
                (newPos.z + length / 2 >= other.transform.position.z - other.length / 2);

            if (overlapY && overlapZ)
                return true;
        }
        return false;
    }
}
