﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake3D : MonoBehaviour
{
    public static CameraShake3D Instance
    {
        get
        {
            if (instance != null)
                return instance;
            instance = FindObjectOfType<CameraShake3D>();
            return instance;
        }
    }
    private static CameraShake3D instance;

    CameraNoise cameraNoise;
    public CameraNoise.Properties noise;

	public void Shake()
    {
        if (cameraNoise == null)
        {
            cameraNoise = GetComponent<CameraNoise>();
            noise = new CameraNoise.Properties(90f, .85f, 45f, 1f, .635f, .4f, 1.85f);
        }

        cameraNoise.StartShake(noise);
    }
}