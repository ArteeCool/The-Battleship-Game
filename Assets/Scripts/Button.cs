using System;
using UnityEngine;

public class Button : MonoBehaviour
{
    private UnityEngine.UI.Image _image;

    private Boolean _wasChosen;
    private Boolean _isHitted;

    private GameStarter _gameStarted;
    
    [SerializeField] private Sprite missSprite;
    [SerializeField] private Sprite hitSprite;
    
    private void Start()
    {
        _image = GetComponent<UnityEngine.UI.Image>();
        _gameStarted = Camera.main.GetComponent<GameStarter>();
    }

    public void OnClick()
    {
        if (_wasChosen) return;

        if (!_gameStarted.IsFirstPlayerChoised)
        {
            Setter(_gameStarted.player1Ships, 0);
        }
        else if (!_gameStarted.IsSecondPlayerChoised)
        {
            Setter(_gameStarted.player2Ships, 1);
        }

        _wasChosen = true;
    }

    private void Setter(GameObject[] array, Int32 switcher)
    {
        foreach (var ship in array)
        {
            var ship1 = ship;
            foreach (var shipPartsPosition in ship.GetComponent<ShipScript>().shipPartsPositions)   
            {
                if (new Vector2(transform.localPosition.x, transform.localPosition.y) == shipPartsPosition)
                {
                    _isHitted = true;
                    ship1.GetComponent<ShipScript>().deadPartsCount++;
                }   
            }
        }
        if (_isHitted)
        {
            
            _image.sprite = hitSprite;
        }
        else
        {
            _image.sprite = missSprite;
            
            if (switcher == 0)
            {
                _gameStarted.IsFirstPlayerChoised = true;
            }
            else
            {
                _gameStarted.IsSecondPlayerChoised = true;
            }
        }
    }

    public void SetterNoCheck()
    {
        if (_wasChosen) return;
        _image.sprite = missSprite;
        _wasChosen = true;   
    }
}
