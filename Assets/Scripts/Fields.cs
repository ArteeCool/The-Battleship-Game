using UnityEngine;

public class Fields : MonoBehaviour
{
    private float _previousPositionX, _previousPositionY, _startPositionX;
    public static readonly Vector2[,] FieldCoordinates = new Vector2[10, 10];
    private static GameObject  _gameObjectParentStatic;
    private void Start()
    {
        _previousPositionX = -250f;
        _previousPositionY = 200f;
            
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                FieldCoordinates[i, j] = new Vector2(_previousPositionX, _previousPositionY);
                _previousPositionX += 50f;
            }
            _previousPositionX = -250f;
            _previousPositionY -= 50f;
        }
    }

    public static void FillFields(GameObject objectToInstantiate, GameObject parent)
    {
        foreach (var coordinate in FieldCoordinates)
        {
            var objectGameObject = Instantiate(objectToInstantiate, parent.transform);

            objectGameObject.GetComponent<RectTransform>().anchoredPosition = coordinate;
        }
    }
}