using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BotController : MonoBehaviour
{
    
    private Boolean wasUsed;
    private Camera _camera;
    private GameStarter _gameStarter;
    
    private void Start()
    {
        _camera = Camera.main;
        _gameStarter = _camera.GetComponent<GameStarter>();
    }
    
    private void Update()   
    {
        if (!_gameStarter.IsGameStared)
        {
            if (_gameStarter.IsFirstPlayerChoised)
            {
                PrepareGame();
                
            }
        }

        if (!_gameStarter.IsGameStared) return;

        if (!_gameStarter.IsFirstPlayerChoised)  
        {
            var attempts = 0;
            var choosed = false;
            
            while (!choosed && attempts < 10000)
            {
                var randomButton = Random.Range(0, _gameStarter.buttonsShipsOne.Count);
                
                if (!_gameStarter.buttonsShipsOne[randomButton].GetComponent<Button>()._wasChosen)
                {
                    _gameStarter.buttonsShipsOne[randomButton].GetComponent<Button>().OnClick();
                    choosed = true;
                }   

                attempts++;
            }
        }
    }

    private void PrepareGame()
    { 
        foreach (var ship in _gameStarter.player2Ships)
            ship.transform.GetChild(0).gameObject.SetActive(false);

        _gameStarter.RandomShipsPositions("player2Ships");

        _gameStarter.StartGame();   
    }
}
