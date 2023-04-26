using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using Random = UnityEngine.Random;

public class BotController : MonoBehaviour
{
    
    private Camera _camera;
    private GameStarter _gameStarter;
    [SerializeField] private List<GameObject> _playerFieldButtons = new List<GameObject>();


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
                StartCoroutine(nameof(Wait));   
            }
        }

        if (!_gameStarter.IsGameStared) return;

        if (!_gameStarter.IsFirstPlayerChoised)  
        {
            var attempts = 0;
            var choosed = false;
            
            while (!choosed && attempts < 10000)
            {
                var randomButton = Random.Range(0, _playerFieldButtons.Count);
                
                if (!_playerFieldButtons[randomButton].GetComponent<Button>()._wasChosen)
                {
                    _playerFieldButtons[randomButton].GetComponent<Button>().OnClick();
                    choosed = true;
                }   

                attempts++;
            }
        }
        else
        {
            _gameStarter.StartGame();
        }   
    }

    private IEnumerator Wait()
    {
        foreach (var ship in _gameStarter.player2Ships)
            ship.transform.GetChild(0).gameObject.SetActive(false);
        
        _gameStarter.RandomShipsPositions("player2Ships");
                
        for (int i = 0; i < _gameStarter.gameObjectParentFieldOne.transform.childCount; i++)
        {
            _playerFieldButtons.Add(_gameStarter.gameObjectParentFieldOne.transform.GetChild(i).gameObject);
        }
        
        yield return new WaitForSeconds(0.1f);

        _gameStarter.StartGame();
    }
}
