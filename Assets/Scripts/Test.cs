using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    void Start()
    {
        inputReader.MoveEvent += OnMove;
        inputReader.PrimaryFireEvent += OnFire;

    }
    void OnDestroy()
    {
        inputReader.MoveEvent -= OnMove;
        inputReader.PrimaryFireEvent -= OnFire;
    }
    public void OnMove(Vector2 dir) => Debug.Log("MOVE INPUT:" + dir);
    public void OnFire(bool isFire) => Debug.Log("Fire INPUT:" + isFire);
}
