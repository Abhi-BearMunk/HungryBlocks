using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnKill : MonoBehaviour, IOnKillProperty
{
    public void OnKill()
    {
        Destroy(gameObject);
    }
}
