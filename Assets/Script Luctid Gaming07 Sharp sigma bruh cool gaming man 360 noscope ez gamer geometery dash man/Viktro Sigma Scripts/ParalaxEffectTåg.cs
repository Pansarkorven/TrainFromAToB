using UnityEngine;

public class ParalaxEffectTåg : MonoBehaviour
{
    public Renderer backgroundRenderer; 
    public float speed = 0.1f;  
    public float radius = 0.2f; 

    private Vector2 offset;  
    private float angle;  

    void Update()
    {
        // Increase the angle over time
        angle += speed * Time.deltaTime;

        // Calculate circular movement
        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;

        // Apply UV offset to material
        offset = new Vector2(x, y);
        backgroundRenderer.material.mainTextureOffset = offset;
    }
}
