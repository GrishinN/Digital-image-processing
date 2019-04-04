using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basics_of_Digital_Image_Processing
{
    static class Converter
    {
        public static (byte,byte,byte) RGBtoHSV(double R, double G, double B)
        {
            double min, max, delta;
            //double R, G, B;
            double X, Y, Z;
            double H = -1, S, V;
            double del_R, del_G, del_B;
            R = R / 255.00;
            G = G / 255.00;
            B = B / 255.00;

            min = Math.Min(Math.Min(R, G), B);
            max = Math.Max(Math.Max(R, G), B);
            delta = max - min;

            V = max;
            if (delta == 0)
            {
                H = 0;
                S = 0;
            }
            else
            {
                S = delta / max;

                del_R = (((max - R) / 6.00) + (delta / 2.00)) / delta;
                del_G = (((max - G) / 6.00) + (delta / 2.00)) / delta;
                del_B = (((max - B) / 6.00) + (delta / 2.00)) / delta;

                if (R == max)
                {
                    H = del_B - del_G;
                }
                else if (G == max)
                {
                    H = (0.33) + del_R - del_B;
                }
                else if (B == max)
                {
                    H = (0.66) + del_G - del_R;
                }

                if (H < 0)
                {
                    H += 1;
                }
                if (H > 1)
                {
                    H -= 1;
                }
            }

            byte componentH = (byte)(Math.Round(H, 2) * 255);
            byte componentS = (byte)(Math.Round(S, 2) * 255);
            byte componentV = (byte)(Math.Round(V, 2) * 255);

            
            return (componentH, componentS, componentV);
        }

        public static (byte, byte, byte) HSVtoRGB(double H , double S , double V)
        {
            
            byte R, G, B;
            
            double var_h, var_1, var_2, var_3;
            double var_r, var_g, var_b;
            int var_i;

          
            H = Math.Round(H / 255, 2);
            S = Math.Round(S / 255, 2);
            V = Math.Round(V / 255, 2);


            if (S == 0)
            {
                R = (byte)(V * 255.00);
                G = (byte)(V * 255.00);
                B = (byte)(V * 255.00);

                return (R, G, B);
            }
            else
            {
                var_h = H * 6;
                if (var_h == 6)
                {
                    var_h = 0;
                }
                var_i = (int)Math.Round(var_h);
                var_1 = V * (1 - S);
                var_2 = V * (1 - S * (var_h - var_i));
                var_3 = V * (1 - S * (1 - (var_h - var_i)));

                switch (var_i)
                {
                    case 0:
                        {
                            var_r = V;
                            var_g = var_3;
                            var_b = var_1;
                            break;
                        }
                    case 1:
                        {
                            var_r = var_2;
                            var_g = V;
                            var_b = var_1;
                            break;
                        }
                    case 2:
                        {
                            var_r = var_1;
                            var_g = V;
                            var_b = var_3;
                            break;
                        }
                    case 3:
                        {
                            var_r = var_1;
                            var_g = var_2;
                            var_b = V;
                            break;
                        }
                    case 4:
                        {
                            var_r = var_3;
                            var_g = var_1;
                            var_b = V;
                            break;
                        }
                    default:
                        {
                            var_r = V;
                            var_g = var_1;
                            var_b = var_2;
                            break;
                        }
                }
                R = (byte)(var_r * 255.00);
                G = (byte)(var_g * 255.00);
                B = (byte)(var_b * 255.00);

                R = R < 0 ? (byte)0 : R;
                G = G < 0 ? (byte)0 : G;
                B = B < 0 ? (byte)0 : B;

                R = R > 255 ? (byte)255 : R;
                G = G > 255 ? (byte)255 : G;
                B = B > 255 ? (byte)255 : B;


                return (R, G, B);
                               
            }
        }
    }
}
