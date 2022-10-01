using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamKordons : MonoBehaviour
{
    public static CamKordons Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }
}
