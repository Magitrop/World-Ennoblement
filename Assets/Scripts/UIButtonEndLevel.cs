using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonEndLevel : MonoBehaviour, IPointerDownHandler
{
    public TilesController tilesController;
    public AudioClip clickSound;

    void Start()
    {
        tilesController = FindObjectOfType<TilesController>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (tilesController.CheckTilesConditions() == true)
        {
            tilesController.completeGoal = true;
            tilesController.stars[0].sprite = tilesController.filledStar;
            tilesController.starsCount++;

            if (tilesController.movesCount <= tilesController.movesToCompleteLevel)
            {
                tilesController.movesNormalGoal = true;
                tilesController.stars[1].sprite = tilesController.filledStar;
                tilesController.starsCount++;
            }

            if (tilesController.movesCount <= tilesController.movesToCompleteLevelPerfect)
            {
                tilesController.movesPerfectGoal = true;
                tilesController.stars[2].sprite = tilesController.filledStar;
                tilesController.starsCount++;
            }

            tilesController.SaveLevel(tilesController.saves[PlayerPrefs.GetInt("levelIndex")]);
        }

        // PLAY SOUND
    }
}