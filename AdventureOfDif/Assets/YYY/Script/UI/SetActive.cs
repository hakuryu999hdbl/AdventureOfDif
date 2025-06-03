using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActive : MonoBehaviour
{
    public bool isCritial = false;

    void SetActivefalse()
    {
        gameObject.SetActive(false);

        if (isCritial)
        {
            Time.timeScale = 1;
        }
       
    }

}
