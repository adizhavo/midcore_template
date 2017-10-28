using Entitas;
using UnityEngine;
using Services.Core;

public sealed partial class GameEntity
{
    public string uniqueId
    {
        get { return gameObject.uniqueId; }
        set { gameObject.uniqueId = value; }
    }

    public string objectId
    {
        get { return gameObject.objectId; }
        set { gameObject.objectId = value; }
    }

    public string typeId
    {
        get { return gameObject.typeId; }
        set { gameObject.typeId = value; }
    }

    public string resourcePath
    {
        get { return resource.path; }
    }

    public GameObject viewObject
    {
        get { return view.view; }
        set { view.view = value; }
    }

    public Transform transform
    {
        get { return view.view.transform; }
    }

    public Vector3 position
    {
        get { return transform.position; }
        set { transform.position = value; }
    }

    public float xPosition
    {
        get { return position.x; }
        set { position = new Vector3(value, position.y, position.z); }
    }

    public float yPosition
    {
        get { return position.y; }
        set { position = new Vector3(position.x, value, position.z); }
    }

    public float zPosition
    {
        get { return position.z; }
        set { position = new Vector3(position.x, position.y, value); }
    }

    public Vector3 localPosition
    {
        get { return transform.localPosition; }
        set { transform.localPosition = value; }
    }

    public float localXPosition
    {
        get { return localPosition.x; }
        set { localPosition = new Vector3(value, localPosition.y, localPosition.z); }
    }

    public float localYPosition
    {
        get { return localPosition.y; }
        set { localPosition = new Vector3(localPosition.x, value, localPosition.z); }
    }

    public float localZPosition
    {
        get { return localPosition.z; }
        set { localPosition = new Vector3(localPosition.x, localPosition.y, value); }
    }

    public Vector3 localScale
    {
        get { return transform.localScale; }
        set { transform.localScale = value; }
    }

    public float localXScale
    {
        get { return localScale.x; }
        set { localScale = new Vector3(value, localScale.y, localScale.z); }
    }

    public float localYScale
    {
        get { return localScale.y; }
        set { localScale = new Vector3(localScale.x, value, localScale.z); }
    }

    public float localZScale
    {
        get { return localScale.z; }
        set { localScale = new Vector3(localScale.x, localScale.y, value); }
    }

    public Vector3 eulerAngles
    {
        get { return transform.eulerAngles; }
        set { transform.eulerAngles = value; }
    }

    public Vector3 localEulerAngles
    {
        get { return transform.localEulerAngles; }
        set { transform.localEulerAngles = value; }
    }

    public Quaternion rotation
    {
        get { return transform.rotation; }
        set { transform.rotation = value; }
    }

    public Quaternion localRotation
    {
        get { return transform.localRotation; }
        set { transform.localRotation = value; }
    }

    public int row
    {
        get { return cell.row; }
        set { cell.row = value; }
    }

    public int column
    {
        get { return cell.column; }
        set { cell.column = value; }
    }
}