using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTest : MonoBehaviour
{
     public Texture2D texture;
    public Texture2D textureMask;
    // Start is called before the first frame update
    void Start()
    {

        // texture = GetComponent<SpriteRenderer>().sprite.texture;
        Sprite sprite = Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),Vector2.zero);
        GetComponent<SpriteRenderer>().sprite = sprite;
        Color pixel;
        for (int y = 0; y < texture.height; y++)
        {


            for (int x = 0; x < texture.width; x++)
            {
                pixel = textureMask.GetPixel(x, y);
                    //Debug.Log(pixel);
                if (pixel.r > 0.665 && pixel.r < 0.67f && pixel.g > 0.85 && pixel.g < 0.86f && pixel.b == 1.000f ) 
                {
                   // Debug.Log("well ya");
                    texture.SetPixel(x, y, new Color(0.9f, 0f, 1f, 1f));

                }
                
                // RGBA(0.667, 0.855, 1.000, 1.000) water color (google maps)
                

            }
        }
        texture.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
