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
    bool isHolding = false;
    public BodySourceManager mBodySourceManager;
    public GameObject mJointObject;
    private GameObject[] piece;

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
        private GameObject[] puzzlePieces;
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
            //check collisions
            if (body.IsTracked)
            {
            puzzlePieces = GameObject.FindGameObjectsWithTag("piece");
            Vector3 rightHandPosition = GetVector3FromJoint(body.Joints[JointType.HandRight]);
            rightHandPosition.z = 0;

            Vector3 leftHandPosition = GetVector3FromJoint(body.Joints[JointType.HandLeft]);
            leftHandPosition.z = 0;

            
            

            foreach (GameObject puzzlePiece in puzzlePieces)
            {
                GameObject pieceSpot = puzzlePiece.GetComponent<Draggable>().pieceSpot;
                bool locked = puzzlePiece.GetComponent<Draggable>().locked;
                float rightDist = Vector3.Distance(puzzlePiece.transform.position, rightHandPosition);
                
                float leftDist = Vector3.Distance(puzzlePiece.transform.position, leftHandPosition);

                if (body.HandRightState == HandState.Closed)
                {
                    Debug.Log("RIGHT HAND IS CLOSED " +  rightDist + " " + puzzlePiece.name);
                    if (rightDist < 1 && (locked == false))
                    {
                        Debug.Log("PIECE GRABBED WITH RIGHT HAND ");
              
                        puzzlePiece.transform.position = rightHandPosition;
                        
                        float pieceSpotDistance = Vector3.Distance(puzzlePiece.transform.position, pieceSpot.transform.position);
                        if (pieceSpotDistance < 0.5)
                        {
                            puzzlePiece.transform.position = pieceSpot.transform.position;
                            puzzlePiece.transform.position = new Vector3(pieceSpot.transform.position.x, pieceSpot.transform.position.y, -1);

                            locked = true;
                            
                        }
                        return;

                        
                    }
                }

                if (body.HandLeftState == HandState.Closed)
                {
                    Debug.Log("LEFT HAND IS CLOSED " + leftDist + " " + puzzlePiece.name);
                    if (leftDist < 1 && (locked == false))
                    {
                        Debug.Log("PIECE GRABBED WITH LEFT HAND ");
                        puzzlePiece.transform.position = leftHandPosition;
                        //puzzlePiece.GetComponent<Draggable>().followingHand = leftHandPosition;
                        
                        float pieceSpotDistance = Vector3.Distance(puzzlePiece.transform.position, pieceSpot.transform.position);
                        if (pieceSpotDistance < 0.5)
                        {
                            puzzlePiece.transform.position = pieceSpot.transform.position;
                            puzzlePiece.transform.position = new Vector3(pieceSpot.transform.position.x, pieceSpot.transform.position.y, -1);

                            locked = true;

                        }
                        return;
                    }
                }

                if (body.HandLeftState == HandState.Open && body.HandRightState == HandState.Open)
                {
                    
                }
            }
            
                
               
            }
    }
       
        Vector3 GetVector3FromJoint(Joint joint)
        {
            return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
        }
    }