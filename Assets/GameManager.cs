using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

// game state Script
public class GameManager : MonoBehaviour 
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public int score = 0;
    public int gameTime = 60;
    public GameBoard gameBoard;

    [SerializeField]
    private TextMeshProUGUI restartText;

    private bool isGameOver = false;

    private void Update()
    {
        // check if game is over ( no more moves )
        if (isGameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))  // listen for r key
            {
                RestartGame();
            }
        }
    }

    private void Start()
    {
        
        StartCoroutine(StartGameTimer()); // start game timer
        UpdateScore();                    // update score
    }

    // BubbleClicked - called when one of the bubble is clicked
    public void BubbleClicked(GameObject bubble)
    {
        if (isGameOver) return; // return if game is over
        // check if bubble can be clicked
        if (gameBoard.CanBubbleBeClicked(bubble))
        {
            
            List<GameObject> matchingBubbles = gameBoard.GetMatchingBubbles(bubble);  // get a list of bubbles that matches the bubble that we've clicked
            int comboBonus = GetComboBonus(matchingBubbles.Count);                    // calculate combo based on the number of matched bubbles 
            score += matchingBubbles.Count + comboBonus;                              // add score - number of matching bubbles along with the combo bonus 
            Debug.Log("Base points: " + matchingBubbles.Count);                       // debug logs
            Debug.Log("Combo bonus: " + comboBonus);                                  // debug logs

            foreach (GameObject matchingBubble in matchingBubbles)                    // loop to destroy each matching bubble
            {
                gameBoard.RemoveBubbleFromGrid(matchingBubble);                       // remove from grid
                Destroy(matchingBubble);                                              // destroy gameobject
            }
            UpdateScore();                                                            // update score after clicking on a bubble
            gameBoard.DropBubbles();                                                  // drop any bubbles to fill the 'gaps'
            gameBoard.CollapseColumns();                                              // get rid of any empty collumns that might be present after popping bubbles
        }

        if (!gameBoard.AreMatchingPairsLeft())                                        // check for remaining moves - are there any pairs left?
        {
            Debug.Log("Game Over - No more matching pairs left");                     // if not - debug log
            restartText.gameObject.SetActive(true);
            isGameOver = true;
        }
    }

    //update score text
    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }

    // coroutine to count down the  game time
    IEnumerator StartGameTimer()
    {
        // each loop iteration waits for (1) seconds and decreases the time 
        while (gameTime > 0)
        {
            yield return new WaitForSeconds(1);
            gameTime--;
            timeText.text = "Time: " + gameTime;// update time text
        }
    }

    int GetComboBonus(int count)                                                    
   {
        if (count <= 6) return 0;
        else if (count <= 10) return count * 2;
        else if (count <= 18) return count * 3;
        else return count * 4;
    }

    private void RestartGame()
    {
        isGameOver = false;
        score = 0;
        gameTime = 60;
        gameBoard.RestartBoard();  // call the RestartBoard method of the GameBoard

        // hide restart text and update the UI
        restartText.gameObject.SetActive(false);
        UpdateScore();
        StartCoroutine(StartGameTimer());
    }
}
