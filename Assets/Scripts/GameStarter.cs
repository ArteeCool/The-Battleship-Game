using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameStarter : MonoBehaviour
{
    public Boolean IsGameStared;
    public Boolean IsFirstPlayerChoised;
    public Boolean IsSecondPlayerChoised;

    public Boolean isRandomising;

    private Boolean _isRandomised;

    [SerializeField] private GameObject blockLeftSide;
    [SerializeField] private GameObject blockRightSide;
    
    public GameObject[] player1Ships;
    public GameObject[] player2Ships;

    [SerializeField] private GameObject interactionButton;

    [SerializeField] private GameObject randomiseButton1;
    [SerializeField] private GameObject randomiseButton2;
    
    [SerializeField] private GameObject gameObjectParentFieldOne;
    [SerializeField] private GameObject gameObjectParentFieldTwo; 
    
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI messageText;

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
        else if (IsGameStared)
        {
            messageText.text = "Game is started!";
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
                
                Fields.FillFields(interactionButton, gameObjectParentFieldOne);
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

                Fields.FillFields(interactionButton, gameObjectParentFieldTwo);
            }

            if (!IsFirstPlayerChoised || !IsSecondPlayerChoised) return;
            IsGameStared = true;
            
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

    public void RandomShipsPositions(String arrayToUse)
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
        
        foreach (var ship in shipsArray)
        {
            var shipScript = ship.GetComponent<ShipScript>();
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

                attempts++;
            }
        }
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
            OnWinDetected("1");
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
            OnWinDetected("2");
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
}