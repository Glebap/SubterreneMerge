using UnityEngine;

public class ParticleManager : MonoBehaviour
{
	[SerializeField] private ParticleSystem _mergeParticle;
	[SerializeField] private ParticleSystem _newHeroParticle;
	
	
	public void ShowMergeParticle(Vector3 position)
	{
		Instantiate(_mergeParticle, position, _mergeParticle.transform.rotation);
	}

	public void ShowNewHeroParticle(Vector3 position)
	{
		Instantiate(_newHeroParticle, position, _mergeParticle.transform.rotation);
	}
}