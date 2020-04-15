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
    public float[,] depthMap;
    public int[,] referenceMap;
    public List<Vector2Int> perimeter;
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
        depthMap = new float[texture.width, texture.height];
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
                   
                    referenceMap[x,y] = 1;

                    depthMap[x,y] = FindDepth(FindMinDistance(new Vector2Int(x, y)));
                }
                else
                {
                    referenceMap[x, y] = 0;
                    depthMap[x, y] = 0;
                    //invasionMassive[x, y] = -1;
                }
                invasionMassive[x, y] = 0;
                invasionMassive2[x, y] = 0;

                // RGBA(0.667, 0.855, 1.000, 1.000) water color (google maps)


            }
        }
        texture.Apply();
        SearchPerimeter();
        DrawPerimiter();
        invasionMassive[1000, 900] = 1;
        invasionMassive[900, 900] = 1;

    }
    public void DrawPerimiter()
    {
        foreach(Vector2Int pixel in perimeter){
            texture.SetPixel(pixel.x,pixel.y, new Color(0f, 0.1f, 0.8f, 1f));
        }
        texture.Apply();
        //0.01.08
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
                if (invasionMassive[x, y] > 0)
                {
                    int limit = 1;
                    float maxDistance = new Vector2(limit, limit).magnitude;

                    for (int dy = -limit; dy <= limit; dy++)
                    {
                        for (int dx = -limit; dx <= limit; dx++)
                        {
                            if (dy == 0 && dx == 0)
                            {
                                // invasionMassive2[x, y] = invasionMassive[x, y];
                                continue;
                            }
                            else
                            {
                                float ranges = new Vector2(dx, dy).magnitude;

                                float maxSquared = maxDistance * maxDistance;
                                float distSqared = ranges * ranges * ranges;



                                float values = 0.18f / distSqared;   // 1, 1.4, 2, 2.23, 2.8

                                float value = invasionMassive2[x + dx, y + dy];
                                value += values * invasionMassive[x, y];
                                if (value > 1.0) {
                                    value = 1.0f;
                                }

                                invasionMassive2[x + dx, y + dy] = value;

                                //invasionMassive2[x  + dx, y + dy ] += values * invasionMassive[x, y];
                                //if (invasionMassive2[x + dx, y + dy] > 1)
                                //   invasionMassive2[x + dx, y + dy] = 1;
                            }
                        }
                    } 
                    
                     
                    
                    //invasionMassive2[x - 1, y] += 0.5f * invasionMassive[x, y];
                    //if (invasionMassive2[x - 1, y] > 1)
                    //    invasionMassive2[x - 1, y] = 1;

                    //invasionMassive2[x , y - 1] += 0.5f * invasionMassive[x, y];
                    //if (invasionMassive2[x , y-1] > 1)
                    //    invasionMassive2[x , y-1] = 1;

                    //invasionMassive2[x +1, y] += 0.5f * invasionMassive[x, y];
                    //if (invasionMassive2[x + 1, y] > 1)
                    //    invasionMassive2[x + 1, y] = 1;

                    //invasionMassive2[x , y + 1 ] += 0.5f * invasionMassive[x, y];
                    //if (invasionMassive2[x , y + 1] > 1)
                    //    invasionMassive2[x , y + 1] = 1;




                }

                // RGBA(0.667, 0.855, 1.000, 1.000) water color (google maps)
                //find nearby pixels
                //find distances to pixels
                //find infection coefficent
            }

        }
        
    }
    public float FindDepth(float distance)
    {
        float depth;
        depth = Mathf.Sqrt(distance);
        return depth;
    }
    public float FindMinDistance(Vector2Int coordinates)
    {
        float minDistance=100000; 
        foreach (Vector2Int point in perimeter)
        {
            float distance = Vector2Int.Distance(point, coordinates);
          
            if ( distance < minDistance )
            {
                minDistance = distance;

            }
        }
        return minDistance;
    }
    public void SearchPerimeter()
    {
        perimeter = new List<Vector2Int>();
        for (int y = 1; y < texture.height-1; y++)
        {


            for (int x = 1; x < texture.width-1; x++)
            {
                if (referenceMap[x, y] == 0) // check water
                {
                    continue;
                }


                for (int dy = -1; dy <= 1; dy++)
                {
                    for (int dx = -1; dx <= 1; dx++)
                    {


                        if (dy == 0 && dx == 0)
                        {
                            continue;
                        }
                        if (referenceMap[x + dx,y + dy] == 0 )
                        {
                            perimeter.Add(new Vector2Int(x + dx,y + dy));
                          //  goto skipChecking;
                        }
                    }
                }

                //skipChecking:;
            }
        }
        Debug.Log(perimeter.Count);
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
                    texture.SetPixel(x, y, new Color(0.9f, 0f, invasionMassive2[x,y], 0.5f));
               invasionMassive[x,y] = invasionMassive2[x,y];
            
                

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
