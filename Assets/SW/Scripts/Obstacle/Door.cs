using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour {

    GameObject door;
    GameObject pos;
    bool _isMoving;

    private void Start()
    {
        door = transform.GetChild(0).gameObject;
        pos = transform.GetChild(1).gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        print("on trigger");
        if (other.tag == "Player" && !_isMoving)
        {
            _isMoving = true;
            StartCoroutine(OpenDoor());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        print("on trigger");
        if (other.tag == "Player" && !_isMoving)
        {
            _isMoving = true;
            StartCoroutine(CloseDoor());
        }
    }

    IEnumerator OpenDoor()
    {
        Vector3 dir = pos.transform.position - door.transform.position;
        while (true)
        {
            door.transform.position += dir * Time.deltaTime;
            if (Vector3.Distance(door.transform.position, pos.transform.position) < 0.01f)
            {
                door.transform.position = pos.transform.position;
                break;
            }
            yield return null;
        }
        _isMoving = false;
    }

    IEnumerator CloseDoor()
    {
        Vector3 dir = Vector3.zero - door.transform.localPosition;
        while (true)
        {
            door.transform.localPosition += dir * Time.deltaTime;
            if (Vector3.Distance(door.transform.localPosition, Vector3.zero) < 0.01f)
            {
                door.transform.localPosition = Vector3.zero;
                break;
            }
            yield return null;
        }
        _isMoving = false;
    }
}
