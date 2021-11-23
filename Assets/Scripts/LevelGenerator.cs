using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public TilesController controller;
    public MovingTile[,] generatedTiles;
    public MovingTile[] possibleTiles;
    public MovingTile emptyTile;

    (int, int) emptyPosition;
    private int seed;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            StartCoroutine(VisualSwap(controller.gridSizeX, controller.gridSizeZ, 50));
        }
    }

    public void Generate(int gridSizeX, int gridSizeZ, int levelSeed)
    {
        seed = levelSeed;
        Random.InitState(seed);
        // count of moves to get stars
        int movesPerfect = 0,
            movesNormal = 0;

        generatedTiles = new MovingTile[gridSizeX, gridSizeZ];
        int shuffleMovesCount = Random.Range(15, 36);
        int emptyTileIndex = Random.Range(0, gridSizeX * gridSizeZ - 2);
        bool hasEmptyTile = false;

        int[] totalCountOfEachType = new int[7];
        for (int i = 0; i < totalCountOfEachType.Length; i++)
            totalCountOfEachType[i] = 0;

        //int successfulConditionsCount = 0;
        int tilesCount = 0;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                MovingTile tile;
                if (tilesCount == emptyTileIndex && hasEmptyTile == false)
                {
                    emptyPosition = (x, z);
                    tile = Instantiate(emptyTile);
                    tile.isEmpty = true;
                    hasEmptyTile = true;
                }
                else
                {
                    tile = Instantiate(possibleTiles[Random.Range(0, possibleTiles.Length)]);
                    totalCountOfEachType[(int)tile.type]++;
                }
                tile.controller = controller;
                tile.transform.position = new Vector3(x, 0, z);
                tile.tileID = tilesCount;
                tile.x = x;
                tile.z = z;
                generatedTiles[x, z] = tile;
                tilesCount++;
            }
        }

        int tilesWithConditionsCount = 0;
        while (tilesWithConditionsCount < 2)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    MovingTile tile;

                    int[] countOfEachTileType = new int[7];
                    for (int i = 0; i < countOfEachTileType.Length; i++)
                        countOfEachTileType[i] = 0;

                    bool rejectCondition = false;
                    tile = generatedTiles[x, z];
                    if (tile.isEmpty == false && tile.conditions.Count == 0)
                    {
                        tile.conditions = new List<TileCondition>();
                        if (tile.z + 1 < gridSizeZ && generatedTiles[tile.x, tile.z + 1].isEmpty == false)
                            countOfEachTileType[(int)generatedTiles[tile.x, tile.z + 1].type]++;
                        if (tile.z - 1 >= 0 && generatedTiles[tile.x, tile.z - 1].isEmpty == false)
                            countOfEachTileType[(int)generatedTiles[tile.x, tile.z - 1].type]++;
                        if (tile.x + 1 < gridSizeX && generatedTiles[tile.x + 1, tile.z].isEmpty == false)
                            countOfEachTileType[(int)generatedTiles[tile.x + 1, tile.z].type]++;
                        if (tile.x - 1 >= 0 && generatedTiles[tile.x - 1, tile.z].isEmpty == false)
                            countOfEachTileType[(int)generatedTiles[tile.x - 1, tile.z].type]++;

                        if (Random.Range(0, 100f) < (300 - tilesWithConditionsCount * 70) / (gridSizeX * gridSizeZ))
                        {
                            for (int i = 2; i < countOfEachTileType.Length; i++)
                            {
                                if (Random.Range(0, 100f) < 75f)
                                {
                                    TileCondition cond = new TileCondition
                                    {
                                        tileType = (TileType)i
                                    };
                                    if (countOfEachTileType[i] > 0)
                                    {
                                        cond.typeCondition = TileCondition.TileTypeCondition.Is;
                                        cond.minMatchesCount = cond.maxMatchesCount = countOfEachTileType[i];
                                    }
                                    else
                                    {
                                        if ((int)tile.type == i)
                                        {
                                            if (totalCountOfEachType[i] > 1)
                                                cond.typeCondition = TileCondition.TileTypeCondition.IsNot;
                                            else rejectCondition = true;
                                        }
                                        else if (totalCountOfEachType[i] > 0)
                                            cond.typeCondition = TileCondition.TileTypeCondition.IsNot;
                                        else rejectCondition = true;
                                    }

                                    if (rejectCondition == false)
                                    {
                                        tile.conditions.Add(cond);
                                        //successfulConditionsCount++;
                                    }
                                }
                            }
                        }

                        if (tile.conditions.Count > 0)
                            tilesWithConditionsCount++;
                    }
                }
            }
        }

        StartCoroutine(VisualSwap(gridSizeX, gridSizeZ, shuffleMovesCount));

        movesPerfect = shuffleMovesCount + 3 * tilesWithConditionsCount;
        controller.movesToCompleteLevelPerfect = movesPerfect;

        movesNormal = shuffleMovesCount + 5 * tilesWithConditionsCount + 7;
        controller.movesToCompleteLevel = movesNormal;

        controller.tiles = generatedTiles;
    }

    public IEnumerator VisualSwap(int gridSizeX, int gridSizeZ, int maxMovesCount)
    {
        Random.InitState(seed);
        int lastMovedTileID = -1;
        for (int i = 0; i < maxMovesCount; i++)
        {
            MovingTile emptyTile = null;
            float randomSeed = Random.Range(0, 100f);
            emptyTile = generatedTiles[emptyPosition.Item1, emptyPosition.Item2];
            Vector3 placeToSwap = new Vector3(emptyTile.x, 0, emptyTile.z);

            if (emptyTile.z + 1 < gridSizeZ && randomSeed < 25f && generatedTiles[emptyTile.x, emptyTile.z + 1].tileID != lastMovedTileID) // upper cell
                placeToSwap = new Vector3(emptyTile.x, 0, emptyTile.z + 1);
            else if (emptyTile.x + 1 < gridSizeX && randomSeed >= 25f && randomSeed < 50f && generatedTiles[emptyTile.x + 1, emptyTile.z].tileID != lastMovedTileID) // right cell
                placeToSwap = new Vector3(emptyTile.x + 1, 0, emptyTile.z);
            else if (emptyTile.z - 1 >= 0 && randomSeed >= 50f && randomSeed < 75f && generatedTiles[emptyTile.x, emptyTile.z - 1].tileID != lastMovedTileID) // lower cell
                placeToSwap = new Vector3(emptyTile.x, 0, emptyTile.z - 1);
            else if (emptyTile.x - 1 >= 0 && generatedTiles[emptyTile.x - 1, emptyTile.z].tileID != lastMovedTileID) // left cell
                placeToSwap = new Vector3(emptyTile.x - 1, 0, emptyTile.z);

            int xMovement = Mathf.Abs(emptyTile.x - (int)placeToSwap.x),
                zMovement = Mathf.Abs(emptyTile.z - (int)placeToSwap.z);

            if ((emptyTile.x == (int)placeToSwap.x && emptyTile.z == (int)placeToSwap.z) || xMovement + zMovement > 1)
            {
                i--;
                continue;
            }

            lastMovedTileID = generatedTiles[(int)placeToSwap.x, (int)placeToSwap.z].tileID;
            //print(emptyPosition + "; " + secondPlace);
            //yield return new WaitForSeconds(0.1f);

            if (emptyTile != null)
            {
                Vector3 emptyTilePlace = new Vector3(emptyTile.x, 0, emptyTile.z);

                MovingTile tileToSwap = generatedTiles[(int)placeToSwap.x, (int)placeToSwap.z];
                emptyPosition = (tileToSwap.x, tileToSwap.z);

                generatedTiles[(int)emptyTilePlace.x, (int)emptyTilePlace.z] = tileToSwap;
                generatedTiles[(int)placeToSwap.x, (int)placeToSwap.z] = emptyTile;

                emptyTile.x = (int)placeToSwap.x;
                emptyTile.z = (int)placeToSwap.z;

                emptyTile.transform.position = new Vector3(placeToSwap.x, 0, placeToSwap.z);
                tileToSwap.transform.position = new Vector3(emptyTilePlace.x, 0, emptyTilePlace.z);
                tileToSwap.x = (int)emptyTilePlace.x;
                tileToSwap.z = (int)emptyTilePlace.z;
                tileToSwap.movementsCount++;
            }
        }
        controller.tiles = generatedTiles;
        yield return new WaitForSeconds(0.1f);
    }
}