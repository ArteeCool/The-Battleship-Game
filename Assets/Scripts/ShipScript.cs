using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShipScript : MonoBehaviour
{   
    [HideInInspector] public Vector2 internalPosition;

    [HideInInspector] public Boolean isDead;

    [HideInInspector] public Vector2 startPosition;
    
    [SerializeField] private Vector2 offset;

    [SerializeField] private List<GameObject> buttonsAround;

    public List<Vector2> shipPartsPositions; 

    [SerializeField] public Int32 partCount;
    
    [SerializeField] public Boolean rotated;
    
    [SerializeField] private Single rotatePreiod;
    [SerializeField] private Single setDistance;

    private const Single DragMultipler = 1f;
    private Single _timer;

    public Boolean isHitting;
    
    private Boolean _isButtonPressed;
    private Boolean _isButtonPressedTwice;

    private Int32 _clicksCount;
    
    [HideInInspector] public Int32 deadPartsCount;

    private GameStarter _gameStarter; 

    private SpriteRenderer _spriteRenderer;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
        internalPosition = transform.localPosition;
        startPosition = transform.localPosition;
        _gameStarter = Camera.main.GetComponent<GameStarter>();
        _spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
    }

    private void Update()   
    {
        if (!_gameStarter.IsGameStared)
        {
            if (_timer >= Time.time)
            {
                if (_clicksCount >= 2)
                {
                    if (rotated)
                    {
                        rotated = false;
                    }
                    else
                    {
                        rotated = true;
                    }

                    _clicksCount = 0;
                }
            }

            if (_timer <= Time.time)
            {
                if (_clicksCount == 1)
                {
                    _clicksCount = 0;
                }
            }

            if (_isButtonPressed)
            {
                Vector2 mousePositon = _camera.ScreenToWorldPoint(Input.mousePosition);
                mousePositon += offset;
                transform.position = Vector2.MoveTowards(transform.position, mousePositon + offset, DragMultipler);
            }
            else
            {
                    transform.localPosition = internalPosition;
            }

            if (rotated)        
            {
                transform.localScale = new Vector3(100f * partCount, 100f, 100f);
            }
            else
            {
                transform.localScale = new Vector3(100f, 100f * partCount, 100f);
            }
        }
        
        if (!_gameStarter.IsGameStared) return;
        
            
        if (rotated)
        {
            transform.localScale = new Vector3(100f * partCount, 100f, 100f);
        }   
        else
        {
            transform.localScale = new Vector3(100f, 100f * partCount, 100f);
        }
        
        transform.localPosition = internalPosition;

        if (isDead) return; 
        
        if (deadPartsCount >= partCount)
        {
            isDead = true;
            OnDead();
        }
         
    }

    private void OnDead()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetComponentInChildren<SpriteRenderer>().color = Color.red;
        for (int i = 0; i < buttonsAround.Count; i++)
        {
            buttonsAround[i].GetComponent<Button>().SetterNoCheck();
        }
    }

    private void OnMouseDown()
    {
        if (_gameStarter.IsGameStared) return;
        
        _isButtonPressed = true;
        _clicksCount++;
        if (_clicksCount == 1)
        {
            _timer = Time.time + rotatePreiod;    
        }
    }

    private void OnMouseUp()
    {
        if (_gameStarter.IsGameStared) return;
        
        Vector2 mouseUpPosition = transform.localPosition;

        List<Single> dist = new List<Single>();

        foreach (var variableVector2 in Fields.FieldCoordinates)
        {
            dist.Add(Vector2.Distance(variableVector2, mouseUpPosition));
        }

        var minDistance = dist.Min();

        foreach (var variableVector2 in Fields.FieldCoordinates)
        {
            var distance = Vector2.Distance(variableVector2, mouseUpPosition);
            if (distance == minDistance && dist.Min()<= setDistance) 
            {   
                internalPosition = variableVector2;     
            } 
        }
        
        _isButtonPressed = false;
    }

    public void AddPartsPositions()
    {
        var step = 50;
        if (rotated)
        {   
            for (int i = 0; i < partCount; i++)
            {
                shipPartsPositions.Add(new Vector2(internalPosition.x + step * i ,internalPosition.y));
            }
        }
        else
        {
            for (int i = 0; i < partCount; i++)
            {
                shipPartsPositions.Add(new Vector2(internalPosition.x ,internalPosition.y + step * i));
            }
        }
    }

    public Boolean CheckForCollisions(GameObject[] ships)
    {
        AddPartsPositions();

        List<Vector2> shipOffsetedPotitions = new List<Vector2>();

        foreach ( var partPosition in shipPartsPositions)
        {
            foreach (var ship in ships)
            {
                if (ship.GetInstanceID() == gameObject.GetInstanceID()) continue;

                ship.GetComponent<ShipScript>().AddPartsPositions();
                foreach (var position in ship.GetComponent<ShipScript>().shipPartsPositions)
                {
                    var startX = -50;
                    var startY = 50;
                    
                    for (int i = 0; i < 3; i++)
                    {   
                        for (int j = 0; j < 3; j++)
                        {
                            shipOffsetedPotitions.Add(partPosition + new Vector2(startX, startY));
                            startY -= 50;
                        }

                        startX += 50;
                        startY = 50;
                    }

                    foreach (var offsetedPotition in shipOffsetedPotitions)
                    {
                        if (offsetedPotition == position)
                        {
                            foreach (var shipClear in ships)
                            {
                                shipClear.GetComponent<ShipScript>().shipPartsPositions.Clear();
                            }
                            return true;
                        }
                    }
                }
            }   
        }
        ClearPartsPosition(ships);
        return false;
    }


    private void ClearPartsPosition(GameObject[] ships)
    {
        foreach (var ship in ships)
        {
            ship.GetComponent<ShipScript>().shipPartsPositions.Clear();
        }
    }
    
    private void OnCollisionStay2D(Collision2D collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag($"Ship"))
        {
            isHitting = true;
            _spriteRenderer.color = Color.red;
        }
    }

    private void OnCollisionExit2D(Collision2D collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag($"Ship"))
        {
            isHitting = false;
            _spriteRenderer.color = Color.black;  
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        buttonsAround.Add(other.gameObject);
    }
}