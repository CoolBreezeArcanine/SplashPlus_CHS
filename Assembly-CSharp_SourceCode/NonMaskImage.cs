using UnityEngine;
using UnityEngine.UI;

public class NonMaskImage : Image
{
	public override Material GetModifiedMaterial(Material baseMaterial)
	{
		return baseMaterial;
	}
}
