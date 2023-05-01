using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Launcher : MonoBehaviour
{
    [SerializeField] public Boolean IsGameStared;
    [SerializeField] public Boolean IsFirstPlayerChoised;
    [SerializeField] public Boolean IsSecondPlayerChoised;

    [SerializeField] public Boolean isRandomising;

    [SerializeField] public Boolean isPVPC;

    [SerializeField] private GameObject blockLeftSide;
    [SerializeField] private GameObject blockRightSide;
    
    [SerializeField] public GameObject[] player1Ships;
    [SerializeField] public GameObject[] player2Ships;

    [SerializeField] private GameObject interactionButton;

    [SerializeField] private GameObject randomiseButton1;
    [SerializeField] private GameObject randomiseButton2;
    
    [SerializeField] public List<GameObject> buttonsShipsOne = new List<GameObject>();
    [SerializeField] public List<GameObject> buttonsShipsTwo = new List<GameObject>();      
    
    [SerializeField] public  GameObject gameObjectParentFieldOne;
    [SerializeField] public GameObject gameObjectParentFieldTwo; 
    
    [SerializeField] public TextMeshProUGUI statusText;
    [SerializeField] public TextMeshProUGUI messageText;

    private Int32 _shipsAtStart;
    
    
    [Header("WinObjects")]
    
    [SerializeField] private GameObject winMenu;
    [SerializeField] private TextMeshProUGUI congratsText;
    

    private void Update()
    {
        if (!IsFirstPlayerChoised)
        {
            statusText.text = "Now it's player - 1";
        }       
        else if (!IsSecondPlayerChoised)
        {
            statusText.text = "Now it's player - 2";
        }

        if (IsFirstPlayerChoised)
        {
            Blocker(false);
                
            if (IsSecondPlayerChoised)
            {
                Blocker(true);
                
                IsFirstPlayerChoised = false;
                IsSecondPlayerChoised = false;
            }
        }
        
        WinDetect();
    }

    public void StartGame()
    {
        if (!IsGameStared)
        {
            Blocker(true);
            
            if (!IsFirstPlayerChoised)
            {
                if (!IsShipsOnPositions(player1Ships))
                {
                    messageText.text = "Not all ships placed!";
                    return;
                }

                if (OnShipsHasColision(player1Ships))    
                {
                    messageText.text = "Invalid position!";
                    return;
                }

                IsFirstPlayerChoised = true;

                Blocker(false);

                randomiseButton1.SetActive(false);
                
                foreach (var ship in player1Ships)
                    ship.GetComponent<ShipScript>().AddPartsPositions();

                foreach (var ship in player1Ships)
                    ship.transform.GetChild(0).gameObject.SetActive(false);
                
                Fields.FillFields(interactionButton, gameObjectParentFieldOne, "One");

                if (isPVPC)
                {
                    gameObject.GetComponent<BotController>().enabled = true;    
                }
            }
            else if (!IsSecondPlayerChoised)
            {
                if (!IsShipsOnPositions(player2Ships))
                {
                    messageText.text = "Not all ships placed!";
                    return;
                }

                if (OnShipsHasColision(player2Ships))
                {
                    messageText.text = "Invalid position!";
                    return;
                }

                IsSecondPlayerChoised = true;
                
                Blocker(true);
                
                randomiseButton2.SetActive(false);
                
                blockRightSide.SetActive(true);
                blockLeftSide.SetActive(false);

                foreach (var ship in player2Ships)
                    ship.GetComponent<ShipScript>().AddPartsPositions();

                foreach (var ship in player2Ships)
                    ship.transform.GetChild(0).gameObject.SetActive(false);

                Fields.FillFields(interactionButton, gameObjectParentFieldTwo, "Two");
            }

            if (!IsFirstPlayerChoised || !IsSecondPlayerChoised) return;
            IsGameStared = true;    
            
            messageText.text = "Game is started!";
            
            IsFirstPlayerChoised = false;
            IsSecondPlayerChoised = false;
        }
    }
    
    private void Blocker(Boolean switcher)
    {
        if (switcher)
        {
            blockLeftSide.SetActive(false);
            blockRightSide.SetActive(true);
        }
        else
        {
            blockLeftSide.SetActive(true);
            blockRightSide.SetActive(false);
        }   
    }

    public void Randomise(String array)
    {
        RandomShipsPositions(array);
    }

    public bool RandomShipsPositions(String arrayToUse)
    {
        GameObject[] shipsArray = new GameObject[] {};
        if (arrayToUse == "player1Ships")
        {
            shipsArray = player1Ships;      
        }
        else if (arrayToUse == "player2Ships")
        {
            shipsArray = player2Ships;
        }

        for (int i = 0; i < shipsArray.Length; i++)
        {
            var shipScript = shipsArray[i].GetComponent<ShipScript>();
            var step = 50;
            var attempts = 0;   
            
            bool spawned = false;   
            while (!spawned && attempts < 10000)
            {       
                int x = Random.Range(-5, 6 - shipScript.partCount);
                int y = Random.Range(-5, 6 - shipScript.partCount);             
                
                bool isVertical = Random.Range(0, 2) == 0;

                shipScript.rotated = isVertical;
                shipScript.internalPosition = new Vector2(x * step, y * step);

                if (!shipScript.CheckForCollisions(shipsArray)) 
                {       
                    spawned = true;    
                }
                else
                {
                    attempts++;    
                }
            }

            if (attempts >= 10000)
            {
                i = -1;
            }
        }
        return false;
    }   
    private Boolean OnShipsHasColision(GameObject[] ships)
    {
        foreach (var ship in ships)
        {
            if (ship.GetComponent<ShipScript>().isHitting) return true;
        }

        return false;
    }
    
    private bool IsShipsOnPositions(GameObject[] shipsArray)
    {
        _shipsAtStart = 0;
        foreach (var ship in shipsArray)
        {
            var shipScript = ship.GetComponent<ShipScript>();
            
            if (shipScript.internalPosition == shipScript.startPosition)
                _shipsAtStart++;
        }

        if (_shipsAtStart == 0) return true;

        return false;
    }

    private void WinDetect()
    {
        int deadShips = 0;
        foreach (var ship in player1Ships)
        {
            if (ship.GetComponent<ShipScript>().isDead)
            {
                deadShips++;
            }             
        }

        if (deadShips == player1Ships.Length)   
        {
            OnWinDetected("2");
        }

        deadShips = 0;
        
        foreach (var ship in player2Ships)
        {
            if (ship.GetComponent<ShipScript>().isDead)
            {
                deadShips++;
            }             
        }

        if (deadShips == player2Ships.Length)
        {
            OnWinDetected("1");
        }
    }

    private void OnWinDetected(String whoWon)
    {
        winMenu.SetActive(true);
        congratsText.text = $"Congratulations! \n Player {whoWon} wins! \n Click to restart!";
    }   

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    private void Quit()
    {
        Application.Quit();
    }
}