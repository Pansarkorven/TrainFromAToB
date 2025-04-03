using UnityEngine;

public class ParalaxEffectTåg : MonoBehaviour
{
    public Renderer backgroundRenderer; 
    public float speed = 0.2f;

    private Vector2 offset;

    void Update()
    {
        
        offset.x += speed * Time.deltaTime;

        backgroundRenderer.material.mainTextureOffset = offset;
    }
}
