//========= Copyright 2016, Enflux Inc. All rights reserved. ===========
//
// Purpose: Lower body mapping and operation with EnfluxVR suit
//
//======================================================================

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EVRLowerLimbMap : EVRHumanoidLimbMap, ILimbAnimator {

    private JointRotations jointRotations = new JointRotations();
    private Quaternion chain;
    private Quaternion initWaistPose = new Quaternion();
    private float[] initWaist = new float[] { 0, 0, 0 };
    private float[] initLeftThigh = new float[] { 0, 0, 0 };
    private float[] initLeftShin = new float[] { 0, 0, 0 };
    private float[] initRightThigh = new float[] { 0, 0, 0 };
    private float[] initRightShin = new float[] { 0, 0, 0 };
    private Queue<Quaternion> waistPose = new Queue<Quaternion>();
    private Queue<Quaternion> rightThighPose = new Queue<Quaternion>();
    private Queue<Quaternion> rightShinPose = new Queue<Quaternion>();
    private Queue<Quaternion> leftThighPose = new Queue<Quaternion>();
    private Queue<Quaternion> leftShinPose = new Queue<Quaternion>();


    // Use this for initialization
    void Start () {
	
	}

    public void setInit()
    {
        initState = InitState.INIT;
        StartCoroutine(setPoses());
    }

    public void resetInit()
    {
        initState = InitState.PREINIT;
        StopAllCoroutines();
    }

    private void setInitRot()
    {
        //initWaistPose = jointRotations.rotateWaist(new float[] { 0, 0, initWaist[2] },
        //    new float[] { 0, 0, 0 }, hmd.localRotation);

        waist.localRotation = Quaternion.AngleAxis(core.localRotation.eulerAngles.y, Vector3.up);
    }

    private IEnumerator setPoses()
    {
        while (true)
        {
            if (waistPose.Count > 0)
            {
                waist.localRotation = waistPose.Dequeue();
            }

            if (rightThighPose.Count > 0)
            {
                rightThigh.localRotation = rightThighPose.Dequeue();
            }

            if (rightShinPose.Count > 0)
            {
                rightShin.localRotation = rightShinPose.Dequeue();
            }

            if (leftThighPose.Count > 0)
            {
                leftThigh.localRotation = leftThighPose.Dequeue();
            }

            if (leftShinPose.Count > 0)
            {
               leftShin.localRotation = leftShinPose.Dequeue();
            }

            yield return null;
        }
    }

    public void operate(float[] angles)
    {
        if(angles != null)
        {
            if (initState == InitState.PREINIT && angles != null)
            {
                Buffer.BlockCopy(angles, 1 * sizeof(float), initWaist, 0, 3 * sizeof(float));
                Buffer.BlockCopy(angles, 5 * sizeof(float), initLeftThigh, 0, 3 * sizeof(float));
                Buffer.BlockCopy(angles, 9 * sizeof(float), initLeftShin, 0, 3 * sizeof(float));
                Buffer.BlockCopy(angles, 13 * sizeof(float), initRightThigh, 0, 3 * sizeof(float));
                Buffer.BlockCopy(angles, 17 * sizeof(float), initRightShin, 0, 3 * sizeof(float));

                setInitRot();
            }
            else if (initState == InitState.INIT)
            {
                //core node 1
                //float[] waistAngles = new float[] { angles[1], angles[2], angles[3] };
                //chain = jointRotations.rotateWaist(waistAngles, initWaist, hmd.localRotation);

                chain = Quaternion.AngleAxis(core.localRotation.eulerAngles.y, Vector3.up);

                waistPose.Enqueue(chain);

                //Left Thigh
                float[] ltAngles = new float[] { angles[5] - initLeftThigh[0], angles[6] - initLeftThigh[1], angles[7] };
                chain = jointRotations.rotateLeftLeg(ltAngles, waist.localRotation,
                    hmd.localRotation);

                leftThighPose.Enqueue(chain);

                //Left shin
                float[] lsAngles = new float[] { angles[9] - initLeftShin[0], angles[10] - initLeftShin[1], angles[11]};
                chain = jointRotations.rotateLeftShin(lsAngles, waist.localRotation,
                    leftThigh.localRotation, hmd.localRotation);

                leftShinPose.Enqueue(chain);

                //Right Thigh
                float[] rtAngles = new float[] { angles[13] - initRightThigh[0], angles[14] - initRightThigh[1], angles[15] };
                chain = jointRotations.rotateRightLeg(rtAngles, waist.localRotation,
                    hmd.localRotation);

                rightThighPose.Enqueue(chain);

                //Right shin
                float[] rsAngles = new float[] { angles[17] - initRightShin[0], angles[18] - initRightShin[1], angles[19] };
                chain = jointRotations.rotateRightShin(rsAngles, waist.localRotation,
                    rightThigh.localRotation, hmd.localRotation);

                rightShinPose.Enqueue(chain);
            }
        }
    }
}
