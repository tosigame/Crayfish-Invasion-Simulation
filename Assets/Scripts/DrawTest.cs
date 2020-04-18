using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DrawTest : MonoBehaviour
{

    public string depthMapFile;
     public Texture2D texture;
    public Texture2D referenceTexture;
    public Texture2D textureMask;
    private Vector2 invasionStart = new Vector2(900f,900f);
    public float[,] invasionMassive;
    public float[,] invasionMassive2;
    public float[,] depthMap;
    public float maxDepth = 0;
    public int[,] referenceMap;
    public List<Vector2Int> perimeter;
    // Start is called before the first frame update
    void Start()
    {
        depthMapFile = "F:/Проекты/юнидестонин свитамином С/Crayfish Invasion Simulation" + "/depthMap";
        texture = Instantiate(referenceTexture);
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
       
        
        invasionMassive[300, 200] = 1f;
        invasionMassive[1380, 900] = 1f;
        if (!CheckFile())                      
        {
            Invoke("FillDepthMap", 3f);
        }
        else
        {

            LoadFile();
            Debug.Log(depthMap[900,950]);
            DrawDepthMap();
        }

    }
    public bool CheckFile()
    {
        bool fileExist = false;

        fileExist = File.Exists(depthMapFile);
        return fileExist;

    }
    public void SaveFile()
    {
        Debug.Log("saving: " + depthMapFile);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file =  File.Create(depthMapFile);
        bf.Serialize(file, maxDepth);
        bf.Serialize(file, depthMap);
        file.Close();


    }
    public void LoadFile()
    {
        Debug.Log("loading: " + depthMapFile);
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(depthMapFile, FileMode.Open);
        maxDepth = (float)bf.Deserialize(file);
        depthMap = (float[,]) bf.Deserialize(file);
        file.Close();

    }
    public void FillTileDepthMap(int dx,int dy)
    {
        Vector2Int coordinates = new Vector2Int(0, 0);
       // int count = 9999;
        Debug.Log("ehllo world");
        for (int y = dy; y < dy+128; y++)
        {
            for (int x = dx; x < dx+128; x++)
            {
                if (y >= texture.height || x >= texture.width)
                {
                    return;
                }
                if (referenceMap[x, y] == 1)
                {
                    coordinates.x = x;
                    coordinates.y = y;
                    depthMap[x, y] = FindDepth(FindMinDistance(coordinates));
                    depthMap[x, y] *= depthMap[x, y];
                    if (depthMap[x,y] > maxDepth)
                    {
                        maxDepth = depthMap[x,y];
                    }
           
                }
            }
        }
        
    }
    int x1 = 0;
    int y1 = 0;
    public void  FillDepthMap()
    {
        Debug.Log("Dirty code");
        FillTileDepthMap(x1, y1);
        DrawDepthMap();
               
        
        x1 += 128;
        if (x1>=texture.width)
        {
            y1 += 128;
            x1 = 0;
            
            if (y1 >= texture.height)
            {
                Debug.Log("Here we save the file");
                SaveFile();                               
                
                return;
            }
            
            
            Debug.Log("Invoke");
        }
        Invoke("FillDepthMap", 0.2f);

    }

    public void DrawDepthMap()
    {
       
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {

                if (referenceMap[x,y]==1)
                {
                    texture.SetPixel(x, y, new Color(0.2f, 0.5f, 1 - depthMap[x, y] / maxDepth, 1f));
                }
                
            }
        }
        texture.Apply();
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
                             
                                continue;
                            }
                            else
                            {
                                float ranges = new Vector2(dx, dy).magnitude;

                                float maxSquared = maxDistance * maxDistance;
                                float distSqared = ranges * ranges * ranges;



                                float values = 0.8f / distSqared;   // 1, 1.4, 2, 2.23, 2.8

                                float value = invasionMassive2[x + dx, y + dy];
                                value += values * invasionMassive[x, y];
                                
                                if (value > 1.0) {
                                    value = 1.0f;
                                }
                                value *= 1f - (depthMap[x + dx, y + dy] / maxDepth)*0.9f ;

                                invasionMassive2[x + dx, y + dy] = value;

                               
                            }
                        }
                    } 


                }

                // RGBA(0.667, 0.855, 1.000, 1.000) water color (google maps)

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
        int count = 0;
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
                        count++;
                        if (referenceMap[x + dx,y + dy] == 0 && count >= 15 )
                        {
                            perimeter.Add(new Vector2Int(x + dx,y + dy));
                            count = 0;
                          //  goto skipChecking;
                        }
                    }
                }

                //skipChecking:;
            }
        }
        Debug.Log(perimeter.Count);
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
                if (invasionMassive2[x, y] == 0)
                {
                    texture.SetPixel(x, y, new Color(0.2f, 0.5f, 1 - depthMap[x, y] / maxDepth, 1f));
                }
                else
                {
                    Color color = Color.HSVToRGB(invasionMassive2[x, y]*0.2f, 1.0f, 1.0f);
                    texture.SetPixel(x, y, color);
                }
               invasionMassive[x,y] = invasionMassive2[x,y];
            
                

                // RGBA(0.667, 0.855, 1.000, 1.000) water color (google maps)


            }
        }
        texture.Apply();
    }
    
    void Update()
    {
      
          DoStep();
            DrawInvasion();
            //DrawDepthMap();
     
    }
}
