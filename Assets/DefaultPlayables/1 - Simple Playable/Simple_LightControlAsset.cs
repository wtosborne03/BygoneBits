using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Audio;

namespace Simple
{
	public class Simple_LightControlAsset : PlayableAsset
	{
		public ExposedReference<AudioMixer> am;
		public Color color = Color.white;
		public float intensity = 0f;

		public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
		{
			var playable = ScriptPlayable<Simple_LightControlBehaviour>.Create(graph);

			var behaviour = playable.GetBehaviour();
			behaviour.am = am.Resolve(graph.GetResolver());
			behaviour.color = color;
			behaviour.intensity = intensity;

			return playable;
		}
	}
}