using System;
using UnityEngine;

public class ClickDetection : MonoBehaviour
{
    private UnityEngine.UI.Image _image;

    public Boolean _wasChosen;
    public Boolean _isHitted;

    private Launcher _launcher;
    
    [SerializeField] private Sprite missSprite;
    [SerializeField] private Sprite hitSprite;
    
    private void Start()    
    {
        _image = transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        _launcher = Camera.main.GetComponent<Launcher>();
    }

    public void OnClick()
    {
        if (_wasChosen) return;

        if (!_launcher.IsFirstPlayerChoised)
        {
            Setter(_launcher.player1Ships, 0);
        }
        else if (!_launcher.IsSecondPlayerChoised)
        {
            Setter(_launcher.player2Ships, 1);
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
            _launcher.messageText.text = "Hit!";
        }
        else
        {
            _image.sprite = missSprite;

            _launcher.messageText.text = "Miss :(";
            
            if (switcher == 0)
            {
                _launcher.IsFirstPlayerChoised = true;
            }
            else
            {
                _launcher.IsSecondPlayerChoised = true;
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
