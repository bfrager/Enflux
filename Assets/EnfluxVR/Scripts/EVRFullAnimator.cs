//========= Copyright 2016, Enflux Inc. All rights reserved. ===========
//
// Purpose: Full body animation using EnfluxVR suit
//
//======================================================================

using UnityEngine;
using System;

public class EVRFullAnimator : EVRHumanoidLimbMap, ILimbAnimator {

    private ILimbAnimator upperAnim;
    private ILimbAnimator lowerAnim;
    private DataRecording recorder;
    private float[] upper = new float[20];
    private float[] lower = new float[20];

    // Use this for initialization
    void Start () {
        upperAnim = GameObject.Find("EVRUpperLimbMap")
            .GetComponent<EVRUpperLimbMap>();

        lowerAnim = GameObject.Find("EVRLowerLimbMap")
            .GetComponent<EVRLowerLimbMap>();

        recorder = GameObject.Find("DataRecording").
            GetComponent<DataRecording>();
	}

    public void setInit()
    {
        upperAnim.setInit();
        lowerAnim.setInit();
    }

    public void resetInit()
    {
        upperAnim.resetInit();
        lowerAnim.resetInit();
    }

    public void operate(float[] angles)
    {
        if(angles != null)
        {
            Buffer.BlockCopy(angles, 0, upper, 0, 20 * sizeof(float));
            //Array.Copy(angles, 0, upper, 0, 20);
            upperAnim.operate((float[])upper.Clone());

            Buffer.BlockCopy(angles, 20 * sizeof(float), lower, 0, 20 * sizeof(float));
            //Array.Copy(angles, 20, lower, 0, 20);
            lowerAnim.operate((float[])lower.Clone());

            //Debug.Log("UPPER: " + upper[1]);

            if (recorder.shouldRecord())
            {
                recorder.addUpper((float[])upper.Clone());                
                recorder.addLower((float[])lower.Clone());
            }
        }
    }
}
