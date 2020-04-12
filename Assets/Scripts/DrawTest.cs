using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTest : MonoBehaviour
{
     public Texture2D texture;
    public Texture2D textureMask;
    private Vector2 invasionStart = new Vector2(900f,900f);
    public float[,] invasionMassive;
    public float[,] invasionMassive2;
    public int[,] referenceMap;
    // Start is called before the first frame update
    void Start()
    {

        // texture = GetComponent<SpriteRenderer>().sprite.texture;
        Sprite sprite = Sprite.Create(texture,new Rect(0,0,texture.width,texture.height),Vector2.zero);
        GetComponent<SpriteRenderer>().sprite = sprite;
        Color pixel;
        invasionMassive = new float[texture.width,texture.height];
        invasionMassive2 = new float[texture.width, texture.height];
        referenceMap = new int[texture.width, texture.height];
        for (int y = 0; y < texture.height; y++)
        {


            for (int x = 0; x < texture.width; x++)
            {
                pixel = textureMask.GetPixel(x, y);
                //Debug.Log(pixel);
                if (pixel.r > 0.665 && pixel.r < 0.67f && pixel.g > 0.85 && pixel.g < 0.86f && pixel.b == 1.000f)
                {
                    // Debug.Log("well ya");
                    texture.SetPixel(x, y, new Color(0.9f, 0f, 1f, 1f));
                    invasionMassive[x, y] = 0;
                    referenceMap[x,y] = 1;

                }
                else
                {
                    referenceMap[x, y] = 0;
                    invasionMassive[x, y] = -1;
                }

                // RGBA(0.667, 0.855, 1.000, 1.000) water color (google maps)


            }
        }
        texture.Apply();
        invasionMassive[900, 900] = 1;

    }
    public void DoStep()
    {
        for (int y = 1; y < invasionMassive.GetLength(1)-1; y++)
        {


            for (int x = 1; x < invasionMassive.GetLength(0)-1; x++)
            {
                if (referenceMap[x,y] == 0)
                {
                    continue;
                }
                if (invasionMassive[x,y] > 0)
                {
                    invasionMassive2[x - 1, y] += 0.5f * invasionMassive[x, y];
                    if (invasionMassive2[x - 1, y] > 1)
                        invasionMassive2[x - 1, y] = 1;

                    invasionMassive2[x , y - 1] += 0.5f * invasionMassive[x, y];
                    if (invasionMassive2[x , y-1] > 1)
                        invasionMassive2[x , y-1] = 1;

                    invasionMassive2[x +1, y] += 0.5f * invasionMassive[x, y];
                    if (invasionMassive2[x + 1, y] > 1)
                        invasionMassive2[x + 1, y] = 1;

                    invasionMassive2[x , y + 1 ] += 0.5f * invasionMassive[x, y];
                    if (invasionMassive2[x , y + 1] > 1)
                        invasionMassive2[x , y + 1] = 1;




                }

                // RGBA(0.667, 0.855, 1.000, 1.000) water color (google maps)
            }
        }
        
    }
    public void Clone()
    {
        
        //invasionMassive2.CopyTo(invasionMassive,0);
        // float[,] invasionMassiveIntermediate= invasionMassive2;
        //invasionMassive2 = invasionMassive;
        // invasionMassive = invasionMassiveIntermediate;
    }
    public void DrawInvasion()
    {
        for (int y = 0; y < texture.height; y++)
        {


            for (int x = 0; x < texture.width; x++)
            {

                if (referenceMap[x,y] == 0)
                {
                    continue;
                }
                    texture.SetPixel(x, y, new Color(0.9f, 0f, invasionMassive[x,y], 0.5f));
                invasionMassive[x,y] = invasionMassive2[x,y];s
            
                

                // RGBA(0.667, 0.855, 1.000, 1.000) water color (google maps)


            }
        }
        texture.Apply();
    }
    // Update is called once per frame
    void Update()
    {
       // if (Input.GetKeyDown(KeyCode.S))
        //{
            DoStep();
            DrawInvasion();
            Clone();
        // }
    }
}
