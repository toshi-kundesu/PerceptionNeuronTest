/************************************************************************************
 Copyright: Copyright 2021 Beijing Noitom Technology Ltd. All Rights reserved.
 Pending Patents: PCT/CN2014/085659 PCT/CN2014/071006

 Licensed under the Perception Neuron SDK License Beta Version (the “License");
 You may only use the Perception Neuron SDK when in compliance with the License,
 which is provided at the time of installation or download, or which
 otherwise accompanies this software in the form of either an electronic or a hard copy.

 A copy of the License is included with this package or can be obtained at:
 http://www.neuronmocap.com

 Unless required by applicable law or agreed to in writing, the Perception Neuron SDK
 distributed under the License is provided on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing conditions and
 limitations under the License.
************************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Neuron
{
    public enum NeuronBoneVersion
    {
        NotSet_AutoByName = -1,
        V1 = 1,
        V2 = 2
    }

    // axis neuron old bones
	public enum NeuronBones
	{
		Hips 				= 0,
		RightUpLeg 			= 1,
		RightLeg 			= 2,
		RightFoot 			= 3,
		LeftUpLeg 			= 4,
		LeftLeg 			= 5,
		LeftFoot 			= 6,
		Spine 				= 7,
		Spine1 				= 8,
		Spine2 				= 9,
		Spine3 				= 10,
		Neck 				= 11,
		Head 				= 12,
		RightShoulder 		= 13,
		RightArm 			= 14,
		RightForeArm 		= 15,
		RightHand 			= 16,
		RightHandThumb1 	= 17,
		RightHandThumb2 	= 18,
		RightHandThumb3 	= 19,
		RightInHandIndex 	= 20,
		RightHandIndex1 	= 21,
		RightHandIndex2 	= 22,
		RightHandIndex3 	= 23,
		RightInHandMiddle 	= 24,
		RightHandMiddle1 	= 25,
		RightHandMiddle2 	= 26,
		RightHandMiddle3 	= 27,
		RightInHandRing 	= 28,
		RightHandRing1 		= 29,
		RightHandRing2 		= 30,
		RightHandRing3 		= 31,
		RightInHandPinky 	= 32,
		RightHandPinky1 	= 33,
		RightHandPinky2 	= 34,
		RightHandPinky3 	= 35,
		LeftShoulder 		= 36,
		LeftArm 			= 37,
		LeftForeArm 		= 38,
		LeftHand 			= 39,
		LeftHandThumb1 		= 40,
		LeftHandThumb2 		= 41,
		LeftHandThumb3 		= 42,
		LeftInHandIndex 	= 43,
		LeftHandIndex1 		= 44,
		LeftHandIndex2 		= 45,
		LeftHandIndex3 		= 46,
		LeftInHandMiddle 	= 47,
		LeftHandMiddle1 	= 48,
		LeftHandMiddle2 	= 49,
		LeftHandMiddle3 	= 50,
		LeftInHandRing 		= 51,
		LeftHandRing1 		= 52,
		LeftHandRing2 		= 53,
		LeftHandRing3 		= 54,
		LeftInHandPinky 	= 55,
		LeftHandPinky1 		= 56,
		LeftHandPinky2 		= 57,
		LeftHandPinky3 		= 58,
		
		NumOfBones
	}

    // from Pn Studio new bones
    public enum NeuronBonesV2
    {
        Hips = 0,
        RightUpLeg = 1,
        RightLeg = 2,
        RightFoot = 3,
        LeftUpLeg = 4,
        LeftLeg = 5,
        LeftFoot = 6,
        Spine = 7,
        Spine1 = 8,
        Spine2 = 9,
        Neck = 10,
        Neck1 = 11,
        Head = 12,
        RightShoulder = 13,
        RightArm = 14,
        RightForeArm = 15,
        RightHand = 16,
        RightHandThumb1 = 17,
        RightHandThumb2 = 18,
        RightHandThumb3 = 19,
        RightInHandIndex = 20,
        RightHandIndex1 = 21,
        RightHandIndex2 = 22,
        RightHandIndex3 = 23,
        RightInHandMiddle = 24,
        RightHandMiddle1 = 25,
        RightHandMiddle2 = 26,
        RightHandMiddle3 = 27,
        RightInHandRing = 28,
        RightHandRing1 = 29,
        RightHandRing2 = 30,
        RightHandRing3 = 31,
        RightInHandPinky = 32,
        RightHandPinky1 = 33,
        RightHandPinky2 = 34,
        RightHandPinky3 = 35,
        LeftShoulder = 36,
        LeftArm = 37,
        LeftForeArm = 38,
        LeftHand = 39,
        LeftHandThumb1 = 40,
        LeftHandThumb2 = 41,
        LeftHandThumb3 = 42,
        LeftInHandIndex = 43,
        LeftHandIndex1 = 44,
        LeftHandIndex2 = 45,
        LeftHandIndex3 = 46,
        LeftInHandMiddle = 47,
        LeftHandMiddle1 = 48,
        LeftHandMiddle2 = 49,
        LeftHandMiddle3 = 50,
        LeftInHandRing = 51,
        LeftHandRing1 = 52,
        LeftHandRing2 = 53,
        LeftHandRing3 = 54,
        LeftInHandPinky = 55,
        LeftHandPinky1 = 56,
        LeftHandPinky2 = 57,
        LeftHandPinky3 = 58,

        NumOfBones
    }
    public static class NeuronHelper
	{
		public static int Bind( Transform root, Transform[] bones, string prefix, bool verbose, NeuronBoneVersion boneVerson)
		{
            return AliceMotionBindHelper.Bind(root, bones, prefix, boneVerson);
            
        }
		
		// add
		public delegate void OnAddBoneComponent<T>( HumanBodyBones bone, T Component, params object[] args ) where T : Component;
		
		public static int AddBonesComponents<T>( Animator animator, OnAddBoneComponent<T> add_delegate, params object[] args ) where T : Component
		{
			if( animator == null )
			{
				return 0;
			}
			
			int counter = 0;
			
			for( HumanBodyBones i = 0; i < HumanBodyBones.LastBone; ++i )
			{
				Transform t = animator.GetBoneTransform( i );
				if( t != null )
				{
					T component = t.gameObject.GetComponent<T>();
					if( component != null )
					{
						GameObject.DestroyImmediate( component );
					}
					
					component = t.gameObject.AddComponent<T>();
					if( add_delegate != null )
					{
						add_delegate( i, component, args );
					}
					++counter;
				}
			}
			
			return counter;
		}
		
		public delegate void OnAddBoneComponentTransform<T>( NeuronBones bone, T Component, params object[] args ) where T : Component;
		
		public static int AddBonesComponentsTransform<T>( Transform root, string prefix, OnAddBoneComponentTransform<T> add_delegate, params object[] args ) where T : Component
		{
			Transform[] bones = new Transform[(int)NeuronBones.NumOfBones];
			//int bound_count = 
            Bind( root, bones, prefix, false , NeuronBoneVersion.NotSet_AutoByName);
			//if( bound_count < (int)NeuronBones.NumOfBones )
			//{
			//	return 0; 
			//}
			
			int counter = 0;
			for( int i = 0; i < (int)NeuronBones.NumOfBones; ++i )
			{
				Transform t = bones[i];
				if( t != null )
				{
					T component = t.gameObject.GetComponent<T>();
					if( component != null )
					{
						GameObject.DestroyImmediate( component );
					}
					
					component = t.gameObject.AddComponent<T>();
					if( add_delegate != null )
					{
						add_delegate( (NeuronBones)i, component, args );
					}
					++counter;
				}
			}
			
			return counter;
		}
		
		// remove
		public delegate void OnRemoveBoneComponent<T>( HumanBodyBones bone, T Component, params object[] args ) where T : Component;
		
		public static int RemoveBonesComponents<T>( Animator animator, OnRemoveBoneComponent<T> remove_delegate, params object[] args ) where T : Component
		{
			if( animator == null )
			{
				return 0;
			}
			
			int counter = 0;
			
			for( HumanBodyBones i = 0; i < HumanBodyBones.LastBone; ++i )
			{
				Transform t = animator.GetBoneTransform( i );
				if( t != null )
				{
					T component = t.gameObject.GetComponent<T>();
					if( component != null )
					{
						if( remove_delegate != null )
						{
							remove_delegate( i, component, args );
						}
						GameObject.DestroyImmediate( component );
					}
					
					++counter;
				}
			}
			
			return counter;
		}
		
		public delegate void OnRemoveBoneComponentTransform<T>( NeuronBones bone, T Component, params object[] args ) where T : Component;
		
		public static int RemoveBonesComponentsTransform<T>( Transform root, string prefix, OnRemoveBoneComponentTransform<T> remove_delegate, params object[] args ) where T : Component
		{
			Transform[] bones = new Transform[(int)NeuronBones.NumOfBones];
			//int bound_count = 
            Bind( root, bones, prefix, false , NeuronBoneVersion.NotSet_AutoByName);
			//if( bound_count < (int)NeuronBones.NumOfBones )
			//{
			//	return 0;
			//}
			
			int counter = 0;
			
			for( int i = 0; i < (int)NeuronBones.NumOfBones; ++i )
			{
				Transform t = bones[i];
				if( t != null )
				{
					T component = t.gameObject.GetComponent<T>();
					if( component != null )
					{
						if( remove_delegate != null )
						{
							remove_delegate( (NeuronBones)i, component, args );
						}
						GameObject.DestroyImmediate( component );
						++counter;
					}
				}
			}
			
			return counter;
		}
		
		public static void BreakHierarchy( Animator animator )
		{
			Transform align_root = animator.GetBoneTransform( HumanBodyBones.Hips ).parent;
			for( HumanBodyBones i = 0; i < HumanBodyBones.LastBone; ++i )
			{
				Transform t = animator.GetBoneTransform( i );
				if( t != null )
				{
                    t.SetParent(align_root, true );
				}
			}
		}
		
		public static void BreakHierarchy( Transform[] transforms )
		{
			Transform align_root = transforms[(int)NeuronBones.Hips].parent;
			for( NeuronBones i = 0; i < NeuronBones.NumOfBones; ++i )
			{
				Transform t = transforms[(int)i];
				if( t != null )
				{
                    t.SetParent(align_root, true);
                }
			}
		}
	}
}