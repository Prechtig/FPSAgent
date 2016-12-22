using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class HeatMap : MonoBehaviour
    {
        public Image[] img;
        int aR = 0; int aG = 0; int aB = 255;
        int bR = 255; int bG = 0; int bB = 0;

        public void UpdateColors(double[] fromCNN)
        {
            for (int i = 0; i < fromCNN.Length - 1; i++)
            {
                Color c = HeatMapColor2((float)fromCNN[i]);
                //Color c = HeatMapColor(Random.value);
                //c.a = 0.4f; ;
                img[i].color = c;
            }
        }

        private Color HeatMapColor2(float value) {
            //value = ValueAdjust(value);

            float red = (float)(bR - aR) * value + aR;      // Evaluated as -255*value + 255.
            float green = (float)(bG - aG) * value + aG;      // Evaluates as 0.
            float blue = (float)(bB - aB) * value + aB;      // Evaluates as 255*value + 0.

            return new Color(red / 255, green / 255, blue / 255, 0.5f);
        }

        private Color HeatMapColor5(float value)
        {
            int NUM_COLORS = 5;
            float[,] color = new float[,] { { 0, 0, 1 }, { 0, 1, 1 }, { 0, 1, 0 }, { 1, 1, 0 }, { 1, 0, 0 } };
            // A static array of 4 colors:  (blue,   green,  yellow,  red) using {r,g,b} for each.

            int idx1;        // |-- Our desired color will be between these two indexes in "color".
            int idx2;        // |
            float fractBetween = 0;  // Fraction between "idx1" and "idx2" where our value is.

            if (value <= 0) { idx1 = idx2 = 0; }    // accounts for an input <=0
            else if (value >= 1) { idx1 = idx2 = NUM_COLORS - 1; }    // accounts for an input >=0
            else
            {
                value = value * (NUM_COLORS - 1);        // Will multiply value by 3.
                idx1 = (int)Mathf.Floor(value);                  // Our desired color will be after this index.
                idx2 = idx1 + 1;                        // ... and before this index (inclusive).
                fractBetween = value - (float)(idx1);    // Distance between the two indexes (0-1).
            }

            float red = (color[idx2,0] - color[idx1,0]) * fractBetween + color[idx1,0];
            float green = (color[idx2,1] - color[idx1,1]) * fractBetween + color[idx1,1];
            float blue = (color[idx2,2] - color[idx1,2]) * fractBetween + color[idx1,2];

            return new Color(red, green, blue, 0.5f);
        }

        private float ValueAdjust(float value)
        {
            return Mathf.Log(value * 20 + 1) / 3.0444682f;
        }


        /*
         * 
         * bool getValueBetweenTwoFixedColors(float value, int &red, int &green, int &blue)
{
  int aR = 0;   int aG = 0; int aB=255;  // RGB for our 1st color (blue in this case).
  int bR = 255; int bG = 0; int bB=0;    // RGB for our 2nd color (red in this case).
 
  red   = (float)(bR - aR) * value + aR;      // Evaluated as -255*value + 255.
  green = (float)(bG - aG) * value + aG;      // Evaluates as 0.
  blue  = (float)(bB - aB) * value + aB;      // Evaluates as 255*value + 0.
}*/
    }
}
