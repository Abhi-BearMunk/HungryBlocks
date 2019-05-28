using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class WeaponChangeEvent: UnityEvent<IWeapon>
{

}

public class WeaponHandler : MonoBehaviour
{
    public WeaponChangeEvent OnWeaponChange;
    public IWeapon weapon;


    public void SetNewWeapon(System.Type type) 
    {        
        if(!GetComponent(type))
        {
            gameObject.AddComponent(type);
        }

        if (GetComponent(type) && (IWeapon)GetComponent(type) != null)
        {
            weapon = (IWeapon)GetComponent(type);
            OnWeaponChange.Invoke(weapon);
        }
    }
}
