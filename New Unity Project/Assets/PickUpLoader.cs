using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpLoader : MonoBehaviour {

    public string[] _prefabNames = new string[] { "DummyPickup", "InvertControls", "LowerSpeed", "SpeedUp" };
    public int count = 5;
    public Vector3 pickupScale = new Vector3(0.5f, 0.5f, 0.5f);

    Pickup[] _placeholders;

    void Start () {
        _placeholders = GetComponentsInChildren<Pickup>();
        for (var index = 0; index < count; ++index)
        {
            CreateRandomItemAtRandomPosition();
        }

        DestroyPlaceHolder();
    }

    void CreateRandomItemAtRandomPosition()
    {
        var pickUpIndex = Random.Range(0, _placeholders.Length);

        var prefabName = SelectRandomPrefabName();
        Debug.Log(prefabName);
        GameObject pickUp = Instantiate(Resources.Load(prefabName), Vector3.zero, Quaternion.identity) as GameObject;
        pickUp.transform.parent = this.transform;
        pickUp.transform.position = _placeholders[pickUpIndex].transform.position;
        pickUp.transform.localScale = pickupScale;
    }

    string SelectRandomPrefabName()
    {
        var index = Random.Range(0, _prefabNames.Length);
        return _prefabNames[index];
    }

    void DestroyPlaceHolder ()
    {
        for (var index = 0; index < _placeholders.Length; ++index)
        {
            Destroy(_placeholders[index].gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Vehicle")
        {
            Destroy(this.gameObject);
        }
    }
}
