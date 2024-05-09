using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.Hands;

namespace Project.Scripts
{
    public sealed class CastHandPose : MonoBehaviour
    {
        private XRHandSubsystem _handSubsystem;

        private void Start()
        {
            RegisterHandSystem().Forget();
        }

        private void OnDestroy()
        {
            if (_handSubsystem == null)
            {
                return;
            }

            _handSubsystem.updatedHands -= UpdatedHands;
        }

        private async UniTaskVoid RegisterHandSystem()
        {
            var handSubsystems = new List<XRHandSubsystem>();

            while (true)
            {
                await UniTask.Yield(cancellationToken: destroyCancellationToken);
                SubsystemManager.GetSubsystems(handSubsystems);
                foreach (var s in handSubsystems)
                {
                    if (s.running)
                    {
                        _handSubsystem = s;
                        break;
                    }
                }

                if (_handSubsystem != null)
                {
                    break;
                }
            }

            while (true)
            {
                await UniTask.Yield(cancellationToken: destroyCancellationToken);
                if (_handSubsystem.running && _handSubsystem.leftHand.isTracked && _handSubsystem.rightHand.isTracked)
                {
                    _leftTable = CreateHand(_handSubsystem, _handSubsystem.leftHand, jointPrefab);
                    _rightTable = CreateHand(_handSubsystem, _handSubsystem.rightHand, jointPrefab);
                    break;
                }
            }

            _handSubsystem.updatedHands += UpdatedHands;
        }

        #region Update

        private void UpdatedHands(XRHandSubsystem handSubsystem, XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags,
            XRHandSubsystem.UpdateType updateType)
        {
            if (updateType == XRHandSubsystem.UpdateType.BeforeRender)
            {
                return;
            }

            if (updateSuccessFlags.HasFlag(XRHandSubsystem.UpdateSuccessFlags.LeftHandJoints) &&
                handSubsystem.leftHand.isTracked)
            {
                UpdateJointTransforms(handSubsystem.leftHand, _leftTable);
            }

            if (updateSuccessFlags.HasFlag(XRHandSubsystem.UpdateSuccessFlags.RightHandJoints) &&
                handSubsystem.rightHand.isTracked)
            {
                UpdateJointTransforms(handSubsystem.rightHand, _rightTable);
            }
        }

        private static void UpdateJointTransforms(XRHand hand,
            Dictionary<XRHandJointID, Transform> displayObjects)
        {
            foreach (var joint in displayObjects)
            {
                var trackingData = hand.GetJoint(joint.Key);
                var displayTransform = joint.Value;
                if (trackingData.TryGetPose(out Pose pose))
                {
                    displayTransform.localPosition = pose.position;
                    displayTransform.localRotation = pose.rotation;
                }
            }
        }

        #endregion

        #region Create

        [SerializeField]
        private GameObject jointPrefab;

        private Dictionary<XRHandJointID, Transform> _leftTable;
        private Dictionary<XRHandJointID, Transform> _rightTable;

        private Dictionary<XRHandJointID, Transform> CreateHand(
            XRHandSubsystem subsystem, XRHand hand, GameObject prefab)
        {
            var root = new GameObject(hand.handedness.ToString()).transform;
            root.parent = this.transform;
            root.localPosition = Vector3.zero;
            root.localRotation = quaternion.identity;
            return CreateHandDisplay(prefab, root, subsystem);
        }

        private static Dictionary<XRHandJointID, Transform> CreateHandDisplay(
            GameObject jointPrefab,
            Transform sceneParent,
            XRHandSubsystem handSubsystem)
        {
            var displayObjects = new Dictionary<XRHandJointID, Transform>();

            for (var i = XRHandJointID.BeginMarker.ToIndex();
                 i < XRHandJointID.EndMarker.ToIndex();
                 i++)
            {
                if (handSubsystem.jointsInLayout[i])
                {
                    XRHandJointID jointID = XRHandJointIDUtility.FromIndex(i);
                    var go = Instantiate(jointPrefab, sceneParent);
                    go.name = jointID.ToString();
                    displayObjects.Add(jointID, go.transform);
                }
            }

            return displayObjects;
        }

        #endregion
    }
}
