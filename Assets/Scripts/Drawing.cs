using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class DrawTest : MonoBehaviour
{
   
    public void DrawDepthMap()
    {

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {

                if (referenceMap[x, y] == 1)
                {
                    texture.SetPixel(x, y, new Color(0.2f, 0.5f, pixels[x, y].depth / maxDepth, 1f));
                }

            }
        }
        texture.Apply();
    }
    public void DrawPerimiter()
    {
        foreach (Vector2Int pixel in perimeter)
        {
            texture.SetPixel(pixel.x, pixel.y, new Color(0f, 0.1f, 0.8f, 1f));
        }
        texture.Apply();
        //0.01.08
    }
    public void DrawInvasion()
    {
        for (int y = 0; y < texture.height; y++)
        {


            for (int x = 0; x < texture.width; x++)
            {

                if (referenceMap[x, y] == 0)
                {
                    continue;
                }
                if (pixels[x, y].infectedPop2 < 0.1f)
                {
                    texture.SetPixel(x, y, new Color(0.2f, 0.5f, 1 - pixels[x, y].maxPop/*depth / maxDepth*/, 1f));
                }
                else
                {
                    Color color = Color.HSVToRGB(0.9f, 0f, 1.0f);
                    color.a = 1 - pixels[x, y].infectedPop2;
                    color.r = pixels[x, y].infectedPop;
                    color.g = pixels[x, y].maxPop;
                    texture.SetPixel(x, y, color);
                }

                pixels[x, y].infectedPop = pixels[x, y].infectedPop2;





                // RGBA(0.667, 0.855, 1.000, 1.000) water color (google maps)


            }
        }
        texture.Apply();
    }
}
