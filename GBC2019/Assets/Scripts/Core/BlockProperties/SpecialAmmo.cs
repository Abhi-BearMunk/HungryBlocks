using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAmmo : MonoBehaviour, IRegisterProperty
{
    Block block;
    private int ammo = 0;

    public void Register(Block _block)
    {
        block = _block;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
