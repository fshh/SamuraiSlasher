using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int numPlayers = 4;
    public PlayerColorManager colorManager;
    public float maxX = 75;
    public float maxY = 35;
    public float distBetweenX = 30;
    public float distBetweenY = 70 / 3;
    public float jiggleAmount = 1;

    void OnEnable()
    {
        numPlayers = GameSettings.NumberOfPlayers;
        RandomizeLevel();
        SpawnPlayers();
    }

    private void Update() {
        if (Input.GetButtonDown("Restart")) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void RandomizeLevel()
    {
        List<List<Vector2>> points = new List<List<Vector2>>();
        int count = 0;
        for (float i = maxX * -1; i <= maxX; i += distBetweenX)
        {
            points.Add(new List<Vector2>());
            for (float j = maxY * -1; j <= maxY; j += distBetweenY)
            {
                points[count].Add(new Vector2(i, j));
            }
            count++;
        }

        // All obstacles and spawns for a given level should already exist in it
        List<GameObject> levelObjects = new List<GameObject>(GameObject.FindGameObjectsWithTag("SpawnPoint"));
        levelObjects.AddRange(GameObject.FindGameObjectsWithTag("Obstacle"));

        foreach(GameObject obj in levelObjects)
        {
            int row = Random.Range(0, points.Count);
            int rowCount = 0;
            while (points[row].Count == 0 && rowCount < points.Count) {
                row = (row + 1) % points.Count;
                rowCount++;
            }
            if (rowCount == points.Count)
            {
                Debug.LogError("Ran out of object positions for randomization!");
                return;
            }
            int col = Random.Range(0, points[row].Count);
            obj.transform.position = points[row][col];
            points[row].RemoveAt(col);
            // Jiggles objects to look less like a grid
            obj.transform.position += new Vector3(Random.Range(-jiggleAmount, jiggleAmount), Random.Range(-jiggleAmount, jiggleAmount));
            // Gives the object a random rotation around the z axis
            obj.transform.Rotate(new Vector3(0, 0, Random.Range(0f, 360f)));
        }
    }

    private void SpawnPlayers()
    {
        List<GameObject> spawns = new List<GameObject>(GameObject.FindGameObjectsWithTag("SpawnPoint"));
        GameObject playerPrefab = Resources.Load<GameObject>("Player");
        for (int i = 0; i < numPlayers; i++)
        {
            int spIndex = Random.Range(0, spawns.Count);
            GameObject player = Instantiate(playerPrefab, spawns[spIndex].transform.position, Quaternion.identity);
            player.name = "Player " + (i + 1);
            player.GetComponent<InputManager>().playerNumber = (InputManager.PlayerNumber)(i + 1);
            colorManager.SetColors(player, (InputManager.PlayerNumber)(i + 1));
            spawns.RemoveAt(spIndex);
        }
    }


}
