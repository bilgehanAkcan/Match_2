using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketScript : MonoBehaviour
{
    private GameObject rightRocket;
    private string direction;

    public void SetRightRocket(GameObject rightRocket)
    {
        this.rightRocket = rightRocket;
    }

    public void SetDirection(string direction) {
        this.direction = direction;
    }

    public GameObject GetRightRocket() {
        return rightRocket;
    }

    public string GetDirection() {
        return direction;
    }

}
