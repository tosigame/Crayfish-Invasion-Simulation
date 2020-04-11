using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTest : MonoBehaviour
{
     public Texture2D texture;
    // Start is called before the first frame update
    void Start()
    {

        // texture = GetComponent<SpriteRenderer>().sprite.texture;
        Sprite sprite = Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),Vector2.zero);
        GetComponent<SpriteRenderer>().sprite = sprite;
        for (int y = 100; y < 300; y++)
        {


            for (int x = 100; x < 300; x++)
            {
                texture.SetPixel(x, y, new Color(0.5f, 0.84f, .9f, 0.88f));
            }
        }
        texture.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
