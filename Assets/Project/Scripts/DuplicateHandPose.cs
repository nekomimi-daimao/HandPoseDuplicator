using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Hands;
using UniVRM10;
using VRM;
using VRMShaders;

namespace Project.Scripts
{
    public class DuplicateHandPose : MonoBehaviour
    {
        private void Start()
        {
            Init().Forget();
        }

        private async UniTask Init()
        {
            var animator = await LoadVrm();
            animator.transform.SetParent(this.transform);
            animator.transform.localPosition = Vector3.zero;
            animator.transform.localRotation = Quaternion.identity;
        }

        private static async UniTask<Animator> LoadVrm()
        {
            var path = Path.Combine(Application.streamingAssetsPath, "5988710431914478867.vrm");
            var readAllBytes = await File.ReadAllBytesAsync(path);
            var runtimeGltfInstance =
                await VrmUtility.LoadBytesAsync(null, readAllBytes, new RuntimeOnlyAwaitCaller(),
                    vrm => new UrpVrm10MaterialDescriptorGenerator());
            runtimeGltfInstance.EnableUpdateWhenOffscreen();
            runtimeGltfInstance.ShowMeshes();
            return runtimeGltfInstance.Root.GetComponent<Animator>();
        }

        private readonly Dictionary<XRHandJointID, HumanBodyBones> _right = new()
        {
            { XRHandJointID.Wrist, HumanBodyBones.RightHand },

            { XRHandJointID.ThumbProximal, HumanBodyBones.RightThumbProximal },
            { XRHandJointID.ThumbDistal, HumanBodyBones.RightThumbDistal },

            { XRHandJointID.IndexProximal, HumanBodyBones.RightIndexProximal },
            { XRHandJointID.IndexIntermediate, HumanBodyBones.RightIndexIntermediate },
            { XRHandJointID.IndexDistal, HumanBodyBones.RightIndexDistal },

            { XRHandJointID.MiddleProximal, HumanBodyBones.RightMiddleProximal },
            { XRHandJointID.MiddleIntermediate, HumanBodyBones.RightMiddleIntermediate },
            { XRHandJointID.MiddleDistal, HumanBodyBones.RightMiddleDistal },

            { XRHandJointID.RingProximal, HumanBodyBones.RightRingProximal },
            { XRHandJointID.RingIntermediate, HumanBodyBones.RightRingIntermediate },
            { XRHandJointID.RingDistal, HumanBodyBones.RightRingDistal },

            { XRHandJointID.LittleProximal, HumanBodyBones.RightLittleProximal },
            { XRHandJointID.LittleIntermediate, HumanBodyBones.RightLittleIntermediate },
            { XRHandJointID.LittleDistal, HumanBodyBones.RightLittleDistal },
        };

        private readonly Dictionary<XRHandJointID, HumanBodyBones> _left = new()
        {
            { XRHandJointID.Wrist, HumanBodyBones.LeftHand },

            { XRHandJointID.ThumbProximal, HumanBodyBones.LeftThumbProximal },
            { XRHandJointID.ThumbDistal, HumanBodyBones.LeftThumbDistal },

            { XRHandJointID.IndexProximal, HumanBodyBones.LeftIndexProximal },
            { XRHandJointID.IndexIntermediate, HumanBodyBones.LeftIndexIntermediate },
            { XRHandJointID.IndexDistal, HumanBodyBones.LeftIndexDistal },

            { XRHandJointID.MiddleProximal, HumanBodyBones.LeftMiddleProximal },
            { XRHandJointID.MiddleIntermediate, HumanBodyBones.LeftMiddleIntermediate },
            { XRHandJointID.MiddleDistal, HumanBodyBones.LeftMiddleDistal },

            { XRHandJointID.RingProximal, HumanBodyBones.LeftRingProximal },
            { XRHandJointID.RingIntermediate, HumanBodyBones.LeftRingIntermediate },
            { XRHandJointID.RingDistal, HumanBodyBones.LeftRingDistal },

            { XRHandJointID.LittleProximal, HumanBodyBones.LeftLittleProximal },
            { XRHandJointID.LittleIntermediate, HumanBodyBones.LeftLittleIntermediate },
            { XRHandJointID.LittleDistal, HumanBodyBones.LeftLittleDistal },
        };
    }
}
