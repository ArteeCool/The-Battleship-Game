using System;
using UnityEngine;

public class Button : MonoBehaviour
{
    private UnityEngine.UI.Image _image;

    public Boolean _wasChosen;
    public Boolean _isHitted;

    private GameStarter _gameStarter;
    
    [SerializeField] private Sprite missSprite;
    [SerializeField] private Sprite hitSprite;
    
    private void Start()
    {
        _image = GetComponent<UnityEngine.UI.Image>();
            _gameStarter = Camera.main.GetComponent<GameStarter>();
    }

    public void OnClick()
    {
        if (_wasChosen) return;

        if (!_gameStarter.IsFirstPlayerChoised)
        {
            Setter(_gameStarter.player1Ships, 0);
        }
        else if (!_gameStarter.IsSecondPlayerChoised)
        {
            Setter(_gameStarter.player2Ships, 1);
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
                _gameStarter.IsFirstPlayerChoised = true;
            }
            else
            {
                _gameStarter.IsSecondPlayerChoised = true;
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
