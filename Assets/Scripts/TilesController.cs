using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class StrictTileCondition
{
    public StrictTileConditionState
        upperTile,
        rightTile,
        bottomTile,
        leftTile;
}

[Serializable]
public class StrictTileConditionState
{
    public enum TileTypeCondition
    {
        NoMatter,
        Is,
        IsNot
    }

    public TileTypeCondition typeCondition;
    public TileType tileType;
}

[Serializable]
public class TileCondition
{
    public int minMatchesCount, maxMatchesCount;

    public enum TileTypeCondition
    {
        Is,
        IsNot
    }

    public TileTypeCondition typeCondition;
    public TileType tileType;
}

[Serializable]
public class TileTypeHandler
{
    public TileType tileType;
    public GameObject[] tileVisuals;
    public Sprite typeSprite;
}

public class TilesController : MonoBehaviour
{
    public int gridSizeX, gridSizeZ;
    public MovingTile[,] tiles;
    public GameObject tilesContainer;

    public MovingTile tileExemplar;
    public MovingTile emptyTileExemplar;
    public GameObject tileWithConditionsMarker;
    public GameObject mapWall;

    public bool isAnyTileMoving;
    public MovingTile selectedTile;
    public float changeLevelSpeed;

    public GameObject tileDescriptionPanel;
    public UICondition strictConditionUpper, strictConditionBottom, strictConditionLeft, strictConditionRight;
    public UICondition simpleConditionExemplar;
    public GameObject simpleConditionsContainer;

    public TileTypeHandler[] tileTypes;

    public Camera cam;
    public string[] saves;

    public int movesToCompleteLevel, movesToCompleteLevelPerfect;
    public int starsCount = 0;

    public Text textLevelNumber, textMovesCount;
    public Button buttonMoveTile, buttonEndLevel;

    public bool completeGoal, movesPerfectGoal, movesNormalGoal;
    public Image[] stars;
    public Sprite emptyStar, filledStar;

    public int movesCount;

    public LevelGenerator generator;

    [SerializeField]
    private int generatedLevelID;

    private void Start()
    {
        if (PlayerPrefs.GetString("cur_level") != string.Empty)
            generatedLevelID = int.Parse(PlayerPrefs.GetString("cur_level"));
        else generatedLevelID = 0;
        UnityEngine.Random.InitState(generatedLevelID);
        gridSizeX = UnityEngine.Random.Range(3, 5);
        gridSizeZ = UnityEngine.Random.Range(3, 5);
        generator.Generate(gridSizeX, gridSizeZ, generatedLevelID); //long.Parse(PlayerPrefs.GetString("cur_level")));
        //LoadLevel(saves[PlayerPrefs.GetInt("levelIndex")]);
        
        starsCount = 0;
        for (int i = 0; i < PlayerPrefs.GetInt(generatedLevelID + "_stars"); i++)
            stars[i].sprite = filledStar;

        /*
        stars[0].sprite = completeGoal ? filledStar : emptyStar;
        stars[1].sprite = movesNormalGoal ? filledStar : emptyStar;
        stars[2].sprite = movesPerfectGoal ? filledStar : emptyStar;
        */

        selectedTile = null;
        textLevelNumber.text = "Level " + (generatedLevelID + 1).ToString();

        buttonMoveTile.interactable = false;
        movesCount = 0;
        textMovesCount.text = movesCount.ToString() + " / " + movesToCompleteLevel + " / " + movesToCompleteLevelPerfect;

        int biggerSize = gridSizeX > gridSizeZ ? gridSizeX : gridSizeZ;
        cam.transform.position = new Vector3(0, biggerSize / 2 + (biggerSize * 0.185f));
        cam.orthographicSize = biggerSize * 0.5f + 1f;
        tileDescriptionPanel.SetActive(false);

        /*
        for (int x = -1; x < gridSizeX + 1; x++)
        {
            GameObject wall = Instantiate(mapWall);
            wall.transform.position = new Vector3(x, 0, -1);
            wall.transform.rotation = Quaternion.Euler(-90, 0, 0);
            wall = Instantiate(mapWall);
            wall.transform.position = new Vector3(x, 0, gridSizeZ);
            wall.transform.rotation = Quaternion.Euler(-90, 0, 0);
        }

        for (int z = 0; z < gridSizeZ; z++)
        {
            GameObject wall = Instantiate(mapWall);
            wall.transform.position = new Vector3(-1, 0, z);
            wall.transform.rotation = Quaternion.Euler(-90, 0, 0);
            wall = Instantiate(mapWall);
            wall.transform.position = new Vector3(gridSizeX, 0, z);
            wall.transform.rotation = Quaternion.Euler(-90, 0, 0);
        }
        */

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                tiles[x, z].transform.SetParent(tilesContainer.transform);
                if (tiles[x, z].strictConditions.Count > 0 || tiles[x, z].conditions.Count > 0)
                {
                    GameObject marker = Instantiate(tileWithConditionsMarker, tiles[x, z].transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity);
                    marker.transform.SetParent(tiles[x, z].transform);
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        int previousStarsCount = PlayerPrefs.GetInt(generatedLevelID + "_stars");
        PlayerPrefs.SetInt(generatedLevelID + "_stars", starsCount < previousStarsCount ? previousStarsCount : starsCount);
        PlayerPrefs.SetString("cur_level", generatedLevelID.ToString());
        PlayerPrefs.Save();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            int previousStarsCount = PlayerPrefs.GetInt(generatedLevelID + "_stars");
            PlayerPrefs.SetInt(generatedLevelID + "_stars", starsCount < previousStarsCount ? previousStarsCount : starsCount);
            generatedLevelID++;
            PlayerPrefs.SetString("cur_level", generatedLevelID.ToString());
            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && generatedLevelID > 0)
        {
            int previousStarsCount = PlayerPrefs.GetInt(generatedLevelID + "_stars");
            PlayerPrefs.SetInt(generatedLevelID + "_stars", starsCount < previousStarsCount ? previousStarsCount : starsCount);
            generatedLevelID--;
            PlayerPrefs.SetString("cur_level", generatedLevelID.ToString());
            PlayerPrefs.Save();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.collider.GetComponent<MovingTile>() != null)
                {
                    if (selectedTile == null || hit.collider.GetComponent<MovingTile>().tileID != selectedTile.tileID)
                    {
                        if (selectedTile != null)
                            selectedTile.outline.render = false;
                        tileDescriptionPanel.SetActive(false);
                        buttonMoveTile.interactable = false;

                        selectedTile = hit.collider.GetComponent<MovingTile>();
                        selectedTile.outline.render = true;
                        buttonMoveTile.interactable = true;

                        if (selectedTile.conditions.Count > 0)
                            tileDescriptionPanel.SetActive(true);

                        foreach (Transform child in simpleConditionsContainer.transform)
                            Destroy(child.gameObject);

                        for (int i = 0; i < selectedTile.conditions.Count; i++)
                        {
                            UICondition cond = Instantiate(simpleConditionExemplar);
                            cond.transform.SetParent(simpleConditionsContainer.transform);
                            cond.image.sprite = tileTypes.ToList().Find(t => t.tileType == selectedTile.conditions[i].tileType).typeSprite;
                            if (selectedTile.conditions[i].typeCondition == TileCondition.TileTypeCondition.Is)
                            {
                                int minMatchesCount = selectedTile.conditions[i].minMatchesCount;
                                int maxMatchesCount = selectedTile.conditions[i].maxMatchesCount;

                                if (minMatchesCount != maxMatchesCount)
                                    cond.count.text = minMatchesCount + "-" + maxMatchesCount;
                                else cond.count.text = minMatchesCount.ToString();

                                cond.isNot.SetActive(false);
                            }
                            else cond.isNot.SetActive(true);
                        }
                    }
                    else MoveTile(selectedTile);
                }
            }
            else
            {
                buttonMoveTile.interactable = false;
                if (selectedTile != null)
                    selectedTile.outline.render = false;
                selectedTile = null;
                tileDescriptionPanel.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (selectedTile != null)
            {
                MoveTile(selectedTile);
                //selectedTile = null;
            }
        }

        // save level
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveLevel(saves[PlayerPrefs.GetInt("levelIndex")]);
        }

        // save and reload level
        if (Input.GetKeyDown(KeyCode.O))
        {
            SaveLevel(saves[PlayerPrefs.GetInt("levelIndex")]);
            Start();
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // refresh level
        if (Input.GetKeyDown(KeyCode.F5))
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    GameObject[] visuals = tileTypes.ToList().Find(c => c.tileType == tiles[x, z].type).tileVisuals;
                    tiles[x, z].tileVisual = visuals[UnityEngine.Random.Range(0, visuals.Length)];
                    tiles[x, z].Start();
                }
            }
        }

        // reset stars and save level
        if (Input.GetKeyDown(KeyCode.T))
        {
            completeGoal = movesNormalGoal = movesPerfectGoal = false;
            SaveLevel(saves[PlayerPrefs.GetInt("levelIndex")]);
            Start();
        }

        /*
        // previous level
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (PlayerPrefs.GetInt("levelIndex") < saves.Length - 1)
            {
                PlayerPrefs.SetInt("levelIndex", PlayerPrefs.GetInt("levelIndex") + 1);
                Start();
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        // next level
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (PlayerPrefs.GetInt("levelIndex") > 0)
            {
                PlayerPrefs.SetInt("levelIndex", PlayerPrefs.GetInt("levelIndex") - 1);
                Start();
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        */
    }
    
    public bool CheckTilesConditions()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                if (CheckTileCondition(tiles[x, z]) == false)
                    return false;
            }
        }

        return true;
    }
    public bool CheckTileCondition(MovingTile tile)
    {
        for (int i = 0; i < tile.strictConditions.Count; i++)
        {
            if (tile.z + 1 < gridSizeZ && tiles[tile.x, tile.z + 1].isEmpty == false)
            {
                switch (tile.strictConditions[i].upperTile.typeCondition)
                {
                    case StrictTileConditionState.TileTypeCondition.Is:
                        {
                            if (tiles[tile.x, tile.z + 1].type != tile.strictConditions[i].upperTile.tileType)
                                return false;
                            break;
                        }

                    case StrictTileConditionState.TileTypeCondition.IsNot:
                        {
                            if (tiles[tile.x, tile.z + 1].type == tile.strictConditions[i].upperTile.tileType)
                                return false;
                            break;
                        }
                }
            }

            if (tile.z - 1 > 0 && tiles[tile.x, tile.z - 1].isEmpty == false)
            {
                switch (tile.strictConditions[i].bottomTile.typeCondition)
                {
                    case StrictTileConditionState.TileTypeCondition.Is:
                        {
                            if (tiles[tile.x, tile.z - 1].type != tile.strictConditions[i].bottomTile.tileType)
                                return false;
                            break;
                        }

                    case StrictTileConditionState.TileTypeCondition.IsNot:
                        {
                            if (tiles[tile.x, tile.z - 1].type == tile.strictConditions[i].bottomTile.tileType)
                                return false;
                            break;
                        }
                }
            }

            if (tile.x + 1 < gridSizeX && tiles[tile.x + 1, tile.z].isEmpty == false)
            {
                switch (tile.strictConditions[i].rightTile.typeCondition)
                {
                    case StrictTileConditionState.TileTypeCondition.Is:
                        {
                            if (tiles[tile.x + 1, tile.z].type != tile.strictConditions[i].rightTile.tileType)
                                return false;
                            break;
                        }

                    case StrictTileConditionState.TileTypeCondition.IsNot:
                        {
                            if (tiles[tile.x + 1, tile.z].type == tile.strictConditions[i].rightTile.tileType)
                                return false;
                            break;
                        }
                }
            }

            if (tile.x - 1 > 0 && tiles[tile.x - 1, tile.z].isEmpty == false)
            {
                switch (tile.strictConditions[i].leftTile.typeCondition)
                {
                    case StrictTileConditionState.TileTypeCondition.Is:
                        {
                            if (tiles[tile.x - 1, tile.z].type != tile.strictConditions[i].leftTile.tileType)
                                return false;
                            break;
                        }

                    case StrictTileConditionState.TileTypeCondition.IsNot:
                        {
                            if (tiles[tile.x - 1, tile.z].type == tile.strictConditions[i].leftTile.tileType)
                                return false;
                            break;
                        }
                }
            }
        }
        for (int i = 0; i < tile.conditions.Count; i++)
        {
            if ((tile.z + 1 < gridSizeZ && tiles[tile.x, tile.z + 1].isEmpty == false) ||
                (tile.z - 1 > 0 && tiles[tile.x, tile.z - 1].isEmpty == false) ||
                (tile.x + 1 < gridSizeX && tiles[tile.x + 1, tile.z].isEmpty == false) ||
                (tile.x - 1 > 0 && tiles[tile.x - 1, tile.z].isEmpty == false))
            {
                int matchesCount = 0;
                switch (tile.conditions[i].typeCondition)
                {
                    case TileCondition.TileTypeCondition.Is:
                        {
                            if (tile.z + 1 < gridSizeZ && tiles[tile.x, tile.z + 1].type == tile.conditions[i].tileType)
                                matchesCount++;
                            if (tile.z - 1 >= 0 && tiles[tile.x, tile.z - 1].type == tile.conditions[i].tileType)
                                matchesCount++;
                            if (tile.x + 1 < gridSizeX && tiles[tile.x + 1, tile.z].type == tile.conditions[i].tileType)
                                matchesCount++;
                            if (tile.x - 1 >= 0 && tiles[tile.x - 1, tile.z].type == tile.conditions[i].tileType)
                                matchesCount++;

                            if (tile.z + 1 < gridSizeZ && tiles[tile.x, tile.z + 1].type != tile.conditions[i].tileType &&
                                tile.z - 1 >= 0 && tiles[tile.x, tile.z - 1].type != tile.conditions[i].tileType &&
                                tile.x + 1 < gridSizeX && tiles[tile.x + 1, tile.z].type != tile.conditions[i].tileType &&
                                tile.x - 1 >= 0 && tiles[tile.x - 1, tile.z].type != tile.conditions[i].tileType)
                                return false;

                            break;
                        }

                    case TileCondition.TileTypeCondition.IsNot:
                        {
                            if (tile.z + 1 < gridSizeZ && tiles[tile.x, tile.z + 1].type == tile.conditions[i].tileType ||
                                tile.z - 1 >= 0 && tiles[tile.x, tile.z - 1].type == tile.conditions[i].tileType ||
                                tile.x + 1 < gridSizeX && tiles[tile.x + 1, tile.z].type == tile.conditions[i].tileType ||
                                tile.x - 1 >= 0 && tiles[tile.x - 1, tile.z].type == tile.conditions[i].tileType)
                                return false;

                            break;
                        }
                }

                if (tile.conditions[i].typeCondition == TileCondition.TileTypeCondition.Is &&
                    (matchesCount > tile.conditions[i].maxMatchesCount || matchesCount < tile.conditions[i].minMatchesCount))
                    return false;
            }
        }

        return true;
    }

    public void MoveTile(MovingTile tile)
    {
        Vector3 firstPlace = new Vector3(tile.x, 0, tile.z);
        Vector3 secondPlace;

        if (tile.z + 1 < gridSizeZ && tiles[tile.x, tile.z + 1].isEmpty == true)
            secondPlace = new Vector3(tiles[tile.x, tile.z + 1].x, 0, tiles[tile.x, tile.z + 1].z);
        else if (tile.z - 1 >= 0 && tiles[tile.x, tile.z - 1].isEmpty == true)
            secondPlace = new Vector3(tiles[tile.x, tile.z - 1].x, 0, tiles[tile.x, tile.z - 1].z);
        else if (tile.x + 1 < gridSizeX && tiles[tile.x + 1, tile.z].isEmpty == true)
            secondPlace = new Vector3(tiles[tile.x + 1, tile.z].x, 0, tiles[tile.x + 1, tile.z].z);
        else if (tile.x - 1 >= 0 && tiles[tile.x - 1, tile.z].isEmpty == true)
            secondPlace = new Vector3(tiles[tile.x - 1, tile.z].x, 0, tiles[tile.x - 1, tile.z].z);
        else return;

        MovingTile firstTile = tiles[(int)firstPlace.x, (int)firstPlace.z],
                   secondTile = tiles[(int)secondPlace.x, (int)secondPlace.z];

        tiles[(int)firstPlace.x, (int)firstPlace.z] = secondTile;
        tiles[(int)secondPlace.x, (int)secondPlace.z] = firstTile;

        firstTile.x = (int)secondPlace.x;
        firstTile.z = (int)secondPlace.z;

        secondTile.transform.position = new Vector3(firstPlace.x, 0, firstPlace.y);
        secondTile.x = (int)firstPlace.x;
        secondTile.z = (int)firstPlace.z;

        movesCount++;
        textMovesCount.text = movesCount.ToString() + " / " + movesToCompleteLevel + " / " + movesToCompleteLevelPerfect;
        StartCoroutine(MoveTileVisual(tile, secondPlace));
    }
    public IEnumerator MoveTileVisual(MovingTile tile, Vector3 posToMove)
    {
        isAnyTileMoving = true;
        while (Vector3.Distance(tile.transform.position, posToMove) > 0.25f)
        {
            tile.transform.position = Vector3.MoveTowards(tile.transform.position, posToMove, 10f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }

        tile.transform.position = posToMove;
        isAnyTileMoving = false;
    }

    public void SaveLevel(string saveName)
    {
        if (Directory.Exists(Application.dataPath + "/Saves/") == false)
            Directory.CreateDirectory(Application.dataPath + "/Saves/");

        saveName.Replace(" ", string.Empty);
        XmlNode userNode1;
        XmlElement element;

        XmlDocument xmlDoc = new XmlDocument();
        XmlNode rootNode = xmlDoc.CreateElement("save_" + saveName);
        xmlDoc.AppendChild(rootNode);

        userNode1 = xmlDoc.CreateElement("Tiles");

        element = xmlDoc.CreateElement("size");
        element.SetAttribute("value", gridSizeX + ";" + gridSizeZ);
        userNode1.AppendChild(element);

        element = xmlDoc.CreateElement("winConditions");
        element.SetAttribute("value", 
            movesToCompleteLevel + 
            ";" + movesToCompleteLevelPerfect + 
            ";" + completeGoal.ToString() + 
            ";" + movesNormalGoal.ToString() + 
            ";" + movesPerfectGoal.ToString());
        userNode1.AppendChild(element);

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                string tileState =
                        tiles[x, z].x + ";" +
                        tiles[x, z].z + ";" +
                        tiles[x, z].isEmpty.ToString() + ";" +
                  ((int)tiles[x, z].type).ToString() + ";" +
                        tiles[x, z].strictConditions.Count + ";" +
                        tiles[x, z].conditions.Count + ";";

                element = xmlDoc.CreateElement("tileState");
                element.SetAttribute("value", tileState);
                userNode1.AppendChild(element);

                for (int i = 0; i < tiles[x, z].strictConditions.Count; i++)
                {
                    tileState = ((int)tiles[x, z].strictConditions[i].upperTile.typeCondition).ToString() + ";"
                               + ((int)tiles[x, z].strictConditions[i].upperTile.tileType).ToString() + ";";

                    tileState += ((int)tiles[x, z].strictConditions[i].rightTile.typeCondition).ToString() + ";"
                               + ((int)tiles[x, z].strictConditions[i].rightTile.tileType).ToString() + ";";

                    tileState += ((int)tiles[x, z].strictConditions[i].bottomTile.typeCondition).ToString() + ";"
                               + ((int)tiles[x, z].strictConditions[i].bottomTile.tileType).ToString() + ";";

                    tileState += ((int)tiles[x, z].strictConditions[i].leftTile.typeCondition).ToString() + ";"
                               + ((int)tiles[x, z].strictConditions[i].leftTile.tileType).ToString();

                    element = xmlDoc.CreateElement("strictConditions" + i);
                    element.SetAttribute("value", tileState);
                    userNode1.AppendChild(element);
                }

                for (int i = 0; i < tiles[x, z].conditions.Count; i++)
                {
                    tileState = ((int)tiles[x, z].conditions[i].typeCondition).ToString() + ";"
                               + ((int)tiles[x, z].conditions[i].tileType).ToString() + ";"
                               + tiles[x, z].conditions[i].minMatchesCount.ToString() + ";"
                               + tiles[x, z].conditions[i].maxMatchesCount.ToString();

                    element = xmlDoc.CreateElement("conditions" + i);
                    element.SetAttribute("value", tileState);
                    userNode1.AppendChild(element);
                }
            }
        }

        rootNode.AppendChild(userNode1);
        xmlDoc.Save(Application.dataPath + "/Saves/" + "save_" + saveName + ".xml");
    }
    public void LoadLevel(string saveName)
    {
        if (Directory.Exists(Application.dataPath + "/Saves/") == false)
            Directory.CreateDirectory(Application.dataPath + "/Saves/");

        if (File.Exists(Application.dataPath + "/Saves/" + "save_" + saveName + ".xml"))
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Application.dataPath + "/Saves/" + "save_" + saveName + ".xml");
            XmlElement xRoot = xDoc.DocumentElement;

            int tileConditionsCount = 0, tileStrictConditionsCount = 0;
            int tileNumber = 0;
            int x = 0, z = 0;
            string[] state = new string[0];

            for (int _x = 0; _x < gridSizeX; _x++)
            {
                for (int _z = 0; _z < gridSizeZ; _z++)
                {
                    if (tiles != null && tiles[_x, _z] != null)
                    {
                        Destroy(tiles[_x, _z].gameObject);
                        tiles[_x, _z] = null;
                    }
                }
            }

            foreach (XmlNode xnode in xRoot)
            {
                foreach (XmlNode childnode in xnode.ChildNodes)
                {
                    if (childnode.Name == "size")
                    {
                        state = childnode.Attributes.GetNamedItem("value").Value.Split(';');
                        gridSizeX = int.Parse(state[0]);
                        gridSizeZ = int.Parse(state[1]);
                        tiles = new MovingTile[gridSizeX, gridSizeZ];
                    }
                    if (childnode.Name == "winConditions")
                    {
                        state = childnode.Attributes.GetNamedItem("value").Value.Split(';');
                        movesToCompleteLevel = int.Parse(state[0]);
                        movesToCompleteLevelPerfect = int.Parse(state[1]);
                        completeGoal = bool.Parse(state[2]);
                        movesNormalGoal = bool.Parse(state[3]);
                        movesPerfectGoal = bool.Parse(state[4]);
                    }
                    if (childnode.Name == "tileState")
                    {
                        state = childnode.Attributes.GetNamedItem("value").Value.Split(';');
                        x = int.Parse(state[0]);
                        z = int.Parse(state[1]);

                        if (bool.Parse(state[2]) == true)
                            tiles[x, z] = Instantiate(emptyTileExemplar, new Vector3(x, 0, z), Quaternion.identity);
                        else
                            tiles[x, z] = Instantiate(tileExemplar, new Vector3(x, 0, z), Quaternion.identity);

                        tiles[x, z].name = "Tile " + tileNumber.ToString();
                        tiles[x, z].x = x;
                        tiles[x, z].z = z;
                        tiles[x, z].isEmpty = bool.Parse(state[2]);
                        tiles[x, z].controller = this;

                        tiles[x, z].type = (TileType)int.Parse(state[3]);
                        GameObject[] visuals = tileTypes.ToList().Find(t => t.tileType == tiles[x, z].type).tileVisuals;
                        tiles[x, z].tileVisual = visuals[UnityEngine.Random.Range(0, visuals.Length)];
                        /*
                        if (tiles[x, z].type != TileType.None)
                        {
                            tiles[x, z].Start();
                        }
                        */
                        tileStrictConditionsCount = int.Parse(state[4]);
                        tileConditionsCount = int.Parse(state[5]);

                        tiles[x, z].strictConditions = new List<StrictTileCondition>();//[tileStrictConditionsCount];
                        for (int i = 0; i < tileStrictConditionsCount; i++) //tiles[x, z].strictConditions.Count; i++)
                        {
                            StrictTileCondition cond = new StrictTileCondition();
                            cond.upperTile = new StrictTileConditionState();
                            cond.bottomTile = new StrictTileConditionState();
                            cond.rightTile = new StrictTileConditionState();
                            cond.leftTile = new StrictTileConditionState();
                            tiles[x, z].strictConditions.Add(cond);
                            /*
                            tiles[x, z].strictConditions[i] = new StrictTileCondition();
                            tiles[x, z].strictConditions[i].upperTile = new StrictTileConditionState();
                            tiles[x, z].strictConditions[i].bottomTile = new StrictTileConditionState();
                            tiles[x, z].strictConditions[i].rightTile = new StrictTileConditionState();
                            tiles[x, z].strictConditions[i].leftTile = new StrictTileConditionState();
                            */
                        }

                        tiles[x, z].conditions = new List<TileCondition>();//[tileConditionsCount];
                        for (int i = 0; i < tileConditionsCount; i++) //tiles[x, z].conditions.Count; i++)
                        {
                            tiles[x, z].conditions.Add(new TileCondition());
                            //tiles[x, z].conditions[i] = new TileCondition();
                        }
                        tileNumber++;
                    }

                    for (int j = 0; j < tileStrictConditionsCount; j++)
                    {
                        if (childnode.Name == "strictConditions" + j)
                        {
                            state = childnode.Attributes.GetNamedItem("value").Value.Split(';');

                            tiles[x, z].strictConditions[j].upperTile.typeCondition = (StrictTileConditionState.TileTypeCondition)int.Parse(state[0]);
                            tiles[x, z].strictConditions[j].upperTile.tileType = (TileType)int.Parse(state[1]);

                            tiles[x, z].strictConditions[j].rightTile.typeCondition = (StrictTileConditionState.TileTypeCondition)int.Parse(state[2]);
                            tiles[x, z].strictConditions[j].rightTile.tileType = (TileType)int.Parse(state[3]);

                            tiles[x, z].strictConditions[j].bottomTile.typeCondition = (StrictTileConditionState.TileTypeCondition)int.Parse(state[4]);
                            tiles[x, z].strictConditions[j].bottomTile.tileType = (TileType)int.Parse(state[5]);

                            tiles[x, z].strictConditions[j].leftTile.typeCondition = (StrictTileConditionState.TileTypeCondition)int.Parse(state[6]);
                            tiles[x, z].strictConditions[j].leftTile.tileType = (TileType)int.Parse(state[7]);
                        }
                    }

                    for (int j = 0; j < tileConditionsCount; j++)
                    {
                        if (childnode.Name == "conditions" + j)
                        {
                            state = childnode.Attributes.GetNamedItem("value").Value.Split(';');
                            tiles[x, z].conditions[j].typeCondition = (TileCondition.TileTypeCondition)int.Parse(state[0]);
                            tiles[x, z].conditions[j].tileType = (TileType)int.Parse(state[1]);
                            tiles[x, z].conditions[j].minMatchesCount = int.Parse(state[2]);
                            tiles[x, z].conditions[j].maxMatchesCount = int.Parse(state[3]);
                        }
                    }
                }
            }
        }
        else
        {
            tiles = new MovingTile[gridSizeX, gridSizeZ];
            int tileNumber = 0;
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int z = 0; z < gridSizeZ; z++, tileNumber++)
                {
                    MovingTile t;
                    if (x == gridSizeX - 1 && z == gridSizeZ - 1)
                    {
                        t = Instantiate(emptyTileExemplar, new Vector3(x, 0, z), Quaternion.identity);
                        t.type = TileType.None;
                    }
                    else
                    {
                        t = Instantiate(tileExemplar, new Vector3(x, 0, z), Quaternion.identity);
                        t.type = TileType.Field;
                    }

                    t.name = "Tile " + tileNumber.ToString();
                    t.x = x;
                    t.z = z;
                    tiles[x, z] = t;
                    GameObject[] visuals = tileTypes.ToList().Find(c => c.tileType == tiles[x, z].type).tileVisuals;
                    tiles[x, z].tileVisual = visuals[UnityEngine.Random.Range(0, visuals.Length)];

                    tiles[x, z].Start();
                    //tiles[x, z].outline = tiles[x, z].visualObject.GetComponent<cakeslice.Outline>();
                    //tiles[x, z].outline.render = false;
                }
            }
        }
    }
}