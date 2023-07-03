using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshRenderer))]
public class ColorModifierScript : MonoBehaviour
{
    public Color color;

    private void Start()
    {
        transform.GetComponent<MeshRenderer>().sharedMaterial.color = color;
    }
}
