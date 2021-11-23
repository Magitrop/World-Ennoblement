using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonMove : MonoBehaviour, IPointerDownHandler
{
    public TilesController tilesController;
    public AudioClip clickSound;

    void Start()
    {
        tilesController = FindObjectOfType<TilesController>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (tilesController.selectedTile != null)
        {
            tilesController.MoveTile(tilesController.selectedTile);
            // PLAY SOUND
        }
    }
}