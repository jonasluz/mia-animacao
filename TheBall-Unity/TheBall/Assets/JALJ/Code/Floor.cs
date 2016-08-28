using UnityEngine;
using System.Collections;

public class Floor : MonoBehaviour {

    //[HideInInspector]
    public GameObject oldOne;

    private float lenght;

	void Awake()
    {
        lenght = GetComponent<MeshRenderer>().bounds.extents.x;
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            Vector3 position = transform.position + 2 * new Vector3(lenght, 0);

            Floor floor = Instantiate(this, position, Quaternion.identity) as Floor;
            floor.transform.SetParent(transform.parent);

            floor.oldOne = gameObject;
            //if (oldOne != null) Destroy(oldOne);
        }
    }
	
}
