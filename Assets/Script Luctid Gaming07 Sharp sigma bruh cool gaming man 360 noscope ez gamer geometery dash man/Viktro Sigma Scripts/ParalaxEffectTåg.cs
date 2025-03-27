using UnityEngine;

public class ParalaxEffectTåg : MonoBehaviour
{
    public Renderer backgroundRenderer;  // Assign the material renderer (plane, quad, etc.)
    public float speed = 0.2f;  // Speed of the scrolling effect

    private Vector2 offset;  // UV offset

    void Update()
    {
        
        offset.x += speed * Time.deltaTime;

        backgroundRenderer.material.mainTextureOffset = offset;
    }
}
