using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item : MonoBehaviour
{
    public enum Type {Ammo,Coin,Grenade,Heart,Wepon}
    public Type type;
    public int value;


     void Update()
    {
        transform.Rotate(Vector3.up * 50 * Time.deltaTime);
    }
}
