using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Windows.Kinect;
using Kinect = Windows.Kinect;
using Joint = Windows.Kinect.Joint;
using HandState = Windows.Kinect.HandState;
using UnityEngine.EventSystems;

public class NEWBodySourceView : MonoBehaviour
{

    public BodySourceManager mBodySourceManager;
    public GameObject mJointObject;

    //public Material BoneMaterial;
    //public GameObject BodySourceManager;

    private Dictionary<ulong, GameObject> mBodies = new Dictionary<ulong, GameObject>();
    private List<JointType> _joints = new List<JointType> 
    //private BodySourceManager _BodyManager;
    
    //private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    //HEY EM SO I LEFT THE JOINTS AS THEY ARE I THINK YOUD JUST HAVE TO GO BACK TO A PREVIOUS VIDEO AND MATCH THEM TO HIS
    {
        JointType.FootLeft,
        JointType.AnkleLeft,
        JointType.KneeLeft,
        JointType.HipLeft,
        JointType.SpineBase,
        JointType.FootRight,
        JointType.AnkleRight,
        JointType.KneeRight,
        JointType.HipRight,
        JointType.HandTipLeft,
        JointType.HandLeft,
        JointType.ThumbLeft,
        JointType.WristLeft,
        JointType.ElbowLeft,
        JointType.ShoulderLeft,
        JointType.SpineShoulder,
        JointType.HandTipRight,
        JointType.HandRight,
        JointType.ThumbRight,
        JointType.WristRight,
        JointType.ElbowRight,
        JointType.ShoulderRight,
        JointType.SpineMid,
        JointType.Neck,
        JointType.Head,
        //{ Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        //{ Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        //{ Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        //{ Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

        //{ Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        //{ Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        //{ Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        //{ Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

        //{ Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        //{ Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        //{ Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        //{ Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        //{ Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        //{ Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

        //{ Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        //{ Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        //{ Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        //{ Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        //{ Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        //{ Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

        //{ Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        //{ Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        //{ Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        //{ Kinect.JointType.Neck, Kinect.JointType.Head }
    };

    void Update()
    {
        #region Get Kinect data
        Body[] data = mBodySourceManager.GetData();
        if (data == null)
        {
            return;
        }


        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data)
        {
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                trackedIds.Add(body.TrackingId);

                if (body.HandRightState == HandState.Closed)
                {
                    Debug.Log("RIGHT HAND IS CLOSED");
                }

                if (body.HandLeftState == HandState.Closed)
                {
                    Debug.Log("LEFT HAND IS CLOSED");
                }
            }
        }
        #endregion

        #region Delete Kinect bodies
        List<ulong> knownIds = new List<ulong>(mBodies.Keys);
        foreach (ulong trackingId in knownIds)
        {
            if (!trackedIds.Contains(trackingId))
            {
                //destroy body object
                Destroy(mBodies[trackingId]);

                //remove from list
                mBodies.Remove(trackingId);
            }
        }
        #endregion

        #region Create Kinect Bodies
        foreach (var body in data)
        {
            //if no body, skip
            if (body == null)
            {
                continue;
            }

            if (body.IsTracked)
            {
                //if body isnt tracked, create body
                if (!mBodies.ContainsKey(body.TrackingId))
                {
                    mBodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }

                //update positions
                UpdateBodyObject(body, mBodies[body.TrackingId]);
            }
        }
        #endregion
    }
        private GameObject CreateBodyObject(ulong id)
        {
            //create body parent
            GameObject body = new GameObject("Body:" + id);

            //create joints
            foreach (JointType joint in _joints)
            {
                //create object
                GameObject newJoint = Instantiate(mJointObject);
                newJoint.name = joint.ToString();

                //parent to body
                newJoint.transform.parent = body.transform;
            }

            return body;
        }

        private void UpdateBodyObject(Body body, GameObject bodyObject)
        {
            // update joints
            foreach (JointType _joint in _joints)
            {
                //get new target position
                Joint sourceJoint = body.Joints[_joint];
                Vector3 targetPosition = GetVector3FromJoint(sourceJoint);
                targetPosition.z = 0;

                //get joint, set new joint position
                Transform jointObject = bodyObject.transform.Find(_joint.ToString());
                jointObject.position = targetPosition;
            }
        }
       
        Vector3 GetVector3FromJoint(Joint joint)
        {
            return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
        }
    }