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


    public void SetNewWeapon<T>() where T: IWeapon
    {        
        if (GetComponent<T>() != null)
        {
            weapon = GetComponent<T>();
            OnWeaponChange.Invoke(weapon);
        }
    }
}
