using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Audio;

namespace Simple
{
	public class Simple_LightControlBehaviour : PlayableBehaviour
	{
		public AudioMixer am;
		public Color color;
		public float intensity;

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			if (am != null)
			{
				am.SetFloat("musVol", intensity);
			}
		}
	}
}